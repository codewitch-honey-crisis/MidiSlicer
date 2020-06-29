using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace M
{
	/// <summary>
	/// Indicates the state of the MIDI stream
	/// </summary>
#if MIDILIB
	public
#endif
	enum MidiStreamState
	{
		/// <summary>
		/// The stream is closed
		/// </summary>
		Closed = -1,
		/// <summary>
		/// The stream is paused
		/// </summary>
		Paused = 0,
		/// <summary>
		/// The stream is stopped
		/// </summary>
		Stopped = 1,
		/// <summary>
		/// The stream is playing
		/// </summary>
		Playing = 2
	}
	/// <summary>
	/// Represents a MIDI stream
	/// </summary>
#if MIDILIB
	public
#endif
	class MidiStream : IDisposable
	{
		#region Win32
		delegate void MidiOutProc(IntPtr handle, int msg, int instance, int param1, int param2);
		[DllImport("winmm.dll")]
		static extern int midiStreamOpen(ref IntPtr handle, ref int deviceID, int cMidi,
			MidiOutProc proc, int instance, int flags);
		[DllImport("winmm.dll")]
		static extern int midiStreamProperty(IntPtr handle, ref MIDIPROPTEMPO tempo, int dwProperty);
		[DllImport("winmm.dll")]
		static extern int midiStreamProperty(IntPtr handle, ref MIDIPROPTIMEDIV timeDiv, int dwProperty);
		[DllImport("winmm.dll")]
		static extern int midiStreamOut(IntPtr handle, ref MIDIHDR lpMidiOutHdr, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiStreamClose(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiStreamRestart(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiStreamPause(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiStreamStop(IntPtr handle);

		[DllImport("winmm.dll")]
		static extern int midiOutPrepareHeader(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutUnprepareHeader(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);
		
		[DllImport("winmm.dll")]
		static extern int midiStreamPosition(IntPtr handle, ref MMTIME lpMMTime, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutShortMsg(IntPtr handle, int message);
		[DllImport("winmm.dll")]
		static extern int midiOutLongMsg(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutGetVolume(IntPtr handle, out int volume);
		[DllImport("winmm.dll")]
		static extern int midiOutSetVolume(IntPtr handle, int volume);
		[DllImport("winmm.dll")]
		static extern int midiOutReset(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiOutGetErrorText(int errCode,
		   StringBuilder message, int sizeOfMessage);

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
		private struct MIDIPROPTIMEDIV
		{
			public int cbStruct;
			public int dwTimeDiv;
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct MIDIPROPTEMPO
		{
			public int cbStruct;
			public int dwTempo;
		}
		[StructLayout(LayoutKind.Explicit)]
		private struct MMTIME
		{
			[FieldOffset(0)] public int wType;
			[FieldOffset(4)] public int ms;
			[FieldOffset(4)] public int sample;
			[FieldOffset(4)] public int cb;
			[FieldOffset(4)] public int ticks;
			[FieldOffset(4)] public byte smpteHour;
			[FieldOffset(5)] public byte smpteMin;
			[FieldOffset(6)] public byte smpteSec;
			[FieldOffset(7)] public byte smpteFrame;
			[FieldOffset(8)] public byte smpteFps;
			[FieldOffset(9)] public byte smpteDummy;
			[FieldOffset(10)] public byte pad0;
			[FieldOffset(11)] public byte pad1;
			[FieldOffset(4)] public int midiSongPtrPos;
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct MIDIEVENT_SHORT
		{
			public int dwDeltaTime;
			public int dwStreamId;
			public int dwEvent;
		}
		const int MEVT_TEMPO = 0x01;
		const int MEVT_NOP = 0x02;
		const int CALLBACK_FUNCTION = 196608;
		const int MOM_OPEN = 0x3C7;
		const int MOM_CLOSE = 0x3C8;
		const int MOM_DONE = 0x3C9;
		const int TIME_MS = 0x0001;
		const int TIME_BYTES = 0x0004;
		const int TIME_SMPTE = 0x0008;
		const int TIME_MIDI = 0x0010;
		const int TIME_TICKS = 0x0020;
		const int MIDIPROP_SET = unchecked((int)0x80000000);
		const int MIDIPROP_GET = 0x40000000;
		const int MIDIPROP_TIMEDIV = 1;
		const int MIDIPROP_TEMPO = 2;
		const int MHDR_DONE = 1;
		const int MHDR_PREPARED = 2;
		const int MEVT_F_LONG = unchecked((int)0x80000000);

		#endregion
		const int _DefaultEventBlockSize = 100;
		int _deviceIndex;
		IntPtr _handle;
		MidiOutProc _callback;
		MIDIHDR _sendHeader;
		IntPtr _sendEventBuffer;
		int _sendEventBlockSize;
		int _sendPosition;
		IEnumerator<MidiEvent> _sendQueue;
		List<MidiEvent> _sendQueueBuffer;
		MidiStreamState _state = MidiStreamState.Closed;
		internal MidiStream(int deviceIndex)
		{
			if (0>deviceIndex)
				throw new ArgumentOutOfRangeException("deviceIndex");
			_deviceIndex = deviceIndex;
			_handle = IntPtr.Zero;
			_sendHeader= default(MIDIHDR);
			_sendEventBuffer= IntPtr.Zero;
			_sendPosition = -1;
			_sendQueue = null;
			_sendEventBlockSize = _DefaultEventBlockSize;
			_sendQueueBuffer = new List<MidiEvent>(_DefaultEventBlockSize);
			_callback = new MidiOutProc(_MidiOutProc);
		}
		
		/// <summary>
		/// Raised when a Send() operation has completed. This only applies to sending MidiEvent items
		/// </summary>
		public event EventHandler SendComplete;
		/// <summary>
		/// Raised when the stream is opened
		/// </summary>
		public event EventHandler Opened;
		/// <summary>
		/// Raised when the stream is closed
		/// </summary>
		public event EventHandler Closed;
		/// <summary>
		/// Indicates the state of the MIDI stream
		/// </summary>
		public MidiStreamState State => _state;
		/// <summary>
		/// Opens the stream
		/// </summary>
		public void Open()
		{
			if (IntPtr.Zero!= _handle)
				throw new InvalidOperationException("The device is already open");
			_sendEventBuffer = Marshal.AllocHGlobal(64 * 1024);
			var di = _deviceIndex;
			_CheckOutResult(midiStreamOpen(ref _handle, ref di, 1, _callback, 0, CALLBACK_FUNCTION));
			_state = MidiStreamState.Paused;
		}
		/// <summary>
		/// Closes the stream
		/// </summary>
		public void Close()
		{
			if (IntPtr.Zero != _handle) {
				Stop();
				Reset();
				_CheckOutResult(midiStreamClose(_handle));
				_handle = IntPtr.Zero;
				Marshal.FreeHGlobal(_sendEventBuffer);
				_sendEventBuffer = IntPtr.Zero;
				GC.SuppressFinalize(this);
				_sendPosition = -1;
				if(null!=_sendQueue)
				{
					_sendQueue.Dispose();
					_sendQueue = null;
				}
				_state = MidiStreamState.Closed;
			}
		}
		/// <summary>
		/// Sends a MIDI event to the stream
		/// </summary>
		/// <param name="events">The events to send</param>
		public void Send(params MidiEvent[] events)
			=> Send((IEnumerable<MidiEvent>)events);
		/// <summary>
		/// Sends a MIDI event to the stream
		/// </summary>
		/// <param name="events">The events to send</param>
		/// <param name="eventBlockSize">The number of events to send to the device in each block</param>
		public void Send(IEnumerable<MidiEvent> events, int eventBlockSize=_DefaultEventBlockSize)
		{
			if(-1!=_sendPosition)
			{
				throw new InvalidOperationException("The device is already sending");
			}
			_sendEventBlockSize = eventBlockSize;
			_sendQueueBuffer.Clear();
			_sendQueue = events.GetEnumerator();
			for(Interlocked.Exchange(ref _sendPosition,0);_sendPosition<eventBlockSize;Interlocked.Increment(ref _sendPosition))
			{
				if (!_sendQueue.MoveNext())
					break;
				_sendQueueBuffer.Add(_sendQueue.Current);
			}
			SendDirect(_sendQueueBuffer);
		}
		/// <summary>
		/// Sends events directly to the event queue without buffering
		/// </summary>
		/// <param name="events">The events to send</param>
		/// <remarks>The total size of the events must be less than 64kb</remarks>
		public void SendDirect(IEnumerable<MidiEvent> events)
		{
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("The stream is closed.");
			if (IntPtr.Zero != _sendHeader.lpData)
				throw new InvalidOperationException("The stream is busy playing.");
			if (null == events)
				throw new ArgumentNullException("events");
			int baseEventSize = Marshal.SizeOf(typeof(MIDIEVENT_SHORT));
			int blockSize = 0;
			foreach (var @event in events)
			{
				if (0xF0 != (@event.Message.Status & 0xF0))
				{
					blockSize += baseEventSize;
				}
				else if (0xFF == @event.Message.Status)
				{
					var mm = @event.Message as MidiMessageMeta;
					if (0x51 == mm.Data1) // tempo
					{
						blockSize += baseEventSize;
					}
					else if (0x2f == mm.Data1) // end track 
					{
						blockSize += baseEventSize;
					}
				}
				else // sysex
				{
					var msx = @event.Message as MidiMessageSysex;
					var dl = msx.Data.Length + 1;
					if(0!=(dl%4))
					{
						dl += 4-(dl % 4);
					}
					blockSize += baseEventSize+dl;
				}
			}
			if (64 * 1024 <= blockSize)
				throw new ArgumentException("There are too many events in the event buffer - maximum size must be 64k", "events");
			IntPtr eventPointer = _sendEventBuffer;
			var ofs = 0;
			var ptrOfs = 0;
			foreach (var @event in events)
			{
				if (0xF0 != (@event.Message.Status & 0xF0))
				{
					var se = new MIDIEVENT_SHORT();
					se.dwDeltaTime = @event.Position + ofs;
					se.dwStreamId = 0;
					se.dwEvent = MidiUtility.PackMessage(@event.Message);
					Marshal.StructureToPtr(se, new IntPtr(ptrOfs + (int)eventPointer), false);
					ptrOfs += baseEventSize;
					ofs = 0;
				}
				else if (0xFF == @event.Message.Status)
				{
					var mm = @event.Message as MidiMessageMeta;
					if (0x51 == mm.Data1) // tempo
					{
						var se = new MIDIEVENT_SHORT();
						se.dwDeltaTime = @event.Position + ofs;
						se.dwStreamId = 0;
						se.dwEvent = (mm.Data[0] << 16) | (mm.Data[1] << 8) | mm.Data[2] | (MEVT_TEMPO << 24);
						Marshal.StructureToPtr(se, new IntPtr(ptrOfs + (int)eventPointer), false);
						ptrOfs += baseEventSize;
						ofs = 0;
					}
					else if (0x2f == mm.Data1) // end track 
					{
						// add a NOP message to it just to pad our output in case we're looping
						var se = new MIDIEVENT_SHORT();
						se.dwDeltaTime = @event.Position + ofs;
						se.dwStreamId = 0;
						se.dwEvent = (MEVT_NOP << 24);
						Marshal.StructureToPtr(se, new IntPtr(ptrOfs + (int)eventPointer), false);
						ptrOfs += baseEventSize;
						ofs = 0;
					} else
						ofs = @event.Position;
				}
				else // sysex
				{
					var msx = @event.Message as MidiMessageSysex;
					var se = new MIDIEVENT_SHORT();
					se.dwDeltaTime = @event.Position + ofs;
					se.dwStreamId = 0;
					se.dwEvent = MEVT_F_LONG | (msx.Data.Length + 1);
					Marshal.StructureToPtr(se, new IntPtr(ptrOfs + (int)eventPointer), false);
					ptrOfs += baseEventSize;
					Marshal.WriteByte(new IntPtr(ptrOfs + (int)eventPointer), msx.Status);
					Marshal.Copy(msx.Data,0,new IntPtr(ptrOfs + (int)eventPointer+1), msx.Data.Length);
					var dl = msx.Data.Length + 1;
					if (0 != (dl % 4))
					{
						dl += 4-(dl % 4);
					}
					ptrOfs += dl;
					ofs = 0;
				}			
			}
			_sendHeader = default(MIDIHDR);
			Interlocked.Exchange(ref _sendHeader.lpData, eventPointer);
			_sendHeader.dwBufferLength = _sendHeader.dwBytesRecorded = unchecked((uint)blockSize);
			_sendEventBuffer = eventPointer;
			int headerSize = Marshal.SizeOf(typeof(MIDIHDR));
			_CheckOutResult(midiOutPrepareHeader(_handle, ref _sendHeader, headerSize));
			_CheckOutResult(midiStreamOut(_handle, ref _sendHeader, headerSize));

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
		
		void IDisposable.Dispose()
		{
			Close();
		}
		/// <summary>
		/// Destroys this instance
		/// </summary>
		~MidiStream()
		{
			Close();
		}
		void _MidiOutProc(IntPtr handle, int msg, int instance, int param1, int param2)
		{
			switch(msg)
			{
				case MOM_OPEN:
					Opened?.Invoke(this, EventArgs.Empty);
					break;
				case MOM_CLOSE:
					Closed?.Invoke(this, EventArgs.Empty);
					break;
				case MOM_DONE:

					if (IntPtr.Zero != _sendHeader.lpData)
					{
						_CheckOutResult(midiOutUnprepareHeader(_handle, ref _sendHeader, Marshal.SizeOf(typeof(MIDIHDR))));
						Interlocked.Exchange(ref _sendHeader.lpData,IntPtr.Zero);
					}

					if(-1==_sendPosition)
						SendComplete?.Invoke(this, EventArgs.Empty);
					else
					{
						_sendQueueBuffer.Clear();
						var ne = _sendPosition + _sendEventBlockSize;
						var done = false;
						for(;_sendPosition<ne;Interlocked.Increment(ref _sendPosition))
						{
							if (!_sendQueue.MoveNext())
							{
								done = true;
								break;
							}
							_sendQueueBuffer.Add(_sendQueue.Current);
						}
						SendDirect(_sendQueueBuffer);
						if (done)
							Interlocked.Exchange(ref _sendPosition, -1);
					}
					break;
				
			}
		}
		/// <summary>
		/// Starts the stream
		/// </summary>
		public void Start()
		{
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("The stream is closed.");
			switch(_state)
			{
				case MidiStreamState.Paused:
				case MidiStreamState.Stopped:
					_CheckOutResult(midiStreamRestart(_handle));
					_state = MidiStreamState.Playing;
					break;		
			}
		}
		/// <summary>
		/// Stops the stream
		/// </summary>
		public void Stop()
		{
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("The stream is closed.");
			switch (_state)
			{
				case MidiStreamState.Paused:
				case MidiStreamState.Playing:
					_CheckOutResult(midiStreamStop(_handle));
					_state = MidiStreamState.Stopped;
					_sendPosition = -1;
					if (null != _sendQueue)
					{
						_sendQueue.Dispose();
						_sendQueue = null;
					}
					break;
			}
		}
		/// <summary>
		/// Pauses the stream
		/// </summary>
		public void Pause()
		{
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("The stream is closed.");
			switch (_state)
			{
				case MidiStreamState.Playing:
					_CheckOutResult(midiStreamPause(_handle));
					_state = MidiStreamState.Paused;
					break;
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
		/// <summary>
		/// Indicates the position in ticks
		/// </summary>
		public int PositionTicks {
			get {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The stream is closed.");
				switch (_state)
				{
					case MidiStreamState.Playing:
					case MidiStreamState.Paused:
						MMTIME mm = new MMTIME();
						mm.wType = TIME_TICKS;
						_CheckOutResult(midiStreamPosition(_handle, ref mm, Marshal.SizeOf(typeof(MMTIME))));
						if (TIME_TICKS != mm.wType)
							throw new NotSupportedException("The position format is not supported.");
						return mm.ticks;
					default:
						return 0;
				}
			}
		}
		/// <summary>
		/// Indicates the position in milliseconds
		/// </summary>
		public int PositionMilliseconds {
			get {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The stream is closed.");
				switch (_state)
				{
					case MidiStreamState.Playing:
					case MidiStreamState.Paused:
						MMTIME mm = new MMTIME();
						mm.wType = TIME_MS;
						_CheckOutResult(midiStreamPosition(_handle, ref mm, Marshal.SizeOf(typeof(MMTIME))));
						if (TIME_MS != mm.wType)
							throw new NotSupportedException("The position format is not supported.");
						return mm.ms;
					default:
						return 0;
				}
			}
		}
		/// <summary>
		/// Indicates the song pointer position
		/// </summary>
		public int PositionSongPointer {
			get {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The stream is closed.");
				switch (_state)
				{
					case MidiStreamState.Playing:
					case MidiStreamState.Paused:
						MMTIME mm = new MMTIME();
						mm.wType = TIME_MIDI;
						_CheckOutResult(midiStreamPosition(_handle, ref mm, Marshal.SizeOf(typeof(MMTIME))));
						if (TIME_MIDI != mm.wType)
							throw new NotSupportedException("The position format is not supported.");
						return mm.midiSongPtrPos;
					default:
						return 0;
				}
			}
		}
		/// <summary>
		/// Indicates the position in bytes
		/// </summary>
		public int PositionBytes {
			get {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The stream is closed.");
				switch (_state)
				{
					case MidiStreamState.Playing:
					case MidiStreamState.Paused:
						MMTIME mm = new MMTIME();
						mm.wType = TIME_BYTES;
						_CheckOutResult(midiStreamPosition(_handle, ref mm, Marshal.SizeOf(typeof(MMTIME))));
						if (TIME_BYTES != mm.wType)
							throw new NotSupportedException("The position format is not supported.");
						return mm.cb;
					default:
						return 0;
				}
			}
		}
		/// <summary>
		/// Indicates the position in SMPTE format
		/// </summary>
		public MidiSmpteTime PositionSmpte {
			get {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The stream is closed.");
				switch (_state)
				{
					case MidiStreamState.Playing:
					case MidiStreamState.Paused:
						MMTIME mm = new MMTIME();
						mm.wType = TIME_SMPTE;
						_CheckOutResult(midiStreamPosition(_handle, ref mm, Marshal.SizeOf(typeof(MMTIME))));
						if (TIME_SMPTE != mm.wType)
							throw new NotSupportedException("The position format is not supported.");
						return new MidiSmpteTime(new TimeSpan(0, mm.smpteHour, mm.smpteMin, mm.smpteSec, 0), mm.smpteFrame, mm.smpteFps);
					default:
						return default(MidiSmpteTime);
				}
			}
		}
		/// <summary>
		/// Indicates the MicroTempo of the stream
		/// </summary>
		public int MicroTempo {
			get {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The stream is closed.");
				var t = new MIDIPROPTEMPO();
				t.cbStruct = Marshal.SizeOf(typeof(MIDIPROPTEMPO));
				_CheckOutResult(midiStreamProperty(_handle, ref t, MIDIPROP_GET | MIDIPROP_TEMPO));
				return unchecked((short)t.dwTempo);
			}
			set {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The stream is closed.");
				var t = new MIDIPROPTEMPO();
				t.cbStruct = Marshal.SizeOf(typeof(MIDIPROPTEMPO));
				t.dwTempo= value;
				_CheckOutResult(midiStreamProperty(_handle, ref t, MIDIPROP_SET | MIDIPROP_TEMPO));
			}
		}
		/// <summary>
		/// Indicates the Tempo of the stream
		/// </summary>
		public double Tempo {
			get {
				return MidiUtility.MicroTempoToTempo(MicroTempo);
			}
			set {
				MicroTempo = MidiUtility.TempoToMicroTempo(value);
			}
		}
		/// <summary>
		/// Indicates the TimeBase of the stream
		/// </summary>
		public short TimeBase {
			get {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The stream is closed.");
				var tb = new MIDIPROPTIMEDIV();
				tb.cbStruct = Marshal.SizeOf(typeof(MIDIPROPTIMEDIV));
				_CheckOutResult(midiStreamProperty(_handle, ref tb, MIDIPROP_GET | MIDIPROP_TIMEDIV));
				return unchecked((short)tb.dwTimeDiv);
			}
			set {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The stream is closed.");
				var tb = new MIDIPROPTIMEDIV();
				tb.cbStruct = Marshal.SizeOf(typeof(MIDIPROPTIMEDIV));
				tb.dwTimeDiv = value;
				_CheckOutResult(midiStreamProperty(_handle, ref tb, MIDIPROP_SET | MIDIPROP_TIMEDIV));
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
