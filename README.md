# MidiSlicer and Midi library

Midi is a small library providing a full featured, easy to use managed wrapper over Microsoft Windows' MIDI API, as well as providing for reading, writing and manipulating MIDI files and in-memory MIDI sequences. It is smaller than other libraries like Wet Dry MIDI and lower level in many ways.

I do some MIDI sequencing and recording and I found it helpful to be able to splice sections out of a MIDI file, but I didn't have a tool that made it easy to do. In the process of creating such a tool, I made a Midi assembly that contained the core MIDI file manipulation options. I also wrote some remedial playback code at first, which used the 32-bit Windows MIDI API.

That library grew as I added more features and shored up what I had. I added some more demos, streaming support, MIDI input support, device enumeration and more. Eventually, I had wrapped maybe 90-95% of the API, and had a battery of MIDI manipulation functions for searching and modifying in memory sequences and files.

In the process, MidiSlicer moved from a first class application to just another demo project, so the solution is still named MidiSlicer - I'm stuck with the GitHub of that name. The core library project is named Midi.

I wrote a [comprehensive article at codeproject](https://www.codeproject.com/Articles/5272315/Midi-A-Windows-MIDI-Library-in-Csharp) on using this library, and I recommend using it. The latest version of the code is always here at GitHub.
