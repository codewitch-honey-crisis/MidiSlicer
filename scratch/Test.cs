using System;
using System.Runtime.InteropServices;

namespace scratch
{
    public delegate void MidiProc(IntPtr hMidiIn, int wMsg, int dwInstance, uint dwParam1, uint dwParam2);

  

    class Test1
    {
        MidiProc midiProc;

        public void BrokenCMajorTriad()
        {
            midiProc = MessageHandler;
            int id = 0;
            IntPtr handle = IntPtr.Zero;
            int result = 0;

            Console.WriteLine("Stream Out Message");
            Console.WriteLine("------------------");
            MIDIHDR header = new MIDIHDR();


            MIDIEVENT ev0 = new MIDIEVENT();
            ev0.dwEvent = 0x00403C90;

            MIDIEVENT ev1 = new MIDIEVENT();
            ev1.dwDeltaTime = 10;
            ev1.dwEvent = 0x00404090;

            MIDIEVENT ev2 = new MIDIEVENT();
            ev2.dwDeltaTime = 10;
            ev2.dwEvent = 0x00404390;

            MIDIEVENT ev3 = new MIDIEVENT();
            ev3.dwDeltaTime = 80;
            ev3.dwEvent = 0x00403C80;

            MIDIEVENT ev4 = new MIDIEVENT();
            ev4.dwDeltaTime = 0;
            ev4.dwEvent = 0x00404080;

            MIDIEVENT ev5 = new MIDIEVENT();
            ev5.dwDeltaTime = 0;
            ev5.dwEvent = 0x00404380;

            MIDIEVENT[] events = new MIDIEVENT[]
            {
                ev0//, ev1, ev2, ev3, ev4, ev5
            };
            int eventSize = Marshal.SizeOf(typeof(MIDIEVENT));
            int blockSize = eventSize * events.Length;
            IntPtr eventPointer = Marshal.AllocHGlobal(blockSize);
            for (int i = 0; i < events.Length; i++)
                Marshal.StructureToPtr(events[i], (IntPtr)((int)eventPointer + (eventSize * i)), false);

            result = midiStreamOpen(ref handle, ref id, 1, midiProc, 0, CALLBACK_FUNCTION);
            header.lpData = eventPointer;
            header.dwBufferLength = blockSize;
            header.dwBytesRecorded = blockSize;
            int headerSize = Marshal.SizeOf(header);
            IntPtr headerPointer = Marshal.AllocHGlobal(headerSize);
            Marshal.StructureToPtr(header, headerPointer, false);
            result = midiOutPrepareHeader(handle, headerPointer, headerSize);
            result = midiStreamOut(handle, headerPointer, headerSize);
            result = midiStreamRestart(handle);
            //while (((BufferFlags)Marshal.ReadInt32(headerPointer, 16) & BufferFlags.Done) != BufferFlags.Done)
            //{ }
            result = midiOutUnprepareHeader(handle, headerPointer, headerSize);
            Marshal.FreeHGlobal(headerPointer);
            Marshal.FreeHGlobal(eventPointer);
            System.Threading.Thread.Sleep(2000);
            result = midiStreamClose(handle);
            handle = IntPtr.Zero;
        }

        void MessageHandler(IntPtr hMidiIn, int wMsg, int dwInstance, uint dwParam1, uint dwParam2)
        {
            switch (wMsg)
            {
                case MOM_OPEN:
                    Console.WriteLine("Opened");
                    break;
                case MOM_CLOSE:
                    Console.WriteLine("Closed");
                    break;
                case MOM_DONE:
                    Console.WriteLine("Done");
                    break;
            }
        }

        [Flags]
        public enum BufferFlags
        {
            None = 0,
            Done = MHDR_DONE,
            Prepared = MHDR_PREPARED,
            Queued = MHDR_INQUEUE,
            ISStream = MHDR_ISSTRM
        }

        // Source MMSystem.h
        public const int CALLBACK_FUNCTION = 0x00030000;
        public const int MOM_OPEN = 0x3C7;
        public const int MOM_CLOSE = 0x3C8;
        public const int MOM_DONE = 0x3C9;
        public const int MHDR_DONE = 0x00000001;
        public const int MHDR_PREPARED = 0x00000002;
        public const int MHDR_INQUEUE = 0x00000004;
        public const int MHDR_ISSTRM = 0x00000008;

        // Function References: http://msdn.microsoft.com/en-us/library/ms712038(VS.85).aspx
        [DllImport("winmm.dll")]
        public static extern int midiOutPrepareHeader(
            IntPtr hmo,
            IntPtr lpMidiOutHdr,
            int cbMidiOutHdr);

        [DllImport("winmm.dll")]
        public static extern int midiOutUnprepareHeader(
            IntPtr hmo,
            IntPtr lpMidiOutHdr,
            int cbMidiOutHdr);

        [DllImport("winmm.dll")]
        public static extern int midiStreamClose(
            IntPtr hStream);

        [DllImport("winmm.dll")]
        public static extern int midiStreamOpen(
            ref IntPtr lphStream,
            ref int puDeviceID,
            int cMidi,
            MidiProc dwCallback,
            int dwInstance,
            int fdwOpen);

        [DllImport("winmm.dll")]
        public static extern int midiStreamOut(
            IntPtr hMidiStream,
            IntPtr lpMidiHdr,
            int cbMidiHdr);

        [DllImport("winmm.dll")]
        public static extern int midiStreamRestart(
            IntPtr hms);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MIDIHDR
    {
        public IntPtr lpData;
        public int dwBufferLength;
        public int dwBytesRecorded;
        public IntPtr dwUser;
        public int dwFlags;
        public IntPtr lpNext;
        public IntPtr reserved;
        public int dwOffset;
        public IntPtr dwReserved0;
        public IntPtr dwReserved1;
        public IntPtr dwReserved2;
        public IntPtr dwReserved3;
        public IntPtr dwReserved4;
        public IntPtr dwReserved5;
        public IntPtr dwReserved6;
        public IntPtr dwReserved7;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct MIDIEVENT
    {
        public uint dwDeltaTime;
        public uint dwStreamID;
        public uint dwEvent;
        //public uint[] dwParms; // Only for Sysex stream
    }
}