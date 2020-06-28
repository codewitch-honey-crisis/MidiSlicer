using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace M
{
	/// <summary>
	/// Represents a base class for MIDI input and output devices
	/// </summary>
#if MIDILIB
	public
#endif
	abstract class MidiDevice : IDisposable
	{
		#region Win32
		[DllImport("winmm.dll")]
		static extern int midiOutGetNumDevs();
		[DllImport("winmm.dll")]
		static extern int midiInGetNumDevs();
		#endregion
		/// <summary>
		/// Indicates the available MIDI input devices
		/// </summary>
		public static IList<MidiInputDevice> Inputs {
			get {
				var count = midiInGetNumDevs();
				var result = new List<MidiInputDevice>(count);
				for (var i = 0; i < count; ++i)
					result.Add(new MidiInputDevice(i));
				return result;
			}
		}
		/// <summary>
		/// Indicates the available MIDI output devices
		/// </summary>
		public static IList<MidiOutputDevice> Outputs {
			get {
				var count = midiOutGetNumDevs();
				var result = new List<MidiOutputDevice>(count);
				for (var i = 0;i<count;++i)
					result.Add(new MidiOutputDevice(i));
				return result;
			}
		}
		
		/// <summary>
		/// Indicates the available MIDI streaming devices
		/// </summary>
		public static IList<MidiStream> Streams {
			get {
				var count = midiOutGetNumDevs();
				var result = new List<MidiStream>(count);
				for (var i = 0; i < count; ++i)
				{
					result.Add(new MidiStream(i));
				}
				return result;
			}
		}
		/// <summary>
		/// Indicates the name of the MIDI device
		/// </summary>
		public abstract string Name { get; }
		/// <summary>
		/// Indicates the index of the MIDI device
		/// </summary>
		public abstract int Index { get; }
		/// <summary>
		/// Indicates whether or not this device is open
		/// </summary>
		public abstract bool IsOpen { get; }
		/// <summary>
		/// Opens the MIDI device
		/// </summary>
		public abstract void Open();
		/// <summary>
		/// Closes the MIDI device
		/// </summary>
		public abstract void Close();

		void IDisposable.Dispose()
		{
			Close();
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Closes the device if it's open
		/// </summary>
		~MidiDevice()
		{
			Close();
		}
	}
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
		public TimeSpan TimeStamp {get;}
		/// <summary>
		/// The message
		/// </summary>
		public MidiMessage Message { get; }
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="timeStamp">The timestamp</param>
		/// <param name="message">The message</param>
		public MidiInputEventArgs(TimeSpan timeStamp,MidiMessage message)
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
		Closed=-1,
		/// <summary>
		/// The device has been started
		/// </summary>
		Started =0,
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
		struct MIDIINCAPS
		{
			public ushort wMid;
			public ushort wPid;
			public uint vDriverVersion;     // MMVERSION
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szPname;
			public uint dwSupport;
		}
		[StructLayout(LayoutKind.Sequential)]
		struct MIDIHDR
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
		internal MidiInputDevice(int deviceIndex,int bufferSize=65536)
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
		/// Opens the MIDI input device
		/// </summary>
		public override void Open()
		{
			Close();
			_CheckOutResult(midiInOpen(out _handle, _index, _callback, 0, CALLBACK_FUNCTION));
			var sz = Marshal.SizeOf(typeof(MIDIHDR));
			_inHeader.dwBufferLength = _inHeader.dwBytesRecorded= unchecked((uint)_bufferSize);
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
			if(MidiInputDeviceState.Closed!=_state)
			{
				if (MidiInputDeviceState.Started == _state)
					Stop();
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
			switch(msg)
			{
				case MIM_OPEN:
					Opened?.Invoke(this, EventArgs.Empty);
					break;
				case MIM_CLOSE:
					Closed?.Invoke(this, EventArgs.Empty);
					break;
				case MIM_DATA:
					Input?.Invoke(this,new MidiInputEventArgs(new TimeSpan(0, 0, 0, 0, wparam), MidiUtility.UnpackMessage(lparam)));
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
					if(MIM_LONGDATA==msg)
						Input?.Invoke(this,new MidiInputEventArgs(new TimeSpan(0,0,0,0,wparam), new MidiMessageSysex(status, payload)));
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
	/// <summary>
	/// Represents a MIDI output device
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiOutputDevice : MidiDevice
	{
		#region Win32
		[DllImport("winmm.dll")]
		static extern int midiOutGetDevCaps(int deviceIndex,
			ref MIDIOUTCAPS caps, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutGetErrorText(int errCode,
		   StringBuilder message, int sizeOfMessage);
		[DllImport("winmm.dll")]
		static extern int midiOutShortMsg(IntPtr handle, int message);
		[DllImport("winmm.dll")]
		static extern int midiOutLongMsg(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutPrepareHeader(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutUnprepareHeader(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutOpen(ref IntPtr handle, int deviceID,
			IntPtr proc, int instance, int flags);
		[DllImport("winmm.dll")]
		static extern int midiOutClose(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiOutGetVolume(IntPtr handle, out int volume);
		[DllImport("winmm.dll")]
		static extern int midiOutSetVolume(IntPtr handle, int volume);
		[DllImport("winmm.dll")]
		static extern int midiOutReset(IntPtr handle);
		
		[StructLayout(LayoutKind.Sequential)]
		struct MIDIHDR
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
		[StructLayout(LayoutKind.Sequential)]
		struct MIDIOUTCAPS
		{
			public ushort wMid;
			public ushort wPid;
			public int vDriverVersion;     //MMVERSION
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szPname;
			public ushort wTechnology;
			public ushort wVoices;
			public ushort wNotes;
			public ushort wChannelMask;
			public uint dwSupport;
		}
		const int MHDR_DONE = 1;
		const int MHDR_PREPARED = 2;
		#endregion
		readonly int _index;
		readonly MIDIOUTCAPS _caps;
		IntPtr _handle;
		internal MidiOutputDevice(int index)
		{
			_index = index;
			_CheckOutResult(midiOutGetDevCaps(index, ref _caps, Marshal.SizeOf(typeof(MIDIOUTCAPS))));
			_handle = IntPtr.Zero;
		}
		/// <summary>
		/// Indicates the name of the MIDI output device
		/// </summary>
		public override string Name => _caps.szPname;
		/// <summary>
		/// Indicates the version of the driver associated with the device
		/// </summary>
		public Version Version {
			get {
				return new Version(_caps.vDriverVersion >> 16, _caps.vDriverVersion & 0xFFFF);
			}
		}
		/// <summary>
		/// Indicates the volume of the device
		/// </summary>
		public MidiVolume Volume {
			get {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The device is closed.");
				int vol;
				_CheckOutResult(midiOutGetVolume(_handle, out vol));
				return new MidiVolume(unchecked((byte)(vol & 0xFF)), unchecked((byte)(vol >> 8)));
			}
			set {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The device is closed.");
				_CheckOutResult(midiOutSetVolume(_handle, value.Right<<8|value.Left));
			}
		}
		/// <summary>
		/// Indicates the device index of the MIDI output device
		/// </summary>
		public override int Index => _index;
		/// <summary>
		/// Indicates whether or not this device is open
		/// </summary>
		public override bool IsOpen => IntPtr.Zero != _handle;
		/// <summary>
		/// Opens the MIDI output device
		/// </summary>
		public override void Open()
		{
			Close();
			_CheckOutResult(midiOutOpen(ref _handle, _index, IntPtr.Zero, 0, 0));
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("Unable to open MIDI output device");
		}
		/// <summary>
		/// Closes the MIDI output device
		/// </summary>
		public override void Close()
		{
			if(IntPtr.Zero !=_handle)
			{
				midiOutClose(_handle);
				_handle = IntPtr.Zero;
			}
		}
		/// <summary>
		/// Sends a message out immediately
		/// </summary>
		/// <param name="message">The message to send</param>
		public void Send(MidiMessage message)
		{
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("The device is closed.");
			if (null == message)
				throw new ArgumentNullException("message");
			if (0xF0 == (message.Status & 0xF0))
			{
				if (0xF != message.Channel)
				{
					var data = MidiUtility.ToMessageBytes(message);
					if (null == data)
						return;
					if (data.Length > (64 * 1024))
						throw new InvalidOperationException("The buffer cannot exceed 64k");

					var hdrSize = Marshal.SizeOf(typeof(MIDIHDR));
					var hdr = new MIDIHDR();
					var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
					try
					{
						hdr.lpData = handle.AddrOfPinnedObject();
						hdr.dwBufferLength = (uint)data.Length;
						hdr.dwFlags = 0;
						_CheckOutResult(midiOutPrepareHeader(_handle, ref hdr, hdrSize));
						while ((hdr.dwFlags & MHDR_PREPARED) != MHDR_PREPARED)
						{
							Thread.Sleep(1);
						}
						_CheckOutResult(midiOutLongMsg(_handle, ref hdr, hdrSize));
						while ((hdr.dwFlags & MHDR_DONE) != MHDR_DONE)
						{
							Thread.Sleep(1);
						}
						_CheckOutResult(midiOutUnprepareHeader(_handle, ref hdr, hdrSize));
					}
					finally
					{
						handle.Free();

					}
				}
			}
			else
			{
				_CheckOutResult(midiOutShortMsg(_handle, MidiUtility.PackMessage(message)));
			}
		}
		/// <summary>
		/// Retrieves the MIDI stream associated with this output device
		/// </summary>
		public MidiStream Stream {
			get {
				return new MidiStream(Index);
			}
		}
		/// <summary>
		/// Resets the MIDI output.
		/// </summary>
		/// <remarks>Terminates any sysex messages and sends note offs to all channels, as well as turning off the sustain controller for each channel</remarks>
		public void Reset()
		{
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("The stream is closed.");
			_CheckOutResult(midiOutReset(_handle));
		}
		static string _GetMidiOutErrorMessage(int errorCode)
		{
			var result = new StringBuilder(256);
			midiOutGetErrorText(errorCode, result, result.Capacity);
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
