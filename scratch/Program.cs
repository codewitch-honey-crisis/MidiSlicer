using M;
using System;
using System.Collections.Generic;
using System.Threading;

namespace scratch
{
	class Program
	{
		static void Main()
		{
			TestBrokenSysex();
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
	}
}
