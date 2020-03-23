namespace M
{
	/// <summary>
	/// Represents a MIDI time signature
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial struct MidiTimeSignature
	{
		/// <summary>
		/// Indicates the numerator of the time signature
		/// </summary>
		public byte Numerator { get; private set; }
		/// <summary>
		/// Indicates the denominator of the time signature
		/// </summary>
		public short Denominator { get; private set; }
		/// <summary>
		/// Indicates the MIDI ticks/pulses per metronome tick
		/// </summary>
		public byte MidiTicksPerMetronomeTick { get; private set; }
		/// <summary>
		/// Indicates the 32nd notes per quarter note
		/// </summary>
		public byte ThirtySecondNotesPerQuarterNote { get; private set; }
		/// <summary>
		/// Creates a new instance of a MIDI time signature with the specified parameters
		/// </summary>
		/// <param name="numerator">The numerator</param>
		/// <param name="denominator">The denominator</param>
		/// <param name="midiTicksPerMetronomeTick">The MIDI ticks per metronome tick</param>
		/// <param name="thirtySecondNotesPerQuarterNote">The 32nd notes per quarter note</param>
		public MidiTimeSignature(byte numerator, short denominator, byte midiTicksPerMetronomeTick, byte thirtySecondNotesPerQuarterNote)
		{
			Numerator = numerator;
			Denominator = denominator;
			MidiTicksPerMetronomeTick = midiTicksPerMetronomeTick;
			ThirtySecondNotesPerQuarterNote = thirtySecondNotesPerQuarterNote;
		}
		/// <summary>
		/// Indicates the default time signature
		/// </summary>
		public static MidiTimeSignature Default 
		{
			get {
				return new MidiTimeSignature(4, 4, 24, 8);
			}
		}
		/// <summary>
		/// Retrieves a string representation of the time signature
		/// </summary>
		/// <returns>A string representing the time signature</returns>
		public override string ToString()
		{
			return Numerator.ToString() + "/" + Denominator.ToString();
		}
	}
}
