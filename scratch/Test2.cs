using System;
using System.Runtime.InteropServices;


namespace scratch
{
	static class Test2
	{
		[DllImport("winmm.dll")]
		static extern int midiOutPrepareHeader(IntPtr hstm, ref MIDIHDR hdr, int cbHdr);
		/* The array of MIDIEVENTs to be output. We only have 2 */
		static readonly int[] myNotes = new int[] { 0, 0, 0x007F3C90, /* A note-on */
192, 0, 0x00003C90 }; /* A note-off. It's the last event in the array */
		public static void DoIt()
		{
			IntPtr hstm=IntPtr.Zero;
			int dev = 0;
			var result = Test1.midiStreamOpen(ref hstm, ref dev, 1, null, 0, 0);
			if (0 == result)
			{
				var hdr = new MIDIHDR();
				hdr.dwBufferLength = hdr.dwBytesRecorded = myNotes.Length * 4;
				hdr.lpData = Marshal.UnsafeAddrOfPinnedArrayElement(myNotes, 0);
				hdr.dwFlags = 0;
				int hdrSiz = Marshal.SizeOf(typeof(MIDIHDR));
				
				result = midiOutPrepareHeader(hstm, ref hdr, hdrSiz);

				Test1.midiStreamClose(hstm);
			}
		}
	}
}
