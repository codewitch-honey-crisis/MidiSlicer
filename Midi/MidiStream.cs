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
		Started = 2
	}
	/// <summary>
	/// Represents a MIDI stream
	/// </summary>
#if MIDILIB
	public
#endif
	class MidiStream : MidiOutputDevice
	{
		#region Win32
		delegate void MidiOutProc(IntPtr handle, int msg, int instance, IntPtr param1, IntPtr param2);
		delegate void TimerProc(IntPtr handle, int msg, int instance, IntPtr param1, IntPtr param2);
		[DllImport("winmm.dll")]
		static extern int midiStreamOpen(ref IntPtr handle, ref int deviceID, int cMidi,
			MidiOutProc proc, int instance, int flags);
		[DllImport("winmm.dll")]
		static extern int midiStreamProperty(IntPtr handle, ref MIDIPROPTEMPO tempo, int dwProperty);
		[DllImport("winmm.dll")]
		static extern int midiStreamProperty(IntPtr handle, ref MIDIPROPTIMEDIV timeDiv, int dwProperty);
		[DllImport("winmm.dll")]
		static extern int midiStreamClose(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiStreamRestart(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiStreamPause(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiStreamStop(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int midiStreamOut(IntPtr handle, IntPtr lpMidiOutHdr, int uSize);

		[DllImport("winmm.dll")]
		static extern int midiOutPrepareHeader(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutPrepareHeader(IntPtr hMidiOut, IntPtr lpMidiOutHdr, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutUnprepareHeader(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutUnprepareHeader(IntPtr hMidiOut, IntPtr lpMidiOutHdr, int uSize);

		[DllImport("winmm.dll")]
		static extern int midiStreamPosition(IntPtr handle, ref MMTIME lpMMTime, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutShortMsg(IntPtr handle, int message);
		[DllImport("winmm.dll")]
		static extern int midiOutLongMsg(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);
		[DllImport("winmm.dll")]
		static extern int midiOutGetErrorText(int errCode,
		   StringBuilder message, int sizeOfMessage);
		[DllImport("winmm.dll")]
		static extern IntPtr timeSetEvent(int delay, int resolution, TimerProc handler, IntPtr user, int eventType);
		[DllImport("winmm.dll")]
		static extern int timeKillEvent(IntPtr handle);
		[DllImport("winmm.dll")]
		static extern int timeBeginPeriod(int msec);
		[DllImport("winmm.dll")]
		static extern int timeEndPeriod(int msec);
		
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
		private struct MIDIEVENT
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
		const int TIME_ONESHOT = 0;
		static readonly int MIDIHDR_SIZE = Marshal.SizeOf(typeof(MIDIHDR));
		static readonly int MIDIEVENT_SIZE = Marshal.SizeOf(typeof(MIDIEVENT));
		
		#endregion
		static readonly int MAX_EVENTBLOCK_SIZE = 65536-MIDIHDR_SIZE;

		IntPtr _timerHandle;
		MidiOutProc _outCallback;
		TimerProc _timerCallback;
		int _sendQueuePosition;
		int _tempoSyncMessageCount;
		int _tempoSyncMessagesSentCount;
		// must be an int to use interlocked
		// 0=false, nonzero = true
		int _tempoSyncEnabled;

		List<MidiEvent> _sendQueue;
		MidiStreamState _state = MidiStreamState.Closed;
		internal MidiStream(int index) : base(index)
		{
			_sendQueue = null;
			_sendQueuePosition = 0;
			_outCallback = new MidiOutProc(_MidiOutProc);
			_timerCallback = new TimerProc(_TimerProc);
			_tempoSyncEnabled = 0;
			_tempoSyncMessageCount = 100;
			_tempoSyncMessagesSentCount = 0;
		}
		
		/// <summary>
		/// Raised when a Send() operation has completed. This only applies to sending MidiEvent items
		/// </summary>
		public event EventHandler SendComplete;
		
		/// <summary>
		/// Indicates the state of the MIDI stream
		/// </summary>
		public MidiStreamState State { get { return _state; } }
		/// <summary>
		/// Indicates whether or not the stream attempts to synchronize the remote device's tempo
		/// </summary>
		public bool TempoSynchronizationEnabled {
			get {
				return 0 != _tempoSyncEnabled;
			} 
			set {
				if(value)
				{
					if(MidiStreamState.Started==_state)
					{
						var tmp = Tempo;
						var spb = 60/tmp;
						var ms = unchecked((int)(Math.Round((1000 * spb)/24)));
						_RestartTimer(ms);
					}
					Interlocked.Exchange(ref _tempoSyncEnabled, 1);
					return;
				}
				Interlocked.Exchange(ref _tempoSyncEnabled, 0);
				Interlocked.Exchange(ref _tempoSyncMessagesSentCount, 0);
				_DisposeTimer();

			}
		}
		
		/// <summary>
		/// Indicates the number of time clock sync messages to send when the tempo is changed. 0 indicates continuous synchronization
		/// </summary>
		public int TempoSynchronizationMessageCount {
			get {
				return _tempoSyncMessageCount;
			}
			set {
				Interlocked.Exchange(ref _tempoSyncMessageCount, value);
			}
		}
		/// <summary>
		/// Opens the stream
		/// </summary>
		public override void Open()
		{
			if (IntPtr.Zero!= Handle)
				throw new InvalidOperationException("The device is already open");
			var di = Index;
			var h = IntPtr.Zero;
			Interlocked.Exchange(ref _sendQueue, null);
			Interlocked.Exchange(ref _sendQueuePosition, 0);
			_CheckOutResult(midiStreamOpen(ref h, ref di, 1, _outCallback, 0, CALLBACK_FUNCTION));
			Handle= h;
			_state = MidiStreamState.Paused;
		}
		/// <summary>
		/// Closes the stream
		/// </summary>
		public override void Close()
		{
			_DisposeTimer();
			if (IntPtr.Zero != Handle) {
				Stop();
				Reset();
				_CheckOutResult(midiStreamClose(Handle));
				Handle = IntPtr.Zero;
				GC.SuppressFinalize(this);
				Interlocked.Exchange(ref _sendQueue, null);
				Interlocked.Exchange(ref _sendQueuePosition, 0);
				_state = MidiStreamState.Closed;
			}
		}
		/// <summary>
		/// Sends MIDI events to the stream
		/// </summary>
		/// <param name="events">The events to send</param>
		public void Send(params MidiEvent[] events)
			=> Send((IEnumerable<MidiEvent>)events);
		
		/// <summary>
		/// Sends a MIDI event to the stream
		/// </summary>
		/// <param name="events">The events to send</param>
		public void Send(IEnumerable<MidiEvent> events) {
			if (null != _sendQueue)
			{
				
				throw new InvalidOperationException("The stream is already sending");
			}
			
			var list = new List<MidiEvent>(128);
			// break out sysex messages into parts
			foreach(var @event in events)
			{
				// sysex
				if(0xF0==@event.Message.Status)
				{
					var data = (@event.Message as MidiMessageSysex).Data;
					if (null == data)
						return;
					if (254 < data.Length)
					{
						var len = 254;
						for (var i = 0; i < data.Length; i += len)
						{
							if (data.Length <= i + len)
							{
								len = data.Length - i;
							}
							var buf = new byte[len];
							if (0 == i)
							{
								Array.Copy(data, 0, buf, 0, len);
								list.Add(new MidiEvent(@event.Position, new MidiMessageSysex(buf)));
							}
							else
							{
								Array.Copy(data, i, buf, 0, len);
								list.Add(new MidiEvent(@event.Position, new MidiMessageSysexPart(buf)));
							}
						}
					}
					else
					{
						list.Add(@event);
					}
				} else
					list.Add(@event);
			}
			Interlocked.Exchange(ref _sendQueue, list);
			Interlocked.Exchange(ref _sendQueuePosition , 0);
			_SendBlock();
		}
		void _SendBlock()
		{
			if (null == _sendQueue)
				return;
			if (IntPtr.Zero == Handle)
				throw new InvalidOperationException("The stream is closed.");
		
			int blockSize = 0;
			IntPtr headerPointer = Marshal.AllocHGlobal(MIDIHDR_SIZE+MAX_EVENTBLOCK_SIZE);
			try
			{
				IntPtr eventPointer = new IntPtr(headerPointer.ToInt64() +MIDIHDR_SIZE);
				var ofs = 0;
				var ptrOfs = 0;
				for (; _sendQueuePosition < _sendQueue.Count; Interlocked.Exchange(ref _sendQueuePosition, _sendQueuePosition + 1))
				{
					var @event = _sendQueue[_sendQueuePosition];
					if (0x00 != @event.Message.Status && 0xF0 != (@event.Message.Status & 0xF0))
					{
						if (MAX_EVENTBLOCK_SIZE < blockSize + MIDIEVENT_SIZE)
							break;
						blockSize += MIDIEVENT_SIZE;
						var se = default(MIDIEVENT);
						se.dwDeltaTime = @event.Position + ofs;
						se.dwStreamId = 0;
						se.dwEvent = MidiUtility.PackMessage(@event.Message);
						Marshal.StructureToPtr(se, new IntPtr(ptrOfs + eventPointer.ToInt64()), false);
						ptrOfs += MIDIEVENT_SIZE;
						ofs = 0;
					}
					else if (0xFF == @event.Message.Status)
					{
						var mm = @event.Message as MidiMessageMeta;
						if (0x51 == mm.Data1) // tempo
						{
							if (MAX_EVENTBLOCK_SIZE < blockSize + MIDIEVENT_SIZE)
								break;
							blockSize += MIDIEVENT_SIZE;
							var se = default(MIDIEVENT);
							se.dwDeltaTime = @event.Position + ofs;
							se.dwStreamId = 0;
							se.dwEvent = (mm.Data[0] << 16) | (mm.Data[1] << 8) | mm.Data[2] | (MEVT_TEMPO << 24);
							Marshal.StructureToPtr(se, new IntPtr(ptrOfs + eventPointer.ToInt64()), false);
							ptrOfs += MIDIEVENT_SIZE;
							ofs = 0;
						}
						else if (0x2f == mm.Data1) // end track 
						{
							if (MAX_EVENTBLOCK_SIZE < blockSize + MIDIEVENT_SIZE)
								break;
							blockSize += MIDIEVENT_SIZE;

							// add a NOP message to it just to pad our output in case we're looping
							var se = default(MIDIEVENT);
							se.dwDeltaTime = @event.Position + ofs;
							se.dwStreamId = 0;
							se.dwEvent = (MEVT_NOP << 24);
							Marshal.StructureToPtr(se, new IntPtr(ptrOfs + eventPointer.ToInt64()), false);
							ptrOfs += MIDIEVENT_SIZE;
							ofs = 0;
						}
						else
							ofs = @event.Position;
					}
					else // sysex or sysex part
					{
						byte[] data;
						if (0 == @event.Message.Status)
							data = (@event.Message as MidiMessageSysexPart).Data;
						else
							data = MidiUtility.ToMessageBytes(@event.Message);


						var dl = data.Length;
						if (0 != (dl % 4))
							dl += 4 - (dl % 4);
						if (MAX_EVENTBLOCK_SIZE < blockSize + MIDIEVENT_SIZE+ dl)
							break;

						blockSize += MIDIEVENT_SIZE + dl;

						var se = default(MIDIEVENT);
						se.dwDeltaTime = @event.Position + ofs;
						se.dwStreamId = 0;
						se.dwEvent = MEVT_F_LONG | data.Length;
						Marshal.StructureToPtr(se, new IntPtr(ptrOfs + eventPointer.ToInt64()), false);
						ptrOfs += MIDIEVENT_SIZE;
						Marshal.Copy(data, 0, new IntPtr(ptrOfs + eventPointer.ToInt64()), data.Length);

						ptrOfs += dl;
						ofs = 0;
					}
				}
				var header = default(MIDIHDR);
				header.dwBufferLength = header.dwBytesRecorded = unchecked((uint)blockSize);
				header.lpData = eventPointer;
				Marshal.StructureToPtr(header, headerPointer, false);
				_CheckOutResult(midiOutPrepareHeader(Handle, headerPointer, MIDIHDR_SIZE));
				_CheckOutResult(midiStreamOut(Handle, headerPointer, MIDIHDR_SIZE));
				headerPointer= IntPtr.Zero;
			}
			finally
			{
				if (IntPtr.Zero != headerPointer)
					Marshal.FreeHGlobal(headerPointer);
			}
			
			
		}
		/// <summary>
		/// Sends events directly to the stream
		/// </summary>
		/// <param name="events">The events to send</param>
		public void SendDirect(params MidiEvent[] events)
			=> SendDirect((IEnumerable<MidiEvent>)events);
		/// <summary>
		/// Sends events directly to the event queue without buffering
		/// </summary>
		/// <param name="events">The events to send</param>
		/// <remarks>The total size of the events must be less than 64kb</remarks>
		public void SendDirect(IEnumerable<MidiEvent> events)
		{
			if (null == events)
				throw new ArgumentNullException("events");
			if (IntPtr.Zero == Handle)
				throw new InvalidOperationException("The stream is closed.");
			if (null != _sendQueue)
				throw new InvalidOperationException("The stream is already sending.");
			int blockSize = 0;
			IntPtr headerPointer = Marshal.AllocHGlobal(MAX_EVENTBLOCK_SIZE + MIDIHDR_SIZE);
			try
			{
				IntPtr eventPointer = new IntPtr(headerPointer.ToInt64() + MIDIHDR_SIZE);
				var ofs = 0;
				var ptrOfs = 0;
				var hasEvents = false;
				foreach (var @event in events)
				{
					hasEvents = true;
					if (0xF0 != (@event.Message.Status & 0xF0))
					{
						blockSize += MIDIEVENT_SIZE;
						if (MAX_EVENTBLOCK_SIZE <= blockSize)
							throw new ArgumentException("There are too many events in the event buffer - maximum size must be 64k", "events");
						var se = default(MIDIEVENT);
						se.dwDeltaTime = @event.Position + ofs;
						se.dwStreamId = 0;
						se.dwEvent = MidiUtility.PackMessage(@event.Message);
						Marshal.StructureToPtr(se, new IntPtr(ptrOfs + eventPointer.ToInt64()), false);
						ptrOfs += MIDIEVENT_SIZE;
						ofs = 0;
					}
					else if (0xFF == @event.Message.Status)
					{
						var mm = @event.Message as MidiMessageMeta;
						if (0x51 == mm.Data1) // tempo
						{
							blockSize += MIDIEVENT_SIZE;
							if (MAX_EVENTBLOCK_SIZE <= blockSize)
								throw new ArgumentException("There are too many events in the event buffer - maximum size must be 64k", "events");

							var se = default(MIDIEVENT);
							se.dwDeltaTime = @event.Position + ofs;
							se.dwStreamId = 0;
							se.dwEvent = (mm.Data[0] << 16) | (mm.Data[1] << 8) | mm.Data[2] | (MEVT_TEMPO << 24);
							Marshal.StructureToPtr(se, new IntPtr(ptrOfs + eventPointer.ToInt64()), false);
							ptrOfs += MIDIEVENT_SIZE;
							ofs = 0;
							// TODO: This signal is sent too early. It should really wait until after the
							// MEVT_TEMPO message is processed by the driver, but i have no easy way to
							// do that. All we can do is hope, here
							Interlocked.Exchange(ref _tempoSyncMessagesSentCount, 0);
						}
						else if (0x2f == mm.Data1) // end track 
						{
							blockSize += MIDIEVENT_SIZE;
							if (MAX_EVENTBLOCK_SIZE <= blockSize)
								throw new ArgumentException("There are too many events in the event buffer - maximum size must be 64k", "events");

							// add a NOP message to it just to pad our output in case we're looping
							var se = default(MIDIEVENT);
							se.dwDeltaTime = @event.Position + ofs;
							se.dwStreamId = 0;
							se.dwEvent = (MEVT_NOP << 24);
							Marshal.StructureToPtr(se, new IntPtr(ptrOfs + eventPointer.ToInt64()), false);
							ptrOfs += MIDIEVENT_SIZE;
							ofs = 0;
						}
						else
							ofs = @event.Position;
					}
					else // sysex
					{
						var msx = @event.Message as MidiMessageSysex;
						var dl = msx.Data.Length + 1;
						if (0 != (dl % 4))
						{
							dl += 4 - (dl % 4);
						}
						blockSize += MIDIEVENT_SIZE + dl;
						if (MAX_EVENTBLOCK_SIZE <= blockSize)
							throw new ArgumentException("There are too many events in the event buffer - maximum size must be 64k", "events");

						var se = default(MIDIEVENT);
						se.dwDeltaTime = @event.Position + ofs;
						se.dwStreamId = 0;
						se.dwEvent = MEVT_F_LONG | (msx.Data.Length + 1);
						Marshal.StructureToPtr(se, new IntPtr(ptrOfs + eventPointer.ToInt64()), false);
						ptrOfs += MIDIEVENT_SIZE;
						Marshal.WriteByte(new IntPtr(ptrOfs + eventPointer.ToInt64()), msx.Status);
						Marshal.Copy(msx.Data, 0, new IntPtr(ptrOfs + eventPointer.ToInt64() + 1), msx.Data.Length);

						ptrOfs += dl;
						ofs = 0;
					}
				}
				if (hasEvents)
				{
					var header = default(MIDIHDR);
					header.lpData = eventPointer;
					header.dwBufferLength = header.dwBytesRecorded = unchecked((uint)blockSize);
					Marshal.StructureToPtr(header, headerPointer, false);
					_CheckOutResult(midiOutPrepareHeader(Handle, headerPointer, MIDIHDR_SIZE));
					_CheckOutResult(midiStreamOut(Handle, headerPointer, MIDIHDR_SIZE));
					headerPointer= IntPtr.Zero;
				}
			}
			finally
			{
				if (IntPtr.Zero != headerPointer)
					Marshal.FreeHGlobal(headerPointer);
			}

		}
		
		void _SendRaw(byte[] data, int startIndex, int length)
		{
			var hdr = default(MIDIHDR);
			var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				hdr.lpData = new IntPtr(handle.AddrOfPinnedObject().ToInt64() + startIndex);
				hdr.dwBufferLength = hdr.dwBytesRecorded = (uint)(length);
				hdr.dwFlags = 0;
				_CheckOutResult(midiOutPrepareHeader(Handle, ref hdr, MIDIHDR_SIZE));
				while ((hdr.dwFlags & MHDR_PREPARED) != MHDR_PREPARED)
				{
					Thread.Sleep(1);
				}
				_CheckOutResult(midiOutLongMsg(Handle, ref hdr, MIDIHDR_SIZE));
				while ((hdr.dwFlags & MHDR_DONE) != MHDR_DONE)
				{
					Thread.Sleep(1);
				}
				_CheckOutResult(midiOutUnprepareHeader(Handle, ref hdr, MIDIHDR_SIZE));
			}
			finally
			{
				handle.Free();

			}
		}
		
		/// <summary>
		/// Destroys this instance
		/// </summary>
		~MidiStream()
		{
			Close();
		}
		void _RestartTimer(int ms)
		{
			if (0 >= ms)
				throw new ArgumentOutOfRangeException("ms");
			_DisposeTimer();
			var h = timeSetEvent(ms, 0, _timerCallback, IntPtr.Zero,TIME_ONESHOT);
			if (IntPtr.Zero == h)
				throw new Exception("Could not create multimedia timer");
			Interlocked.Exchange(ref _timerHandle, h);
		}
		void _DisposeTimer()
		{
			if(null!=_timerHandle)
			{
				timeKillEvent(_timerHandle);
				Interlocked.Exchange(ref _timerHandle,IntPtr.Zero);
			}
		}
		void _TimerProc(IntPtr handle, int msg, int user, IntPtr param1, IntPtr param2)
		{
			if (IntPtr.Zero!=Handle && _timerHandle==handle && 0!=_tempoSyncEnabled)
			{
				if (0==_tempoSyncMessageCount || _tempoSyncMessagesSentCount < _tempoSyncMessageCount)
				{
					// quickly send a time sync message
					midiOutShortMsg(Handle, 0xF8);
					Interlocked.Increment(ref _tempoSyncMessagesSentCount);
				}
				var tmp = Tempo;
				var spb = 60 / tmp;
				var ms = unchecked((int)(Math.Round((1000 * spb) / 24)));
				_RestartTimer(ms);
				
			}
		}
		void _MidiOutProc(IntPtr handle, int msg, int instance, IntPtr param1, IntPtr param2)
		{
			switch(msg)
			{
				case MOM_OPEN:
					OnOpened(EventArgs.Empty);
					break;
				case MOM_CLOSE:
					OnClosed(EventArgs.Empty);
					break;
				case MOM_DONE:

					if (IntPtr.Zero != param1)
					{
						var header = Marshal.PtrToStructure<MIDIHDR>(param1);
						_CheckOutResult(midiOutUnprepareHeader(Handle, param1, Marshal.SizeOf(typeof(MIDIHDR))));
						Marshal.FreeHGlobal(param1);
						Interlocked.Exchange(ref _sendQueuePosition, 0);
						Interlocked.Exchange(ref _sendQueue, null);
					}

					if(null==_sendQueue)
						SendComplete?.Invoke(this, EventArgs.Empty);
					else
					{
						_SendBlock();
					}
					break;
				
			}
		}
		/// <summary>
		/// Starts the stream
		/// </summary>
		public void Start()
		{
			if (IntPtr.Zero == Handle)
				throw new InvalidOperationException("The stream is closed.");
			switch(_state)
			{
				case MidiStreamState.Paused:
				case MidiStreamState.Stopped:
					
					var tmp = Tempo;
					var spb = 60 / tmp;
					var ms = unchecked((int)(Math.Round((1000 * spb) / 24)));
					Interlocked.Exchange(ref _tempoSyncMessagesSentCount, 0);
					_RestartTimer(ms);
					
					_CheckOutResult(midiStreamRestart(Handle));
					_state = MidiStreamState.Started;
					break;		
			}
		}
		/// <summary>
		/// Stops the stream
		/// </summary>
		public void Stop()
		{
			if (IntPtr.Zero == Handle)
				throw new InvalidOperationException("The stream is closed.");
			switch (_state)
			{
				case MidiStreamState.Paused:
				case MidiStreamState.Started:
					_DisposeTimer();
					_CheckOutResult(midiStreamStop(Handle));
					Interlocked.Exchange(ref _tempoSyncMessagesSentCount, 0);
					_state = MidiStreamState.Stopped;
					
					Interlocked.Exchange(ref _sendQueuePosition, 0);
					
					if(null!=_sendQueue)
					{
						Interlocked.Exchange(ref _sendQueue, null);
					}
					break;
			}
		}
		/// <summary>
		/// Pauses the stream
		/// </summary>
		public void Pause()
		{
			if (IntPtr.Zero == Handle)
				throw new InvalidOperationException("The stream is closed.");
			switch (_state)
			{
				case MidiStreamState.Started:
					_CheckOutResult(midiStreamPause(Handle));
					_state = MidiStreamState.Paused;
					Interlocked.Exchange(ref _tempoSyncMessagesSentCount, 0);
					break;
			}
		}
		
		/// <summary>
		/// Indicates the position in ticks
		/// </summary>
		public int PositionTicks {
			get {
				if (IntPtr.Zero == Handle)
					throw new InvalidOperationException("The stream is closed.");
				switch (_state)
				{
					case MidiStreamState.Started:
					case MidiStreamState.Paused:
						MMTIME mm = new MMTIME();
						mm.wType = TIME_TICKS;
						_CheckOutResult(midiStreamPosition(Handle, ref mm, Marshal.SizeOf(typeof(MMTIME))));
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
				if (IntPtr.Zero == Handle)
					throw new InvalidOperationException("The stream is closed.");
				switch (_state)
				{
					case MidiStreamState.Started:
					case MidiStreamState.Paused:
						MMTIME mm = new MMTIME();
						mm.wType = TIME_MS;
						_CheckOutResult(midiStreamPosition(Handle, ref mm, Marshal.SizeOf(typeof(MMTIME))));
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
				if (IntPtr.Zero == Handle)
					throw new InvalidOperationException("The stream is closed.");
				switch (_state)
				{
					case MidiStreamState.Started:
					case MidiStreamState.Paused:
						MMTIME mm = new MMTIME();
						mm.wType = TIME_MIDI;
						_CheckOutResult(midiStreamPosition(Handle, ref mm, Marshal.SizeOf(typeof(MMTIME))));
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
				if (IntPtr.Zero == Handle)
					throw new InvalidOperationException("The stream is closed.");
				switch (_state)
				{
					case MidiStreamState.Started:
					case MidiStreamState.Paused:
						MMTIME mm = new MMTIME();
						mm.wType = TIME_BYTES;
						_CheckOutResult(midiStreamPosition(Handle, ref mm, Marshal.SizeOf(typeof(MMTIME))));
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
				if (IntPtr.Zero == Handle)
					throw new InvalidOperationException("The stream is closed.");
				switch (_state)
				{
					case MidiStreamState.Started:
					case MidiStreamState.Paused:
						MMTIME mm = new MMTIME();
						mm.wType = TIME_SMPTE;
						_CheckOutResult(midiStreamPosition(Handle, ref mm, Marshal.SizeOf(typeof(MMTIME))));
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
				if (IntPtr.Zero == Handle)
					throw new InvalidOperationException("The stream is closed.");
				var t = new MIDIPROPTEMPO();
				t.cbStruct = Marshal.SizeOf(typeof(MIDIPROPTEMPO));
				_CheckOutResult(midiStreamProperty(Handle, ref t, MIDIPROP_GET | MIDIPROP_TEMPO));
				return unchecked(t.dwTempo);
			}
			set {
				if (IntPtr.Zero == Handle)
					throw new InvalidOperationException("The stream is closed.");
				Interlocked.Exchange(ref _tempoSyncMessagesSentCount, 0);
				var t = new MIDIPROPTEMPO();
				t.cbStruct = Marshal.SizeOf(typeof(MIDIPROPTEMPO));
				t.dwTempo= value;
				_CheckOutResult(midiStreamProperty(Handle, ref t, MIDIPROP_SET | MIDIPROP_TEMPO));
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
				if (IntPtr.Zero == Handle)
					throw new InvalidOperationException("The stream is closed.");
				var tb = new MIDIPROPTIMEDIV();
				tb.cbStruct = Marshal.SizeOf(typeof(MIDIPROPTIMEDIV));
				_CheckOutResult(midiStreamProperty(Handle, ref tb, MIDIPROP_GET | MIDIPROP_TIMEDIV));
				return unchecked((short)tb.dwTimeDiv);
			}
			set {
				if (IntPtr.Zero == Handle)
					throw new InvalidOperationException("The stream is closed.");
				var tb = new MIDIPROPTIMEDIV();
				tb.cbStruct = Marshal.SizeOf(typeof(MIDIPROPTIMEDIV));
				tb.dwTimeDiv = value;
				_CheckOutResult(midiStreamProperty(Handle, ref tb, MIDIPROP_SET | MIDIPROP_TIMEDIV));
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
