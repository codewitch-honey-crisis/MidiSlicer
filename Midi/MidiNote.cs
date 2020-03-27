namespace M
{
	/// <summary>
	/// Represents a single note in a MIDI note map
	/// </summary>
#if MIDILIB
	public
#endif
	partial struct MidiNote
	{
		/// <summary>
		/// Indicates the absolute note position, in ticks
		/// </summary>
		public int Position { get; }
		/// <summary>
		/// Indicates the MIDI channel (0-15)
		/// </summary>
		public byte Channel { get; }
		/// <summary>
		/// Indicates the MIDI note id (0-127)
		/// </summary>
		public byte NoteId { get; }
		/// <summary>
		/// Indicates the MIDI note
		/// </summary>
		public string Note { 
			get {
				return MidiUtility.NoteIdToNote(NoteId);
			} 
		}
		/// <summary>
		/// Indicates the velocity (0-127)
		/// </summary>
		public byte Velocity { get; }
		/// <summary>
		/// Gets the length of the note, in ticks
		/// </summary>
		public int Length { get; }
		/// <summary>
		/// Creates a new MIDI note instance
		/// </summary>
		/// <param name="position">The position</param>
		/// <param name="channel">The channel (0-15)</param>
		/// <param name="noteId">The note id (0-127)</param>
		/// <param name="velocity">The velocity (0-127)</param>
		/// <param name="length">The length</param>
		public MidiNote(int position,byte channel, byte noteId, byte velocity, int length)
		{
			Position = position;
			Channel = unchecked((byte)(channel & 0x0F));
			NoteId = unchecked((byte)(noteId & 0x7F));
			Velocity= unchecked((byte)(velocity & 0x7F));
			Length = length;
		}
		/// <summary>
		/// Creates a new MIDI note instance
		/// </summary>
		/// <param name="position">The position</param>
		/// <param name="channel">The channel (0-15)</param>
		/// <param name="note">The note</param>
		/// <param name="velocity">The velocity (0-127)</param>
		/// <param name="length">The length</param>
		public MidiNote(int position, byte channel, string note, byte velocity, int length)
		{
			Position = position;
			Channel = unchecked((byte)(channel & 0x0F));
			NoteId = MidiUtility.NoteToNoteId(note);
			Velocity = unchecked((byte)(velocity & 0x7F));
			Length = length;
		}
		/// <summary>
		/// Gets a string representation of the note
		/// </summary>
		/// <returns>A string representing the note</returns>
		public override string ToString()
		{
			return "Position: " + Position.ToString() + ", Channel: " + Channel.ToString() + ", Note: " + Note + ", Velocity: " + Velocity.ToString() + ", Length: " + Length.ToString();
		}
	}
}
