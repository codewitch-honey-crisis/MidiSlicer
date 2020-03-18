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
		/// Indicates the channel of the MIDI message
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
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageMeta(Status, Data1, Data);
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
		/// <summary>
		/// When overridden in a derived class, implements Clone()
		/// </summary>
		/// <returns>The cloned MIDI message</returns>
		protected override MidiMessage CloneImpl()
		{
			return new MidiMessageSysex(Status, Data);
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
		/// Indicates the MIDI note id to play
		/// </summary>
		public byte NoteId {
			get {
				return Data1;
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
		/// Indicates the MIDI note id to turn off
		/// </summary>
		public byte NoteId {
			get {
				return Data1;
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
		/// Indicates the assocated MIDI note id
		/// </summary>
		public byte NoteId {
			get {
				return Data1;
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
	}
}
