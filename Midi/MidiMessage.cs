namespace M
{
    using System;
    using System.Text;

	/// <summary>
	/// Represents a MIDI message
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial class MidiMessage : ICloneable
	{
		/// <summary>
		/// Creates a MIDI message with the specified status byte
		/// </summary>
		/// <param name="status">The MIDI status byte</param>
		public MidiMessage(byte status) { Status = status; }
		/// <summary>
		/// Indicates the MIDI status byte
		/// </summary>
		public byte Status { get; private set; }
		/// <summary>
		/// Indicates the channel of the MIDI message. Only applies to MIDI channel messages, not MIDI system messages
		/// </summary>
		public byte Channel { get { return unchecked((byte)(Status & 0xF)); } }
		/// <summary>
		/// Indicates the length of the message payload
		/// </summary>
		public virtual int PayloadLength { get { return 0; } }
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected virtual MidiMessage CloneImpl()
		{
			return new MidiMessage(Status);
		}
		/// <summary>
		/// Creates a deep copy of the message
		/// </summary>
		/// <returns>A message that is equivelent to the specified message</returns>
		public MidiMessage Clone()
		{
			return CloneImpl();
		}
		object ICloneable.Clone()
		{
			return Clone();
		}
	}

	/// <summary>
	/// Represents a MIDI message with a single payload byte
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif

	partial class MidiMessageByte : MidiMessage
	{
		/// <summary>
		/// Creates a MIDI message with the specified status and payload
		/// </summary>
		/// <param name="status">The MIDI status byte</param>
		/// <param name="data1">The data byte</param>
		public MidiMessageByte(byte status,byte data1) : base(status) { Data1= data1; }
		/// <summary>
		/// Indicates the data byte for the MIDI message
		/// </summary>
		public byte Data1 { get; private set; }
		/// <summary>
		/// Indicates the payload length for this MIDI message
		/// </summary>
		public override int PayloadLength => 1;
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageByte(Status, Data1);
		}
	}
	/// <summary>
	/// Represents a MIDI message a payload word (2 bytes)
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif

	partial class MidiMessageWord : MidiMessageByte
	{
		/// <summary>
		/// Creates a MIDI message with the specified status and payload
		/// </summary>
		/// <param name="status">The MIDI status byte</param>
		/// <param name="data1">The first data byte</param>
		/// <param name="data2">The second data byte</param>
		public MidiMessageWord(byte status, byte data1,byte data2) : base(status,data1) { Data2 = data2; }
		/// <summary>
		/// Creates a MIDI message with the specified status and payload
		/// </summary>
		/// <param name="status">The MIDI status byte</param>
		/// <param name="data">The data word</param>
		public MidiMessageWord(byte status, short data) : base(
			status,
			unchecked((byte)(data&0x7F)))
		{
			Data2= unchecked((byte)((data / 256) & 0x7F));
		}
		/// <summary>
		/// Indicates the payload length for this MIDI message
		/// </summary>
		public override int PayloadLength => 2;
		/// <summary>
		/// Indicates the second data byte
		/// </summary>
		public byte Data2 { get; private set; }
		/// <summary>
		/// Indicates the data word
		/// </summary>
		public short Data {
			get {
				return unchecked((short)(Data1 +Data2 * 256));
			}
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageWord(Status, Data1,Data2);
		}
	}
	/// <summary>
	/// Represents a MIDI meta-event message with an arbitrary length payload
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial class MidiMessageMeta : MidiMessageByte
	{
		/// <summary>
		/// Creates a MIDI message with the specified status, type and payload
		/// </summary>
		/// <param name="type">The type of the MIDI message</param>
		/// <param name="data">The payload of the MIDI message, as bytes</param>
		public MidiMessageMeta(byte type,byte[] data) : base(0xFF,type)
		{
			Data = data;
		}
		/// <summary>
		/// Creates a MIDI message with the specified status, type and payload
		/// </summary>
		/// <param name="type">The type of the MIDI message</param>
		/// <param name="text">The payload of the MIDI message, as ASCII text</param>
		public MidiMessageMeta(byte type, string text) : base(0xFF, type)
		{
			Data = Encoding.ASCII.GetBytes(text);
		}
		/// <summary>
		/// Indicates the type of the meta-message
		/// </summary>
		public byte Type {
			get {
				return Data1;
			}
		}
		/// <summary>
		/// Indicates the payload length for this MIDI message
		/// </summary>
		public override int PayloadLength => -1;
		/// <summary>
		/// Indicates the payload data, as bytes
		/// </summary>
		public byte[] Data { get; private set; }
		/// <summary>
		/// Indicates the payload data, as ASCII text
		/// </summary>
		public string Text { get {
				return Encoding.ASCII.GetString(Data);
			}
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMeta(Type, Data);
		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Meta: " + Type.ToString("x2") + ", Length: " + Data.Length;
		}
	}
	/// <summary>
	/// Represents a MIDI sequence number meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaSequenceNumber : MidiMessageMeta
	{
		internal MidiMessageMetaSequenceNumber(byte[] data) : base(0,data) { }
		/// <summary>
		/// Creates a new message with the specified sequence number
		/// </summary>
		/// <param name="sequenceNumber">The sequence number</param>
		public MidiMessageMetaSequenceNumber(short sequenceNumber) : base(0, new byte[] { unchecked((byte)(sequenceNumber & 0x7F)),unchecked((byte)((sequenceNumber/ 256) & 0x7F))})
		{

		}
		/// <summary>
		/// Creates a new message with the default sequence number
		/// </summary>
		public MidiMessageMetaSequenceNumber() : base(0, new byte[0])
		{

		}

		/// <summary>
		/// Indicates the sequence number, or -1 if there was none specified
		/// </summary>
		public short SequenceNumber {
			get {
				if (0 == Data.Length)
					return -1;
				return unchecked((short)(Data[0]+ Data[1] * 256));
			}
		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Sequence Number: " + (0 == Data.Length ? "<default>" : SequenceNumber.ToString());
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaSequenceNumber(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI text meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaText : MidiMessageMeta
	{
		internal MidiMessageMetaText(byte[] data) : base(1, data) { }
		/// <summary>
		/// Creates a new instance with the specified text
		/// </summary>
		/// <param name="text">The text</param>
		public MidiMessageMetaText(string text) : base( 1,text??"")
		{

		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Text: " + Text;
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaText(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI copyright meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaCopyright : MidiMessageMeta
	{
		internal MidiMessageMetaCopyright(byte[] data) : base(2, data) { }
		/// <summary>
		/// Creates a new instance with the specified text
		/// </summary>
		/// <param name="text">The text</param>
		public MidiMessageMetaCopyright(string text) : base(2, text ?? "")
		{

		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Copyright: " + Text;
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaCopyright(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI sequence/track name meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaSequenceOrTrackName : MidiMessageMeta
	{
		internal MidiMessageMetaSequenceOrTrackName(byte[] data) : base(3, data) { }
		/// <summary>
		/// Creates a new instance with the specified text
		/// </summary>
		/// <param name="text">The text</param>
		public MidiMessageMetaSequenceOrTrackName(string text) : base(3, text ?? "")
		{

		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Sequence/Track Name: " + Text;
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaSequenceOrTrackName(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI instrument name meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaInstrumentName : MidiMessageMeta
	{
		internal MidiMessageMetaInstrumentName(byte[] data) : base(4, data) { }
		/// <summary>
		/// Creates a new instance with the specified text
		/// </summary>
		/// <param name="text">The text</param>
		public MidiMessageMetaInstrumentName(string text) : base(4, text ?? "")
		{

		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Instrument Name: " + Text;
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaInstrumentName(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI lyric meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaLyric : MidiMessageMeta
	{
		internal MidiMessageMetaLyric(byte[] data) : base(5, data) { }
		/// <summary>
		/// Creates a new instance with the specified text
		/// </summary>
		/// <param name="text">The text</param>
		public MidiMessageMetaLyric(string text) : base(5, text ?? "")
		{

		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Lyric: " + Text;
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaLyric(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI marker meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaMarker : MidiMessageMeta
	{
		internal MidiMessageMetaMarker(byte[] data) : base(6, data) { }
		/// <summary>
		/// Creates a new instance with the specified text
		/// </summary>
		/// <param name="text">The text</param>
		public MidiMessageMetaMarker(string text) : base(6, text ?? "")
		{

		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Marker: " + Text;
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaMarker(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI cue point meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaCuePoint : MidiMessageMeta
	{
		internal MidiMessageMetaCuePoint(byte[] data) : base(7, data) { }
		/// <summary>
		/// Creates a new instance with the specified text
		/// </summary>
		/// <param name="text">The text</param>
		public MidiMessageMetaCuePoint(string text) : base(7, text ?? "")
		{

		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Cue Point: " + Text;
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaCuePoint(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI program name meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaProgramName : MidiMessageMeta
	{
		internal MidiMessageMetaProgramName(byte[] data) : base(8, data) { }
		/// <summary>
		/// Creates a new instance with the specified text
		/// </summary>
		/// <param name="text">The text</param>
		public MidiMessageMetaProgramName(string text) : base(8, text ?? "")
		{

		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Program Name: " + Text;
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaProgramName(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI device port name meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaDevicePortName : MidiMessageMeta
	{
		internal MidiMessageMetaDevicePortName(byte[] data) : base(9, data) { }
		/// <summary>
		/// Creates a new instance with the specified text
		/// </summary>
		/// <param name="text">The text</param>
		public MidiMessageMetaDevicePortName(string text) : base(9, text ?? "")
		{

		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Device Port Name: " + Text;
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaDevicePortName(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI channel prefix meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaChannelPrefix : MidiMessageMeta
	{
		internal MidiMessageMetaChannelPrefix(byte[] data) : base(0x20, data) { }
		/// <summary>
		/// Creates a new instance with the specified channel
		/// </summary>
		/// <param name="channelPrefix">The channel (0-15)</param>
		public MidiMessageMetaChannelPrefix(byte channelPrefix) : base(0x20, new byte[] { unchecked((byte)(channelPrefix & 0x0F)) })
		{

		}
		/// <summary>
		/// Indicates the channel for the channel prefix
		/// </summary>
		public byte ChannelPrefix { get { return Data[0]; } }
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Channel Prefix: " + ChannelPrefix;
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaChannelPrefix(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI port meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaPort : MidiMessageMeta
	{
		internal MidiMessageMetaPort(byte[] data) : base(0x21, data) { }
		/// <summary>
		/// Creates a new instance with the specified port
		/// </summary>
		/// <param name="port">The port (0-127)</param>
		public MidiMessageMetaPort(byte port) : base(0x21, new byte[] { unchecked((byte)(port & 0x7F)) })
		{

		}
		/// <summary>
		/// Indicates the port
		/// </summary>
		public byte Port { get { return Data[0]; } }
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Port: " + Port.ToString();
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaPort(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI end of track meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaEndOfTrack : MidiMessageMeta
	{
		internal MidiMessageMetaEndOfTrack(byte[] data) : base(0x2F, data) { }
		/// <summary>
		/// Creates a new instance 
		/// </summary>
		public MidiMessageMetaEndOfTrack() : base(0x2F,new byte[0]) {}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "<End of Track>";
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaEndOfTrack(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI tempo meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaTempo : MidiMessageMeta
	{
		internal MidiMessageMetaTempo(byte[] data) : base(0x51, data) { }
		/// <summary>
		/// Creates a new instance with the specified tempo
		/// </summary>
		/// <param name="tempo">The tempo</param>
		public MidiMessageMetaTempo(double tempo) :this(MidiUtility.TempoToMicroTempo(tempo))
		{

		}
		/// <summary>
		/// Creates a new instance with the specified microtempo
		/// </summary>
		/// <param name="microTempo">The microtempo</param>
		public MidiMessageMetaTempo(int microTempo) : base(0x51, BitConverter.IsLittleEndian ?
								new byte[] { unchecked((byte)(microTempo >> 16)), unchecked((byte)((microTempo >> 8) & 0xFF)), unchecked((byte)(microTempo & 0xFF)) } :
								new byte[] { unchecked((byte)(microTempo & 0xFF)), unchecked((byte)((microTempo >> 8) & 0xFF)), unchecked((byte)(microTempo >> 16)) })
		{ }
		/// <summary>
		/// Indicates the microtempo of the MIDI message
		/// </summary>
		public int MicroTempo {
			get {
					return BitConverter.IsLittleEndian ?
						(Data[0] << 16) | (Data[1] << 8) | Data[2] :
						(Data[2] << 16) | (Data[1] << 8) | Data[0];
			}
		}
		/// <summary>
		/// Indicates the tempo of the MIDI message
		/// </summary>
		public double Tempo {
			get { return MidiUtility.MicroTempoToTempo(MicroTempo); }
		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Tempo: " + Tempo.ToString();
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaTempo(Data);
		}
	}

	/// <summary>
	/// Represents a MIDI time signature meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaTimeSignature : MidiMessageMeta
	{
		internal MidiMessageMetaTimeSignature(byte[] data) : base(0x58, data) { }
		/// <summary>
		/// Creates a new instance with the specified tempo
		/// </summary>
		/// <param name="timeSignature">The time signature</param>
		public MidiMessageMetaTimeSignature(MidiTimeSignature timeSignature) : base(0x58, new byte[] {
						timeSignature.Numerator,
						unchecked((byte)(Math.Log(timeSignature.Denominator)/Math.Log(2))),
						timeSignature.MidiTicksPerMetronomeTick,
						timeSignature.ThirtySecondNotesPerQuarterNote })
		{

		}
		/// <summary>
		/// Indicates the time signature
		/// </summary>
		public MidiTimeSignature TimeSignature {
			get {
				var num = Data[0];
				var den = Data[1];
				var met = Data[2];
				var q32 = Data[3];
				return new MidiTimeSignature(num, (short)Math.Pow(2, den), met, q32);
			}
		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Time Signature: " + TimeSignature.ToString();
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaTimeSignature(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI time signature meta message
	/// </summary>
#if MIDILIB
	public
#endif
	partial class MidiMessageMetaKeySignature : MidiMessageMeta
	{
		internal MidiMessageMetaKeySignature(byte[] data) : base(0x59, data) { }
		/// <summary>
		/// Creates a new instance with the specified tempo
		/// </summary>
		/// <param name="keySignature">The time signature</param>
		public MidiMessageMetaKeySignature(MidiKeySignature keySignature) : base(0x59, new byte[] {
						unchecked((byte)(0<keySignature.FlatsCount?-keySignature.FlatsCount:keySignature.SharpsCount)),
						unchecked((byte)(keySignature.IsMinor?1:0))})
		{

		}
		/// <summary>
		/// Indicates the key signature
		/// </summary>
		public MidiKeySignature KeySignature {
			get {
				return new MidiKeySignature(unchecked((sbyte)Data[0]), 0 != Data[1]);
			}
		}
		/// <summary>
		/// Retrieves a string representation of the message
		/// </summary>
		/// <returns>A string representing the message</returns>
		public override string ToString()
		{
			return "Key Signature: " + KeySignature.ToString();
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMetaKeySignature(Data);
		}
	}

	/// <summary>
	/// Represents a MIDI system exclusive message with an arbitrary length payload
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial class MidiMessageSysex : MidiMessage
	{
		/// <summary>
		/// Creates a MIDI message with the specified status, type and payload
		/// </summary>
		/// <param name="data">The payload of the MIDI message, as bytes</param>
		public MidiMessageSysex(byte[] data) : base(0xF0)
		{
			Data = data;
		}

		/// <summary>
		/// Indicates the payload length for this MIDI message
		/// </summary>
		public override int PayloadLength => -1;
		/// <summary>
		/// Indicates the payload data, as bytes
		/// </summary>
		public byte[] Data { get; private set; }
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageSysex(Data);
		}
		/// <summary>
		/// Returns a string representation of the message
		/// </summary>
		/// <returns>The string representation of the message</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("Sysex: ");
			for (var i = 0; i < Data.Length; i++)
				sb.Append(Data[i].ToString("X2"));
			return sb.ToString();
		}
	}
	
	// internal class used by the framework
	sealed class MidiMessageSysexPart : MidiMessage
	{
		internal MidiMessageSysexPart(byte[] data) : base(0)
		{
			Data = data;
		}
		public byte[] Data { get; set; }
		public override int PayloadLength => -1;
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageSysexPart(Data);
		}
	}
	/// <summary>
	/// Represents a MIDI song position message
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiMessageSongPosition : MidiMessageWord
	{
		/// <summary>
		/// Creates a new MIDI song position message
		/// </summary>
		/// <param name="position">Indicates the new song position in beats since the start</param>
		public MidiMessageSongPosition(short position) : base(0xF2,position)
		{
		}
		/// <summary>
		/// Creates a new MIDI song position message
		/// </summary>
		/// <param name="position1">The high part of the position</param>
		/// <param name="position2">The low part of the position</param>
		public MidiMessageSongPosition(byte position1,byte position2) : base(0xF2, position1,position2)
		{
		}
		/// <summary>
		/// Indicates the new song position in beats since the start
		/// </summary>
		public short Position {
			get {
				return Data;
			}
		}
		/// <summary>
		/// Clones the message
		/// </summary>
		/// <returns>The new message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageSongPosition(Position);
		}
	}

	/// <summary>
	/// Represents a MIDI song select message
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiMessageSongSelect : MidiMessageByte
	{
		/// <summary>
		/// Creates a new MIDI song select message
		/// </summary>
		/// <param name="songId">Indicates the new song id</param>
		public MidiMessageSongSelect(byte songId) : base(0xF3, songId)
		{
		}
		
		/// <summary>
		/// Indicates the song id
		/// </summary>
		public byte SongId {
			get {
				return Data1;
			}
		}
		/// <summary>
		/// Clones the message
		/// </summary>
		/// <returns>The new message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageSongSelect(SongId);
		}
	}

	/// <summary>
	/// Represents a MIDI tune request message
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiMessageTuneRequest : MidiMessage
	{
		/// <summary>
		/// Creates a new MIDI tune request message
		/// </summary>
		public MidiMessageTuneRequest() : base(0xF6)
		{
		}
		/// <summary>
		/// Clones the message
		/// </summary>
		/// <returns>The new message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageTuneRequest();
		}
	}

	/// <summary>
	/// Represents a MIDI note on message
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial class MidiMessageNoteOn : MidiMessageWord
	{
		/// <summary>
		/// Creates a new MIDI note on message
		/// </summary>
		/// <param name="noteId">The MIDI note id (0-127)</param>
		/// <param name="velocity">The MIDI velocity (0-127)</param>
		/// <param name="channel">The MIDI channel (0-15)</param>
		public MidiMessageNoteOn(byte noteId,byte velocity, byte channel) : base(unchecked((byte)(0x90|channel)),noteId,velocity)
		{

		}
		/// <summary>
		/// Creates a new MIDI note on message
		/// </summary>
		/// <param name="note">The MIDI note</param>
		/// <param name="velocity">The MIDI velocity (0-127)</param>
		/// <param name="channel">The MIDI channel (0-15)</param>
		public MidiMessageNoteOn(string note, byte velocity, byte channel) : this(MidiUtility.NoteToNoteId(note),velocity,channel)
		{

		}
		/// <summary>
		/// Indicates the MIDI note id to play
		/// </summary>
		public byte NoteId {
			get {
				return Data1;
			}
		}
		/// <summary>
		/// Indicates the note for the message
		/// </summary>
		public string Note {
			get {
				return MidiUtility.NoteIdToNote(NoteId, true);
			}
		}
		/// <summary>
		/// Indicates the velocity of the note to play
		/// </summary>
		public byte Velocity {
			get {
				return Data2;
			}
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageNoteOn(NoteId, Velocity, Channel);
		}
		/// <summary>
		/// Gets a string representation of this message
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "Note On: " + Note + ", Velocity: " + Velocity.ToString() + ", Channel: " + Channel.ToString();
		}
		
	}
	/// <summary>
	/// Represents a MIDI note off message
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial class MidiMessageNoteOff : MidiMessageWord
	{
		/// <summary>
		/// Creates a new MIDI note off message
		/// </summary>
		/// <param name="noteId">The MIDI note id (0-127)</param>
		/// <param name="velocity">The MIDI velocity (0-127)</param>
		/// <param name="channel">The MIDI channel (0-15)</param>
		/// <remarks><paramref name="velocity"/> is not used</remarks>
		public MidiMessageNoteOff(byte noteId, byte velocity, byte channel) : base(unchecked((byte)(0x80 | channel)), noteId, velocity)
		{

		}
		/// <summary>
		/// Creates a new MIDI note off message
		/// </summary>
		/// <param name="note">The MIDI note</param>
		/// <param name="velocity">The MIDI velocity (0-127)</param>
		/// <param name="channel">The MIDI channel (0-15)</param>
		/// <remarks><paramref name="velocity"/> is not used</remarks>
		public MidiMessageNoteOff(string note, byte velocity, byte channel) : this(MidiUtility.NoteToNoteId(note),velocity,channel)
		{

		}
		/// <summary>
		/// Indicates the MIDI note id to turn off
		/// </summary>
		public byte NoteId {
			get {
				return Data1;
			}
		}
		/// <summary>
		/// Indicates the note for the message
		/// </summary>
		public string Note {
			get {
				return MidiUtility.NoteIdToNote(NoteId, true);
			}
		}
		/// <summary>
		/// Indicates the velocity of the note to turn off
		/// </summary>
		/// <remarks>This value is not used</remarks>
		public byte Velocity {
			get {
				return Data2;
			}
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageNoteOff(NoteId, Velocity, Channel);
		}
		/// <summary>
		/// Gets a string representation of this message
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "Note Off: " + MidiUtility.NoteIdToNote(NoteId) + ", Velocity: " + Velocity.ToString()+", Channel: "+Channel.ToString();
		}
	}
	/// <summary>
	/// Represents a MIDI key pressure/aftertouch message
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial class MidiMessageKeyPressure : MidiMessageWord
	{
		/// <summary>
		/// Creates a new MIDI key pressure/aftertouch message
		/// </summary>
		/// <param name="noteId">The MIDI note id (0-127)</param>
		/// <param name="pressure">The MIDI pressure (0-127)</param>
		/// <param name="channel">The MIDI channel (0-15)</param>
		public MidiMessageKeyPressure(byte noteId, byte pressure, byte channel) : base(unchecked((byte)(0xA0 | channel)), noteId, pressure)
		{

		}
		/// <summary>
		/// Creates a new MIDI key pressure/aftertouch message
		/// </summary>
		/// <param name="note">The MIDI note</param>
		/// <param name="pressure">The MIDI pressure (0-127)</param>
		/// <param name="channel">The MIDI channel (0-15)</param>
		public MidiMessageKeyPressure(string note, byte pressure, byte channel) : this(MidiUtility.NoteToNoteId(note),pressure,channel)
		{

		}
		/// <summary>
		/// Indicates the assocated MIDI note id
		/// </summary>
		public byte NoteId {
			get {
				return Data1;
			}
		}
		/// <summary>
		/// Indicates the note for the message
		/// </summary>
		public string Note {
			get {
				return MidiUtility.NoteIdToNote(NoteId, true);
			}
		}
		/// <summary>
		/// Indicates the pressure of the note (aftertouch)
		/// </summary>
		public byte Pressure {
			get {
				return Data2;
			}
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageKeyPressure(NoteId, Pressure, Channel);
		}
		/// <summary>
		/// Gets a string representation of this message
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "Key Pressure: " + Note + ", Pressure: " + Pressure.ToString()+", Channel: "+Channel.ToString();
		}
	}
	/// <summary>
	/// Represents a MIDI continuous controller message
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial class MidiMessageCC : MidiMessageWord
	{
		/// <summary>
		/// Creates a new MIDI continuous controller message
		/// </summary>
		/// <param name="controlId">The MIDI controller id (0-127)</param>
		/// <param name="value">The MIDI value (0-127)</param>
		/// <param name="channel">The MIDI channel (0-15)</param>
		public MidiMessageCC(byte controlId, byte value, byte channel) : base(unchecked((byte)(0xB0 | channel)), controlId, value)
		{

		}
		/// <summary>
		/// Indicates the assocated MIDI controller id
		/// </summary>
		public byte ControlId {
			get {
				return Data1;
			}
		}
		/// <summary>
		/// Indicates the value of the controller
		/// </summary>
		public byte Value {
			get {
				return Data2;
			}
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageCC(ControlId, Value, Channel);
		}
		/// <summary>
		/// Gets a string representation of this message
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "CC: " + ControlId.ToString()+ ", Value: " + Value.ToString() + ", Channel: " + Channel.ToString();
		}
	}
	/// <summary>
	/// Represents a MIDI key pressure/aftertouch message
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial class MidiMessagePatchChange : MidiMessageByte
	{
		/// <summary>
		/// Creates a new MIDI key pressure/aftertouch message
		/// </summary>
		/// <param name="patchId">The MIDI patch Id (0-127)</param>
		/// <param name="channel">The MIDI channel (0-15)</param>
		public MidiMessagePatchChange(byte patchId, byte channel) : base(unchecked((byte)(0xC0 | channel)), patchId)
		{

		}
		/// <summary>
		/// Indicates the assocated MIDI patch id
		/// </summary>
		public byte PatchId {
			get {
				return Data1;
			}
		}
		
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessagePatchChange(PatchId, Channel);
		}
		/// <summary>
		/// Gets a string representation of this message
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "Patch Change: "+PatchId.ToString()+", Channel: " + Channel.ToString();
		}
	}
	/// <summary>
	/// Represents a MIDI key pressure/aftertouch message
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial class MidiMessageChannelPressure : MidiMessageByte
	{
		/// <summary>
		/// Creates a new MIDI key pressure/aftertouch message
		/// </summary>
		/// <param name="pressure">The MIDI pressure (0-127)</param>
		/// <param name="channel">The MIDI channel (0-15)</param>
		public MidiMessageChannelPressure(byte pressure, byte channel) : base(unchecked((byte)(0xD0 | channel)), pressure)
		{

		}
		/// <summary>
		/// Indicates the pressure of the channel (aftertouch)
		/// </summary>
		/// <remarks>Indicates the single greatest pressure/aftertouch off all pressed notes</remarks>
		public byte Pressure {
			get {
				return Data1;
			}
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageChannelPressure(Pressure, Channel);
		}
		/// <summary>
		/// Gets a string representation of this message
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "Channel Pressure: " + Pressure.ToString() + ", Channel: " + Channel.ToString();
		}
	}
	/// <summary>
	/// Represents a MIDI channel pitch/pitch wheel message
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial class MidiMessageChannelPitch : MidiMessageWord
	{
		/// <summary>
		/// Creates a new MIDI channel pitch message
		/// </summary>
		/// <param name="pitch">The MIDI pressure (0-16383)</param>
		/// <param name="channel">The MIDI channel (0-15)</param>
		public MidiMessageChannelPitch(short pitch, byte channel) : base(unchecked((byte)(0xE0 | channel)), BitConverter.IsLittleEndian?MidiUtility.Swap(pitch):pitch)
		{

		}
		internal MidiMessageChannelPitch(byte pitch1,byte pitch2, byte channel) : base(unchecked((byte)(0xE0 | channel)), pitch1,pitch2)
		{

		}
		/// <summary>
		/// Indicates the pitch of the channel (pitch wheel position)
		/// </summary>
		public short Pitch {
			get {
				if (BitConverter.IsLittleEndian)
					return MidiUtility.Swap(Data);
				return Data;
			}
		}
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageChannelPitch(Data1,Data2, Channel);
		}
		/// <summary>
		/// Gets a string representation of this message
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "Channel Pitch: " + Pitch.ToString() + ", Channel: " + Channel.ToString();
		}
	}
	/// <summary>
	/// Represents a MIDI real-time message
	/// </summary>
#if MIDILIB
	public
#endif
	abstract class MidiMessageRealTime : MidiMessage
	{
		/// <summary>
		/// Creates a MIDI real-time message
		/// </summary>
		/// <param name="status"></param>
		protected MidiMessageRealTime(byte status) : base(status)
		{

		}
	}
	/// <summary>
	/// Represents a MIDI reset message
	/// </summary>
	/// <remarks>This message shares the same status code with MIDI a meta-message. The meta-messages come from files but this comes over the wire whereas meta-messages do not.</remarks>
#if MIDILIB
	public
#endif
	sealed class MidiMessageRealTimeReset : MidiMessageRealTime
	{
		/// <summary>
		/// Creates a new MIDI reset message
		/// </summary>
		public MidiMessageRealTimeReset() : base(0xFF)
		{
		}
		/// <summary>
		/// Clones the message
		/// </summary>
		/// <returns>The new message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageRealTimeReset();
		}
		/// <summary>
		/// Indicates the payload of the message
		/// </summary>
		public override int PayloadLength => 0;
	}
	/// <summary>
	/// Represents a MIDI active sensing message
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiMessageRealTimeActiveSensing : MidiMessageRealTime
	{
		/// <summary>
		/// Creates a new MIDI active sensing message
		/// </summary>
		public MidiMessageRealTimeActiveSensing() : base(0xFE)
		{
		}
		/// <summary>
		/// Clones the message
		/// </summary>
		/// <returns>The new message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageRealTimeActiveSensing();
		}
		/// <summary>
		/// Indicates the payload of the message
		/// </summary>
		public override int PayloadLength => 0;
	}
	/// <summary>
	/// Represents a MIDI start message
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiMessageRealTimeStart : MidiMessageRealTime
	{
		/// <summary>
		/// Creates a new MIDI start message
		/// </summary>
		public MidiMessageRealTimeStart() : base(0xFA)
		{
		}
		/// <summary>
		/// Clones the message
		/// </summary>
		/// <returns>The new message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageRealTimeStart();
		}
		/// <summary>
		/// Indicates the payload of the message
		/// </summary>
		public override int PayloadLength => 0;
	}
	/// <summary>
	/// Represents a MIDI continue message
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiMessageRealTimeContinue : MidiMessageRealTime
	{
		/// <summary>
		/// Creates a new MIDI continue message
		/// </summary>
		public MidiMessageRealTimeContinue() : base(0xFB)
		{
		}
		/// <summary>
		/// Clones the message
		/// </summary>
		/// <returns>The new message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageRealTimeContinue();
		}
		/// <summary>
		/// Indicates the payload of the message
		/// </summary>
		public override int PayloadLength => 0;
	}
	/// <summary>
	/// Represents a MIDI stop message
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiMessageRealTimeStop : MidiMessageRealTime
	{
		/// <summary>
		/// Creates a new MIDI stop message
		/// </summary>
		public MidiMessageRealTimeStop() : base(0xFC)
		{
		}
		/// <summary>
		/// Clones the message
		/// </summary>
		/// <returns>The new message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageRealTimeStop();
		}
		/// <summary>
		/// Indicates the payload of the message
		/// </summary>
		public override int PayloadLength => 0;
	}
	/// <summary>
	/// Represents a MIDI start message
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiMessageRealTimeTimingClock : MidiMessageRealTime
	{
		/// <summary>
		/// Creates a new MIDI timing clock message
		/// </summary>
		public MidiMessageRealTimeTimingClock() : base(0xF8)
		{
		}
		/// <summary>
		/// Clones the message
		/// </summary>
		/// <returns>The new message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageRealTimeTimingClock();
		}
		/// <summary>
		/// Indicates the payload of the message
		/// </summary>
		public override int PayloadLength => 0;
	}
}
