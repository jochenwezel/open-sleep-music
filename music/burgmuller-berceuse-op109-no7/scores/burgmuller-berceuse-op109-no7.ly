\version "2.26.0"
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
    \fill-line { \tiny "BastienM / Open Sleep Music contributors | CC BY-SA 3.0" \fromproperty #'page:page-number-string }
    \fill-line { \tiny \with-url #"https://creativecommons.org/licenses/by-sa/3.0/" "https://creativecommons.org/licenses/by-sa/3.0/" \tiny \with-url #"https://commons.wikimedia.org/wiki/File:Berceuse_Burgmuller.ogg" "Source recording: Wikimedia Commons" }
  }
  evenFooterMarkup = \oddFooterMarkup
}
\header {
  title = "Berceuse op. 109 Nr. 7"
  composer = "Friedrich Burgmüller"
  arranger = "MIDI-Bearbeitung: Open Sleep Music contributors"
  subsubtitle = \markup \center-column { "Automatische Arbeitsausgabe - Tonhöhen und Rhythmik musikalisch prüfen" "Einschlaftempo: Achtel ca. 80" }
  copyright = \markup \tiny "https://creativecommons.org/licenses/by-sa/3.0/"
  tagline = ##f
}
global = {
  \time 3/8
  \key f \major
  \tempo "Originaltempo ca." 8 = 96
\mark \markup \box "1"
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
\mark \markup \box "9"
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
\mark \markup \box "17"
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
\mark \markup \box "25"
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
\mark \markup \box "33"
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
\mark \markup \box "41"
s1*3/8 |
\bar "|."
}
melody = {
a'4 c''16 c''16 | % 1
f''8~ f''8 a'8 | % 2
c''4 g'8 | % 3
a'8~ a'8 f'8 | % 4
a'4 c''16 c''16 | % 5
a''8~ a''8 f''8 | % 6
e''4 d''16 c''16 | % 7
f''8~ f''4 | % 8
a'4 c''16 c''16 | % 9
f''8~ f''8 a'8 | % 10
c''4 g'8 | % 11
a'8~ a'8 f'8 | % 12
a'4 c''16 c''16 | % 13
a''8~ a''8 f''8 | % 14
e''4 d''16 c''16 | % 15
f''8~ f''4 | % 16
f''4 d''8 | % 17
c''8~ c''8 a'8 | % 18
a'4 a'16 a'16 | % 19
<bes' c''>8~ <bes' c''>8 a'16 f'16 | % 20
f''4 e''16 d''16 | % 21
c''8~ c''8 f''8 | % 22
<e'' f''>16 r8. r16 e''16 | % 23
f''8~ f''4 | % 24
f''4 d''8 | % 25
c''8~ c''8 a'8 | % 26
a'4 a'16 r16 | % 27
<a' bes' c''>8~ <a' bes' c''>8 a'16 <f' f''>16 | % 28
f''4 e''16 d''16 | % 29
c''8~ c''8 f''8 | % 30
e''16 r8. r16 e''16 | % 31
f''8~ f''8 a'16 bes'16 | % 32
c''4 g'8 | % 33
a'8~ a'8. bes'16 | % 34
c''4 g'8 | % 35
a'8~ a'8 f'16 f'16 | % 36
a'4 f'16 f'16 | % 37
a'8~ a'4 | % 38
bes'16 a'16 g'16 f'16~ f'8 | % 39
f'8~ f'4~ | % 40
f'16 f'8.~ f'8\fermata | % 41
}
upper = {
s16 f'16 c'8 c'8 | % 1
s16 a'16 f'4 | % 2
s16 e'16 c'8~ c'8 | % 3
s16 f'16 c'8. c'16 | % 4
s8 <c' f'>8~ <c' f'>8 | % 5
s16 c''16 a'16 c'8. | % 6
s16 bes'16 e'8~ e'8 | % 7
s16 a'16 f'4 | % 8
s16 f'16 c'8 s8 | % 9
s16 a'16 f'4 | % 10
s16 e'16 c'8~ c'8 | % 11
s16 f'16 c'4 | % 12
s16 f'16 c'8 s8 | % 13
s16 c''16 a'16 c'8. | % 14
s16 bes'16 e'8~ e'8 | % 15
s16 a'16 f'4~ | % 16
f'16 s16 <f' bes'>16 d'16~ d'8 | % 17
s16 a'16 f'16 c'8. | % 18
s16 g'16 cis'8~ cis'8 | % 19
s16 f'16 <d' d''>4 | % 20
s16 aes'16 f'16 d'16~ d'8 | % 21
s16 a'16 f'16 c'8. | % 22
s16 <ees'' e''>16 <e'' d'''>8~ <e'' d'''>8 | % 23
s16 a'16 f'4 | % 24
s16 bes'16 f'16 d'16~ d'8 | % 25
s16 a'16 f'16 c'8. | % 26
s8 g'16 cis'16~ cis'8 | % 27
s16 f'16 d'8 s8 | % 28
s16 <f' aes'>16 f'16 d'16~ d'8 | % 29
s16 a'16 f'16 c'8. | % 30
s16 <c' ees'' e''>16 <e'' d'''>8~ <e'' d'''>8 | % 31
s16 a'16 f'4 | % 32
f'8 <c' e'>8~ <c' e'>8 | % 33
s16 f'16 c'16 f'16 c'8 | % 34
s16 e'16 c'8~ c'8 | % 35
s16 f'16 c'4 | % 36
s16 f'16 c'8 s16 c'16 | % 37
f'8 c'16 c'8 c'16 | % 38
d'16 c'16 s8 d'8 | % 39
c'8 s4 | % 40
c'4.\fermata | % 41
}
lower = {
r8. a16 f16 c16 | % 1
r8 r16 a16 f16 c16 | % 2
r8. bes16 e16 c16 | % 3
r8 r16 a16 f16 c16 | % 4
c16 r8 a16 f16 c16 | % 5
r8 r8 f16 c16 | % 6
r8. bes16 g16 c16 | % 7
r8 r16 <f a>16 c8 | % 8
r8. a16 f16 c16 | % 9
r8 r16 a16 f16 c16 | % 10
r8. bes16 e16 c16~ | % 11
c8~ c16 a16 f16 c16 | % 12
r8. a16 f16 c16 | % 13
r8 r8 f16 c16 | % 14
r8. bes16 g16 <c bes>16 | % 15
r8 r16 a16 f16 <c a>16 | % 16
bes4 bes16 f16 | % 17
r8 r8 a16 f16 | % 18
e8. a16 g16 e16 | % 19
d8~ d16 a16 <d f>8 | % 20
r4 aes16 f16 | % 21
c8~ c8 a16 f16 | % 22
c8. bes16 g16 c16~ | % 23
c8 r16 a16 f16 c16 | % 24
bes8 r8 bes16 f16 | % 25
r8 r8 a16 f16 | % 26
e8. a16 g16 e16 | % 27
d8~ d16 a16 <d f>8~ | % 28
<d f>4 aes16 f16 | % 29
c8~ c8 <c a>16 f16 | % 30
c8 c16 bes16 g16 <c bes>16~ | % 31
<c bes>8 r16 a16 f16 c16 | % 32
r8. <c bes>16 f16 c16~ | % 33
c8~ c16 <f a>16 c8~ | % 34
c16 r8 bes16 g16 <c bes>16 | % 35
f8~ f16 a16 <c f>8 | % 36
c8. a16 <c f>8 | % 37
c16 r16 c16 f16 a8 | % 38
r8 bes16 a16 bes8 | % 39
a8 <g bes>4 | % 40
<c f a>4.\fermata | % 41
}
bass = {
f,4. | % 1
f,8~ f,4 | % 2
c,4. | % 3
f,8~ f,8 f,16 f,16 | % 4
f,4. | % 5
f,8~ f,4 | % 6
c,4. | % 7
f,8~ f,4 | % 8
f,4. | % 9
f,8~ f,4 | % 10
c,4. | % 11
f,8~ f,4 | % 12
f,4 f,8 | % 13
f,8~ f,8 f,8 | % 14
c,16 s8. s8 | % 15
f,8~ f,4 | % 16
bes,4. | % 17
f,8~ f,4 | % 18
s1*3/8 | % 19
s1*3/8 | % 20
b,4 s8 | % 21
s1*3/8 | % 22
s1*3/8 | % 23
f,8~ f,4 | % 24
bes,4 bes,8 | % 25
f,8~ f,4 | % 26
s8. a,16~ a,16 s16 | % 27
s1*3/8 | % 28
b,8 s8 s8 | % 29
s1*3/8 | % 30
s1*3/8 | % 31
f,8~ f,8 f,8 | % 32
f,4. | % 33
f,8 f,4 | % 34
f,4. | % 35
f,8~ f,4 | % 36
f,8 f,8~ f,8 | % 37
f,8~ f,4 | % 38
s1*3/8 | % 39
s1*3/8 | % 40
f,4.\fermata | % 41
}
violin = {
c'4.~\pp ^\markup \italic "dolce" | % 1
c'8~ c'4 | % 2
e'4. | % 3
c'8~ c'4~ | % 4
c'4.~ | % 5
c'8~ c'4 | % 6
e'4. | % 7
c'8~ c'4~ | % 8
c'4.~ | % 9
c'8~ c'4 | % 10
e'4. | % 11
c'8~ c'4~ | % 12
c'4.~ | % 13
c'8~ c'4 | % 14
e'4. | % 15
c'8~ c'4 | % 16
d'4. | % 17
c'8~ c'4 | % 18
cis'4. | % 19
f'8~ f'4 | % 20
d'4. | % 21
f'8~ f'4 | % 22
e'4. | % 23
c'8~ c'4 | % 24
d'4. | % 25
c'8~ c'4 | % 26
cis'4. | % 27
f'8~ f'4 | % 28
d'4. | % 29
f'8~ f'4 | % 30
e'4. | % 31
c'8~ c'4 | % 32
e'4. | % 33
c'8~ c'4 | % 34
e'4. | % 35
c'8~ c'4~ | % 36
c'4.~ | % 37
c'8~ c'4 | % 38
f'16 c'16 d'16 c'16 f'8 | % 39
c'8 f'4 | % 40
c'4.\fermata | % 41
}
viola = {
a4.~\pp ^\markup \italic "dolce" | % 1
a8~ a4 | % 2
bes4. | % 3
a8~ a4~ | % 4
a4.~ | % 5
a8~ a4 | % 6
bes4. | % 7
a8~ a4~ | % 8
a4.~ | % 9
a8~ a4 | % 10
bes4. | % 11
a8~ a4~ | % 12
a4.~ | % 13
a8~ a4 | % 14
bes4. | % 15
a8~ a4 | % 16
bes4. | % 17
a8~ a4~ | % 18
a4.~ | % 19
a8~ a4 | % 20
aes4. | % 21
a8~ a4 | % 22
bes4. | % 23
a8~ a4 | % 24
bes4. | % 25
a8~ a4~ | % 26
a4.~ | % 27
a8~ a4 | % 28
aes4. | % 29
a8~ a4 | % 30
bes4. | % 31
a8~ a4 | % 32
bes4. | % 33
a8~ a4 | % 34
bes4. | % 35
a8~ a4~ | % 36
a4.~ | % 37
a8~ a4 | % 38
bes16 a16 bes16 a16 bes8 | % 39
a8 bes4 | % 40
a4.\fermata | % 41
}
cello = {
f4.~\pp ^\markup \italic "dolce" | % 1
f8~ f4 | % 2
c4. | % 3
f8~ f4~ | % 4
f4.~ | % 5
f8~ f4 | % 6
c4. | % 7
f8~ f4~ | % 8
f4.~ | % 9
f8~ f4 | % 10
c4. | % 11
f8~ f4~ | % 12
f4.~ | % 13
f8~ f4 | % 14
c4. | % 15
f8~ f4~ | % 16
f4.~ | % 17
f8~ f4 | % 18
e4. | % 19
d8~ d4 | % 20
f4. | % 21
c8~ c4~ | % 22
c4. | % 23
f8~ f4~ | % 24
f4.~ | % 25
f8~ f4 | % 26
e4. | % 27
d8~ d4 | % 28
f4. | % 29
c8~ c4~ | % 30
c4. | % 31
f8~ f4 | % 32
c4. | % 33
f8~ f4 | % 34
c4. | % 35
f8~ f4~ | % 36
f4.~ | % 37
f8~ f4 | % 38
d16 f16 g16 f16 d8 | % 39
f8 g4 | % 40
f4.\fermata | % 41
}
pianoDynamics = {
s1*3/8\p |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8\mp |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8\p |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
s1*3/8 |
}

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
\book {
\bookOutputSuffix "piano-v1"
\header { subtitle = "Klavier solo" }
\paper { max-systems-per-page = #4 }
\score {
\pianoMusic
\layout { \context { \Score \override BarNumber.break-visibility = ##(#f #t #t) } }
}
\markup \column { \small "Rhythmisch quantisiert; Handverteilung und Notenlängen redaktionell geglättet." \small "Pedal nach Harmonie; Anschlag und Pedal im MIDI automatisch geschätzt." \small "Wiederholungen ausgeschrieben. Beide Tempi verwenden dieselben Noten." }
}
\book {
\bookOutputSuffix "piano-strings-v1"
\header { subtitle = "Klavier mit Streichern - Partitur" }
\paper { max-systems-per-page = #2 }
\score {
<<
\new Staff \with { instrumentName = "Violine" shortInstrumentName = "Vl." } << { \clef treble \global } { \violin } >>
\new Staff \with { instrumentName = "Viola" shortInstrumentName = "Va." } << { \clef alto \global } { \viola } >>
\new Staff \with { instrumentName = "Cello" shortInstrumentName = "Vc." } << { \clef bass \global } { \cello } >>
\pianoMusic
>>
\layout { \context { \Score \override BarNumber.break-visibility = ##(#f #t #t) } }
}
\markup \column { \small "Rhythmisch quantisiert; Handverteilung und Notenlängen redaktionell geglättet." \small "Pedal nach Harmonie; Anschlag und Pedal im MIDI automatisch geschätzt." \small "Wiederholungen ausgeschrieben. Beide Tempi verwenden dieselben Noten." }
}
\book {
\bookOutputSuffix "violin-v1"
\header { subtitle = "Violine - Einzelstimme" }
\paper { max-systems-per-page = #5 }
\score {
\new Staff \with { instrumentName = "Violine" shortInstrumentName = "Violine" } << { \clef treble \global } { \violin } >>
\layout { \context { \Score \override BarNumber.break-visibility = ##(#f #t #t) } }
}
\markup \column { \small "Rhythmisch quantisiert; Handverteilung und Notenlängen redaktionell geglättet." \small "Pedal nach Harmonie; Anschlag und Pedal im MIDI automatisch geschätzt." \small "Wiederholungen ausgeschrieben. Beide Tempi verwenden dieselben Noten." }
}
\book {
\bookOutputSuffix "viola-v1"
\header { subtitle = "Viola - Einzelstimme" }
\paper { max-systems-per-page = #5 }
\score {
\new Staff \with { instrumentName = "Viola" shortInstrumentName = "Viola" } << { \clef alto \global } { \viola } >>
\layout { \context { \Score \override BarNumber.break-visibility = ##(#f #t #t) } }
}
\markup \column { \small "Rhythmisch quantisiert; Handverteilung und Notenlängen redaktionell geglättet." \small "Pedal nach Harmonie; Anschlag und Pedal im MIDI automatisch geschätzt." \small "Wiederholungen ausgeschrieben. Beide Tempi verwenden dieselben Noten." }
}
\book {
\bookOutputSuffix "cello-v1"
\header { subtitle = "Cello - Einzelstimme" }
\paper { max-systems-per-page = #5 }
\score {
\new Staff \with { instrumentName = "Cello" shortInstrumentName = "Cello" } << { \clef bass \global } { \cello } >>
\layout { \context { \Score \override BarNumber.break-visibility = ##(#f #t #t) } }
}
\markup \column { \small "Rhythmisch quantisiert; Handverteilung und Notenlängen redaktionell geglättet." \small "Pedal nach Harmonie; Anschlag und Pedal im MIDI automatisch geschätzt." \small "Wiederholungen ausgeschrieben. Beide Tempi verwenden dieselben Noten." }
}
