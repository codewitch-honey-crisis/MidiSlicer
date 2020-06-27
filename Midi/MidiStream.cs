using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		int _deviceIndex;
		IntPtr _handle;
		IntPtr _sendHandle;
		MidiStreamState _state = MidiStreamState.Closed;
		internal MidiStream(int deviceIndex)
		{
			if (0>deviceIndex || MidiUtility.GetOutputDeviceCount() <= deviceIndex)
				throw new ArgumentOutOfRangeException("deviceIndex");
			_deviceIndex = deviceIndex;
			_handle = IntPtr.Zero;
			_sendHandle = IntPtr.Zero;
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
			_handle = MidiUtility.OpenStream(_deviceIndex, _MidiOutProc);
			_state = MidiStreamState.Paused;
		}
		/// <summary>
		/// Closes the stream
		/// </summary>
		public void Close()
		{
			if (IntPtr.Zero != _handle) {
				MidiUtility.CloseStream(_handle);
				_handle = IntPtr.Zero;
				GC.SuppressFinalize(this);
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
		public void Send(IEnumerable<MidiEvent> events)
		{
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("The stream is closed.");
			if (IntPtr.Zero != _sendHandle)
				throw new InvalidOperationException("The stream is busy playing.");
			MidiUtility.SendStream(_handle, events);
		}
		/// <summary>
		/// Sends a message out immediately
		/// </summary>
		/// <param name="message">The message to send</param>
		public void Send(MidiMessage message)
		{
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("The device is closed.");
			if (0xF0 == (message.Status & 0xF0))
				MidiUtility.SendLong(_handle, message);
			else
				MidiUtility.Send(_handle, message);
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
				case 0x3C7://MidiUtility.MOM_OPEN:
					Opened?.Invoke(this, EventArgs.Empty);
					break;
				case 0x3C8://MidiUtility.MOM_CLOSE:
					Closed?.Invoke(this, EventArgs.Empty);
					break;
				case 0x3c9://MidiUtility.MOM_DONE:
					if (IntPtr.Zero != _sendHandle)
					{
						MidiUtility.SendStreamComplete(_handle, _sendHandle);
						_sendHandle = IntPtr.Zero;
					}
					SendComplete?.Invoke(this, EventArgs.Empty);
					
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
					MidiUtility.RestartStream(_handle);
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
					MidiUtility.StopStream(_handle);
					_state = MidiStreamState.Stopped;
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
					MidiUtility.PauseStream(_handle);
					_state = MidiStreamState.Paused;
					break;
			}
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
						return MidiUtility.GetStreamPositionTicks(_handle);
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
						return MidiUtility.GetStreamPositionMilliseconds(_handle);
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
						return MidiUtility.GetStreamPositionSongPointer(_handle);
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
						return MidiUtility.GetStreamPositionBytes(_handle);
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
						return MidiUtility.GetStreamPositionSmpte(_handle);
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
				return MidiUtility.GetStreamMicroTempo(_handle);
			}
			set {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The stream is closed.");
				MidiUtility.SetStreamMicroTempo(_handle,value);
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
				return MidiUtility.GetStreamTimeBase(_handle);
			}
			set {
				if (IntPtr.Zero == _handle)
					throw new InvalidOperationException("The stream is closed.");
				MidiUtility.SetStreamTimeBase(_handle, value);
			}
		}

	}
}
