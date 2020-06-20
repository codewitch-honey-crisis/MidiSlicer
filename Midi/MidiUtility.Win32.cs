namespace M
{
	using System;
	using System.Runtime.InteropServices;
	using System.Text;

	static partial class MidiUtility
	{
		internal static DateTime PreciseUtcNow {
			get {
				long filetime;
				_GetSystemTimePreciseAsFileTime(out filetime);
				return DateTime.FromFileTimeUtc(filetime);

			}
		}
		internal static long PreciseUtcNowTicks {
			get {
				long filetime;
				_GetSystemTimePreciseAsFileTime(out filetime);

				return filetime + 504911232000000000;
			}
		}
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
		static extern int midiInPrepareHeader(IntPtr handle,
			IntPtr headerPtr, int sizeOfMidiHeader);

		[DllImport("winmm.dll")]
		static extern int midiInUnprepareHeader(IntPtr handle,
			IntPtr headerPtr, int sizeOfMidiHeader);

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
		static extern int midiOutPrepareHeader(IntPtr handle,
			IntPtr headerPtr, int sizeOfMidiHeader);

		[DllImport("winmm.dll")]
		static extern int midiOutUnprepareHeader(IntPtr handle,
			IntPtr headerPtr, int sizeOfMidiHeader);

		[DllImport("winmm.dll")]
		static extern int midiOutLongMsg(IntPtr handle,
			IntPtr headerPtr, int sizeOfMidiHeader);

		[DllImport("winmm.dll")]
		static extern int midiOutGetDevCaps(int deviceIndex,
			ref MidiOutCaps caps, int sizeOfMidiOutCaps);

		[DllImport("winmm.dll")]
		static extern int midiOutGetNumDevs();


		[DllImport("winmm.dll")]
		static extern int midiOutOpen(ref IntPtr handle, int deviceID,
			MidiOutProc proc, int instance, int flags);

		[DllImport("winmm.dll")]
		static extern int midiOutClose(IntPtr handle);

		const int MOM_OPEN = 0x3C7;
		const int MOM_CLOSE = 0x3C8;
		const int MOM_DONE = 0x3C9;

		// Represents the method that handles messages from Windows.
		internal delegate void MidiOutProc(IntPtr handle, int msg, int instance, int param1, int param2);

		/// <summary>
		/// Represents MIDI output device capabilities.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		internal struct MidiOutCaps
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
		internal struct MidiInCaps
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
		const int CALLBACK_FUNCTION = 196608;

		#endregion Win32
		
	}
}
