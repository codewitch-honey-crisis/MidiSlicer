using System;
using System.Runtime.InteropServices;
using System.Text;

namespace M
{
	/// <summary>
	/// Represents the arguments to the Input event
	/// </summary>
#if MIDILIB
	public
#endif
	class MidiInputEventArgs
	{
		/// <summary>
		/// The timestamp of the message
		/// </summary>
		public TimeSpan TimeStamp { get; }
		/// <summary>
		/// The message
		/// </summary>
		public MidiMessage Message { get; }
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="timeStamp">The timestamp</param>
		/// <param name="message">The message</param>
		public MidiInputEventArgs(TimeSpan timeStamp, MidiMessage message)
		{
			TimeStamp = timeStamp;
			Message = message;
		}
	}
	/// <summary>
	/// Represents the delegate for the Input event
	/// </summary>
	/// <param name="sender">The sender</param>
	/// <param name="args">The arguments</param>
#if MIDILIB
	public
#endif
		delegate void MidiInputEventHandler(object sender, MidiInputEventArgs args);
	/// <summary>
	/// Indicates the state of the MIDI input device
	/// </summary>
#if MIDILIB
	public
#endif
	enum MidiInputDeviceState
	{
		/// <summary>
		/// The device is closed
		/// </summary>
		Closed = -1,
		/// <summary>
		/// The device has been started
		/// </summary>
		Started = 0,
		/// <summary>
		/// The device is stopped
		/// </summary>
		Stopped = 1
	}
	/// <summary>
	/// Represents a MIDI input device
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiInputDevice : MidiDevice
	{
		#region Win32
		delegate void MidiInProc(IntPtr handle, int wMsg, int dwInstance, int dwParam1, int dwParam2);

		[DllImport("winmm.dll")]
		static extern int midiInGetErrorText(int errCode,
		   StringBuilder message, int sizeOfMessage);
		[DllImport("winmm.dll")]
		static extern int midiInGetDevCaps(int deviceIndex,
			ref MIDIINCAPS caps, int sizeOfMidiInCaps);
		[DllImport("winmm.dll")]
		static extern int midiInOpen(out IntPtr refHandle, int uDeviceID, MidiInProc dwCallback, int dwInstance, int dwFlags);
		[DllImport("winmm.dll")]
		static extern int midiInClose(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiInStart(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiInStop(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiInReset(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiInAddBuffer(IntPtr handle, ref MIDIHDR lpMidiHeader, int wSize);
		[DllImport("winmm.dll")]
		static extern int midiInPrepareHeader(IntPtr handle, ref MIDIHDR lpMidiHeader, int wSize);
		[DllImport("winmm.dll")]
		static extern int midiInUnprepareHeader(IntPtr handle, ref MIDIHDR lpMidiHeader, int wSize);
		[StructLayout(LayoutKind.Sequential)]
		private struct MIDIINCAPS
		{
			public short wMid;
			public short wPid;
			public int vDriverVersion;     // MMVERSION
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szPname;
			public uint dwSupport;
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct MIDIHDR
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
		const int CALLBACK_FUNCTION = 196608;
		const int MIM_OPEN = 961;
		const int MIM_CLOSE = 962;
		const int MIM_DATA = 963;
		const int MIM_LONGDATA = 964;
		const int MIM_ERROR = 965;
		const int MIM_LONGERROR = 966;
		const int MIM_MOREDATA = 972;

		#endregion
		readonly MidiInProc _callback;
		readonly int _index;
		MIDIINCAPS _caps;
		IntPtr _handle;
		MIDIHDR _inHeader;
		int _bufferSize;
		MidiInputDeviceState _state;
		internal MidiInputDevice(int deviceIndex, int bufferSize = 65536)
		{
			if (0 > bufferSize)
				bufferSize = 65536;
			if (0 > deviceIndex)
				throw new ArgumentOutOfRangeException("deviceIndex");
			_handle = IntPtr.Zero;
			_callback = new MidiInProc(_MidiInProc);
			_index = deviceIndex;
			_state = MidiInputDeviceState.Closed;
			_bufferSize = bufferSize;
			_CheckOutResult(midiInGetDevCaps(deviceIndex, ref _caps, Marshal.SizeOf(typeof(MIDIINCAPS))));
		}
		/// <summary>
		/// Raised when the device is opened
		/// </summary>
		public event EventHandler Opened;
		/// <summary>
		/// Raised when the device is closed
		/// </summary>
		public event EventHandler Closed;

		/// <summary>
		/// Raised when incoming messages occur
		/// </summary>
		public event MidiInputEventHandler Input;
		/// <summary>
		/// Raised when incoming messages occur
		/// </summary>
		public event MidiInputEventHandler Error;
		/// <summary>
		/// Indicates the state of the device
		/// </summary>
		public MidiInputDeviceState State => _state;
		/// <summary>
		/// Indicates whether the input device is open
		/// </summary>
		public override bool IsOpen => IntPtr.Zero != _handle;
		/// <summary>
		/// Indicates the index of the input device
		/// </summary>
		public override int Index => _index;
		/// <summary>
		/// Indicates the name of the input device
		/// </summary>
		public override string Name => _caps.szPname;
		/// <summary>
		/// Indicates the version of the driver associated with the device
		/// </summary>
		public override Version Version {
			get {
				return new Version(_caps.vDriverVersion >> 16, _caps.vDriverVersion & 0xFFFF);
			}
		}
		/// <summary>
		/// Indicates the product code for the device
		/// </summary>
		public override short ProductId {
			get {
				return _caps.wPid;
			}
		}
		/// <summary>
		/// Indicates the manufacturer code for the device
		/// </summary>
		public override short ManufacturerId {
			get {
				return _caps.wMid;
			}
		}
		/// <summary>
		/// Opens the MIDI input device
		/// </summary>
		public override void Open()
		{
			Close();
			_CheckOutResult(midiInOpen(out _handle, _index, _callback, 0, CALLBACK_FUNCTION));
			var sz = Marshal.SizeOf(typeof(MIDIHDR));
			_inHeader.dwBufferLength = _inHeader.dwBytesRecorded = unchecked((uint)_bufferSize);
			_inHeader.lpData = Marshal.AllocHGlobal(_bufferSize);
			_CheckOutResult(midiInPrepareHeader(_handle, ref _inHeader, sz));
			_CheckOutResult(midiInAddBuffer(_handle, ref _inHeader, sz));
			_state = MidiInputDeviceState.Stopped;
		}
		/// <summary>
		/// Closes the MIDI input device
		/// </summary>
		public override void Close()
		{
			if (MidiInputDeviceState.Closed != _state)
			{
				if (MidiInputDeviceState.Started == _state)
					Stop();
				// flush any pending events
				Reset();
				var sz = Marshal.SizeOf(typeof(MIDIHDR));
				var ptr = _inHeader.lpData;
				_CheckOutResult(midiInUnprepareHeader(_handle, ref _inHeader, sz));
				_CheckOutResult(midiInClose(_handle));
				Marshal.FreeHGlobal(ptr);
				_state = MidiInputDeviceState.Closed;
			}
		}
		/// <summary>
		/// Starts the MIDI input device
		/// </summary>
		public void Start()
		{
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("The device is closed.");
			_CheckOutResult(midiInStart(_handle));
			_state = MidiInputDeviceState.Started;
		}
		/// <summary>
		/// Stops the MIDI input device
		/// </summary>
		public void Stop()
		{
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("The device is closed.");
			_CheckOutResult(midiInStop(_handle));
			_state = MidiInputDeviceState.Stopped;
		}
		/// <summary>
		/// Resets the MIDI input device
		/// </summary>
		public void Reset()
		{
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("The device is closed.");
			_CheckOutResult(midiInReset(_handle));
		}
		void _MidiInProc(IntPtr handle, int msg, int instance, int lparam, int wparam)
		{
			switch (msg)
			{
				case MIM_OPEN:
					Opened?.Invoke(this, EventArgs.Empty);
					break;
				case MIM_CLOSE:
					Closed?.Invoke(this, EventArgs.Empty);
					break;
				case MIM_DATA:
					Input?.Invoke(this, new MidiInputEventArgs(new TimeSpan(0, 0, 0, 0, wparam), MidiUtility.UnpackMessage(lparam)));
					break;
				case MIM_ERROR:
					Error?.Invoke(this, new MidiInputEventArgs(new TimeSpan(0, 0, 0, 0, wparam), MidiUtility.UnpackMessage(lparam)));
					break;
				case MIM_LONGDATA:
				case MIM_LONGERROR:
					// Not tested because I can't get this to fire
					var hdr = (MIDIHDR)Marshal.PtrToStructure(new IntPtr(lparam), typeof(MIDIHDR));
					if (0 == hdr.dwBytesRecorded)
						return; // no message
					var status = Marshal.ReadByte(hdr.lpData, 0);
					//if (0xF0 != (status & 0xF0))
					//	return; // not a sysex message - not sure what to do 
					var payload = new byte[hdr.dwBytesRecorded - 1];
					Marshal.Copy(new IntPtr((int)hdr.lpData + 1), payload, 0, payload.Length);
					if (MIM_LONGDATA == msg)
						Input?.Invoke(this, new MidiInputEventArgs(new TimeSpan(0, 0, 0, 0, wparam), new MidiMessageSysex(status, payload)));
					else
						Error?.Invoke(this, new MidiInputEventArgs(new TimeSpan(0, 0, 0, 0, wparam), new MidiMessageSysex(status, payload)));
					break;
				case MIM_MOREDATA:
					break;
				default:
					break;
			}
		}
		static string _GetMidiOutErrorMessage(int errorCode)
		{
			var result = new StringBuilder(256);
			midiInGetErrorText(errorCode, result, result.Capacity);
			return result.ToString();
		}
		[System.Diagnostics.DebuggerNonUserCode()]
		static void _CheckOutResult(int errorCode)
		{
			if (0 != errorCode)
				throw new Exception(_GetMidiOutErrorMessage(errorCode));
		}
	}
}
