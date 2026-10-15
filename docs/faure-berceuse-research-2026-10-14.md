# Fauré Berceuse recording and MIDI options

The maintainer reports a rattling or metallic piano sound after slowing the current Berceuse op. 56 No. 1. A comparison at the original tempo should precede replacement: the Android player preserves pitch at 0.833333 speed, which can affect piano attacks and sustained resonances. This is a possible contributor, not a confirmed diagnosis of the source piano. The catalog recording and its speed/gain remain unchanged.

## Current recording

The [specific Commons file](https://commons.wikimedia.org/wiki/File:Berceuse_by_Gabriel_Faur%C3%A9_op56_no1.ogg) credits Brian M. Jones, a solo-piano transcription of the first Dolly Suite movement, and CC BY 3.0 for the performance. It was downloaded from YouTube and converted to Ogg; the current file is approximately 78 kbps. Duration is 214.622 seconds before playback speed. The app uses 0.833333 speed, giving approximately 4:18 playback, and volume gain 0.75. Android uses `PlaybackParams.SetSpeed(...).SetPitch(1)`. Source piano sound, prior encoding and pitch-preserving time stretching may contribute independently to the reported sound. A higher-bitrate transcode of this same file would not restore lost source information.

## Alternative audio leads

| Source recording | Recording grant and provenance | Assessment |
| --- | --- | --- |
| [Ingo Lutter, Berceuse for cello and piano](https://imslp.org/wiki/Dolly_Suite,_Op.56_(Faur%C3%A9,_Gabriel)#Recordings), recording #897828 | Performer-uploaded cello recording with Finale GPO piano, MP4, 3:04, CC BY-SA 4.0. The same page supplies a synthesized cello/piano MP3 #827886 under CC BY-SA 4.0. | Credible compatible-license audition lead with a different timbre. No claim that it sounds better until heard in full. The piano-only accompaniment #827887 is not a complete replacement for the melody. |
| [Pracchia-78, Berceuse dalla suite Dolly](https://commons.wikimedia.org/wiki/File:Gabriel_Faur%C3%A9_-_Berceuse_dalla_suite_Dolly.ogg) | Uploader's own synthesized piano audio, expressly dedicated to the public domain; 189.199 seconds. The page names Conexant GM500 Beta.sf2 as its sound bank. | An existing digital-piano audition lead. Check the underlying sound-bank/render rights before production admission; the uploader's dedication alone does not independently establish them. |
| [Gabriel Antonio Hernandez Romero and Shawn Christopher White](https://imslp.org/wiki/Dolly_Suite,_Op.56_(Faur%C3%A9,_Gabriel)#Recordings), #517196 | Full four-hand Dolly Suite performance, CC BY-NC 4.0. | Initially excluded under the previous policy; admitted for noncommercial Preview audition on 2026-10-15 after the maintainer's policy change, with the first movement selected through playback offsets. |

Following the maintainer's request, Ingo Lutter's complete cello/piano recording #897828, his complete synthesized version #827886, and Pracchia-78's digital-piano version are external audition candidates in Preview's pre-qualify collection. Their recording-rights status remains explicit and unchecked, with original-source links, credits and instrument tags retained. No production recording is replaced. The non-commercial piano-duet recording is subsequently added as the separate downloadable Preview audition described below. A better unrestricted freely reusable acoustic solo/duet recording is not established by this search.

Each new candidate explicitly uses `playbackSpeed: 1`, normal gain and zero trims. This records the requested original-tempo comparison; pre-qualify opens the external source player, whose playback rate the app cannot enforce. No download URI is assigned, so the generator emits these entries as external references rather than offline tracks. The two IMSLP sources use verified permanent file links that resolve to their individual recording anchors. Source-listed IMSLP durations are rounded, not independently decoded measurements.

## MIDI sources

[Standard MIDI Files](https://midi.org/standard-midi-files) remain a supported interchange format, with timed notes, tempo, channels and controller events rather than recorded sound. MIDI 2.0 also has a newer clip-file specification; the presence of a newer specification does not prevent use of traditional `.mid` files. The final sound comes from the chosen synthesizer and instrument samples.

| Source | Checked file | Reuse result |
| --- | --- | --- |
| [Mutopia Berceuse, Mike Blackstock](https://www.mutopiaproject.org/cgibin/piece-info.cgi?id=1536) | [MIDI](https://www.mutopiaproject.org/ftp/FaureG/O56/faure_dolly_berceuse/faure_dolly_berceuse.mid): HTTP 200, 8,342 bytes, `MThd`, Standard MIDI File format 1, three tracks, 384 ticks per quarter note. LilyPond source also supplied. | Item explicitly declares Public Domain, but this is a flute/guitar adaptation rather than the original four-hand or current solo-piano version. Useful free preparation source if that arrangement is wanted. |
| [mfiles piano-duet Berceuse](https://www.mfiles.co.uk/scores/faure-dolly-suite-1-berceuse.htm) | MIDI: HTTP 200, 9,915 bytes, `MThd`, format 1, three tracks, 256 ticks per quarter note. Piano-solo versions also linked. | [Publisher terms](https://www.mfiles.co.uk/copyright.htm) restrict personal/non-commercial use and redistribution, including adaptations. Excluded from app reuse without a separate grant. Free download does not establish a free license. |

The MIDI files were inspected in the local research cache and are not committed or added to the audio catalog. Following the maintainer's request, the Mutopia MIDI item is also included as an external pre-qualify source reference, explicitly labeled MIDI and flute/guitar. Its source declares the score/MIDI public domain; no recording-rights approval is inferred. No audio download URI or audio duration is assigned, and `playbackSpeed: 1` records original-tempo intent only. MIDI playback is controlled by external software. The IMSLP heading Synthesized/MIDI includes already-rendered MP3s; it does not prove a raw MIDI file is available for each entry.

## Playback in Open Sleep Music

Direct MIDI playback is not currently supported by the app. The Core download inspector admits MP3, compatible Ogg, FLAC, WAV and MP4/M4A signatures, and rejects MIDI's `MThd` header. The app has no MIDI sequencer, synthesizer or bundled instrument sample bank. Native platform capabilities alone do not bypass that catalog/download policy.

A separate preparation step can render an admitted MIDI or score into MP3/FLAC/WAV using a quality piano sound. [FluidSynth](https://www.fluidsynth.org/) is one available SoundFont synthesizer. The resulting audio can use the existing player. Set the desired tempo before rendering, then play the finished recording at 100% speed. This changes note timing before sound generation and avoids stretching the waveform. Instrument-bank rights must allow the intended generated-audio distribution; arrangement/MIDI attribution must also be retained. Any project-produced media needs its own stable hosting and full admission metadata.

## Audio to MIDI transcription

[Spotify Basic Pitch](https://github.com/spotify/basic-pitch) produces MIDI from polyphonic audio and works best with one instrument at a time. [Piano Transcription](https://github.com/bytedance/piano_transcription) specifically models piano notes and pedal events. Either can provide an initial transcription of the original-speed piano recording. It would infer notes, timings and some performance information, then a new synthesizer would create fresh piano sound; it does not repair the existing waveform.

Piano chords, pedal overlap, harmonics, noise and compressed source audio can produce missing/extra notes, wrong octaves or incorrect note lengths. A transcription needs manual comparison with the score, plus deliberate dynamics, phrasing and pedal work before rendering. The current recording's CC BY 3.0 permits adaptations with attribution and change disclosure; conversion does not erase its performer/arrangement provenance. Model/software and instrument-sample licensing remain separate.

For a clean piano replacement, prefer a verified score-derived MIDI/MusicXML performance with a suitable licensed piano bank, rendered at the intended calm tempo. A checked existing MIDI/score avoids inferring notes from the noisy recording. Full listening, source rights and delivery checks still precede catalog replacement. No transcription, synthesized audio or native MIDI feature is generated as part of this research.

## Noncommercial piano-duet audition, 2026-10-15

The maintainer selects IMSLP #517196 for its perceived recording quality and authorizes CC BY-NC 4.0 recordings for the project's noncommercial use. The [specific recording](https://imslp.org/wiki/Special:ReverseLookup/517196) credits Gabriel Antonio Hernandez Romero, piano primo, and Shawn Christopher White, piano secondo; recording ℗ 2017, Montreal, uploaded by performer Gabrielhero on 2018-03-29. The authoritative work page explicitly declares [CC BY-NC 4.0](https://creativecommons.org/licenses/by-nc/4.0/). This grant is recorded as verified for noncommercial use, independently of the MIT application license. The revised `AGENTS.md` requires removal from any monetized app offering and its associated download/catalog-update delivery before commercial use, unless separate commercial permission is documented. No automatic commercial-build filter is introduced.

The public embedded player supplies the stable original [complete-suite MP3](https://s9.imslp.org/files/imglnks/usimg/4/43/IMSLP517196-PMLP10472-dolly_March_15_2017_final_MP3.mp3). Checked HTTP 200, MPEG audio signature `fffb9064`, 16,090,174 bytes; complete decoding yields 1,005.636 seconds. The source supplies no upstream checksum. The app downloads this entire original file; playback metadata selects the first movement without publishing a cut or transcoded audio file.

| Setting | Value in the original recording's timeline |
| --- | --- |
| Start offset | 2,600 ms; first attack near 2.85 s |
| Selected end | 175.300 s; second movement attack near 175.40 s |
| End offset | 830,336 ms from the complete 1,005.636-second original |
| Playback speed | 0.69 after trimming |
| Playable duration | 250.290 s, about 4:10 |
| Gain | 1.0 |

Signal-level inspection preserves the final resonance; the last 50 ms windows before the selected end are below -85 dBFS RMS. Chroma-based dynamic alignment of corresponding musical phrases against the Brian M. Jones original estimates speed 0.688824 when that reference plays at its existing 0.833333 speed. Rounding to 0.69 approximates the musical pulse instead of equating total recording lengths. Rubato and ending length differ between performances. This is a measured initial tempo proposal, not an independent listening-quality approval.

Candidate `candidate-faure-berceuse-piano-duet-517196` is added to Preview's downloadable **Vorauswahl**, with explicit recording grant, both performers, copyright notice, original source, license link and modification notes. The existing production Berceuse and its child-collection order remain unchanged. Final tempo, sound quality and transitions await the maintainer's audition. This brings Preview to 51 downloadable auditions, 16 external references and 71 generated candidate audit records; production remains 138 unique tracks.
