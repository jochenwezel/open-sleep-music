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
    \fill-line { \tiny "Brian M. Jones / Open Sleep Music contributors | CC BY 3.0" \fromproperty #'page:page-number-string }
    \fill-line { \tiny \with-url #"https://creativecommons.org/licenses/by/3.0/" "https://creativecommons.org/licenses/by/3.0/" \tiny \with-url #"https://commons.wikimedia.org/wiki/File:Berceuse_by_Gabriel_Faur%C3%A9_op56_no1.ogg" "Source recording: Wikimedia Commons" }
  }
  evenFooterMarkup = \oddFooterMarkup
}
\header {
  title = "Berceuse op. 56 Nr. 1"
  composer = "Gabriel Fauré"
  arranger = "MIDI-Bearbeitung: Open Sleep Music contributors"
  subsubtitle = \markup \center-column { "Automatische Arbeitsausgabe - Tonhöhen und Rhythmik musikalisch prüfen" "Einschlaftempo: Viertel ca. 39" }
  copyright = \markup \tiny "https://creativecommons.org/licenses/by/3.0/"
  tagline = ##f
}
global = {
  \time 2/4
  \key e \major
  \tempo "Originaltempo ca." 4 = 47
\mark \markup \box "1"
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
\mark \markup \box "9"
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
\mark \markup \box "17"
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
\mark \markup \box "25"
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
\mark \markup \box "33"
s1*2/4 |
s1*2/4 |
\key c \major
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
\mark \markup \box "41"
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
\mark \markup \box "49"
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
\mark \markup \box "57"
s1*2/4 |
s1*2/4 |
\key e \major
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
\mark \markup \box "65"
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
\mark \markup \box "73"
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
\mark \markup \box "81"
s1*2/4 |
s1*2/4 |
s1*2/4 |
\bar "|."
}
melody = {
R1*2/4 | % 1
R1*2/4 | % 2
<b' b''>4~ <b' b''>16 <b' b''>8. | % 3
r16 gis'8 <gis' gis''>16~ <gis' gis''>16 <a' a''>8 <b' b''>16~ | % 4
<b' b''>16 <gis' gis''>8 <e' e''>16~ <e' e''>16 <fis' fis''>8 gis''16 | % 5
<e' e''>8 e''16 r16 r16 b'8. | % 6
b'8 r16 b''16 r16 <b' b''>8. | % 7
r16 gis'8 <gis' gis''>16~ <gis' gis''>16 <a' a''>8 <b' b''>16~ | % 8
<b' b''>16 <gis' gis''>16 <e' e''>8~ <e' e''>16 <e'' e'''>16 <dis'' dis'''>8 | % 9
<cis'' cis'''>4 b'8. fis''16 | % 10
fis''8 fis''8 <fis'' fis'''>4~ | % 11
<fis'' fis'''>8 <dis'' dis'''>8 <e'' e'''>8 <fis'' fis'''>8 | % 12
<dis'' dis'''>8 <b' b''>8 cis''8 <dis'' dis'''>16 <b' b''>16~ | % 13
<b' b''>4 fis''4 | % 14
<fis'' fis'''>8. fis''16 <fis'' fis'''>4~ | % 15
<fis'' fis'''>8 <gis'' gis'''>8 <e'' e'''>8 <cis'' cis'''>8 | % 16
<e'' e'''>8 <fis'' fis'''>8 <dis'' dis'''>8 <b' b''>8 | % 17
dis'''16 dis''8. <cis'' cis'''>4 | % 18
b'8 r8 b'4~ | % 19
b'16 gis'16 r16 <gis' gis''>16 r16 <a' a''>16 <b' gis'' b''>16 <gis' b' gis''>16 | % 20
<gis' gis''>16 gis'16 <e' e''>8 <fis' fis''>8. <gis' gis''>16 | % 21
<e' e''>8. r16 r16 b'8. | % 22
b'8 r8 r16 b'8.~ | % 23
b'8 r16 <gis' gis''>16~ <gis' gis''>16 <a' a''>8 <b' b''>16~ | % 24
<b' b''>16 <gis' gis''>16 <e' e''>8 <e'' e'''>8 <dis'' dis'''>8 | % 25
<cis'' cis'''>8. r16 <b' b''>4 | % 26
<e'' e'''>4 r16 e''8. | % 27
r8. <cis'' cis'''>16~ <cis'' cis'''>16 <b' b''>8 <a' a''>16~ | % 28
<a' a''>16 <e'' e'''>8. r16 <e'' e'''>8.~ | % 29
<e'' e'''>8 <cis'' cis'''>8 <b' b''>8 <a' a''>16 <b' b''>16~ | % 30
<b' b''>4 b''16 b'8 r16 | % 31
gis'16 gis'16 <gis' gis''>16 fis''16~ fis''16 e''16 e'16 <gis' gis''>16~ | % 32
<gis' gis''>8 gis'8 fis''4 | % 33
e''16 <e' e''>8. r4 | % 34
e''4 <e' e''>8 r8 | % 35
r8 c''8 d''8 <e' e''>16 e'16 | % 36
e''4 <c' c''>16 c'16 r8 | % 37
r8 <g g'>16 g16 a'16 c'16 <c' c''>8 | % 38
<b b'>8. d''16~ d''4~ | % 39
d''8 e'16 e''16~ e''16 <g' g''>8 r16 | % 40
r8 <bes bes'>16 <bes bes'>16 d''8 g''16 g'16 | % 41
f''8 e''8 d''8 e''8~ | % 42
e''16 f''8. <f' f''>8. r16 | % 43
r8 d''8 e''8 f''16 <f' f''>16 | % 44
<f' f''>8. r16 d''16 <d' d''>8. | % 45
r8 <a a'>16 <a a'>16 <b b'>16 <b d'>16 <d' d''>8 | % 46
c''8 <e' e''>16 a'16 a''4~ | % 47
a''16 f''8 <f' a''>16 a'4 | % 48
r8 c''8 <e' e''>8 <a' a''>8 | % 49
g''8 fis''8~ fis''8 fis''8 | % 50
g''4 <g' g''>8. r16 | % 51
r8 e''8 fis''8 g''16 g'16 | % 52
g''4 e''16 e'8. | % 53
r8 <b b'>16 b16 <b' cis''>16 <e' e''>8. | % 54
d''8 fis''16 b'16 <b' b''>4~ | % 55
<b' b''>8 g''16 b''16~ b''4 | % 56
r8. <fis' fis''>16 ees''4 | % 57
b'8. r16 r16 ees'8. | % 58
<b b'>8 r16 <b b'>16~ <b b'>16 b'8 r16 | % 59
r16 <gis gis'>8 <gis gis'>16~ <gis gis'>16 a8 <b b'>16 | % 60
gis'16 <gis gis'>8 e'16~ e'16 fis'8 <gis gis'>16 | % 61
e'8. r16 r8 <b b'>16 <b b'>16 | % 62
b'8 r16 b'16~ b'16 r16 <b b'>8 | % 63
r8 gis'8 r16 a'8 <b b'>16 | % 64
r16 <gis gis'>8 e'16~ e'16 fis'8 gis'16 | % 65
<e e'>4 r4 | % 66
<e' e''>8 r16 <e' e''>16~ <e' e''>16 e''8 r16 | % 67
r16 <cis' cis''>8 <cis' cis''>16~ <cis' cis''>16 <d' d''>8 <e' e''>16 | % 68
cis''16 cis'16 <a a'>8 b'8 <cis' cis''>8 | % 69
<a a'>16 e'16 e''16 <e' e''>16 dis''8 cis''8 | % 70
<b b'>8 r8 r16 cis''8. | % 71
r16 d''8 r16 r16 dis''8. | % 72
r16 b'8 r16 b''4~ | % 73
b''8. a''16~ a''8. <b' gis'' b''>16~ | % 74
<b' gis'' b''>16 <gis' gis''>8 <e' e''>16 fis''8 <gis' gis''>8 | % 75
e''4 r16 <b b'>8. | % 76
b'4~ b'16 r8. | % 77
r16 gis'16 gis'8~ gis'8. <gis' b'>16~ | % 78
<gis' b'>16 e'8 fis'16~ fis'16 gis'8 r16 | % 79
e'4 b8. r16 | % 80
e'4 b4 | % 81
e'8. b16~ b8.\fermata r16 | % 82
R1*2/4 | % 83
}
upper = {
s8 e'16 cis'16~ cis'16 dis'8.~ | % 1
dis'8 e'16 cis'16 s16 dis'8. | % 2
gis'16 gis'16 b'16 <e' e''>16 <cis' cis''>16 <dis' dis''>8 <b' gis''>16~ | % 3
<b' gis''>8 b'16 <e' e''>16 cis'16 dis'8 s16 | % 4
s8. gis'16 cis'16 <dis' dis''>8. | % 5
s8 b'16 <cis' e' cis'' e''>16 gis'16 <dis' dis''>8. | % 6
<gis' gis''>16 gis'16 b'16 <e' e''>16 <cis' cis''>16 <dis' dis'' e''>8. | % 7
b'8. <e' e''>16 cis'16 <dis' dis''>8.~ | % 8
<dis' dis''>8 gis'16 cis'16~ cis'8 <e' b''>8 | % 9
s16 <e' e''>16 <fis' e''>16 <e' e''>16 <dis' dis'' fis''>8. <fis' b'>16 | % 10
<fis' dis''>16 <dis' dis''>16 <fis' b'>16 <gis' dis'' gis''>16 <ais' cis'' ais''>16 <cis' e'>16 <e' fis'>16 fis'16 | % 11
dis'16 fis'16 <dis' b' b''>16 <fis' gis'>16 <cis' e' cis''>8 <e' fis' ais''>16 s16 | % 12
<dis' fis'' b''>16 fis'16 dis''16 gis'16 <cis' e' fis' ais'>8 b''16 fis''16 | % 13
dis'16 <dis' fis'>16 <dis' b'>16 gis'16 <cis' e' ais'>4 | % 14
s16 <dis' dis''>16 <dis' fis' b'>16 <dis' fis' cis'''>16 s8 <cis' fis'>8 | % 15
fis'16 <cis' gis'>16 <b' b''>16 <fis' gis'>16 <cis' e' gis'' b'' cis'''>8 <cis' gis'>8 | % 16
<cis' e' fis' ais''>16 <cis' fis'>16 <cis' e' cis'' cis'''>16 <cis' fis'>16 <dis' ais'' b''>8 <dis' fis' fis''>16 <dis' fis''>16 | % 17
<fis' b''>16 dis'16 <dis' fis'>16 dis'16 <dis' e' dis''>8 <cis' gis'>8 | % 18
s16 a'16 b'16 <cis' a'>16 s16 fis'8.~ | % 19
fis'8 b'16 <cis' cis'' e''>16 s16 <dis' fis''>8.~ | % 20
<dis' fis''>16 b'8 cis'16 <dis' dis''>4 | % 21
<gis' b'>16 gis'16 b'16 <e' e''>16 cis'16 <dis' dis''>8.~ | % 22
<dis' dis''>16 gis'16 b'16 <e' e''>16 <cis' cis''>16 dis'8.~ | % 23
dis'8 b'16 <e' e''>16 cis'16 <dis' fis''>8.~ | % 24
<dis' fis''>8. cis'16 ais''8 <cis' e' gis' b''>8 | % 25
s16 <e' e''>16 <e' fis' e'' fis''>16 s16 <dis' dis''>8 b'8 | % 26
<e' ais''>16 b'16 <dis' fis' dis'' fis''>16 <dis' dis''>16 s16 <d' gis'>8 <e' b'>16 | % 27
<e' b' e''>16 cis'16 <cis' f' f''>16 <cis' f' gis' e'' f''>16~ <cis' f' gis' e'' f''>16 <cis' e' fis' e'' fis''>8 <cis' fis''>16 | % 28
<e' b'>16 <d' d''>16 g'16 a'16 <g' g''>16 <d' gis' gis''>8 e'16 | % 29
cis'16 <d' e' gis' e''>16 b'16 <gis' gis''>16 <cis' e' a' cis'' e'' a''>8 <cis' e''>16 cis'16 | % 30
<fis' a' fis''>16 <ais' fis''>16 <cis' cis'' fis''>16 <ais' fis''>16 s16 <dis' a'>8 b'16 | % 31
<e' b'>8 <b' gis'''>16 <cis' e' fis'''>16~ <cis' e' fis'''>16 <gis' cis'' gis'' cis''' e'''>16 cis'16 <cis' cis'' cis''' gis'''>16 | % 32
<cis' cis''>4 <dis' ais'' dis''' fis'''>4 | % 33
<gis' e'''>16 gis'16 b'16 <e' gis'>16 <gis' b'>8 gis'8 | % 34
s8 g'8~ g'16 <c' g'>16 <e' e''>8 | % 35
<e' c'' e''>8. a'16 <e' e''>16 c'16 a'8 | % 36
g'8 g'8 e'8 <c' e' g'>16 <c' e'>16 | % 37
<d' e'>8 e'8 e'8 g'16 e'16 | % 38
g'8. b'16~ b'8 <g' g''>8 | % 39
<e' e''>16 <g' b'>16 b'16 e'''16 b'16 <e' g'''>16 b'16 <g' g''>16 | % 40
<g' b'>4 g'16 <e' e''>16 bes'8 | % 41
a'8 a'16 a'16 g'16 <cis' cis''>16 <cis' a'>16 g'16 | % 42
<d' a'>8 a'16 a'16~ a'16 <d' d''>16 a'16 <f' f''>16 | % 43
<f' bes' f''>8 f''16 bes'16 f'16 <d' d''>16 bes'8 | % 44
<a' d''>8 <a' f''>16 <f' a' f''>16 <f' f''>8 a'16 <f' f''>16 | % 45
f'4 f'8 a'16 f'16 | % 46
<e' a'>8 <a' c''>8 <e' e''>16 <c' c''>16 <c' a'>16 <e' e''>16 | % 47
<c' d' a'>16 <c' f'''>16 a'16 <d' e' a'''>16 f'16 f'8. | % 48
d'8 <f' a'>8~ <f' a'>16 c'16 <e'' f''>16 <e' e''>16 | % 49
<a' b' ees''>8 <a' b'>16 a'16 <ees' ees''>16 b'16 <a' ees''>16 <ees' ees''>16 | % 50
<e' b'>8 b'8~ b'16 <e' e''>16 b'16 <g' g''>16 | % 51
<c' c''>8 <c' c''>16 c'16 <c' g'>16 <e' e''>16 <c' c''>8 | % 52
s4 g'8 b'16 <g' g''>16~ | % 53
<g' g''>4 g'16 b'16 <g' g''>8 | % 54
s8 b'16 s16 <fis' fis''>16 <d' d''>16 b'16 fis'16 | % 55
<e' b'>16 g'''16 b'16 <e' b'''>16 <cis' cis'' cis'''>16 <g' g''>16 <cis' e' cis'' e''>8 | % 56
<aes' b' e'' aes''>16 b'8 b'16 b'16 <b' fis'' b''>16 <cis' a' a''>16 e''16 | % 57
<aes' e'' aes''>16 <fis' ees'' fis''>16 cis''16 <ees' ees''>16 a'16 b'8 fis'16 | % 58
<dis' gis'>16 gis'16 b'16 <e' gis'>16 <cis' cis''>16 <dis' dis''>8 <fis' b' dis''>16 | % 59
<gis' b'>8 b'16 <e' b'>16 cis'16 dis'8.~ | % 60
dis'8 b'8 cis'16 dis'8. | % 61
<gis' e''>16 b'16 <gis' b' e''>16 <a' dis''>16~ <a' dis''>16 b'16 gis'16 gis'16 | % 62
<e' gis'>16 cis'16 s16 dis'16~ dis'8 gis'8 | % 63
<e' gis' b'>8 e'16 cis'16 dis'4 | % 64
b'8. <gis' b'>16 cis'16 <dis' b'>8 dis'16 | % 65
gis'16 <gis' b'>16 <gis' b' e''>16 cis''16 <cis' e' gis' cis'' e''>8 <d' gis' b' d''>8 | % 66
a'16 <cis' cis''>16 <e' cis'' e''>16 <cis' a' cis''>16 fis'16 <d' gis' b'>8 e''16 | % 67
<e' e'' fis''>8 <e' e''>16 a'16 fis'16 gis'8 d'16 | % 68
e'16 <e' e''>16 <cis' cis'' e''>16 <cis' fis' fis''>16 <cis' d' e'>8 fis'8 | % 69
<cis' cis''>8 <cis' a' cis''>8 <e' fis' cis'' e''>8 <g' g''>8 | % 70
gis'16 fis'16 gis'8 e'16 gis'8. | % 71
f'16 a'16 <f' gis'>16 <fis' a' b'>16~ <fis' a' b'>16 a'16 b'8 | % 72
<e' e''>16 gis'16 <e' e''>16 <cis' cis''>16 <dis' dis''>4 | % 73
<b' e'' gis''>8 cis'16 fis''16 <dis' dis''>8. e''16~ | % 74
e''8 b'16 <cis' cis''>16 <dis' dis''>8 fis''8 | % 75
gis'8 e'16 cis'16 s16 dis'8. | % 76
gis'8. e'16 cis'16 s16 <dis' b'>8 | % 77
gis'16 s8 e'16 cis'16 dis'8.~ | % 78
dis'8 cis'16 dis'16~ dis'8. s16 | % 79
s4 a'8. s16 | % 80
s8 d'8~ d'8.\fermata s16 | % 81
s1*2/4 | % 82
s1*2/4 | % 83
}
lower = {
r16 gis16 <e gis b>8~ <e gis b>16 <fis a>8 <fis a>16 | % 1
gis16 gis16 <gis b>16 gis16 r16 <fis a>8 <fis a b>16 | % 2
b16 gis16 b8 gis16 <fis a>8 b16~ | % 3
b16 <gis a>16 b16 <e gis>16 a16 <dis fis a>8 b16 | % 4
r16 gis16 b16 <gis b>16 <gis b>16 <fis a>8 gis16 | % 5
r16 gis16 b8 r16 <fis a>8 b16 | % 6
b16 gis16 b8 gis16 <fis a>8 b16 | % 7
<e b>16 <gis b>16 b16 <e gis b>16~ <e gis b>16 <fis a>8 b16~ | % 8
b16 gis16 <e gis b>8 cis8 <e b>8 | % 9
<fis ais>8 fis8 b8. b16 | % 10
<fis b>4 fis8 fis16 <fis b>16 | % 11
<fis b>16 fis16 <fis b>16 fis16 fis8 <fis b>16 <fis b>16 | % 12
<fis b>16 <fis b>16 <fis b>16 <fis b>16 fis8 fis16 fis16 | % 13
<fis b>16 <fis b>16 <fis b>16 <fis b>16 fis8. <fis b>16 | % 14
<fis b>16 <fis b>16 <fis b>16 fis16 <fis a>8 fis8 | % 15
<fis gis>8 gis16 b16 r8 b8 | % 16
<fis b>16 b16 fis8 <fis b>8 b8 | % 17
<dis b>8 dis16 b16 <dis fis b>16 <dis b>16 b8 | % 18
<dis b>16 <dis a>16 <dis a b>16 <dis a>16 <dis a b>8. b16 | % 19
<gis b>16 gis16 b16 <e gis b>16 r16 <fis a>16 d8 | % 20
<e gis>16 b8. r16 a16 e16 <gis b>16 | % 21
r16 gis16 b8~ b16 <fis a>8.~ | % 22
<fis a>16 gis16 b8 b16 <fis a>8. | % 23
<gis b>16 <fis gis>16 b16 <e gis b>16 gis16 <fis a>8 b16~ | % 24
b16 gis16 <e gis b>8 cis8 b8 | % 25
<fis ais>8 <fis ais>16 r16 b8 b8 | % 26
e16 b8.~ b16 <e gis b>16 b16 <e b>16 | % 27
<e gis a>8. <e b>16~ <e b>16 a8 <e a b>16 | % 28
<e b>16 e16 b16 <e b>16 <e b>16 <e b>8 <e b>16 | % 29
cis16 <e b>16 <e b>16 e16 e16 r16 e16 <dis b>16 | % 30
dis16 <dis b>16 <d e>16 d16 r8. b16 | % 31
<e b>16 <e b>16 b16 r16 r16 b16 <cis gis a>8 | % 32
b8 b8 b8. b16 | % 33
gis16 gis16 <e b>16 <e b>16 b8 gis8 | % 34
c8 g16 g16~ g4 | % 35
a8 a16 a16~ a4 | % 36
c8 g16 g16 g16 g16 g16 g16 | % 37
e8 <e b>16 e16 <e g b>16 e16 <e g>16 <e g>16 | % 38
g8. <g b>16~ <g b>16 <g b>16 g16 b16 | % 39
e16 <e g b>16 <e g>16 <d e b>16 <e b>16 <e b>16 b16 b16 | % 40
<e b>8 e16 e16 <e g bes>16 <e bes>16 <e g bes>16 bes16 | % 41
r8 a16 a16 e8. a16 | % 42
d8 a16 a16~ a16 d16 <d f>16 a16 | % 43
r8 bes16 bes16~ bes8 bes16 bes16 | % 44
d8 a16 a16 a16 a16 <d f a>16 a16 | % 45
<f a>4 <f a>16 f16 <f a>16 <f a>16 | % 46
r8 a16 a16~ a8 a8 | % 47
d16 <d a>16 <d a>16 d16 <d f a>16 <d f a>8. | % 48
<d a>8 a16 a16 a8 a8 | % 49
r8 a16 a16 <ees fis>16 b8 fis16 | % 50
e8 b16 b16 e8 b16 e16 | % 51
c8 c8 c4 | % 52
e8 b16 b16 e8 b16 <e b>16 | % 53
g4 g16 <g b>16 b8~ | % 54
b8 b16 b16 <fis b>8 b16 b16 | % 55
e8 <d e b>16 e16 e16 <e b>16 e8 | % 56
<e b>16 b8 fis16 b16 b16 b8 | % 57
b16 a8 fis16 <a b>16 <ees b>8. | % 58
r16 gis16 b16 gis16~ gis16 <fis a>8 <gis b>16 | % 59
b8 b16 b16 gis16 fis8 fis16 | % 60
b8 b16 <gis b>16 gis16 <a b>8 b16 | % 61
<dis gis b>16 b16 <gis b>16 <fis a>16~ <fis a>16 b16 r16 gis16 | % 62
<e gis>8 r16 fis16~ fis16 gis8. | % 63
<e gis b>16 b16 b8~ b16 fis8. | % 64
<e b>16 b16 b16 b16~ b16 <a b>8 b16 | % 65
<gis b>16 <gis b>16 b8 e16 r16 <e b>8 | % 66
e8 e16 e16 e16 <e b>8 e16 | % 67
<e a>8 e16 <e a>16 e16 <e b>8 <e b>16 | % 68
e16 e16 e16 e16 e8 e8 | % 69
e8 e16 e16 e8 <cis ais>8 | % 70
<cis ais>16 r8 b16~ b16 b8. | % 71
<d f gis b>16 <fis b>16 f16 <a b>16~ <a b>16 b16 b8 | % 72
r16 <gis b>8 gis16 <fis a>8 b16 <gis b>16 | % 73
<e gis b>8 gis16 r16 <gis a>4~ | % 74
<gis a>16 gis16 <gis b>16 <e gis>16 <e fis a b>8 b16 <gis b>16 | % 75
<e gis>16 <gis b>16 gis16 <e gis>16 r16 <fis a>8 <gis b>16 | % 76
b16 <gis b>16 <gis b>16 <gis b>16 <gis b>16 r16 <dis fis a b>8 | % 77
<gis b>16 <gis b>16 <gis b>16 <e gis b>16 b16 <fis a>8 b16 | % 78
<gis b>16 <fis gis b>16 b16 <fis a>16~ <fis a>16 b8 r16 | % 79
b16 gis16 <e gis>8 <fis a>8 gis16 r16 | % 80
<e b>8 b16 <fis a>16~ <fis a>8 gis8 | % 81
b8 <e gis>16 fis16~ fis16 gis8. | % 82
<e gis b>2\fermata | % 83
}
bass = {
<e, b,>8. b,16 s8. b,16 | % 1
<e, b,>4 s8. b,16 | % 2
<e, b,>4~ <e, b,>16 b,8 b,16 | % 3
<e, b,>4~ <e, b,>16 b,8 b,16 | % 4
<e, b,>4 b,16 s8 b,16 | % 5
<e, b,>4 s8. b,16 | % 6
<e, b,>4~ <e, b,>16 s8 b,16 | % 7
e,16 b,8 s16 s16 b,8 b,16 | % 8
<e, b,>2 | % 9
s4. s16 b,16 | % 10
b,4 s8. b,16 | % 11
b,16 b,8. s8. b,16 | % 12
b,8 b,16 b,16 ais,16 s8 b,16~ | % 13
b,8. b,16 s4 | % 14
b,8 b,8 s4 | % 15
s1*2/4 | % 16
s1*2/4 | % 17
s1*2/4 | % 18
s4. s16 b,16 | % 19
<e, b,>8. b,16 s16 b,16 b,16 <e, gis, b,>16~ | % 20
<e, gis, b,>4 b,8. b,16 | % 21
<e, b,>4.~ <e, b,>16 b,16 | % 22
<e, b,>4~ <e, b,>16 s8. | % 23
<e, b,>4~ <e, b,>16 b,8 b,16 | % 24
<e, b,>4 s4 | % 25
s4. b,8 | % 26
gis,4 s4 | % 27
a,4~ a,16 s8. | % 28
b,4 s4 | % 29
s1*2/4 | % 30
s16 b,8 b,16 s4 | % 31
s4. a,8~ | % 32
a,4 s8. b,16 | % 33
<e, b,>16 s8. s4 | % 34
s1*2/4 | % 35
a,2 | % 36
s1*2/4 | % 37
s1*2/4 | % 38
s1*2/4 | % 39
s4 s16 b,16 s8 | % 40
e,2 | % 41
a,2 | % 42
s1*2/4 | % 43
bes,16 s8. s16 bes,8. | % 44
s1*2/4 | % 45
s1*2/4 | % 46
a,16 s8. s4 | % 47
s1*2/4 | % 48
a,4. a,8 | % 49
b,16 s16 b,16 s16 s4 | % 50
s1*2/4 | % 51
s1*2/4 | % 52
s1*2/4 | % 53
s1*2/4 | % 54
b,8 s8 s4 | % 55
s1*2/4 | % 56
b,2 | % 57
s1*2/4 | % 58
<e, gis, b,>8. b,16~ b,8. b,16 | % 59
<e, b,>8. b,16~ b,16 s8 b,16 | % 60
<e, b,>8. b,16~ b,8. b,16 | % 61
b,8 b,16 s16 s16 b,16 b,8 | % 62
b,8. b,16~ b,16 b,8. | % 63
e,8 b,8 s16 b,8 b,16 | % 64
e,16 b,8 b,16~ b,8. b,16 | % 65
<e, b,>16 b,16 b,8 s8 b,8 | % 66
a,8. a,16 a,16 s8. | % 67
a,4~ a,16 s8. | % 68
a,4 s4 | % 69
a,8 b,8 ais,8 ais,8 | % 70
b,16 s8 b,16~ b,16 fis,8. | % 71
b,16 b,8 b,16~ b,8 <b,, b,>8 | % 72
<e, b,>8. s16 s8 <e, b,>8~ | % 73
<e, b,>8. s16 s8. b,16 | % 74
<e, b,>4. b,16 <e, b,>16~ | % 75
<e, b,>4 s8. b,16 | % 76
<e, b,>8 b,16 b,16~ b,4 | % 77
b,16 b,16 b,16 b,16~ b,16 b,8 b,16 | % 78
b,16 b,16 b,16 s16 s16 b,8 s16 | % 79
<e, b,>8 b,8~ b,8. s16 | % 80
e,8 <e, b,>8~ <e, b,>4 | % 81
e,8 b,16 s16 s16 b,8. | % 82
<e, b,>2\fermata | % 83
}
violin = {
r8 e'8\pp ^\markup \italic "dolce" r4 | % 1
r8 e'8 r4 | % 2
r16 gis'8.~ gis'16 dis'8.~ | % 3
dis'4~ dis'16 r8 b'16~ | % 4
b'4~ b'16 dis'8. | % 5
r8 b'8 r16 b'8. | % 6
r16 gis'8.~ gis'16 dis'8.~ | % 7
dis'4~ dis'16 a'8.~ | % 8
a'4 r16 e'8. | % 9
cis'4 b'4~ | % 10
b'4 ais'4~ | % 11
ais'4 e'8. r16 | % 12
r16 fis'8. cis'4~ | % 13
cis'4 ais'4 | % 14
r16 dis'8. r4 | % 15
fis'8. r16 e'4~ | % 16
e'8. r16 dis'4 | % 17
r16 dis'8.~ dis'4 | % 18
r8 b'8~ b'4~ | % 19
b'4 r16 a'16 r8 | % 20
r16 b'8. dis'4 | % 21
r16 gis'8.~ gis'16 dis'8.~ | % 22
dis'4 r16 b'8.~ | % 23
b'4~ b'16 a'8.~ | % 24
a'4 e'4 | % 25
cis'8. r16 b'4~ | % 26
b'4 r16 e'8. | % 27
b'4~ b'16 e'8.~ | % 28
e'4 r16 e'8.~ | % 29
e'4 b'4~ | % 30
b'4 r16 a'8.~ | % 31
a'8. e'16~ e'4~ | % 32
e'4 r4 | % 33
e'4 r8 gis'8 | % 34
e'4. r8 | % 35
e'4 d'4 | % 36
g'8 r8 c'4~ | % 37
c'4 a'4 | % 38
g'4. r8 | % 39
e'2~ | % 40
e'4 d'4 | % 41
r8 e'8~ e'4~ | % 42
e'4. r8 | % 43
f'4 e'4 | % 44
d'4 f'8. r16 | % 45
f'4 b'4 | % 46
c'2~ | % 47
c'8. e'16~ e'4 | % 48
r8 c'8~ c'4 | % 49
r8 b'8~ b'4 | % 50
e'2 | % 51
r8 e'8~ e'4 | % 52
r4 g'4~ | % 53
g'4 r16 e'8. | % 54
d'2~ | % 55
d'8 b'8~ b'4 | % 56
e'8. r16 ees'4 | % 57
aes'8. ees'16~ ees'16 r8 fis'16~ | % 58
fis'4~ fis'16 b'8 fis'16~ | % 59
fis'4~ fis'16 r8 b'16~ | % 60
b'4~ b'16 r8. | % 61
e'8. dis'16~ dis'4 | % 62
r8. b'16~ b'16 r8. | % 63
b'4 r16 a'8. | % 64
b'8. e'16~ e'16 fis'8.~ | % 65
fis'8 e'8~ e'4 | % 66
a'4~ a'16 b'8.~ | % 67
b'4~ b'16 gis'8.~ | % 68
gis'4 e'4~ | % 69
e'16 r16 cis'8 fis'4~ | % 70
fis'2 | % 71
f'8 r16 b'16~ b'4~ | % 72
b'8. r16 dis'4~ | % 73
dis'8. r16 r8. e'16~ | % 74
e'2~ | % 75
e'4 r16 b'8.~ | % 76
b'4~ b'16 r16 b'8~ | % 77
b'16 r16 gis'8~ gis'4~ | % 78
gis'8. fis'16~ fis'8. r16 | % 79
e'4 r4 | % 80
e'4~ e'16 r8. | % 81
e'8.\fermata r16 r4 | % 82
R1*2/4 | % 83
}
viola = {
r8 b8\pp ^\markup \italic "dolce" r16 dis'8.~ | % 1
dis'4 r16 dis'8. | % 2
r8 b8~ b16 dis'8.~ | % 3
dis'4~ dis'16 a8. | % 4
r16 gis8 r16 r16 dis'8. | % 5
r16 gis8. r16 dis'8. | % 6
r8 b8~ b16 dis'8.~ | % 7
dis'8. r16 r16 dis'8. | % 8
r16 gis8.~ gis4 | % 9
ais4 dis'4~ | % 10
dis'4 r16 cis'8.~ | % 11
cis'4.~ cis'16 r16 | % 12
dis'4 cis'4~ | % 13
cis'2 | % 14
r4 a4~ | % 15
a4 e'4~ | % 16
e'8 r16 cis'16~ cis'4 | % 17
b8. r16 dis'4 | % 18
r8 b8 a4 | % 19
r4 r16 a8 r16 | % 20
gis4 dis'4 | % 21
r16 gis8. r16 dis'8.~ | % 22
dis'4 r16 dis'8.~ | % 23
dis'4~ dis'16 a8.~ | % 24
a16 r16 b8 r8 e'8 | % 25
ais8. r16 b4~ | % 26
b4~ b16 gis8. | % 27
e'2~ | % 28
e'4~ e'16 d'8.~ | % 29
d'4 e'4~ | % 30
e'8. r16 r16 dis'8.~ | % 31
dis'8. cis'16~ cis'8 gis8~ | % 32
gis4 dis'4 | % 33
r16 gis8 b16~ b4 | % 34
r8. g16~ g4~ | % 35
g8. a16~ a4 | % 36
R1*2/4 | % 37
r4 b4 | % 38
R1*2/4 | % 39
e'8. r16 r16 e'8.~ | % 40
e'4 r16 e'8. | % 41
r8. a16~ a8. r16 | % 42
d'2 | % 43
f'4.~ f'16 r16 | % 44
f'8. r16 f'8. r16 | % 45
f'4 a8 d'8 | % 46
r8. a16~ a8 c'8 | % 47
d'2 | % 48
r8 f'8~ f'4 | % 49
r8. a16~ a4 | % 50
e'2 | % 51
r4 r16 e'8. | % 52
r4 r16 e'8. | % 53
g4 r16 e'8. | % 54
r4 r16 d'8.~ | % 55
d'8 b8~ b4 | % 56
r16 b8. r8 cis'8~ | % 57
cis'8. ees'16~ ees'4~ | % 58
dis'2~ | % 59
dis'4 r16 dis'8.~ | % 60
dis'4 r16 dis'8.~ | % 61
dis'8. a16~ a4 | % 62
r8. dis'16~ dis'4~ | % 63
dis'8 r8 dis'4 | % 64
r16 gis8.~ gis16 a8 r16 | % 65
e'4 r8 d'8~ | % 66
d'4~ d'16 b8.~ | % 67
b2~ | % 68
b4 e'4~ | % 69
e'16 r16 cis'8 e'4~ | % 70
e'8 r8 e'4 | % 71
gis8. a16~ a8 b8 | % 72
r16 b8 r16 dis'4~ | % 73
dis'8. r16 a4~ | % 74
a16 r16 b8~ b8 r16 b16~ | % 75
b4 r16 dis'8.~ | % 76
dis'4~ dis'16 r16 dis'8~ | % 77
dis'16 r8 e'16~ e'4~ | % 78
e'16 r16 cis'8~ cis'8. r16 | % 79
e'4 b8. r16 | % 80
b8. r16 b4 | % 81
e'8. b16~ b8. r16 | % 82
gis8.\fermata r16 r4 | % 83
}
cello = {
e4\pp ^\markup \italic "dolce" r16 fis8.~ | % 1
fis4 r16 fis8 r16 | % 2
e4~ e16 fis8.~ | % 3
fis2 | % 4
b4 r4 | % 5
e4 r16 fis8. | % 6
b4~ b16 fis8.~ | % 7
fis8. r16 r16 fis8.~ | % 8
fis4 cis4 | % 9
fis2~ | % 10
fis4. r8 | % 11
r16 b8. fis8 r8 | % 12
r4 fis8 r16 b16~ | % 13
b8. r16 fis8. r16 | % 14
r8 b8 fis8 r8 | % 15
fis4~ fis16 r8. | % 16
r8 fis8~ fis4 | % 17
dis4 fis4 | % 18
R1*2/4 | % 19
e4 r16 b16 r16 e16~ | % 20
e4 b8. r16 | % 21
b4.~ b16 r16 | % 22
e4~ e16 fis8.~ | % 23
fis2~ | % 24
fis16 r16 e8~ e4 | % 25
fis8 r8 r8 b8~ | % 26
b4 r16 e8 r16 | % 27
a4~ a16 r8. | % 28
b4 r16 e8 r16 | % 29
cis4 r8 e8~ | % 30
e4 r4 | % 31
r16 e8 r16 r8 a8~ | % 32
a4 r4 | % 33
b16 r8. r4 | % 34
c2 | % 35
a2 | % 36
c2 | % 37
R1*2/4 | % 38
R1*2/4 | % 39
r8. d16~ d16 e8.~ | % 40
e2 | % 41
a2 | % 42
d4~ d16 r8. | % 43
r4 r16 bes8. | % 44
d4. r8 | % 45
f4 r4 | % 46
R1*2/4 | % 47
r8. d16 r16 d8.~ | % 48
d2 | % 49
r4 ees4 | % 50
e4.~ e16 r16 | % 51
r4 c4 | % 52
e4 r4 | % 53
R1*2/4 | % 54
b8 r8 fis4~ | % 55
fis8 r8 r8 e8 | % 56
b2 | % 57
r8. fis16~ fis16 ees8. | % 58
r8. b16~ b8. r16 | % 59
e4~ e16 r8. | % 60
e2 | % 61
dis8. fis16~ fis8 r8 | % 62
r8. b16~ b4~ | % 63
b4 r16 b8 r16 | % 64
e2~ | % 65
e8 b8 r8 b8~ | % 66
b4 r4 | % 67
a4~ a16 e8 r16 | % 68
a4 e4 | % 69
a4 r8 ais8~ | % 70
ais16 r8 b16~ b4 | % 71
d8. b16~ b4~ | % 72
b8. r16 fis4~ | % 73
fis8. r16 r4 | % 74
b4. r16 e16~ | % 75
e4 r16 fis8.~ | % 76
fis4~ fis16 r16 dis8~ | % 77
dis16 r8 b16~ b16 fis8.~ | % 78
fis16 r8 fis16~ fis8. r16 | % 79
b4.~ b16 r16 | % 80
e8 b8~ b8 r8 | % 81
e8. r16 r16 b8.~ | % 82
b8.\fermata r16 r4 | % 83
}
pianoDynamics = {
s1*2/4\p |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4\pp |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4\p |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4\pp |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4\p |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4\pp |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4\p |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
s1*2/4 |
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
\markup \small "Takt-/Formabgleich: Mike Blackstock, Mutopia 1536 (Public Domain)."
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
\markup \small "Takt-/Formabgleich: Mike Blackstock, Mutopia 1536 (Public Domain)."
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
\markup \small "Takt-/Formabgleich: Mike Blackstock, Mutopia 1536 (Public Domain)."
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
\markup \small "Takt-/Formabgleich: Mike Blackstock, Mutopia 1536 (Public Domain)."
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
\markup \small "Takt-/Formabgleich: Mike Blackstock, Mutopia 1536 (Public Domain)."
}
