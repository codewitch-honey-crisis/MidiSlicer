using M;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace scratch
{
	class Program
	{
		static void Main()
		{
			SimpleBuildingDemo();
			//SimpleStreamingDemo();
			//ComplexStreamingDemo();
			//SimpleRecordingDemo();
			//TestTiming();
		}
		static void SimpleBuildingDemo()
		{
			var seq = new MidiSequence();
			// set the tempo
			seq.Events.Add(new MidiEvent(0, new MidiMessageMetaTempo(100d)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("C5", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("E5", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("G5", 127, 0)));
			seq.Events.Add(new MidiEvent(12, new MidiMessageNoteOn("C5", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("E5", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("G5", 127, 0)));
			seq.Events.Add(new MidiEvent(12, new MidiMessageNoteOn("C5", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("E5", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("G5", 127, 0)));
			seq.Events.Add(new MidiEvent(12, new MidiMessageNoteOn("A4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("C5", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("E5", 127, 0)));
			seq.Events.Add(new MidiEvent(12, new MidiMessageNoteOn("A4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("C5", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("E5", 127, 0)));
			seq.Events.Add(new MidiEvent(12, new MidiMessageNoteOn("A4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("C5", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("E5", 127, 0)));
			seq.Events.Add(new MidiEvent(12, new MidiMessageNoteOn("F4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("A4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("C5", 127, 0)));
			seq.Events.Add(new MidiEvent(12, new MidiMessageNoteOn("F4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("A4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("C5", 127, 0)));
			seq.Events.Add(new MidiEvent(12, new MidiMessageNoteOn("F4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("A4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("C5", 127, 0)));
			seq.Events.Add(new MidiEvent(12, new MidiMessageNoteOn("G4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("B4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("D5", 127, 0)));
			seq.Events.Add(new MidiEvent(12, new MidiMessageNoteOn("G4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("B4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("D5", 127, 0)));
			seq.Events.Add(new MidiEvent(12, new MidiMessageNoteOn("G4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("B4", 127, 0)));
			seq.Events.Add(new MidiEvent(0, new MidiMessageNoteOn("D5", 127, 0)));
			seq.Events.Add(new MidiEvent(12, new MidiMessageMetaEndOfTrack()));
			using (var stm = MidiDevice.Streams[0])
			{
				stm.SendComplete += delegate (object s, EventArgs e)
				{
					stm.Send(seq.Events);
				};
				Console.WriteLine("Press a key to exit...");
				// open it
				stm.Open();
				stm.Start();
				stm.Send(seq.Events);
				Console.ReadKey();
			}
		}
		static void SimpleStreamingDemo()
		{
			// just grab the first output stream
			using (var stm = MidiDevice.Streams[0])
			{
				// open it
				stm.Open();
				// read a MIDI file
				var midiFile = MidiFile
				//.ReadFrom(@"..\..\Feel_good_4beatsBass.mid");
				//.ReadFrom(@"..\..\Bohemian-Rhapsody-1.mid"); // > 64kb!
				.ReadFrom(@"..\..\A-Warm-Place.mid");
				//.ReadFrom(@"..\..\50_cent_-_In_Da_Club.mid");
				//.ReadFrom(@"..\..\Beethoven-Moonlight-Sonata.mid");
				//.ReadFrom(@"..\..\Peter-Gunn-1.mid");
				//.ReadFrom(@"..\..\THE BEASTIE BOYS.Sabotage.mid");
				Console.WriteLine(Path.GetFileName(midiFile.FilePath)+" @ "+midiFile.TimeBase+" PQN");
				var tsig = midiFile.TimeSignature;
				Console.WriteLine("Tempo: " + midiFile.Tempo + " BPM @ " + tsig.Numerator + "/" + tsig.Denominator + " time");
				Console.WriteLine("Tracks:");
				for(var i = 0;i<midiFile.Tracks.Count;++i)
				{
					var track = midiFile.Tracks[i];
					Console.Write("\t");
					var name = track.Name;
					if (string.IsNullOrEmpty(name))
						name = "Track " + i;
					Console.WriteLine(name+" \tEvents: "+track.Events.Count);
				}
				Console.WriteLine();
				// merge the tracks for playback
				var seq = MidiSequence.Merge(midiFile.Tracks);
				// set the stream timebase
				stm.TimeBase = midiFile.TimeBase;
				// start the playback
				stm.Start();
				Console.Error.WriteLine("Press any key to exit...");
				// if we weren't looping
				// we wouldn't need to
				// hook this:
				stm.SendComplete += delegate (object s, EventArgs e)
				{
					// loop
					stm.Send(seq.Events);
				};
				// kick things off
				stm.Send(seq.Events);
		
				// wait for exit
				Console.ReadKey();

			}
		}
		static void ComplexStreamingDemo()
		{
			// demonstrate streaming a midi file 100 events at a time
			// this allows you to handle files with more than 64kb
			// of in-memory events (not the same as "on disk" size)
			// this replays the events in a loop
			var midiFile = MidiFile
			//.ReadFrom(@"..\..\Bohemian-Rhapsody-1.mid"); // > 64kb!
			.ReadFrom(@"..\..\A-Warm-Place.mid");
			//.ReadFrom(@"..\..\GORILLAZ_-_Feel_Good_Inc.mid");
			//.ReadFrom(@"..\..\Feel_good_4beatsBass.mid");
			//.ReadFrom(@"..\..\THE BEASTIE BOYS.Sabotage.mid");
			//.ReadFrom(@"..\..\Peter-Gunn-1.mid");
			Console.WriteLine(Path.GetFileName(midiFile.FilePath) + " @ " + midiFile.TimeBase + " PQN");
			var tsig = midiFile.TimeSignature;
			Console.WriteLine("Tempo: " + midiFile.Tempo + " BPM @ " + tsig.Numerator + "/" + tsig.Denominator + " time");
			Console.WriteLine("Tracks:");
			for (var i = 0; i < midiFile.Tracks.Count; ++i)
			{
				var track = midiFile.Tracks[i];
				Console.Write("\t");
				var name = track.Name;
				if (string.IsNullOrEmpty(name))
					name = "Track " + i;
				Console.WriteLine(name + " \tEvents: " + track.Events.Count);
			}
			Console.WriteLine();
			// we use 100 events, which should be safe and allow
			// for some measure of SYSEX messages in the stream
			// without bypassing the 64kb limit
			const int EVENT_COUNT = 100;
			// our current cursor pos
			int pos = 0;
			// merge our file for playback
			var seq = MidiSequence.Merge(midiFile.Tracks);
			// the number of events in the seq
			int len = seq.Events.Count;
			// stores the next set of events
			var eventList = new List<MidiEvent>(EVENT_COUNT);
			// just grab the first output stream
			// should be the wavetable synth
			using (var stm = MidiDevice.Streams[0])
			{
				// open the stream
				stm.Open();
				// start it
				stm.Start();
				// first set the timebase
				stm.TimeBase = midiFile.TimeBase;
				
				// set up our send complete handler
				stm.SendComplete += delegate (object sender,EventArgs eargs)
				{
					// clear the list	
					eventList.Clear();
					// iterate through the next events
					var next = pos+EVENT_COUNT;
					for(;pos<next;++pos)
					{
						// if it's past the end, loop it
						if (len <= pos)
						{
							pos = 0;
							break;
						}
						// otherwise add the next event
						eventList.Add(seq.Events[pos]);
					}
					// send the list of events
					stm.SendDirect(eventList);
				};
				// add the first events
				for(pos = 0;pos<EVENT_COUNT;++pos)
				{
					// if it's past the end, loop it
					if (len <= pos)
					{
						pos = 0;
						break;
					}
					// otherwise add the next event
					eventList.Add(seq.Events[pos]);
				}
				// send the list of events
				stm.SendDirect(eventList);
				
				// loop until a key is pressed
				Console.Error.WriteLine("Press any key to exit...");
				Console.ReadKey();
				// close the stream
				stm.Close();
			}
		}
		static void SimpleRecordingDemo()
		{
			MidiFile mf;
			using (var idev = MidiDevice.Inputs[0])
			{
				using (var odev = MidiDevice.Outputs[0])
				{
					idev.Input += delegate (object s, MidiInputEventArgs e)
					{
						// this is so we can pass through and hear 
						// our input while recording
						odev.Send(e.Message);
					};
					idev.TempoChanged += delegate (object s, EventArgs e)
					{
						Console.WriteLine("New Tempo: " + idev.Tempo);
					};
					// open the input
					// and output
					idev.Open();
					// set our timebase
					idev.TimeBase = 384;
					odev.Open();
					idev.Start();
					// start recording, waiting for input
					idev.StartRecording(true);
					// wait to end it
					Console.Error.WriteLine("Press any key to stop recording...");
					Console.ReadKey();
					// get our MidiFile from this
					mf = idev.EndRecording();
					// the MIDI file is always two
					// tracks, with the first track
					// being the tempo map
				}
			}
			if (null == mf)
				return;
			// merge for playback
			var seq = MidiSequence.Merge(mf.Tracks);

			// now stream the file to the output
			// just grab the first output stream
			using (var stm = MidiDevice.Streams[0])
			{
				// open it
				stm.Open();
				// merge the tracks for playback
				seq = MidiSequence.Merge(mf.Tracks);
				// set the stream timebase
				stm.TimeBase = mf.TimeBase;
				// start the playback
				stm.Start();
				Console.Error.WriteLine("Press any key to exit...");
				// if we weren't looping
				// we wouldn't need to
				// hook this:
				stm.SendComplete += delegate (object s, EventArgs e)
				{
					// loop
					stm.Send(seq.Events);
				};
				// kick things off
				stm.Send(seq.Events);
				// wait for exit
				Console.ReadKey();
			}
		}
		static void ComplexRecordingDemo()
		{
			using (var idev = MidiDevice.Inputs[0])
			{
				// TODO: currently this doesn't let you
				// change the tempo in the middle of recording

				// match these two variables to your input rate
				short timeBase = 480;
				var microTempo = MidiUtility.TempoToMicroTempo(120);

				// track 0 - meta track for tempo info
				var tr0 = new MidiSequence();
				// our seq for recording
				var seq = new MidiSequence();
				// compute our timing based on current microTempo and timeBase
				var ticksusec = microTempo / (double)timeBase;
				var tickspertick = ticksusec / (TimeSpan.TicksPerMillisecond / 1000) * 100;
				var pos = 0;
				// set this to _PreciseUtcNowTicks in order
				// to start recording now. Otherwise it will
				// not record until the first message is 
				// recieved:
				var startTicks = 0L;

				using (var odev = MidiDevice.Outputs[0])
				{
					// hook up the delegate
					idev.Input += delegate (object s, MidiInputEventArgs ea)
					{
						// initialize start ticks with the current time in ticks
						if (0 == startTicks)
							startTicks = _PreciseUtcNowTicks;
						// compute our current MIDI ticks
						var midiTicks = (int)Math.Round((_PreciseUtcNowTicks - startTicks) / tickspertick);
						// pass through to play
						odev.Send(ea.Message);
						// HACK: technically the sequence isn't threadsafe but as long as this event
						// is not reentrant and the MidiSequence isn't touched outside this it should
						// be fine
						seq.Events.Add(new MidiEvent(midiTicks - pos, ea.Message));
						// this is to track our old position
						// so we can compute deltas
						pos = midiTicks;
					};
					// open the input device
					idev.Open();
					// open the output device
					odev.Open();
					// add our tempo to the beginning of track 0
					tr0.Events.Add(new MidiEvent(0, new MidiMessageMetaTempo(microTempo)));
					// start listening
					idev.Start();
					Console.Error.WriteLine("Recording started.");
					// wait
					Console.Error.WriteLine("Press any key to stop recording...");
					Console.ReadKey();
					// stop the buffer and flush any pending events
					idev.Stop();
					idev.Reset();
				}
				
				// create termination track
				var endTrack = new MidiSequence();
				var len = seq.Length;
				// comment the following to terminate 
				// without the trailing empty score:
				len = unchecked((int)((_PreciseUtcNowTicks - startTicks) / tickspertick));
				endTrack.Events.Add(new MidiEvent(len, new MidiMessageMetaEndOfTrack()));
				
				// terminate the tracks
				tr0 = MidiSequence.Merge(tr0, endTrack);
				seq = MidiSequence.Merge(seq, endTrack);
				
				// build a type 1 midi file
				var mf = new MidiFile(1, timeBase);
				// add both tracks
				mf.Tracks.Add(tr0);
				mf.Tracks.Add(seq);

				// now stream the file to the output
				// just grab the first output stream
				using (var stm = MidiDevice.Streams[0])
				{
					// open it
					stm.Open();
					// merge the tracks for playback
					seq = MidiSequence.Merge(mf.Tracks);
					// set the stream timebase
					stm.TimeBase = mf.TimeBase;
					// start the playback
					stm.Start();
					Console.Error.WriteLine("Press any key to exit...");
					// if we weren't looping
					// we wouldn't need to
					// hook this:
					stm.SendComplete += delegate (object s, EventArgs e)
					{
						// loop
						stm.Send(seq.Events);
					};
					// kick things off
					stm.Send(seq.Events);
					// wait for exit
					Console.ReadKey();
				}
			}
		}
		static void SendDemo()
		{
			// just grab the first output device
			using(var dev = MidiDevice.Outputs[0])
			{
				// open the device
				dev.Open();
				// send a C5 major chord
				dev.Send(new MidiMessageNoteOn("C5", 127, 0));
				dev.Send(new MidiMessageNoteOn("E5", 127, 0));
				dev.Send(new MidiMessageNoteOn("G5", 127, 0));
				Console.Error.WriteLine("Press any key to exit...");
				Console.ReadKey();
				// note offs
				dev.Send(new MidiMessageNoteOff("C5", 127, 0));
				dev.Send(new MidiMessageNoteOff("E5", 127, 0));
				dev.Send(new MidiMessageNoteOff("G5", 127, 0));
			}
		}
		static void DeviceListDemo()
		{
			Console.WriteLine("Output devices:");
			Console.WriteLine();
			foreach (var dev in MidiDevice.Outputs)
			{
				var kind = "";
				switch (dev.Kind)
				{
					case MidiOutputDeviceKind.MidiPort:
						kind = "MIDI Port";
						break;
					case MidiOutputDeviceKind.Synthesizer:
						kind = "Synthesizer";
						break;
					case MidiOutputDeviceKind.SquareWaveSynthesizer:
						kind = "Square wave synthesizer";
						break;
					case MidiOutputDeviceKind.FMSynthesizer:
						kind = "FM synthesizer";
						break;
					case MidiOutputDeviceKind.WavetableSynthesizer:
						kind = "Wavetable synthesizer";
						break;
					case MidiOutputDeviceKind.SoftwareSynthesizer:
						kind = "Software synthesizer";
						break;
					case MidiOutputDeviceKind.MidiMapper:
						kind = "MIDI Mapper";
						break;
				}
				Console.WriteLine(dev.Name + " " + dev.Version + " " + kind);
			}
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Input devices:");
			Console.WriteLine();
			foreach (var dev in MidiDevice.Inputs)
			{
				Console.WriteLine(dev.Name + " " + dev.Version);
			}
		}
		static void MonitorDemo()
		{
			// just grab the first input device
			using(var dev = MidiDevice.Inputs[0])
			{
				Console.Error.WriteLine("Press any key to exit...");
				// hook the input
				dev.Input += delegate(object s,MidiInputEventArgs ea) {
					Console.WriteLine(ea.Message);
				};
				// open the device
				dev.Open();
				// start capturing
				dev.Start();
				// wait for keypress
				Console.ReadKey();
			}
		}

		static void TestSysex()
		{
			// select the LoopBE device
			using (var dev = MidiDevice.Outputs[1])
			{
				dev.Open();
				var buf = new byte[650];
				Console.Error.WriteLine("Press any key to exit...");
				var b = 0;
				while (true)
				{
					if (0xF7 != b)
					{
						for (var i = 0; i < buf.Length - 1; ++i)
						{
							buf[i] = (byte)((b+i)&0x7F);
						}
						buf[buf.Length - 1] = 0xF7;
						var sysex = new MidiMessageSysex(buf);
						// send sysex message
						if (Console.KeyAvailable)
							return;
						dev.Send(sysex);
						Thread.Sleep(50);
					}
					++b;
					if (0x80 == b)
						b = 0;
				}
				
			}
		}
		static void TestSysexStream()
		{
			var b = 0;
			var buf = new byte[650];

			// select the LoopBE device
			using (var stm = MidiDevice.Streams[1])
			{
				stm.SendComplete += delegate (object o, EventArgs e)
				{
					if (0xF7 != b)
					{
						for (var i = 0; i < buf.Length - 1; ++i)
						{
							buf[i] = (byte)((b + i) & 0x7F);
						}
						++b;
						buf[buf.Length - 1] = 0xF7;
						var sysex = new MidiMessageSysex(buf);
						Thread.Sleep(100);
						// send sysex message
						stm.Send(new MidiEvent(0, sysex));
						
					}
					if (0x80 == b)
						b = 0;
				};
				stm.Open();
				stm.TimeBase = 480;
				stm.Start();
				Console.Error.WriteLine("Press any key to exit...");
				if (0xF7 != b)
				{
					for (var i = 0; i < buf.Length - 1; ++i)
					{
						buf[i] = (byte)((b + i) & 0x7F);
					}
					++b;
					buf[buf.Length - 1] = 0xF7;
					var sysex = new MidiMessageSysex(buf);
					// send sysex message
					stm.Send(new MidiEvent(0, sysex));
					
				}
				Console.ReadKey();
			}
		}
		static void TestTiming()
		{
			// read a MIDI file
			var mf = MidiFile
			//.ReadFrom(@"..\..\Feel_good_4beatsBass.mid");
			//.ReadFrom(@"..\..\Bohemian-Rhapsody-1.mid"); // > 64kb!
			//.ReadFrom(@"..\..\Beethoven-Moonlight-Sonata.mid");
			//.ReadFrom(@"..\..\THE BEASTIE BOYS.Sabotage.mid");
			.ReadFrom(@"..\..\A-Warm-Place.mid");
			Console.WriteLine("Timebase: " + mf.TimeBase);
			var time = new TimeSpan(0, 0, 0, 3, 0);
			Console.WriteLine("Time " + time);
			var ticks = mf.Tracks[0].GetPositionAtTime(time, mf.TimeBase);
			Console.WriteLine("Ticks " + ticks);
			time = new TimeSpan(mf.Tracks[0].GetContext(ticks, mf.TimeBase).SystemTicks);
			Console.WriteLine("Time " + time);
			ticks = mf.Tracks[0].GetPositionAtTime(time, mf.TimeBase);
			Console.WriteLine("Ticks: " + ticks);
		}
		static void TestPlaybackTiming()
		{
			var mf = MidiFile.ReadFrom(@"..\..\GORILLAZ_-_Feel_Good_Inc.mid");
			var seq = MidiSequence.Merge(mf.Tracks);
			Console.WriteLine(mf.Duration);
			var st = _PreciseUtcNowTicks;
			var et = 0L;
			using (var stm = MidiDevice.Streams[0])
			{
				stm.SendComplete += delegate (object s, EventArgs ea)
				{
					Interlocked.Exchange(ref et, _PreciseUtcNowTicks);
				};
				stm.Open();
				stm.TimeBase = mf.TimeBase;
				stm.Start();
				stm.Send(seq.Events);
				while (0 == et)
				{
					Thread.Sleep(1);
				}

			}

			Console.WriteLine(new TimeSpan(et - st));
		}
		static void TestTapTempo()
		{
			const int MIN_TEMPO= 50;
			Console.Error.WriteLine("Press escape to exit. Any other key to tap tempo");
			using (var stm = MidiDevice.Streams[1])
			{
				
				stm.Open();
				stm.TimeBase = 480;
				//stm.TempoSynchronizationMessageCount = 0; // continuous
				stm.TempoSynchronizationEnabled = true;
				stm.Start();
				long oldTicks = 0;
				var amnt=0d;
				while(true)
				{
					if (0 != oldTicks)
					{
						var dif = (_PreciseUtcNowTicks - oldTicks);
						var tpm = TimeSpan.TicksPerMillisecond * 60000;
						amnt = tpm / (double)dif;
						if (amnt < MIN_TEMPO)
						{
							oldTicks = 0;
							Console.Error.WriteLine("Tap tempo timed out for tempo less than " + MIN_TEMPO + "bpm");
						}
					}
					if (Console.KeyAvailable)
					{
						if (ConsoleKey.Escape == Console.ReadKey(true).Key)
							return; // exit
						if (0 == oldTicks)
						{
							Console.Error.WriteLine("Tap Started");
							oldTicks = _PreciseUtcNowTicks;
						}
						else
						{
							var dif = _PreciseUtcNowTicks - oldTicks;
							var tpm = TimeSpan.TicksPerMillisecond * 60000;
							amnt = tpm / (double)dif;
							stm.Tempo = amnt;
							oldTicks = _PreciseUtcNowTicks;
							var pos = stm.TimeBase/ 24;
							Console.Error.WriteLine("Tapped @ " + amnt);
							
						}
					}
					else
						Thread.Sleep(1);
				}

			}
		}
		#region Win32
		[DllImport("Kernel32.dll", EntryPoint = "GetSystemTimePreciseAsFileTime", CallingConvention = CallingConvention.Winapi)]
		static extern void _GetSystemTimePreciseAsFileTime(out long filetime);
		#endregion
		static long _PreciseUtcNowTicks {
			get {
				long filetime;
				_GetSystemTimePreciseAsFileTime(out filetime);

				return filetime + 504911232000000000;
			}
		}
	}
}
