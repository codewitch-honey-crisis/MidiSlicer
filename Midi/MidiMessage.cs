namespace M
{
	using System.Text;

	/// <summary>
	/// Represents a MIDI message
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial class MidiMessage
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
		/// Indicates the length of the message payload
		/// </summary>
		public virtual int PayloadLength { get { return 0; } }
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
			Data2= unchecked((byte)((data >> 7) & 0x7F));
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
				return unchecked((short)(Data1 + (Data2 << 7)));
			}
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
		/// <param name="status">The MIDI status byte</param>
		/// <param name="type">The type of the MIDI message</param>
		/// <param name="data">The payload of the MIDI message, as bytes</param>
		public MidiMessageMeta(byte status,byte type,byte[] data) : base(status,type)
		{
			Data = data;
		}
		/// <summary>
		/// Creates a MIDI message with the specified status, type and payload
		/// </summary>
		/// <param name="status">The MIDI status byte</param>
		/// <param name="type">The type of the MIDI message</param>
		/// <param name="text">The payload of the MIDI message, as ASCII text</param>
		public MidiMessageMeta(byte status, byte type, string text) : base(status, type)
		{
			Data = Encoding.ASCII.GetBytes(text);
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
		/// <param name="status">The MIDI status byte</param>
		/// <param name="data">The payload of the MIDI message, as bytes</param>
		public MidiMessageSysex(byte status, byte[] data) : base(status)
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
	
	}
}
