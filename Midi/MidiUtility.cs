
namespace M
{
	using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Represents a utility class for performing low level MIDI operations
    /// </summary>
#if MIDILIB
    public
#else
	internal
#endif
	static partial class MidiUtility
	{
		const string _Notes = "C C#D D#E F F#G G#A A#B ";
		/// <summary>
		/// Converts a MIDI note id into a string note representation
		/// </summary>
		/// <param name="noteId">The note id (0-127)</param>
		/// <param name="withOctave">Indicates whether or not the octave should be returned</param>
		/// <returns>The string note</returns>
		public static string NoteIdToNote(byte noteId, bool withOctave=true)
		{
			noteId = unchecked((byte)(noteId & 0x7F));
			if(withOctave)
				return _Notes.Substring((noteId % 12) * 2, 2).TrimEnd() + ((int)(noteId / 12)).ToString();
			return _Notes.Substring((noteId % 12) * 2, 2).TrimEnd();
		}
		/// <summary>
		/// Converts a string representation of a note to a MIDI note id
		/// </summary>
		/// <param name="note">The note</param>
		/// <returns>A MIDI note id</returns>
		public static byte NoteToNoteId(string note)
		{
			if (null == note)
				throw new ArgumentNullException("note");
			if (0 == note.Length)
				throw new ArgumentException("The note must not be empty", "note");
			var bn = "";
			for(var i = 0;i<note.Length;++i)
			{
				var ch = note[i];
				if (!char.IsLetter(ch) && '#'!=ch)
					break;
				bn += ch.ToString().ToUpperInvariant();
			}
			if (0 == bn.Length || 2 < bn.Length || '#' == bn[0])
				throw new ArgumentException("Not a valid note", "note");
			var j = _Notes.IndexOf(bn);
			if (0 > j)
				throw new ArgumentException("Note a valid note", "note");
			var oct = 5;
			if(note.Length>bn.Length)
			{
				var num = note.Substring(bn.Length);
				if(!int.TryParse(num,out oct))
					throw new ArgumentException("Note a valid note", "note");
				if(10<oct)
					throw new ArgumentException("Note a valid note", "note");
			}
			return unchecked((byte)(12 * oct + (j/2)));
		}
		/// <summary>
		/// Converts a microtempo to a tempo
		/// </summary>
		/// <param name="microTempo">The microtempo</param>
		/// <returns>The tempo</returns>
		public static double MicroTempoToTempo(int microTempo)
		{
			return 60000000 / ((double)microTempo);
		}
		/// <summary>
		/// Converts a tempo to a microtempo
		/// </summary>
		/// <param name="tempo">The tempo</param>
		/// <returns>The microtempo</returns>
		public static int TempoToMicroTempo(double tempo)
		{
			return (int)(500000 * (120d/tempo));
		}
		/// <summary>
		/// Converts MIDI ticks/pulses to a <see cref="TimeSpan"/>
		/// </summary>
		/// <param name="timeBase">The timebase in pulses/ticks per quarter note</param>
		/// <param name="microTempo">The microtempo</param>
		/// <param name="ticks">The ticks to convert</param>
		/// <returns>A <see cref="TimeSpan"/> representing the length</returns>
		public static TimeSpan TicksToTimeSpan(short timeBase, int microTempo, int ticks)
		{
			var ticksusec = microTempo / (double)timeBase;
			var tickspertick = ticksusec / (TimeSpan.TicksPerMillisecond / 1000) * 100;
			return new TimeSpan((long)(ticks * tickspertick));
		}
		/// <summary>
		/// Creates a MIDI note on message packed as an integer
		/// </summary>
		/// <param name="note">The note</param>
		/// <param name="velocity">The velocity</param>
		/// <param name="channel">The channel</param>
		/// <returns>A MIDI note on message as a packed integer</returns>
		public static int NoteOn(byte note, byte velocity, byte channel = 0) => PackMessage(0x90, note, velocity, channel);
		/// <summary>
		/// Creates a MIDI note off message packed as an integer
		/// </summary>
		/// <param name="note">The note</param>
		/// <param name="velocity">The velocity</param>
		/// <param name="channel">The channel</param>
		/// <returns>A MIDI note off message as a packed integer</returns>
		public static int NoteOff(byte note, byte velocity, byte channel = 0) => PackMessage(0x80, note, velocity, channel);
		/// <summary>
		/// Retrieves the count of MIDI output devices
		/// </summary>
		/// <returns>The count of devices</returns>
		public static int GetOutputDeviceCount()
		{
			return midiOutGetNumDevs();
		}
		/// <summary>
		/// Retrieves the count of MIDI input devices
		/// </summary>
		/// <returns>The count of devices</returns>
		public static int GetInputDeviceCount()
		{
			return midiInGetNumDevs();
		}
		/// <summary>
		/// Retrieves the name of the MIDI output device
		/// </summary>
		/// <param name="index">The device index</param>
		/// <returns>The device name</returns>
		public static string GetOutputDeviceName(int index)
		{
			MidiOutCaps caps = default(MidiOutCaps);
			_CheckOutResult(midiOutGetDevCaps(index, ref caps, Marshal.SizeOf(typeof(MidiOutCaps))));
			return caps.name;
		}
		/// <summary>
		/// Retrieves the name of the MIDI input device
		/// </summary>
		/// <param name="index">The device index</param>
		/// <returns>The device name</returns>
		public static string GetInputDeviceName(int index)
		{
			MidiInCaps caps = default(MidiInCaps);
			_CheckOutResult(midiInGetDevCaps(index, ref caps, Marshal.SizeOf(typeof(MidiInCaps))));
			return caps.name;
		}
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
		/// Opens a MIDI input device
		/// </summary>
		/// <param name="deviceIndex">The device index</param>
		/// <param name="callback">The callback method to use</param>
		/// <returns>A handle to the specified device</returns>
		public static IntPtr OpenInputDevice(int deviceIndex, MidiInProc callback)
		{
			IntPtr handle = IntPtr.Zero;
			_CheckOutResult(midiInOpen(ref handle, deviceIndex, callback, 0, CALLBACK_FUNCTION | MIDI_IO_STATUS));
			return handle;
		}
		/// <summary>
		/// Closes the specified MIDI stream
		/// </summary>
		/// <param name="handle"></param>
		public static void CloseStream(IntPtr handle)
		{
			_CheckOutResult(midiStreamClose(handle));
		}
		/// <summary>
		/// Opens a MIDI stream
		/// </summary>
		/// <param name="deviceIndex">The output device index</param>
		/// <param name="callback">The callback method to use</param>
		/// <returns></returns>
		public static IntPtr OpenStream(int deviceIndex,MidiOutProc callback)
		{
			IntPtr handle = IntPtr.Zero;
			_CheckOutResult(midiStreamOpen(ref handle, ref deviceIndex, 1, callback, 0, CALLBACK_FUNCTION));
			return handle;
		}
		/// <summary>
		/// Restarts a MIDI stream
		/// </summary>
		/// <param name="streamHandle">The stream handle</param>
		public static void RestartStream(IntPtr streamHandle)
		{
			_CheckOutResult(midiStreamRestart(streamHandle));
		}
		/// <summary>
		/// Stops a MIDI stream
		/// </summary>
		/// <param name="streamHandle">The stream handle</param>
		public static void StopStream(IntPtr streamHandle)
		{
			_CheckOutResult(midiStreamStop(streamHandle));
		}
		/// <summary>
		/// Pauses a MIDI stream
		/// </summary>
		/// <param name="streamHandle">The stream handle</param>
		public static void PauseStream(IntPtr streamHandle)
		{
			_CheckOutResult(midiStreamPause(streamHandle));
		}
		/// <summary>
		/// Closes the specified MIDI output device
		/// </summary>
		/// <param name="handle">The handle of the device</param>
		public static void CloseOutputDevice(IntPtr handle)
		{
			_CheckOutResult(midiOutClose(handle));
		}
		/// <summary>
		/// Closes the specified MIDI input device
		/// </summary>
		/// <param name="handle">The handle of the device</param>
		public static void CloseInputDevice(IntPtr handle)
		{
			_CheckOutResult(midiInClose(handle));
		}
		/// <summary>
		/// Gets the SMPTE position of the stream
		/// </summary>
		/// <param name="streamHandle">The stream handle</param>
		/// <returns>The time in SMPTE format</returns>
		public static MidiSmpteTime GetStreamPositionSmpte(IntPtr streamHandle)
		{
			var mmtime = default(MMTime);
			mmtime.wType = TIME_SMPTE;
			_CheckOutResult(midiStreamPosition(streamHandle, ref mmtime, Marshal.SizeOf(typeof(MMTime))));
			if (TIME_SMPTE != mmtime.wType)
				throw new NotSupportedException("The time format is not supported");
			return new MidiSmpteTime(new TimeSpan(0, mmtime.smpteHour, mmtime.smpteMin, mmtime.smpteSec, 0), mmtime.smpteFrame, mmtime.smpteFps);
		}
		/// <summary>
		/// Gets the stream position in ticks
		/// </summary>
		/// <param name="streamHandle">The stream handle</param>
		/// <returns>The position in ticks</returns>
		public static int GetStreamPositionTicks(IntPtr streamHandle)
		{
			var mmtime = default(MMTime);
			mmtime.wType = TIME_TICKS;
			_CheckOutResult(midiStreamPosition(streamHandle, ref mmtime, Marshal.SizeOf(typeof(MMTime))));
			if (TIME_TICKS != mmtime.wType)
				throw new NotSupportedException("The time format is not supported");
			return mmtime.ticks;
		}
		/// <summary>
		/// Gets the stream position in bytes
		/// </summary>
		/// <param name="streamHandle">The stream handle</param>
		/// <returns>The position in bytes</returns>
		public static int GetStreamPositionBytes(IntPtr streamHandle)
		{
			var mmtime = default(MMTime);
			mmtime.wType = TIME_BYTES;
			_CheckOutResult(midiStreamPosition(streamHandle, ref mmtime, Marshal.SizeOf(typeof(MMTime))));
			if (TIME_BYTES != mmtime.wType)
				throw new NotSupportedException("The time format is not supported");
			return mmtime.cb;
		}
		/// <summary>
		/// Gets the stream position in milliseconds
		/// </summary>
		/// <param name="streamHandle">The stream handle</param>
		/// <returns>The position in milliseconds</returns>
		public static int GetStreamPositionMilliseconds(IntPtr streamHandle)
		{
			var mmtime = default(MMTime);
			mmtime.wType = TIME_MS;
			_CheckOutResult(midiStreamPosition(streamHandle, ref mmtime, Marshal.SizeOf(typeof(MMTime))));
			if (TIME_MS != mmtime.wType)
				throw new NotSupportedException("The time format is not supported");
			return mmtime.ms;
		}
		/// <summary>
		/// Gets the stream song pointer position
		/// </summary>
		/// <param name="streamHandle">The stream handle</param>
		/// <returns>The song pointer position</returns>
		public static int GetStreamPositionSongPointer(IntPtr streamHandle)
		{
			var mmtime = default(MMTime);
			mmtime.wType = TIME_MIDI;
			_CheckOutResult(midiStreamPosition(streamHandle, ref mmtime, Marshal.SizeOf(typeof(MMTime))));
			if (TIME_MIDI != mmtime.wType)
				throw new NotSupportedException("The time format is not supported");
			return mmtime.midiSongPtrPos;
		}
		[System.Diagnostics.DebuggerNonUserCode()]
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
		public static void Send(IntPtr deviceHandle, MidiMessage message)
		{
			var m = 0;
			switch (message.Status & 0xF0)
			{
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
					m |= mcm.Data1 << 8;
					Send(deviceHandle, m);
					break;
			}

		}
		
