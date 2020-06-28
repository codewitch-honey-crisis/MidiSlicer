using M;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace scratch
{
	class Program
	{
		static void Main()
		{
			StreamingDemo();
		}
		static void StreamingDemo()
		{
			// demonstrate streaming a midi file 100 events at a time
			// this allows you to handle files with more than 64kb
			// of in-memory events (not the same as "on disk" size)
			// this replays the events in a loop
			var mf = MidiFile
			//.ReadFrom(@"..\..\Bohemian-Rhapsody-1.mid"); // > 64kb!
			.ReadFrom(@"..\..\A-Warm-Place.mid");
			//.ReadFrom(@"..\..\GORILLAZ_-_Feel_Good_Inc.mid");
			//.ReadFrom(@"..\..\Feel_good_4beatsBass.mid");
			//.ReadFrom(@"..\..\THE BEASTIE BOYS.Sabotage.mid");
			//.ReadFrom(@"..\..\Peter-Gunn-1.mid");
			// we use 100 events, which should be safe and allow
			// for some measure of SYSEX messages in the stream
			// without bypassing the 64kb limit
			const int EVENT_COUNT = 100;
			// our current cursor pos
			int pos = 0;
			// merge our file for playback
			var seq = MidiSequence.Merge(mf.Tracks);
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
				stm.TimeBase = mf.TimeBase;
				
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
					stm.Send(eventList);
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
				stm.Send(eventList);
				
				// loop until a key is pressed
				Console.Error.WriteLine("Press any key to exit...");
				Console.ReadKey();
				// close the stream
				stm.Close();
			}
		}
		static void RecordingDemo()
		{
			using (var dev = MidiDevice.Inputs[0])
			{
				// TODO: currently this doesn't let you
				// change the tempo in the middle of recording

				// match these two variables to your input rate
				short timeBase = 480;
				var microTempo = MidiUtility.TempoToMicroTempo(139.000217767008);

				// track 0 - meta track for tempo info
				var tr0 = new MidiSequence();
				// our seq for recording
				var seq = new MidiSequence();
				var pos = 0;
				// set this to _PreciseUtcNowTicks in order
				// to start recording now. Otherwise it will
				// not record until the first message is 
				// recieved:
				var startTicks = 0L;

				// recompute our timing based on current microTempo and timeBase
				var ticksusec = microTempo / (double)timeBase;
				var tickspertick = ticksusec / (TimeSpan.TicksPerMillisecond / 1000) * 100;

				// hook up the delegate
				dev.Input += delegate (object s, MidiInputEventArgs ea)
				{
					// initialize start ticks with the current time in ticks
					if (0 == startTicks)
						startTicks = _PreciseUtcNowTicks;
					// compute our current MIDI ticks
					var midiTicks = (int)Math.Round((_PreciseUtcNowTicks - startTicks) / tickspertick);

					// HACK: technically the sequence isn't threadsafe but as long as this event
					// is not reentrant and the MidiSequence isn't touched outside this it should
					// be fine
					seq.Events.Add(new MidiEvent(midiTicks - pos, ea.Message));
					// this is to track our old position
					// so we can compute deltas
					pos = midiTicks;
				};
				// open the device
				dev.Open();
				// add our tempo to the beginning of track 0
				tr0.Events.Add(new MidiEvent(0, new MidiMessageMetaTempo(microTempo)));
				// start listening
				dev.Start();
				Console.Error.WriteLine("Recording started.");
				// wait
				Console.Error.WriteLine("Press any key to stop recording...");
				Console.ReadKey();
				// build a type 1 midi file and preview it
				var mf = new MidiFile(1, timeBase);
				// add both tracks
				mf.Tracks.Add(tr0);
				mf.Tracks.Add(seq);
				mf.Preview();
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

		static void TestBrokenSysex()
		{
			// select the LoopBE device
			using (var dev = MidiDevice.Outputs[1])
			{
				dev.Open();
				var sysex = new MidiMessageSysex(0xF0, new byte[] { 1, 2, 3, 4, 5,0xF7 });
				// send sysex message
				while (true)
				{
					dev.Send(sysex);
					Thread.Sleep(100);
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
