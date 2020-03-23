namespace M
{
	/// <summary>
	/// Represents a MIDI key signature
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	struct MidiKeySignature
	{
		/// <summary>
		/// The number of sharps in the signature (0-7)
		/// </summary>
		public byte SharpsCount { get; private set; }
		/// <summary>
		/// The number of flats in the signature (0-7)
		/// </summary>
		public byte FlatsCount { get; private set; }
		/// <summary>
		/// Indicates true if the scale is minor, otherwise false if it is major
		/// </summary>
		public bool IsMinor { get; private set; }
		/// <summary>
		/// Creates a new instance with the specified paramters
		/// </summary>
		/// <param name="sigCode">The signature code: negative for flats, positive for sharps (-7 to 7, inclusive)</param>
		/// <param name="isMinor">Indicates whether or not the scale is minor</param>
		public MidiKeySignature(sbyte sigCode,bool isMinor)
		{
			if (0 > sigCode)
			{
				FlatsCount = unchecked((byte)-sigCode);
				SharpsCount = 0;
			}
			else
			{
				FlatsCount = 0;
				SharpsCount = unchecked((byte)sigCode);
			}
			IsMinor = isMinor;
		}
		/// <summary>
		/// Indicates the default value for the MIDI key signature
		/// </summary>
		public static MidiKeySignature Default {
			get {
				return new MidiKeySignature(0, false);
			}
		}
		/// <summary>
		/// Retrieves a string representation of the key signature
		/// </summary>
		/// <returns>A string representing the key signature</returns>
		public override string ToString()
		{
			sbyte scode;
			if (0 < FlatsCount)
				scode = unchecked((sbyte)-FlatsCount);
			else
				scode = unchecked((sbyte)SharpsCount);
			// TODO: return an actual key sig here
			return scode.ToString() + " " + (IsMinor ? "minor" : "major");
		}
	}
}
