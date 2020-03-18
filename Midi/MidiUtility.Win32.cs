namespace M
{
	using System;
	using System.Runtime.InteropServices;
	using System.Text;

	static partial class MidiUtility
	{
		[DllImport("Kernel32.dll", EntryPoint = "GetSystemTimePreciseAsFileTime", CallingConvention = CallingConvention.Winapi)]
		static extern void _GetSystemTimePreciseAsFileTime(out long filetime);
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
		// Represents the method that handles messages from Windows.
		internal delegate void MidiInProc(IntPtr handle, int msg, int instance, int param1, int param2);

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
		/// <summary>
		/// Retrieves the error message for the specified error code
		/// </summary>
		/// <param name="errorCode">The error code</param>
		/// <returns>A text error message for the error code</returns>
		public static string GetMidiOutErrorMessage(int errorCode)
		{
			var result = new StringBuilder(256);
			midiOutGetErrorText(errorCode, result, result.Capacity);
			return result.ToString();
		}
		/// <summary>
		/// Opens a MIDI output device
		/// </summary>
		/// <param name="deviceIndex">The device index</param>
		/// <returns>A handle to the specified device</returns>
		public static IntPtr OpenOutputDevice(int deviceIndex)
		{
			IntPtr handle = IntPtr.Zero;
			_CheckOutResult(midiOutOpen(ref handle, deviceIndex, null, 0, 0));
			return handle;
		}
		/// <summary>
		/// Closes the specified MIDI output device
		/// </summary>
		/// <param name="handle">The handle of the device</param>
		public static void CloseOutputDevice(IntPtr handle)
		{
			_CheckOutResult(midiOutClose(handle));
		}
		static void _CheckOutResult(int errorCode)
		{
			if (0 != errorCode)
				throw new Exception(GetMidiOutErrorMessage(errorCode));
		}
		/// <summary>
		/// Indicates the number of MIDI output devices
		/// </summary>
		public static int OutputDeviceCount { get { return midiOutGetNumDevs(); } }
		/// <summary>
		/// Sends a MIDI message to the specified device
		/// </summary>
		/// <param name="deviceHandle">The device handle</param>
		/// <param name="message">The packed MIDI message</param>
		public static void Send(IntPtr deviceHandle, int message)
		{
			_CheckOutResult(midiOutShortMsg(deviceHandle, message));
		}
		/// <summary>
		/// Sends a MIDI message to the specified device
		/// </summary>
		/// <param name="deviceHandle">The device handle</param>
		/// <param name="message">The MIDI message</param>
		public static void Send(IntPtr deviceHandle,MidiMessage message)
		{
			var m = 0;
			switch(message.Status & 0xF0) {
				case 0x80:
				case 0x90:
				case 0xA0:
				case 0xB0:
				case 0xE0:
					var mcmdw = message as MidiMessageWord;
					m = mcmdw.Status;
					m |= mcmdw.Data2 << 16;
					m |= mcmdw.Data1 << 8;
					Send(deviceHandle, m);
					break;
				case 0xC0:
				case 0xD0:
					var mcm = message as MidiMessageByte;
					m = mcm.Status;
					//m |= mcmdw.Data2 << 0;
					m |= mcm.Data1 << 8;
					Send(deviceHandle, m);
					break;
			}

		}
		/// <summary>
		/// Packs a MIDI message as an int
		/// </summary>
		/// <param name="status">The status byte</param>
		/// <param name="data1">The first data byte</param>
		/// <param name="data2">The second data byte</param>
		/// <param name="channel">The channel</param>
		/// <returns>An integer representing the packed MIDI message</returns>
		public static int PackMessage(byte status, byte data1, byte data2,byte channel = 0)
		{
			if(0==channel)
				return ((data2 & 0x7F) << 16) +
				((data1 & 0x7F) << 8) + status;
			return ((data2 & 0x7F) << 16) +
			((data1 & 0x7F) << 8) + ((status&0xF0)|(channel&0xF));

		}
	
	}
}
