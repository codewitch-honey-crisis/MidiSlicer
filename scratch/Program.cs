using M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scratch
{
	class Program
	{
		static void Main(string[] args)
		{
			goto foo;
			goto bar;
			Test1 midi = new Test1();
			midi.BrokenCMajorTriad();
			Console.ReadKey();
			return;
			foo:
			var mf = MidiFile.ReadFrom(@"..\..\Feel_good_4beatsBass.mid");
			var events = MidiSequence.Merge(mf.Tracks).Events;
			using (var stm = MidiDevice.Streams[0])
			{
				stm.Open();
				stm.Start();
				stm.TimeBase = mf.TimeBase;
				stm.SendComplete += delegate (object sender,EventArgs eargs){
					stm.Send(events);
				};
				stm.Send(events);
				Console.WriteLine("Press a key...");
				Console.ReadKey();
				stm.Close();
			}
			return;
bar:
			Test2.DoIt();
		}

	}
}
