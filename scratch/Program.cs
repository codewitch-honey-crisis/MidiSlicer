﻿using M;
using System;
using System.Collections.Generic;

namespace scratch
{
	class Program
	{
		static void Main()
		{
			// demonstrate streaming a midi file 100 events at a time
			// this allows you to handle files with more than 64k
			// of in-memory events (not the same as "on disk" size)
			// this replays the events in a loop
			var mf = MidiFile
			.ReadFrom(@"..\..\Bohemian-Rhapsody-1.mid"); // > 64kb!
			//.ReadFrom(@"..\..\A-Warm-Place.mid");
			//.ReadFrom(@"..\..\GORILLAZ_-_Feel_Good_Inc.mid");
			//.ReadFrom(@"..\..\Feel_good_4beatsBass.mid");

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
				Console.WriteLine("Press a key...");
				Console.ReadKey();
				// close the stream
				stm.Close();
			}
		}
	}
}