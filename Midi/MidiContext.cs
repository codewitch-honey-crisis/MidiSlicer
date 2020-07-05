namespace M
{
	using System;
	/// <summary>
	/// Represents the current state of a playing MIDI sequence
	/// </summary>
#if MIDILIB
	public
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
		/// Indicates the micro tempo for the current MIDI context
		/// </summary>
		public int MicroTempo { get; private set; }
		/// <summary>
		/// Indicates the time base for the current MIDI context
		/// </summary>
		public short TimeBase { get; private set; }
		/// <summary>
		/// Indicates the number of MIDI ticks that have elapsed
		/// </summary>
		public int Ticks { get; private set; }
		/// <summary>
		/// Indicates the number of system ticks that have elapsed
		/// </summary>
		public long SystemTicks { get; private set; }
		/// <summary>
		/// Indicates the time of the current position
		/// </summary>
		public TimeSpan Time { get { return new TimeSpan(SystemTicks); } }
		/// <summary>
		/// Indicates the time signature for the current MIDI context
		/// </summary>
		public MidiTimeSignature TimeSignature { get; private set; }
		/// <summary>
		/// Indicates the key signature for the current MIDI context
		/// </summary>
		public MidiKeySignature KeySignature { get; private set; }
		
		static Channel[] _InitChannels()
		{
			Channel[] result = new Channel[16];
			for (int i = 0; i < result.Length; i++)
				result[i] = new Channel();
			return result;
		}
		/// <summary>
		/// Constructs a new instance of a MidiContext
		/// </summary>
		/// <param name="timeBase">the time base to use</param>
		public MidiContext(short timeBase = 24)
		{
			Channels=_InitChannels();
			RunningStatus = 0;
			ChannelPrefix = 0xFF;
			MicroTempo = 500000;
			TimeBase = timeBase;
			TimeSignature = MidiTimeSignature.Default;
		}
		/// <summary>
		/// Processes an event 
		/// </summary>
		/// <param name="event">The event to process. The event.Message member can be null in which case the message part is not processed</param>
		public void Process(MidiEvent @event) 
		{
			var delta = @event.Position;
			// recompute our timing based on current microTempo and timeBase
			var ticksusec = MicroTempo / (double)TimeBase;
			var tickspertick = ticksusec / (TimeSpan.TicksPerMillisecond / 1000) * 100;
			Ticks += delta;
			SystemTicks += unchecked((long)(delta * tickspertick));
			if (null != @event.Message)
				Process(@event.Message);
		}
		/// <summary>
		/// Process a message, adjusting the MIDI context
		/// </summary>
		/// <param name="message">The message to process</param>
		public void Process(MidiMessage message)
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
					Channels[cn].Program = mb.Data1;
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
									TimeSignature = new MidiTimeSignature(mbs.Data[0], (byte)Math.Pow(2,mbs.Data[1]), mbs.Data[2], mbs.Data[3]);
									break;
								case 0x59:
									KeySignature = new MidiKeySignature(unchecked((sbyte)mbs.Data[0]), 0 != mbs.Data[1]);
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
			public byte Program { get; internal set; }

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
				Program = 0xFF;
			}
		}
		/// <summary>
		/// Creates a deep copy of the MIDI context
		/// </summary>
		/// <returns>A new copy of the MIDI context</returns>
		public MidiContext Clone()
		{
			var result = new MidiContext(TimeBase);
			result.ChannelPrefix = ChannelPrefix;
			for(var i = 0;i<16;i++)
			{
				var dst = result.Channels[i];
				var src = Channels[i];
				for(var j=0;j<128; j++)
				{
					dst.Controls[j] = src.Controls[j];
					dst.Notes[j] = src.Notes[j];
				}
				dst.ChannelPressure = src.ChannelPressure;
				dst.KeyPressure = src.KeyPressure;
				dst.Program = src.Program;
				dst.PitchWheel = src.PitchWheel;
			}
			result.KeySignature = KeySignature;
			result.MicroTempo = MicroTempo;
			result.RunningStatus = RunningStatus;
			result.SystemTicks = SystemTicks;
			result.Ticks = Ticks;
			result.TimeSignature = TimeSignature;
			return result;
		}
	}
}
