using M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace taptempo
{
	class Program
	{
		static void Main()
		{
			const int MIN_TEMPO = 50;
			const int MAX_MSG_COUNT = 100;
			Console.Error.WriteLine("Press escape to exit. Any other key to tap tempo");
			using (var dev = MidiDevice.Outputs[1])
			{
				dev.Open();
				long oldTicks = 0;
				var amnt = 0d;
				var next = 0L;
				var dif = 0L;
				var msgCount = 0;
				while (true)
				{
					if (0 != oldTicks)
					{
						var dif2 = (_PreciseUtcNowTicks - oldTicks);
						var tpm = TimeSpan.TicksPerMillisecond * 60000;
						var amnt2 = tpm / (double)dif2;
						if (amnt2 < MIN_TEMPO)
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
							dif = _PreciseUtcNowTicks - oldTicks;
							var ts = new TimeSpan(dif);
							var ms = ts.TotalMilliseconds;
							var tpm = TimeSpan.TicksPerMillisecond * 60000;
			
							amnt = tpm / (double)dif;
							oldTicks = _PreciseUtcNowTicks;
							Console.Error.WriteLine("Tapped @ " + amnt+"bpm "+ms+"ms");
							next = _PreciseUtcNowTicks + (dif/24);
							dev.Send(new MidiMessageRealTimeTimingClock());
							msgCount=0;
						}
						
					}
					else
					{
						if (MAX_MSG_COUNT > msgCount && 0 != next && _PreciseUtcNowTicks >= next)
						{
							next += dif / 24;
							dev.Send(new MidiMessageRealTimeTimingClock());
							++msgCount;
						}
					}
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
