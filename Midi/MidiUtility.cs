
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
		/// <remarks>This is geared for the wire protocol, not files. The message must not be a sysex message</remarks>
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
					if(0xF0==message.Status)
						throw new NotSupportedException("The message must not be a sysex message");
					switch(message.Status & 0xF)
					{
						case 2:
							mw = message as MidiMessageWord;
							return PackMessage(mw.Status, mw.Data1, mw.Data2);
						case 3:
							mb = message as MidiMessageByte;
							return PackMessage(mb.Status, mb.Data1, 0);
						case 6:
						case 8:
						case 0xA:
						case 0xB:
						case 0xC:
						case 0xE:
						case 0xF:
							return PackMessage(message.Status, 0, 0);
					}
					throw new NotSupportedException("Unsupported message");

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
				case 0xF0:
					switch(message & 0x0F)
					{
						case 0:
							throw new NotSupportedException("The MIDI sysex message is not supported in this context.");
						case 2:
							return new MidiMessageSongPosition(unchecked((byte)((message >> 8) & 0xFF)), unchecked((byte)((message >> 16) & 0xFF)));
						case 3:
							return new MidiMessageSongSelect(unchecked((byte)((message >> 8) & 0xFF)));
						case 6:
							return new MidiMessageTuneRequest();
						case 7:
							// we should not get this here!
							throw new NotSupportedException("Illegal MIDI message data found.");
						case 8:
							return new MidiMessageRealTimeTimingClock();
						case 0xA:
							return new MidiMessageRealTimeStart();
						case 0xB:
							return new MidiMessageRealTimeContinue();
						case 0xC:
							return new MidiMessageRealTimeStop();
						case 0xE:
							return new MidiMessageRealTimeActiveSensing();
						case 0xF:
							return new MidiMessageRealTimeReset();
						default:
							throw new NotSupportedException("The MIDI message is not recognized.");
					}
				default:
					throw new NotSupportedException("Illegal MIDI message data found.");
			}

		}
		
		/// <summary>
		/// Gets the bytes that make up a MIDI message
		/// </summary>
		/// <param name="message">The message to retrieve the bytes for</param>
		/// <returns>An array of bytes containing the message data</returns>
		/// <remarks>This is geared for the wire protocol, not files</remarks>
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
					switch(message.Status & 0xF)
					{
						case 0:
							var msgsx = message as MidiMessageSysex;
							data = new byte[1 + msgsx.Data.Length];
							data[0] = msgsx.Status;
							Array.Copy(msgsx.Data, 0, data, 1, msgsx.Data.Length);
							return data;
						case 2:
							msgw= message as MidiMessageWord;
							data = new byte[3];
							data[0] = msgw.Status;
							data[1] = msgw.Data1;
							data[2] = msgw.Data2;
							return data;
						case 3:
							msgb = message as MidiMessageByte;
							data = new byte[2];
							data[0] = msgb.Status;
							data[1] = msgb.Data1;
							return data;
						case 6:
						case 8:
						case 0xA:
						case 0xB:
						case 0xC:
						case 0xE:
							data = new byte[1];
							data[0] = message.Status;
							return data;
						case 0xF:
							var msgm = message as MidiMessageMeta;
							if (null != msgm)
								throw new InvalidOperationException("MIDI meta messages cannot be used in this context.");
							data = new byte[1];
							data[0] = message.Status;
							return data;
						case 7:
							throw new InvalidProgramException("Illegal MIDI message");
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
