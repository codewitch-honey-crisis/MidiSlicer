namespace M
{
	using System;
	/// <summary>
	/// Represents the current state of a playing MIDI sequence
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	sealed partial class MidiContext
	{
		/// <summary>
		/// Indicates the channels for the current MIDI context
		/// </summary>
		public Channel[] Channels { get; private set; }
		/// <summary>
		/// Indicates the running status byte for the current MIDI context
		/// </summary>
		public byte RunningStatus { get; private set; }
		/// <summary>
		/// Indicates the channel prefix for the current MIDI context
		/// </summary>
		public byte ChannelPrefix { get; private set; }
		/// <summary>
		/// Indicates the microtempo for the current MIDI context
		/// </summary>
		public int MicroTempo { get; private set; }
		/// <summary>
		/// Indicates the time signature for the current MIDI context
		/// </summary>
		public MidiTimeSignature TimeSignature { get; private set; }
		
		static Channel[] _InitChannels()
		{
			Channel[] result = new Channel[16];
			for (int i = 0; i < result.Length; i++)
				result[i] = new Channel();
			return result;
		}

		internal MidiContext()
		{
			Channels=_InitChannels();
			RunningStatus = 0;
			ChannelPrefix = 0xFF;
			MicroTempo = 0;
			TimeSignature = MidiTimeSignature.Default;
		}
		/// <summary>
		/// Process a message, adjusting the MIDI context
		/// </summary>
		/// <param name="message">The message to process</param>
		public void ProcessMessage(MidiMessage message)
		{
			var hasStatus = true;
			if (0 != message.Status)
				RunningStatus = message.Status;
			else
				hasStatus = false;
			int cn = (hasStatus ? message.Status & 0xF : (0XFF != ChannelPrefix ? ChannelPrefix : (RunningStatus & 0xF)));
			switch (RunningStatus & 0xF0)
			{
				case 0xC0:
					var mb = message as MidiMessageByte;
					Channels[cn].Patch = mb.Data1;
					break;
				case 0xD0:
					mb = message as MidiMessageByte;
					Channels[cn].ChannelPressure = mb.Data1;
					break;
				case 0x80:
					var mw = message as MidiMessageWord;
					Channels[cn].Notes[mw.Data1] = 0;
					break;
				case 0x90:
					mw = message as MidiMessageWord;
					Channels[cn].Notes[mw.Data1] = mw.Data2;
					break;
				case 0xA0:
					mw = message as MidiMessageWord;
					Channels[cn].KeyPressure[mw.Data1] = mw.Data2;
					break;
				case 0xB0:
					mw = message as MidiMessageWord;
					// respect send all notes off as necessary
					if (mw.Data1 >= 123 && mw.Data2 >= 127)
					{
						for (int i = 0; i < 128; ++i)
						{
							var b = Channels[cn].Notes[i];
							if (0xFF != b)
								Channels[cn].Notes[i] = 0;
						}
					}
					Channels[cn].Controls[mw.Data1] = mw.Data2;
					break;
				case 0xE0:
					mw = message as MidiMessageWord;
					Channels[cn].PitchWheel = mw.Data;
					break;
				case 0xF0:
					switch(RunningStatus & 0xF)
					{
						case 0xF:
							var mbs = message as MidiMessageMeta;
							switch(mbs.Data1)
							{
								case 0x20:
									ChannelPrefix = mbs.Data[0];
									break;
								case 0x51:
									if (BitConverter.IsLittleEndian)
										MicroTempo=(mbs.Data[0] << 16) | (mbs.Data[1] << 8) | mbs.Data[2];
									else
										MicroTempo=(mbs.Data[2] << 16) | (mbs.Data[1] << 8) | mbs.Data[0];
									break;
								case 0x58:
									TimeSignature = new MidiTimeSignature(mbs.Data[0], (byte)Math.Pow(mbs.Data[1], 2), mbs.Data[2], mbs.Data[3]);
									break;
							}
							break;
							
					}
					break;
			}
		}
		/// <summary>
		/// Represents the status of a MIDI channel
		/// </summary>
#if MIDILIB
		public
#else
		internal
#endif
		sealed partial class Channel
		{
			/// <summary>
			/// Indicates the current MIDI CC values for the channel
			/// </summary>
			public byte[] Controls { get; internal set; }
			/// <summary>
			/// Indicates the current MIDI note states for the channel
			/// </summary>
			public byte[] Notes { get; internal set; }
			/// <summary>
			/// Indicates the current MIDI aftertouch for the current key
			/// </summary>
			public byte[] KeyPressure { get; internal set; }
			/// <summary>
			/// Indicates the current MIDI aftertouch for the current channel
			/// </summary>
			public byte ChannelPressure { get; internal set; }
			/// <summary>
			/// Indicates the current position of the MIDI pitch wheel
			/// </summary>
			public short PitchWheel { get; internal set; }
			/// <summary>
			/// Indicates the current MIDI patch for the channel
			/// </summary>
			public byte Patch { get; internal set; }

			internal Channel()
			{
				Controls = new byte[128];
				for (int i = 0; i < 128; ++i) Controls[i] = 0xFF;
				Notes = new byte[128];
				for (int i = 0; i < 128; ++i) Notes[i] = 0xFF;
				KeyPressure = new byte[128];
				for (int i = 0; i < 128; ++i) KeyPressure[i] = 0xFF;
				ChannelPressure = 0xFF;
				PitchWheel = -1;
				Patch = 0xFF;
			}
		}
	}
}
