# Fauré Berceuse recording and MIDI options

The maintainer reports a rattling or metallic piano sound after slowing the current Berceuse op. 56 No. 1. A comparison at the original tempo should precede replacement: the Android player preserves pitch at 0.833333 speed, which can affect piano attacks and sustained resonances. This is a possible contributor, not a confirmed diagnosis of the source piano. The catalog recording and its speed/gain remain unchanged.

## Current recording

The [specific Commons file](https://commons.wikimedia.org/wiki/File:Berceuse_by_Gabriel_Faur%C3%A9_op56_no1.ogg) credits Brian M. Jones, a solo-piano transcription of the first Dolly Suite movement, and CC BY 3.0 for the performance. It was downloaded from YouTube and converted to Ogg; the current file is approximately 78 kbps. Duration is 214.622 seconds before playback speed. The app uses 0.833333 speed, giving approximately 4:18 playback, and volume gain 0.75. Android uses `PlaybackParams.SetSpeed(...).SetPitch(1)`. Source piano sound, prior encoding and pitch-preserving time stretching may contribute independently to the reported sound. A higher-bitrate transcode of this same file would not restore lost source information.

## Alternative audio leads

| Source recording | Recording grant and provenance | Assessment |
| --- | --- | --- |
| [Ingo Lutter, Berceuse for cello and piano](https://imslp.org/wiki/Dolly_Suite,_Op.56_(Faur%C3%A9,_Gabriel)#Recordings), recording #897828 | Performer-uploaded cello recording with Finale GPO piano, MP4, 3:04, CC BY-SA 4.0. The same page supplies a synthesized cello/piano MP3 #827886 under CC BY-SA 4.0. | Credible compatible-license audition lead with a different timbre. No claim that it sounds better until heard in full. The piano-only accompaniment #827887 is not a complete replacement for the melody. |
| [Pracchia-78, Berceuse dalla suite Dolly](https://commons.wikimedia.org/wiki/File:Gabriel_Faur%C3%A9_-_Berceuse_dalla_suite_Dolly.ogg) | Uploader's own synthesized piano audio, expressly dedicated to the public domain; 189.199 seconds. The page names Conexant GM500 Beta.sf2 as its sound bank. | An existing digital-piano audition lead. Check the underlying sound-bank/render rights before production admission; the uploader's dedication alone does not independently establish them. |
| [Gabriel Antonio Hernandez Romero and Shawn Christopher White](https://imslp.org/wiki/Dolly_Suite,_Op.56_(Faur%C3%A9,_Gabriel)#Recordings), #517196 | Full four-hand Dolly Suite performance, CC BY-NC 4.0. | Excluded from the freely licensed app catalog because of its non-commercial restriction. |

These are source-page references, not new catalog candidates or replacements. No commercial recording is treated as free merely because Fauré's composition is public domain. A better freely reusable acoustic solo/duet recording is not established by this search.

## MIDI sources

[Standard MIDI Files](https://midi.org/standard-midi-files) remain a supported interchange format, with timed notes, tempo, channels and controller events rather than recorded sound. MIDI 2.0 also has a newer clip-file specification; the presence of a newer specification does not prevent use of traditional `.mid` files. The final sound comes from the chosen synthesizer and instrument samples.

| Source | Checked file | Reuse result |
| --- | --- | --- |
| [Mutopia Berceuse, Mike Blackstock](https://www.mutopiaproject.org/cgibin/piece-info.cgi?id=1536) | [MIDI](https://www.mutopiaproject.org/ftp/FaureG/O56/faure_dolly_berceuse/faure_dolly_berceuse.mid): HTTP 200, 8,342 bytes, `MThd`, Standard MIDI File format 1, three tracks, 384 ticks per quarter note. LilyPond source also supplied. | Item explicitly declares Public Domain, but this is a flute/guitar adaptation rather than the original four-hand or current solo-piano version. Useful free preparation source if that arrangement is wanted. |
| [mfiles piano-duet Berceuse](https://www.mfiles.co.uk/scores/faure-dolly-suite-1-berceuse.htm) | MIDI: HTTP 200, 9,915 bytes, `MThd`, format 1, three tracks, 256 ticks per quarter note. Piano-solo versions also linked. | [Publisher terms](https://www.mfiles.co.uk/copyright.htm) restrict personal/non-commercial use and redistribution, including adaptations. Excluded from app reuse without a separate grant. Free download does not establish a free license. |

The MIDI files were inspected in the local research cache and are not committed or added to the audio catalog. The IMSLP heading Synthesized/MIDI includes already-rendered MP3s; it does not prove a raw MIDI file is available for each entry.

## Playback in Open Sleep Music

Direct MIDI playback is not currently supported by the app. The Core download inspector admits MP3, compatible Ogg, FLAC, WAV and MP4/M4A signatures, and rejects MIDI's `MThd` header. The app has no MIDI sequencer, synthesizer or bundled instrument sample bank. Native platform capabilities alone do not bypass that catalog/download policy.

A separate preparation step can render an admitted MIDI or score into MP3/FLAC/WAV using a quality piano sound. [FluidSynth](https://www.fluidsynth.org/) is one available SoundFont synthesizer. The resulting audio can use the existing player. Set the desired tempo before rendering, then play the finished recording at 100% speed. This changes note timing before sound generation and avoids stretching the waveform. Instrument-bank rights must allow the intended generated-audio distribution; arrangement/MIDI attribution must also be retained. Any project-produced media needs its own stable hosting and full admission metadata.

## Audio to MIDI transcription

[Spotify Basic Pitch](https://github.com/spotify/basic-pitch) produces MIDI from polyphonic audio and works best with one instrument at a time. [Piano Transcription](https://github.com/bytedance/piano_transcription) specifically models piano notes and pedal events. Either can provide an initial transcription of the original-speed piano recording. It would infer notes, timings and some performance information, then a new synthesizer would create fresh piano sound; it does not repair the existing waveform.

Piano chords, pedal overlap, harmonics, noise and compressed source audio can produce missing/extra notes, wrong octaves or incorrect note lengths. A transcription needs manual comparison with the score, plus deliberate dynamics, phrasing and pedal work before rendering. The current recording's CC BY 3.0 permits adaptations with attribution and change disclosure; conversion does not erase its performer/arrangement provenance. Model/software and instrument-sample licensing remain separate.

For a clean piano replacement, prefer a verified score-derived MIDI/MusicXML performance with a suitable licensed piano bank, rendered at the intended calm tempo. A checked existing MIDI/score avoids inferring notes from the noisy recording. Full listening, source rights and delivery checks still precede catalog replacement. No transcription, synthesized audio or native MIDI feature is generated as part of this research.
