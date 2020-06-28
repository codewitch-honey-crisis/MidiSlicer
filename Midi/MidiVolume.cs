using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M
{
	/// <summary>
	/// Represents the mono or stereo volume for a MIDI output device or string
	/// </summary>
#if MIDILIB
	public
#endif
	struct MidiVolume
	{
		/// <summary>
		/// Creates a new instance of the structure
		/// </summary>
		/// <param name="mono">The mono volume</param>
		public MidiVolume(byte mono)
		{
			Left = mono;
			Right = 0;
		}
		/// <summary>
		/// Creates a new instance of the structure
		/// </summary>
		/// <param name="left">The left volume</param>
		/// <param name="right">The right volume</param>
		public MidiVolume(byte left,byte right)
		{
			Left = left;
			Right = right;
		}
		/// <summary>
		/// Indicates the left or mono volume
		/// </summary>
		public byte Left { get; set; }
		/// <summary>
		/// Indicates the right volume
		/// </summary>
		public byte Right { get; set; }
	}
}
