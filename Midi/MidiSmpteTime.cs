using System;

namespace M
{
	/// <summary>
	/// Represents an SMPTE timestamp
	/// </summary>
#if MIDILIB
	public
#endif
	struct MidiSmpteTime
	{
		/// <summary>
		/// The time
		/// </summary>
		public TimeSpan Time { get; }
		/// <summary>
		/// The frames
		/// </summary>
		public byte Frames { get; }
		/// <summary>
		/// The frames per second
		/// </summary>
		public byte FramesPerSecond { get; }
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="time"></param>
		/// <param name="frames"></param>
		/// <param name="framesPerSecond"></param>
		public MidiSmpteTime(TimeSpan time,byte frames, byte framesPerSecond)
		{
			Time = time;
			Frames = frames;
			FramesPerSecond = framesPerSecond;
		}

	}
}
