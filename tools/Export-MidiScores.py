"""Engrave reviewed working notation from the published performance MIDIs.

Requires Python, mido and numpy, plus a separate LilyPond executable. The supplied
engraving.json retains the original-performance bar grid and editorial settings.
This never edits a published MIDI or MP3. UTF-8 is explicit at every text boundary.
"""
import argparse,bisect,collections,hashlib,json,math,subprocess
from pathlib import Path
import mido,numpy as np

def read_midi(path):
    active={}; notes=[]; seconds=0
    for message in mido.MidiFile(path):
        seconds+=message.time
        if message.type=='note_on' and message.velocity>0:
            active[(message.channel,message.note)]=(seconds,message.velocity)
        elif message.type=='note_off' or message.type=='note_on' and message.velocity==0:
            start,velocity=active.pop((message.channel,message.note))
            notes.append(dict(start=start,end=seconds,pitch=message.note,velocity=velocity,channel=message.channel))
    assert not active
    return notes

def sha(path): return hashlib.sha256(path.read_bytes()).hexdigest()
def save_json(path,data): path.write_bytes((json.dumps(data,indent=2,ensure_ascii=False)+'\n').encode('utf-8'))
def quote(text): return '"'+text.replace('\\','/').replace('"','\\"')+'"'

def render_song(song,exe,output):
    metadata=json.loads((song/'song.json').read_text(encoding='utf-8'))
    config=json.loads((song/'scores/engraving.json').read_text(encoding='utf-8'))
    bars=config['barStartSeconds']; per_bar=config['sixteenthsPerBar']; count=len(bars)-1
    total=count*per_bar; ref=config.get('melodyReference',[])
    variant=next(v for v in metadata['variants'] if v['id']=='piano-original')
    midi=song/variant['midiPath']; assert sha(midi)==variant['midiSha256']
    source=read_midi(midi)

    def position(t):
        i=max(0,min(count-1,bisect.bisect_right(bars,t)-1))
        return max(0,min(total,i*per_bar+(t-bars[i])/(bars[i+1]-bars[i])*per_bar))
    def quant(t): return max(0,min(total,round(position(t))))
    def key_at(bar):
        return next((v for k,v in sorted(config['keyChanges'].items(),key=lambda x:int(x[0]),reverse=True) if bar+1>=int(k)),config['key'])
    def pitch(number,bar):
        sharp=['c','cis','d','dis','e','f','fis','g','gis','a','ais','b']
        flat=['c','cis','d','ees','e','f','fis','g','aes','a','bes','b']
        name=(sharp if key_at(bar)=='e' else flat)[number%12]
        octave=number//12-4
        return name+("'"*octave if octave>=0 else ','*(-octave))

    raw_parts={p:[] for p in ['melody','upper','lower','bass']}
    dropped=0
    for n in source:
        if n['end']-n['start']<config['minimumNoteSeconds']:
            dropped+=1; continue
        begin=quant(n['start']); end=max(begin+1,quant(n['end'])); end=min(total,end)
        if begin>=total: continue
        bar=min(count-1,begin//per_bar); phase=begin%per_bar
        if config['melodyMethod']=='metrical-burgmuller':
            melodic=n['pitch']>=64 and (phase in [0,4,5] or bar>=38) and n['velocity']>=48
        else:
            q=position(n['start'])/4
            melodic=any(abs(q-r['start'])<.42 and n['pitch'] in [r['pitch']-12,r['pitch']] for r in ref)
        part='melody' if melodic else 'bass' if n['pitch']<48 else 'upper' if n['pitch']>=60 else 'lower'
        raw_parts[part].append(dict(start=begin,end=end,pitch=n['pitch'],velocity=n['velocity']))

    # Chordify attacks within a sixteenth; clip inner voices at their next attack
    # instead of printing pedal resonance as many overlapping written voices.
    def chordify(notes):
        groups=collections.defaultdict(list)
        for n in notes: groups[n['start']].append(n)
        result=[]; starts=sorted(groups)
        for index,start in enumerate(starts):
            group=groups[start]; end=min(max(n['end'] for n in group),starts[index+1] if index+1<len(starts) else total)
            result.append(dict(start=start,end=end,pitches=sorted({n['pitch'] for n in group}),velocity=round(np.mean([n['velocity'] for n in group]))))
        return result
    parts={name:chordify(notes) for name,notes in raw_parts.items()}
    expected_attacks={(n['start'],n['pitch']) for values in raw_parts.values() for n in values}
    written_attacks={(n['start'],p) for values in parts.values() for n in values for p in n['pitches']}
    assert expected_attacks==written_attacks,'Quantized piano pitches/attacks were lost during voice assignment'
    arrangement=json.loads((song/'sources/string-arrangement-original-timing.json').read_text(encoding='utf-8'))
    for name,notes in arrangement['parts'].items():
        values=[]
        for n in notes:
            begin=quant(n['start']); end=max(begin+1,quant(n['soundingEnd']))
            end=min(total,end)
            if begin>=total: continue
            if values and values[-1]['pitches']==[n['note']] and begin<=values[-1]['end']:
                values[-1]['end']=max(end,values[-1]['end']); continue
            if values and values[-1]['end']>begin: values[-1]['end']=begin
            values.append(dict(start=begin,end=end,pitches=[n['note']],velocity=n['velocity']))
        parts[name]=values

    durations={16:'1',12:'2.',8:'2',6:'4.',4:'4',3:'8.',2:'8',1:'16'}
    def fragments(start,end):
        while start<end:
            remain=end-start
            if start%4: remain=min(remain,4-start%4)
            amount=max(n for n in durations if n<=remain)
            yield amount
            start+=amount
    def voice(name,show_rests=True):
        notes=parts[name]; result=[]; first_string_note=True
        for bar in range(count):
            start=bar*per_bar; end=start+per_bar; cursor=start; content=[]
            relevant=[n for n in notes if n['start']<end and n['end']>start]
            if not relevant:
                content.append('R1*'+config['timeSignature'] if show_rests else 's1*'+config['timeSignature'])
            for n in relevant:
                a=max(start,n['start']); b=min(end,n['end'])
                if a>cursor:
                    content.extend(('r' if show_rests else 's')+durations[d] for d in fragments(cursor,a))
                pitches=' '.join(pitch(p,bar) for p in n['pitches'])
                token=pitches if len(n['pitches'])==1 else '<'+pitches+'>'
                steps=list(fragments(a,b))
                for i,d in enumerate(steps):
                    continued=i<len(steps)-1 or b<n['end']
                    text=token+durations[d]+('~' if continued else '')
                    if name in ['violin','viola','cello'] and first_string_note:
                        text+='\\pp ^\\markup \\italic "dolce"'
                        first_string_note=False
                    if n is notes[-1] and not continued: text+='\\fermata'
                    content.append(text)
                cursor=b
            if relevant and cursor<end:
                content.extend(('r' if show_rests else 's')+durations[d] for d in fragments(cursor,end))
            result.append(' '.join(content)+' | % '+str(bar+1))
        return '\n'.join(result)

    mean_quarter_tempo=60*(total/4)/(bars[-1]-bars[0])
    tempo=round(mean_quarter_tempo)
    tempo_sleep=round(mean_quarter_tempo*.833333)
    tempo_unit=8 if per_bar==6 else 4; displayed=tempo*2 if tempo_unit==8 else tempo
    sleep_displayed=tempo_sleep*2 if tempo_unit==8 else tempo_sleep
    license_name=metadata['license']; performer=metadata['sourcePerformer'].split(';')[0]
    short='op. 109 Nr. 7' if '109' in song.name else 'op. 56 Nr. 1'
    composer=metadata['composer']; credit=performer+' / Open Sleep Music contributors | '+license_name
    title='Berceuse '+short
    text=r'''\version "2.26.0"
\pointAndClickOff
#(set-global-staff-size 17)
\paper {
  #(set-paper-size "a4")
  top-margin = 13\mm
  bottom-margin = 15\mm
  left-margin = 16\mm
  right-margin = 14\mm
  ragged-last-bottom = ##t
  print-page-number = ##t
  indent = 15\mm
  short-indent = 7\mm
  system-system-spacing.basic-distance = #18
  system-system-spacing.minimum-distance = #15
  system-system-spacing.padding = #4
  oddFooterMarkup = \markup \column {
    \fill-line { \tiny "CREDIT" \fromproperty #'page:page-number-string }
    \fill-line { \tiny \with-url #"LICENSE_URI" "LICENSE_URI" \tiny \with-url #"SOURCE_URI" "Source recording: Wikimedia Commons" }
  }
  evenFooterMarkup = \oddFooterMarkup
}
\header {
  title = "TITLE"
  composer = "COMPOSER"
  arranger = "MIDI-Bearbeitung: Open Sleep Music contributors"
  subsubtitle = \markup \center-column { "Automatische Arbeitsausgabe - Tonhöhen und Rhythmik musikalisch prüfen" "SLEEP_DESCRIPTION" }
  copyright = \markup \tiny "LICENSE"
  tagline = ##f
}
global = {
  \time METER
  \key KEY \major
  \tempo "Originaltempo ca." UNIT = TEMPO
'''.replace('CREDIT',credit).replace('TITLE',title).replace('COMPOSER',composer).replace('LICENSE_URI',metadata['licenseUri']).replace('SOURCE_URI',metadata['sourcePageUri']).replace('LICENSE',metadata['licenseUri']).replace('METER',config['timeSignature']).replace('KEY',config['key']).replace('SLEEP_DESCRIPTION','Einschlaftempo: '+('Achtel' if tempo_unit==8 else 'Viertel')+' ca. '+str(sleep_displayed)).replace('UNIT',str(tempo_unit)).replace('TEMPO',str(displayed))
    for i in range(count):
        if str(i+1) in config['keyChanges'] and i>0: text+='\\key '+key_at(i)+' \\major\n'
        if i%8==0: text+='\\mark \\markup \\box '+quote(str(i+1))+'\n'
        text+='s1*'+config['timeSignature']+' |\n'
    text+='\\bar "|."\n}\n'
    names={'melody':'melody','upper':'upper','lower':'lower','bass':'bass','violin':'violin','viola':'viola','cello':'cello'}
    for name in names:
        text+=name+' = {\n'+voice(name,show_rests=name in ['melody','lower','violin','viola','cello'])+'\n}\n'
    # One expressive dynamic per four bars; no false precision about velocities.
    text+='pianoDynamics = {\n'
    last=None
    for bar in range(count):
        ns=[n for n in source if bars[bar]<=n['start']<bars[bar+1] and n['end']-n['start']>=.08]
        v=np.median([n['velocity'] for n in ns]) if ns else 45
        dynamic='pp' if v<42 else 'p' if v<53 else 'mp' if v<63 else 'mf'
        command='\\'+dynamic if bar%4==0 and dynamic!=last else ''
        if command: last=dynamic
        text+='s1*'+config['timeSignature']+command+' |\n'
    text+='}\n'
    text+=r'''
pianoMusic = \new PianoStaff \with { instrumentName = "Klavier" shortInstrumentName = "Kl." } <<
  \new Staff = "RH" <<
    \new Voice { \global }
    \new Voice { \voiceOne \melody }
    \new Voice { \voiceTwo \upper }
  >>
  \new Dynamics { \pianoDynamics }
  \new Staff = "LH" <<
    \clef bass
    \new Voice { \global }
    \new Voice { \voiceOne \lower }
    \new Voice { \voiceTwo \bass }
  >>
>>
'''
    # Separate simultaneous global and music contexts, not sequential, for parts.
    def single(part,long,short,clef):
        return '\\new Staff \\with { instrumentName = '+quote(long)+' shortInstrumentName = '+quote(short)+' } << { \\clef '+clef+' \\global } { \\'+part+' } >>'
    string_staffs=[single('violin','Violine','Vl.','treble'),single('viola','Viola','Va.','alto'),single('cello','Cello','Vc.','bass')]
    book_specs=[('piano','Klavier solo','\\pianoMusic'),('piano-strings','Klavier mit Streichern - Partitur','<<\n'+'\n'.join(string_staffs)+'\n\\pianoMusic\n>>')]
    book_specs +=[(p,label+' - Einzelstimme',single(p,label,label,clef)) for p,label,clef in [('violin','Violine','treble'),('viola','Viola','alto'),('cello','Cello','bass')]]
    for suffix,subtitle,music in book_specs:
        text+='\\book {\n\\bookOutputSuffix '+quote(suffix+'-v1')+'\n\\header { subtitle = '+quote(subtitle)+' }\n'
        if suffix=='piano': text+='\\paper { max-systems-per-page = #4 }\n'
        elif suffix=='piano-strings': text+='\\paper { max-systems-per-page = #2 }\n'
        else: text+='\\paper { max-systems-per-page = #5 }\n'
        text+='\\score {\n'+music+'\n\\layout { \\context { \\Score \\override BarNumber.break-visibility = ##(#f #t #t) } }\n}\n'
        text+='\\markup \\column { \\small "Rhythmisch quantisiert; Handverteilung und Notenlängen redaktionell geglättet." \\small "Pedal nach Harmonie; Anschlag und Pedal im MIDI automatisch geschätzt." \\small "Wiederholungen ausgeschrieben. Beide Tempi verwenden dieselben Noten." }\n'
        if '56' in song.name:
            text+='\\markup \\small "Takt-/Formabgleich: Mike Blackstock, Mutopia 1536 (Public Domain)."\n'
        text+='}\n'
    source_path=song/'scores'/(song.name+'.ly'); source_path.write_bytes(text.encode('utf-8'))
    output.mkdir(parents=True,exist_ok=True)
    process=subprocess.run([str(exe.resolve()),'-dno-point-and-click','-o',str((output/song.name).resolve()),str(source_path.resolve())],capture_output=True,text=True,encoding='utf-8',errors='replace')
    log=output/(song.name+'-engraving.log'); log.write_bytes((process.stdout+'\n'+process.stderr).encode('utf-8'))
    assert process.returncode==0,(process.returncode,process.stderr[-4000:])
    config['notationAudit']=dict(sourcePianoMidiPath=variant['midiPath'],sourcePianoMidiSha256=sha(midi),
        sourceNoteAttacks=len(source),excludedNotesShorterThanThreshold=dropped,
        pianoNotatedChordAttacks=sum(len(parts[p]) for p in ['melody','upper','lower','bass']),
        writtenStringAttacks={p:len(parts[p]) for p in ['violin','viola','cello']},
        measures=count,originalTempoReference=displayed,sleepTempoReference=sleep_displayed,tempoUnit=tempo_unit,
        quantization='Nearest sixteenth on the saved measure grid; simultaneous attacks chordified, inner voice lengths clipped at following attacks; complete string sounding ends used instead of early sample-release note-offs.',
        quantizedPianoPitchAttackSetsMatch=True,
        reviewStatus='automatic-working-edition; layout-reviewed; musical-proofreading-required')
    save_json(song/'scores/engraving.json',config)
    print(song.name,'engraved',count,'measures;',len(source),'source notes;',dropped,'very short attacks excluded from working notation',flush=True)

if __name__=='__main__':
    parser=argparse.ArgumentParser(); parser.add_argument('--lilypond',required=True,type=Path); parser.add_argument('--output',type=Path,default=Path('output/pdf')); parser.add_argument('--music-root',type=Path,default=Path('music'))
    args=parser.parse_args()
    for song in sorted(args.music_root.iterdir()):
        if (song/'scores/engraving.json').exists(): render_song(song,args.lilypond,args.output)