		/// <summary>
		/// Starts the MIDI input device
		/// </summary>
		/// <param name="deviceHandle">The handle of the device</param>
		public static void StartInput(IntPtr deviceHandle)
		{
			_CheckOutResult(midiInStart(deviceHandle));
		}
		/// <summary>
		/// Stops the MIDI input device
		/// </summary>
		/// <param name="deviceHandle">The handle of the device</param>
		public static void StopInput(IntPtr deviceHandle)
		{
			_CheckOutResult(midiInStop(deviceHandle));
		}
		/// <summary>
		/// Resets the MIDI input device
		/// </summary>
		/// <param name="deviceHandle">The handle of the device</param>
		public static void ResetInput(IntPtr deviceHandle)
		{
			_CheckOutResult(midiInReset(deviceHandle));
		}
		/// <summary>
		/// Sends a long message to the specified device
		/// </summary>
		/// <param name="deviceHandle">The MIDI output device</param>
		/// <param name="message">The message</param>
		public static void SendLong(IntPtr deviceHandle,MidiMessage message)
		{
			if (null == message)
				throw new ArgumentNullException("message");
			var data = ToMessageBytes(message);
			if (null == data)
				return;
			if (data.Length > (64 * 1024))
			{
				throw new ArgumentOutOfRangeException();
			}

			int hdrSize = Marshal.SizeOf(typeof(MidiHdr));
			byte[] hdrReserved = new byte[8];
			MidiHdr hdr = new MidiHdr();
			GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			GCHandle revHandle = GCHandle.Alloc(hdrReserved, GCHandleType.Pinned);
			try
			{
				hdr.lpData = handle.AddrOfPinnedObject();
				hdr.dwBufferLength = (uint)data.Length;
				hdr.dwFlags = 0;
				_CheckOutResult(midiOutPrepareHeader(deviceHandle, ref hdr, hdrSize));
				while ((hdr.dwFlags & MHDR_PREPARED) != MHDR_PREPARED)
				{
					Thread.Sleep(1);
				}
				_CheckOutResult(midiOutLongMsg(deviceHandle, ref hdr, hdrSize));
				while ((hdr.dwFlags & MHDR_DONE) != MHDR_DONE)
				{
					Thread.Sleep(1);
				}
				_CheckOutResult(midiOutUnprepareHeader(deviceHandle, ref hdr, hdrSize));
			}
			finally
			{
				handle.Free();
				revHandle.Free();
			}
		}
		/// <summary>
		/// Gets the tempo for the stream
		/// </summary>
		/// <param name="streamHandle">The handle of the stream</param>
		/// <returns>The microtempo</returns>
		public static int GetStreamMicroTempo(IntPtr streamHandle)
		{
			var tempo = default(MidiPropTempo);
			tempo.cbStruct = Marshal.SizeOf(typeof(MidiPropTempo));
			_CheckOutResult(midiStreamProperty(streamHandle, ref tempo, MIDIPROP_GET | MIDIPROP_TEMPO));
			return tempo.dwTempo;
		}
		/// <summary>
		/// Sets the tempo for the stream
		/// </summary>
		/// <param name="streamHandle">The handle of the stream</param>
		/// <param name="microTempo">The new microtempo</param>
		public static void SetStreamMicroTempo(IntPtr streamHandle, int microTempo)
		{
			var tempo = default(MidiPropTempo);
			tempo.cbStruct = Marshal.SizeOf(typeof(MidiPropTempo));
			tempo.dwTempo = microTempo;
			_CheckOutResult(midiStreamProperty(streamHandle, ref tempo, MIDIPROP_SET | MIDIPROP_TEMPO));
		}
		/// <summary>
		/// Gets the timebase for the stream
		/// </summary>
		/// <param name="streamHandle">The handle of the stream</param>
		/// <returns>The timebase</returns>
		public static short GetStreamTimeBase(IntPtr streamHandle)
		{
			var tdiv = default(MidiPropTimeDiv);
			tdiv.cbStruct = Marshal.SizeOf(typeof(MidiPropTimeDiv));
			_CheckOutResult(midiStreamProperty(streamHandle, ref tdiv, MIDIPROP_GET | MIDIPROP_TIMEDIV));
			return unchecked((short)tdiv.dwTimeDiv);
		}
		/// <summary>
		/// Sets the timebase for the stream
		/// </summary>
		/// <param name="streamHandle">The handle of the stream</param>
		/// <param name="timeBase">The new timebase</param>
		public static void SetStreamTimeBase(IntPtr streamHandle, short timeBase)
		{
			var tdiv = default(MidiPropTimeDiv);
			tdiv.cbStruct = Marshal.SizeOf(typeof(MidiPropTimeDiv));
			tdiv.dwTimeDiv = timeBase;
			_CheckOutResult(midiStreamProperty(streamHandle, ref tdiv, MIDIPROP_SET | MIDIPROP_TIMEDIV));
		}
		/// <summary>
		/// Sends an event to the specified stream
		/// </summary>
		/// <param name="streamHandle">The MIDI stream</param>
		/// <param name="events">The events</param>
		/// <remarks>A pointer to the MIDIHDR used to play these notes. Must be freed with SendStreamComplete()</remarks>
		public static IntPtr SendStream(IntPtr streamHandle, IEnumerable<MidiEvent> events)
		{
			if (null == events)
				throw new ArgumentNullException("events");
			var elist = new List<MidiShortEvent>();
			int ofs = 0;
			foreach(var @event in events)
			{
				// we currently skip sysex messages
				// there is no way to handle meta
				// messages at this stage so we
				// skip those
				if (0xF0 != (@event.Message.Status & 0xF0))
				{
					var se = new MidiShortEvent();
					se.deltaTime = @event.Position + ofs;
					se.streamId = 0;
					se.@event = PackMessage(@event.Message);
					elist.Add(se);
					ofs = 0;
				}
				else if (0xFF == @event.Message.Status)
				{
					var mm = @event.Message as MidiMessageMeta;
					if (0x51 == mm.Data1) // tempo
					{
						var se = new MidiShortEvent();
						se.deltaTime = @event.Position + ofs;
						se.streamId = 0;
						se.@event = (mm.Data[0] << 16) | (mm.Data[1] << 8) | mm.Data[2] | (MEVT_TEMPO << 24);
						elist.Add(se);
					}
					else if (0x2f == mm.Data1) // end track 
					{
						// add a NOP message to it just to pad our output in case we're looping
						var se = new MidiShortEvent();
						se.deltaTime = @event.Position + ofs;
						se.streamId = 0;
						se.@event = (MEVT_NOP << 24);
						elist.Add(se);
					}

				}
				else
					ofs = @event.Position;
				
			}
			int eventSize = Marshal.SizeOf(typeof(MidiShortEvent));
			int blockSize = eventSize * elist.Count;
			if (64 * 1024 <= blockSize)
				throw new ArgumentException("There are too many events in the event buffer - maximum size must be 64k", "events");
			IntPtr eventPointer = Marshal.AllocHGlobal(blockSize);
			var i = 0;
			foreach (var @event in elist)
			{
				Marshal.StructureToPtr(@event, (IntPtr)((int)eventPointer + (eventSize * i)), false);
				++i;
			}
			MidiHdr header = default(MidiHdr);
			header.lpData = eventPointer;
			header.dwBufferLength = header.dwBytesRecorded= unchecked((uint)blockSize);
			int headerSize = Marshal.SizeOf(header);
			IntPtr headerPointer = Marshal.AllocHGlobal(headerSize);
			Marshal.StructureToPtr(header, headerPointer, false);
			_CheckOutResult(midiOutPrepareHeader(streamHandle, headerPointer, headerSize));
			_CheckOutResult(midiStreamOut(streamHandle, headerPointer, headerSize));
			return headerPointer;
		}
		/// <summary>
		/// Completes the SendStream() operation
		/// </summary>
		/// <param name="streamHandle">The stream handle on which SendStream() was previously called</param>
		/// <param name="sendHandle">The handle returned from SendStream()</param>
		public static void SendStreamComplete(IntPtr streamHandle,IntPtr sendHandle)
		{
			var hdrSize = Marshal.SizeOf(typeof(MidiHdr));
			_CheckOutResult(midiOutUnprepareHeader(streamHandle, sendHandle, hdrSize));
			Marshal.FreeHGlobal(sendHandle);
		}
		/// <summary>
		/// Gets the current UTC date and time as a high precision datetime
		/// </summary>
		public static DateTime PreciseUtcNow {
			get {
				long filetime;
				_GetSystemTimePreciseAsFileTime(out filetime);
				return DateTime.FromFileTimeUtc(filetime);

			}
		}
		/// <summary>
		/// Gets a high precision tick count of the current time in UTC
		/// </summary>
		public static long PreciseUtcNowTicks {
			get {
				long filetime;
				_GetSystemTimePreciseAsFileTime(out filetime);

				return filetime + 504911232000000000;
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
		/// <remarks>Must not be a sysex message</remarks>
		public static int PackMessage(byte status, byte data1, byte data2, byte channel = 0)
		{
			if (0 == channel)
				return ((data2 & 0x7F) << 16) +
				((data1 & 0x7F) << 8) + status;
			return ((data2 & 0x7F) << 16) +
			((data1 & 0x7F) << 8) + ((status & 0xF0) | (channel & 0xF));
		}
		/// <summary>
		/// Packs a MIDI message as an int
		/// </summary>
		/// <param name="message">The MIDI message</param>
		/// <returns>An integer representing the packed MIDI message</returns>
		/// <remarks>Must not be a sysex message</remarks>
		public static int PackMessage(MidiMessage message)
		{
			
			switch (message.Status & 0xF0)
			{
				case 0x80:
				case 0x90:
				case 0xA0:
				case 0xB0:
				case 0xE0:
					var mw = message as MidiMessageWord;
					return PackMessage(mw.Status, mw.Data1, mw.Data2, mw.Channel);
				case 0xC0:
				case 0xD0:
					var mb = message as MidiMessageByte;
					return PackMessage(mb.Status, mb.Data1, 0, mb.Channel);
				case 0xF0:
					if(0xFF!=message.Status)
						throw new NotSupportedException("The message must not be a sysex message");
					return -1;
				default: // should never happen
					throw new NotSupportedException("Unsupported message");
			}
		}
		/// <summary>
		/// Unpacks a MIDI message from an int
		/// </summary>
		/// <returns>An integer representing the packed MIDI message</returns>
		public static MidiMessage UnpackMessage(int message)
		{
			switch (message & 0xF0)
			{
				case 0x80:
					return new MidiMessageNoteOff(unchecked((byte)((message >> 8) & 0xFF)), unchecked((byte)((message >> 16) & 0xFF)), unchecked((byte)(message & 0xF)));
				case 0x90:
					return new MidiMessageNoteOn(unchecked((byte)((message >> 8) & 0xFF)), unchecked((byte)((message >> 16) & 0xFF)), unchecked((byte)(message & 0xF)));
				case 0xA0:
					return new MidiMessageKeyPressure(unchecked((byte)((message >> 8) & 0xFF)), unchecked((byte)((message >> 16) & 0xFF)), unchecked((byte)(message & 0xF)));
				case 0xB0:
					return new MidiMessageCC(unchecked((byte)((message >> 8) & 0xFF)), unchecked((byte)((message >> 16) & 0xFF)), unchecked((byte)(message & 0xF)));
				case 0xC0:
					return new MidiMessagePatchChange(unchecked((byte)((message >> 8) & 0xFF)), unchecked((byte)(message & 0xF)));
				case 0xD0:
					return new MidiMessageChannelPressure(unchecked((byte)((message >> 8) & 0xFF)), unchecked((byte)(message & 0xF)));
				case 0xE0:
					return new MidiMessageChannelPitch(unchecked((byte)((message >> 8) & 0xFF)), unchecked((byte)((message >> 16) & 0xFF)), unchecked((byte)(message & 0xF)));
				default:
					throw new NotSupportedException("The MIDI message is not recognized.");
			}

		}
		/// <summary>
		/// Gets the bytes that make up the MIDI event
		/// </summary>
		/// <param name="event">The event to pack</param>
		/// <returns>A byte array representing the event</returns>
		public static byte[] ToEventBytes(MidiEvent @event)
		{
			var isLong = (0xF0 == (@event.Message.Status & 0xF0));
			var msg = ToMessageBytes(@event.Message);
			var result = new byte[9 + msg.Length];
			Array.Copy(BitConverter.GetBytes(@event.Position), 0, result, 0, 4);
			Array.Copy(msg, 0, result, 9, msg.Length);
			if (isLong)
				Array.Copy(BitConverter.GetBytes(MidiUtility.MEVT_LONGMSG), 0, result, 4, 4);
			return result;
		}
		/// <summary>
		/// Gets the bytes that make up a MIDI message
		/// </summary>
		/// <param name="message">The message to retrieve the bytes for</param>
		/// <returns>An array of bytes containing the message data</returns>
		public static byte[] ToMessageBytes(MidiMessage message)
		{
			byte[] data;
			switch (message.Status & 0xF0)
			{
				case 0x80:
				case 0x90:
				case 0xA0:
				case 0xB0:
				case 0xE0:
					var msgw = message as MidiMessageWord;
					data = new byte[3];
					data[0] = msgw.Status;
					data[1] = msgw.Data1;
					data[2] = msgw.Data2;
					return data;
				case 0xC0:
				case 0xD0:
					var msgb = message as MidiMessageByte;
					data = new byte[2];
					data[0] = msgb.Status;
					data[1] = msgb.Data1;
					return data;
				case 0xF0:
					var msgsx = message as MidiMessageSysex;
					if(null!=msgsx)
					{
						data = new byte[1 + msgsx.Data.Length];
						data[0] = msgsx.Status;
						Array.Copy(msgsx.Data, 0, data, 1, msgsx.Data.Length);
						return data;
					}
					break;
			}
			return null;
		}
		/// <summary>
		/// Swaps byte order
		/// </summary>
		/// <param name="x">The word</param>
		/// <returns>A word with swapped byte order</returns>
		public static ushort Swap(ushort x) { return (ushort)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff)); }
		/// <summary>
		/// Swaps byte order
		/// </summary>
		/// <param name="x">The dword</param>
		/// <returns>A dword with swapped byte order</returns>
		public static uint Swap(uint x) { return ((x & 0x000000ff) << 24) + ((x & 0x0000ff00) << 8) + ((x & 0x00ff0000) >> 8) + ((x & 0xff000000) >> 24); }
		/// <summary>
		/// Swaps byte order
		/// </summary>
		/// <param name="x">The word</param>
		/// <returns>A word with swapped byte order</returns>
		public static short Swap(short x) => unchecked((short)Swap(unchecked((ushort)x)));
		/// <summary>
		/// Swaps byte order
		/// </summary>
		/// <param name="x">The dword</param>
		/// <returns>A dword with swapped byte order</returns>
		public static int Swap(int x) => unchecked((int)Swap(unchecked((uint)x)));
	}
}
