
namespace M
{
	using System;
    using System.Runtime.InteropServices;
    using System.Text;

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
		public static int GetMidiOutputDeviceCount()
		{
			return midiOutGetNumDevs();
		}
		/// <summary>
		/// Retrieves the count of MIDI input devices
		/// </summary>
		/// <returns>The count of devices</returns>
		public static int GetMidiInputDeviceCount()
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
			_CheckOutResult(midiInOpen(ref handle, deviceIndex, callback, 0, CALLBACK_FUNCTION));
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
		/// <summary>
		/// Closes the specified MIDI input device
		/// </summary>
		/// <param name="handle">The handle of the device</param>
		public static void CloseInputDevice(IntPtr handle)
		{
			_CheckOutResult(midiInClose(handle));
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
		/// Packs a MIDI message as an int
		/// </summary>
		/// <param name="status">The status byte</param>
		/// <param name="data1">The first data byte</param>
		/// <param name="data2">The second data byte</param>
		/// <param name="channel">The channel</param>
		/// <returns>An integer representing the packed MIDI message</returns>
		public static int PackMessage(byte status, byte data1, byte data2, byte channel = 0)
		{
			if (0 == channel)
				return ((data2 & 0x7F) << 16) +
				((data1 & 0x7F) << 8) + status;
			return ((data2 & 0x7F) << 16) +
			((data1 & 0x7F) << 8) + ((status & 0xF0) | (channel & 0xF));

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
