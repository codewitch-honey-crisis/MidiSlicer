namespace M
{
	using System;
	using System.Runtime.InteropServices;
	using System.Text;

	static partial class MidiUtility
	{

		#region Win32
		// HACK: there's no easy way to wrap this so we have to expose it
		// Represents the method that handles messages from Windows.
		/// <summary>
		/// Used by the framework and not intended for use by anything else
		/// </summary>
		/// <param name="handle">The handle</param>
		/// <param name="msg">The message</param>
		/// <param name="instance">The hWnd</param>
		/// <param name="param1">The lparam</param>
		/// <param name="param2">The wparam</param>
#if MIDILIB
		public
#endif
		delegate void MidiInProc(IntPtr handle, int msg, int instance, int param1, int param2);
		
		/// <summary>
		/// Used by the framework and not intended for use by anything else
		/// </summary>
		/// <param name="handle">The handle</param>
		/// <param name="msg">The message</param>
		/// <param name="instance">The hWnd</param>
		/// <param name="param1">The lparam</param>
		/// <param name="param2">The wparam</param>
#if MIDILIB
		public
#endif
		delegate void MidiOutProc(IntPtr handle, int msg, int instance, int param1, int param2);

		[DllImport("Kernel32.dll", EntryPoint = "GetSystemTimePreciseAsFileTime", CallingConvention = CallingConvention.Winapi)]
		static extern void _GetSystemTimePreciseAsFileTime(out long filetime);

		[DllImport("winmm.dll")]
		static extern int midiInOpen(ref IntPtr handle, int deviceID,
			MidiInProc proc, int instance, int flags);

		[DllImport("winmm.dll")]
		static extern int midiInClose(IntPtr handle);

		[DllImport("winmm.dll")]
		static extern int midiInStart(IntPtr handle);

		[DllImport("winmm.dll")]
		static extern int midiInStop(IntPtr handle);

		[DllImport("winmm.dll")]
		static extern int midiInReset(IntPtr handle);

		[DllImport("winmm.dll")]
		static extern int midiInAddBuffer(IntPtr handle,
			IntPtr headerPtr, int sizeOfMidiHeader);

		[DllImport("winmm.dll")]
		static extern int midiInGetDevCaps(int deviceIndex,
			ref MidiInCaps caps, int sizeOfMidiInCaps);

		[DllImport("winmm.dll")]
		static extern int midiInGetNumDevs();

		[DllImport("winmm.dll")]
		static extern int midiOutGetErrorText(int errCode,
		   StringBuilder message, int sizeOfMessage);

		[DllImport("winmm.dll")]
		static extern int midiOutReset(IntPtr handle);

		[DllImport("winmm.dll")]
		static extern int midiOutShortMsg(IntPtr handle, int message);

		[DllImport("winmm.dll")]
		static extern int midiOutGetDevCaps(int deviceIndex,
			ref MidiOutCaps caps, int sizeOfMidiOutCaps);

		[DllImport("winmm.dll")]
		static extern int midiOutGetNumDevs();


		[DllImport("winmm.dll")]
		static extern int midiOutOpen(ref IntPtr handle, int deviceID,
			MidiOutProc proc, int instance, int flags);
		[DllImport("winmm.dll")]
		static extern int midiStreamOpen(ref IntPtr handle, ref int deviceID,int cMidi,
			MidiOutProc proc, int instance, int flags);
		[DllImport("winmm.dll")]
		static extern int midiStreamProperty(IntPtr handle, ref MidiPropTempo tempo, int dwProperty);
		[DllImport("winmm.dll")]
		static extern int midiStreamProperty(IntPtr handle, ref MidiPropTimeDiv timeDiv, int dwProperty);
		[DllImport("winmm.dll")]
		static extern int midiOutClose(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiStreamOut(IntPtr handle, ref MidiHdr lpMidiOutHdr, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiStreamOut(IntPtr handle, IntPtr lpMidiOutHdr, int uSize);

		[DllImport("winmm.dll")]
		static extern int midiStreamClose(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiStreamRestart(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiStreamPause(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiStreamStop(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiStreamPosition(IntPtr handle, ref MMTime lpMMTime, int uSize);

		[DllImport("winmm.dll")]
		static extern int midiOutLongMsg(IntPtr hMidiOut, ref MidiHdr lpMidiOutHdr, int uSize);

		[DllImport("winmm.dll")]
		static extern int midiOutPrepareHeader(IntPtr hMidiOut, ref MidiHdr lpMidiOutHdr, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutPrepareHeader(IntPtr hMidiOut, IntPtr lpMidiOutHdr, int uSize);

		[DllImport("winmm.dll")]
		static extern int midiOutUnprepareHeader(IntPtr hMidiOut, ref MidiHdr lpMidiOutHdr, int uSize);

		[DllImport("winmm.dll")]
		static extern int midiOutUnprepareHeader(IntPtr hMidiOut, IntPtr lpMidiOutHdr, int uSize);

		[StructLayout(LayoutKind.Sequential)]
		struct MidiHdr
		{
			public IntPtr lpData;          // offset  0- 3
			public uint dwBufferLength;  // offset  4- 7
			public uint dwBytesRecorded; // offset  8-11
			public IntPtr dwUser;          // offset 12-15
			public uint dwFlags;         // offset 16-19
			public IntPtr lpNext;          // offset 20-23
			public IntPtr reserved;        // offset 24-27
			public uint dwOffset;        // offset 28-31
			public IntPtr dwReserved0;
			public IntPtr dwReserved1;
			public IntPtr dwReserved2;
			public IntPtr dwReserved3;
			public IntPtr dwReserved4;
			public IntPtr dwReserved5;
			public IntPtr dwReserved6;
			public IntPtr dwReserved7;
		}

		
		const int MHDR_DONE = 1;
		const int MHDR_PREPARED = 2;
		const int MHDR_INQUEUE = 4;
		const int MHDR_ISSTRM = 8;
		
		const int MOM_OPEN = 0x3C7;
		const int MOM_CLOSE = 0x3C8;
		const int MOM_DONE = 0x3C9;
		const int MEVT_LONGMSG = 128;
		const int MEVT_F_LONG = unchecked((int)0x80000000);
		const int MEVT_SHORTMSG = 0x00;
		const int MEVT_TEMPO = 0x01;
		const int MEVT_NOP = 0x02;
		const int MEVT_COMMENT = 0x82;
		private const byte MEVT_VERSION = 0x84;
		const int TIME_MS = 0x0001;
		const int TIME_SAMPLES = 0x0002;
		const int TIME_BYTES = 0x0004;
		const int TIME_SMPTE = 0x0008;
		const int TIME_MIDI = 0x0010;
		const int TIME_TICKS = 0x0020;
		const int MIDIPROP_SET = unchecked((int)0x80000000);
		const int MIDIPROP_GET = 0x40000000;

		const int MIDIPROP_TIMEDIV = 1;
		const int MIDIPROP_TEMPO = 2;

		[StructLayout(LayoutKind.Sequential)]
		struct MidiPropTimeDiv
		{
			public int cbStruct;
			public int dwTimeDiv;
		}
		[StructLayout(LayoutKind.Sequential)]
		struct MidiPropTempo
		{
			public int cbStruct;
			public int dwTempo;
		}
		/// <summary>
		/// Represents MIDI output device capabilities.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		struct MidiOutCaps
		{
			#region MidiOutCaps Members

			/// <summary>
			/// Manufacturer identifier of the device driver for the Midi output 
			/// device. 
			/// </summary>
			public short mid;

			/// <summary>
			/// Product identifier of the Midi output device. 
			/// </summary>
			public short pid;

			/// <summary>
			/// Version number of the device driver for the Midi output device. The 
			/// high-order byte is the major version number, and the low-order byte 
			/// is the minor version number. 
			/// </summary>
			public int driverVersion;

			/// <summary>
			/// Product name.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string name;

			/// <summary>
			/// Flags describing the type of the Midi output device. 
			/// </summary>
			public short technology;

			/// <summary>
			/// Number of voices supported by an internal synthesizer device. If 
			/// the device is a port, this member is not meaningful and is set 
			/// to 0. 
			/// </summary>
			public short voices;

			/// <summary>
			/// Maximum number of simultaneous notes that can be played by an 
			/// internal synthesizer device. If the device is a port, this member 
			/// is not meaningful and is set to 0. 
			/// </summary>
			public short notes;

			/// <summary>
			/// Channels that an internal synthesizer device responds to, where the 
			/// least significant bit refers to channel 0 and the most significant 
			/// bit to channel 15. Port devices that transmit on all channels set 
			/// this member to 0xFFFF. 
			/// </summary>
			public short channelMask;

			/// <summary>
			/// Optional functionality supported by the device. 
			/// </summary>
			public int support;

			#endregion
		}
		/// <summary>
		/// Represents MIDI input device capabilities.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		struct MidiInCaps
		{
			#region MidiInCaps Members

			/// <summary>
			/// Manufacturer identifier of the device driver for the Midi output 
			/// device. 
			/// </summary>
			public short mid;

			/// <summary>
			/// Product identifier of the Midi output device. 
			/// </summary>
			public short pid;

			/// <summary>
			/// Version number of the device driver for the Midi output device. The 
			/// high-order byte is the major version number, and the low-order byte 
			/// is the minor version number. 
			/// </summary>
			public int driverVersion;

			/// <summary>
			/// Product name.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string name;

			/// <summary>
			/// Optional functionality supported by the device. 
			/// </summary>
			public int support;

			#endregion
		}
		[StructLayout(LayoutKind.Sequential)]
		struct MidiShortEvent
		{
			public int deltaTime;
			public int streamId;
			public int @event;
		}
		[StructLayout(LayoutKind.Explicit)]
		struct MMTime
		{
			[FieldOffset(0)] public int wType;
			[FieldOffset(4)] public int ms;
			[FieldOffset(4)] public int sample;
			[FieldOffset(4)] public int cb;
			[FieldOffset(4)] public int ticks;
			[FieldOffset(4)] public Byte smpteHour;
			[FieldOffset(5)] public Byte smpteMin;
			[FieldOffset(6)] public Byte smpteSec;
			[FieldOffset(7)] public Byte smpteFrame;
			[FieldOffset(8)] public Byte smpteFps;
			[FieldOffset(9)] public Byte smpteDummy;
			[FieldOffset(10)] public Byte pad0;
			[FieldOffset(11)] public Byte pad1;
			[FieldOffset(4)] public int midiSongPtrPos;
		}
		const int CALLBACK_FUNCTION = 196608;
		const int MIDI_IO_STATUS = 32;
		#endregion Win32

	}
}
