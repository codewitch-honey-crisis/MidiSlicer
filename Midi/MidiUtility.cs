
namespace M
{
	using System;
	using System.IO;

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
				bn += ch.ToString();
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
		/// <summary>
		/// Reads the specified number of bytes from a stream. If the specified number of bytes is not present, this routine throws
		/// </summary>
		/// <param name="stm">The stream to read</param>
		/// <param name="buffer">The buffer that holds the data</param>
		/// <param name="len">The number of bytes to read</param>
		public static void ReadChecked(Stream stm, byte[] buffer, int len)
		{
			if (len > stm.Read(buffer, 0, len))
				throw new EndOfStreamException();
		}
		/// <summary>
		/// Reads a MIDI variable length value from the stream
		/// </summary>
		/// <param name="stm">The stream to read from</param>
		/// <param name="val">The value read</param>
		/// <param name="firstByte">The first byte of the value, if already read</param>
		/// <returns>The number of bytes read</returns>
		public static int ReadVariableLength(Stream stm, out int val, int firstByte = -1)
		{
			val = 0;
			var read = 0;
			var b = firstByte;
			if (b == -1)
			{
				b=stm.ReadByte();
				if (b == -1) return 0;
				++read;
			}
			while (b > 0x7F)
			{
				val = (val << 8) | (b & 0x7F);
				b = stm.ReadByte();
				if (-1 == b)
					return read;
				++read;
			}
			val = (val << 8) | b;
			return read;
		}
		
	}
}
