using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace M
{
	/// <summary>
	/// Represents the kind of volume settings are available for the device
	/// </summary>
#if MIDILIB
	public
#endif
	enum MidiOutputDeviceVolumeSupport
	{
		/// <summary>
		/// Volume controls are not available
		/// </summary>
		None = 0,
		/// <summary>
		/// Only mono or single volume controls are supported
		/// </summary>
		Mono = 1,
		/// <summary>
		/// Stereo volume controls are supported
		/// </summary>
		Stereo = 2
	}
	/// <summary>
	/// Represents the kind of MIDI output device
	/// </summary>
#if MIDILIB
	public
#endif
	enum MidiOutputDeviceKind
	{
		/// <summary>
		/// Unknown MIDI device.
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// MIDI Port
		/// </summary>
		MidiPort = 1,
		/// <summary>
		/// Synthesizer
		/// </summary>
		Synthesizer = 2,
		/// <summary>
		/// Square wave synthesizer
		/// </summary>
		SquareWaveSynthesizer = 3,
		/// <summary>
		/// FM synthesizer
		/// </summary>
		FMSynthesizer = 4,
		/// <summary>
		/// MIDI mapper
		/// </summary>
		MidiMapper = 5,
		/// <summary>
		/// Wavetable synthesizer
		/// </summary>
		WavetableSynthesizer = 6,
		/// <summary>
		/// Software synthesizer
		/// </summary>
		SoftwareSynthesizer = 7
	}
	/// <summary>
	/// Represents the supported channels for the MIDI output device
	/// </summary>
	[Flags]
#if MIDILIB
	public
#endif
	enum MidiChannels : short
	{
		/// <summary>
		/// Channel 0
		/// </summary>
		Channel0 = 0x0001,
		/// <summary>
		/// Channel 1
		/// </summary>
		Channel1 = 0x0002,
		/// <summary>
		/// Channel 2
		/// </summary>
		Channel2 = 0x0004,
		/// <summary>
		/// Channel 3
		/// </summary>
		Channel3 = 0x0008,
		/// <summary>
		/// Channel 4
		/// </summary>
		Channel4 = 0x0010,
		/// <summary>
		/// Channel 5
		/// </summary>
		Channel5 = 0x0020,
		/// <summary>
		/// Channel 6
		/// </summary>
		Channel6 = 0x0040,
		/// <summary>
		/// Channel 7
		/// </summary>
		Channel7 = 0x0080,
		/// <summary>
		/// Channel 8
		/// </summary>
		Channel8 = 0x0100,
		/// <summary>
		/// Channel 9
		/// </summary>
		Channel9 = 0x0200,
		/// <summary>
		/// Channel 10
		/// </summary>
		Channel10 = 0x0400,
		/// <summary>
		/// Channel 11
		/// </summary>
		Channel11 = 0x0800,
		/// <summary>
		/// Channel 12
		/// </summary>
		Channel12 = 0x1000,
		/// <summary>
		/// Channel 13
		/// </summary>
		Channel13 = 0x2000,
		/// <summary>
		/// Channel 14
		/// </summary>
		Channel14 = 0x4000,
		/// <summary>
		/// Channel 15
		/// </summary>
		Channel15 = unchecked((short)0x8000)
	}
	/// <summary>
	/// Represents a MIDI output device
	/// </summary>
#if MIDILIB
	public
#endif
	class MidiOutputDevice : MidiDevice
	{
		#region Win32
		delegate void MidiOutProc(IntPtr handle, int msg, int instance, IntPtr param1, IntPtr param2);
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
			MidiOutProc proc, int instance, int flags);
		[DllImport("winmm.dll")]
		static extern int midiOutClose(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiOutGetVolume(IntPtr handle, out int volume);
		[DllImport("winmm.dll")]
		static extern int midiOutSetVolume(IntPtr handle, int volume);
		[DllImport("winmm.dll")]
		static extern int midiOutReset(IntPtr handle);


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
		[StructLayout(LayoutKind.Sequential)]
		private struct MIDIOUTCAPS
		{
			public short wMid;
			public short wPid;
			public int vDriverVersion;     //MMVERSION
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szPname;
			public ushort wTechnology;
			public short wVoices;
			public short wNotes;
			public ushort wChannelMask;
			public uint dwSupport;
		}
		const int MHDR_DONE = 1;
		const int MHDR_PREPARED = 2;
		const uint MIDICAPS_VOLUME = 1;      // supports volume control
		const uint MIDICAPS_LRVOLUME = 2;    // separate left-right volume control
		const uint MIDICAPS_CACHE = 4;
		const uint MIDICAPS_STREAM = 8;      // driver supports midiStreamOut directly
		const int CALLBACK_FUNCTION = 196608;
		const int MOM_OPEN = 0x3C7;
		const int MOM_CLOSE = 0x3C8;


		#endregion
		MidiOutProc _outCallback;
		readonly int _index;
		readonly MIDIOUTCAPS _caps;
		IntPtr _handle;
		internal MidiOutputDevice(int index)
		{
			_index = index;
			_CheckOutResult(midiOutGetDevCaps(index, ref _caps, Marshal.SizeOf(typeof(MIDIOUTCAPS))));
			_handle = IntPtr.Zero;
			_outCallback = new MidiOutProc(_MidiOutProc);

		}
		/// <summary>
		/// Raised when the device is opened
		/// </summary>
		public event EventHandler Opened;
		/// <summary>
		/// Invokes the opened event
		/// </summary>
		protected void OnOpened(EventArgs args)
		{
			Opened?.Invoke(this, args);
		}
		/// <summary>
		/// Raised when the device is closed
		/// </summary>
		public event EventHandler Closed;

		/// <summary>
		/// Invokes the closed event
		/// </summary>
		protected void OnClosed(EventArgs args)
		{
			Closed?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the handle of the device
		/// </summary>
		protected IntPtr Handle {
			get {
				return _handle;
			}
			set {
				Interlocked.Exchange(ref _handle, value);
			}
		}
	
		/// <summary>
		/// Indicates the name of the MIDI output device
		/// </summary>
		public override string Name => _caps.szPname;
		/// <summary>
		/// Indicates the kind of MIDI output device
		/// </summary>
		public MidiOutputDeviceKind Kind => (MidiOutputDeviceKind)_caps.wTechnology;
		/// <summary>
		/// Indicates whether or not the device supports hardware accelerated streaming
		/// </summary>
		public virtual bool SupportsHardwareStreaming {
			get {
				return MIDICAPS_STREAM == (_caps.dwSupport & MIDICAPS_STREAM);
			}
		}
		/// <summary>
		/// Indicates whether or not the device supports patch caching
		/// </summary>
		public bool SupportsPatchCaching {
			get {
				return MIDICAPS_CACHE == (_caps.dwSupport & MIDICAPS_CACHE);
			}
		}
		/// <summary>
		/// Indicates the channels which the MIDI device responds to
		/// </summary>
		/// <remarks>These are flags</remarks>
		public MidiChannels Channels {
			get {
				return (MidiChannels)_caps.wChannelMask;
			}
		}
		/// <summary>
		/// Indicates the number of voices the device supports or 0 if it can't be determined
		/// </summary>
		public short VoiceCount {
			get {
				return _caps.wVoices;
			}
		}
		/// <summary>
		/// Indicates the number of simultaneous notes the device supports or 0 if it can't be determined
		/// </summary>
		public short NoteCount {
			get {
				return _caps.wNotes;
			}
		}
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
		/// Indicates what kind of volume control is supported, if any
		/// </summary>
		public virtual MidiOutputDeviceVolumeSupport VolumeSupport {
			get {
				if (MIDICAPS_VOLUME == (_caps.dwSupport & MIDICAPS_VOLUME))
				{
					if (MIDICAPS_LRVOLUME == (_caps.dwSupport & MIDICAPS_LRVOLUME))
					{
						return MidiOutputDeviceVolumeSupport.Stereo;
					}
					return MidiOutputDeviceVolumeSupport.Mono;
				}
				return MidiOutputDeviceVolumeSupport.None;
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
				_CheckOutResult(midiOutSetVolume(_handle, value.Right << 8 | value.Left));
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
			_CheckOutResult(midiOutOpen(ref _handle, _index, _outCallback, 0,CALLBACK_FUNCTION));
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("Unable to open MIDI output device");
		}
		/// <summary>
		/// Closes the MIDI output device
		/// </summary>
		public override void Close()
		{
			if (IntPtr.Zero != _handle)
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
			if (0xF0 == message.Status) // sysex
			{
				var data = MidiUtility.ToMessageBytes(message);
				if (null == data)
					return;
				if (254 < data.Length)
				{
					var len = 254;
					for (var i = 0; i < data.Length; i += len)
					{
						if (data.Length <= i + len )
						{
							len = data.Length - i ;
						}
						_SendRaw(data,i,len);
						
					}
				} else
					_SendRaw(data,0,data.Length);
				
			}
			else
			{
				_CheckOutResult(midiOutShortMsg(_handle, MidiUtility.PackMessage(message)));
			}
		}

		void _SendRaw(byte[] data,int startIndex,int length)
		{
			var hdrSize = Marshal.SizeOf(typeof(MIDIHDR));
			var hdr = new MIDIHDR();
			var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				hdr.lpData = new IntPtr(handle.AddrOfPinnedObject().ToInt64()+startIndex);
				hdr.dwBufferLength = hdr.dwBytesRecorded = (uint)(length);
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
		void _MidiOutProc(IntPtr handle, int msg, int instance, IntPtr param1, IntPtr param2)
		{
			switch (msg)
			{
				case MOM_OPEN:
					OnOpened(EventArgs.Empty);
					break;
				case MOM_CLOSE:
					OnClosed(EventArgs.Empty);
					break;
				

			}
		}
	}
}
