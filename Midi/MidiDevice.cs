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
		#endregion
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
	/// <summary>
	/// Represents the arguments for an incoming MIDI event
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiEventArgs : EventArgs
	{
		/// <summary>
		/// Constructs a new MIDI event arguments instance
		/// </summary>
		/// <param name="message">The associated MIDI message</param>
		public MidiEventArgs(MidiMessage message)
		{
			Message = message;
		}
		/// <summary>
		/// Indicates the MIDI message associated with the event
		/// </summary>
		public MidiMessage Message { get;  }
	}
	
}
