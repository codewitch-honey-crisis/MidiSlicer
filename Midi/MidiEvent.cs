namespace M
{
	using System;
	using System.IO;
	/// <summary>
	/// Represents a single MIDI event
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	partial class MidiEvent
	{
		/// <summary>
		/// Creates an event at the specified position with the specified MIDI message
		/// </summary>
		/// <param name="position">The position in MIDI ticks</param>
		/// <param name="message">The MIDI message for this event</param>
		public MidiEvent(int position,MidiMessage message)
		{
			Position = position;
			Message = message;
		}
		/// <summary>
		/// Indicates the position in MIDI ticks
		/// </summary>
		public int Position { get; private set; }
		/// <summary>
		/// Indicates the MIDI message associated with this event
		/// </summary>
		public MidiMessage Message { get; private set; }
	}
}
