namespace M
{

	using System;
	using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a MIDI sequence
    /// </summary>
    /// <remarks>Each of these corresponds to one MIDI track</remarks>
#if MIDILIB
    public
#else
	internal
#endif
	sealed partial class MidiSequence : ICloneable
	{
		#region Win32
		[DllImport("Kernel32.dll", EntryPoint = "GetSystemTimePreciseAsFileTime", CallingConvention = CallingConvention.Winapi)]
		static extern void _GetSystemTimePreciseAsFileTime(out long filetime);
		#endregion
		static long _PreciseUtcNowTicks {
			get {
				long filetime;
				_GetSystemTimePreciseAsFileTime(out filetime);

				return filetime + 504911232000000000;
			}
		}
		/// <summary>
		/// Creates a new MIDI sequence
		/// </summary>
		public MidiSequence()
		{
			Events = new List<MidiEvent>();
		}
		/// <summary>
		/// Indicates the events of the MIDI sequence
		/// </summary>
		public IList<MidiEvent> Events { get; private set; }
		
		/// <summary>
		/// Indicates the first downbeat of the MIDI sequence
		/// </summary>
		public int FirstDownBeat {
			get {
				foreach (var e in AbsoluteEvents)
				{
					var m = e.Message;
					if (0x99 == m.Status) // note down, percussion track
					{
						var mcm = m as MidiMessageWord;
						if (0!=mcm.Data2 && (35 == mcm.Data1 || 36==mcm.Data1) ) // bass drum or accoustic bass drum
						{
							return e.Position;
						}
					}
				}
				return -1;

			}
		}
		/// <summary>
		/// Indicates the first note on message in the MIDI sequence
		/// </summary>
		public int FirstNoteOn {
			get {
				foreach (var e in AbsoluteEvents)
				{
					var m = e.Message;
					if (0x90 == (m.Status & 0xF0)) // note down
					{
						var mcm = m as MidiMessageWord;
						if (0!=mcm.Data2)
						{
							return e.Position;
						}
					}
				}
				return -1;

			}
		}
		/// <summary>
		/// Gets the root note id of the sequence or > 127 if not found
		/// </summary>
		public byte RootNoteId {
			get {
				foreach(var ev in Events)
				{
					var m = ev.Message;
					if(0x90==(m.Status & 0xF0))
					{
						var mw = m as MidiMessageWord;
						return mw.Data1;
					}
				}
				return 0x80;
			}
		}
		/// <summary>
		/// Retrieves the root note of the sequence or null if not found
		/// </summary>
		public string RootNote {
			get {
				var noteId = RootNoteId;
				if (0x7F < noteId)
					return null;
				return MidiUtility.NoteIdToNote(noteId,true);
			}
		}
		/// <summary>
		/// Gets the <see cref="MidiContext"/> at the specified position
		/// </summary>
		/// <param name="position">The position to retrieve the context from, in ticks</param>
		/// <param name="timeBase">The time base to use</param>
		/// <returns>A MIDI context for this position</returns>
		public MidiContext GetContext(int position=0,short timeBase = 24)
		{
			var result = new MidiContext(timeBase);
			int pos = 0;
			foreach(var e in Events)
			{
				pos += e.Position;
				if (pos > position)
				{
					// advance the cursor by the remainder
					var tev = new MidiEvent((pos - position), null);
					result.Process(tev);
					return result;
				}
				result.Process(e);
			}
			if(pos<position)
			{
				var tev = new MidiEvent((position - pos), null);
				result.Process(tev);
			}
			return result;
		}
		/// <summary>
		/// Gets the MIDI tick position for the current sequence at the current time
		/// </summary>
		/// <param name="systemTicks">The number of system ticks elapsed</param>
		/// <param name="timeBase">The time base to use</param>
		/// <returns>The number of ticks <paramref name="systemTicks"/> corresponds to for this sequence</returns>
		public int GetPositionAtTime(long systemTicks,short timeBase=24)
		{
			var microTempo = 500000; // 120bpm default
			var result = 0;
			var elapsed = 0L;
			var ticksusec = microTempo / (double)timeBase;
			var tickspertick = ticksusec / (TimeSpan.TicksPerMillisecond / 1000) * 100;
			foreach (var e in Events)
			{
				var len = unchecked((long)Math.Round(e.Position * tickspertick));
				if(len+elapsed>systemTicks)
				{
					var dif = (len + elapsed) - systemTicks;
					return unchecked((int)Math.Round(dif / tickspertick)) + result;
				}
				result += e.Position;
				elapsed += len;
				switch(e.Message.Status)
				{
					case 0xFF:
						var mbs = e.Message as MidiMessageMeta;
						switch (mbs.Data1)
						{
							case 0x51:
								if (BitConverter.IsLittleEndian)
									microTempo = (mbs.Data[0] << 16) | (mbs.Data[1] << 8) | mbs.Data[2];
								else
									microTempo = (mbs.Data[2] << 16) | (mbs.Data[1] << 8) | mbs.Data[0];
								ticksusec = microTempo / (double)timeBase;
								tickspertick = ticksusec / (TimeSpan.TicksPerMillisecond / 1000) * 100;
								break;
						}
						break;
				}

			}
			if (systemTicks > elapsed)
				result+= unchecked((int)((systemTicks - elapsed) / tickspertick));
			return result;
		}
		/// <summary>
		/// Retrieves the next event in the sequence from the position specified by <paramref name="position"/>. The event's delta is modified such that it reflects the difference between the requested position and the time the note should be played.
		/// </summary>
		/// <param name="position">The position within the sequence</param>
		/// <param name="loop">True if the sequence should be treated as a loop, and the position logically wrapped if it's past the end of the track, otherwise false</param>
		/// <returns>A <see cref="MidiEvent"/> with an adjusted delta, or null if the sequence is empty or if <paramref name="loop"/> is false and there are no more events after <paramref name="position"/></returns>
		public MidiEvent GetNextEventAtPosition(int position,bool loop=false)
		{
			if (0 == Events.Count)
				return null;
			var pos = 0;
			foreach(var e in Events)
			{
				var delta = e.Position;
				if (delta + pos >=position)
					return new MidiEvent(delta + pos - position, e.Message);
				pos += delta;
			}
			if (!loop)
				return null;
			position %= (pos+1);
			return GetNextEventAtPosition(position);
		}
		/// <summary>
		/// Returns an enumeration of events targeting the specified <paramref name="channels"/>.
		/// </summary>
		/// <param name="channels">The channels to return as <see cref="MidiChannels"/> flags</param>
		/// <returns>All of the events targeting the specified channel, with deltas adjusted</returns>
		public IEnumerable<MidiEvent> GetEventsByChannel(MidiChannels channels)
		{
			var ofs = 0;
			foreach(var ev in Events)
			{
				switch(ev.Message.Status & 0xF0)
				{
					case 0x80:
					case 0x90:
					case 0xA0:
					case 0xB0:
					case 0xC0:
					case 0xD0:
					case 0xE0:
						switch(ev.Message.Channel)
						{
							case 0:
								if (MidiChannels.Channel0 == (channels & MidiChannels.Channel0))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 1:
								if (MidiChannels.Channel1 == (channels & MidiChannels.Channel1))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 2:
								if (MidiChannels.Channel2 == (channels & MidiChannels.Channel2))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 3:
								if (MidiChannels.Channel3 == (channels & MidiChannels.Channel3))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 4:
								if (MidiChannels.Channel4 == (channels & MidiChannels.Channel4))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 5:
								if (MidiChannels.Channel5 == (channels & MidiChannels.Channel5))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 6:
								if (MidiChannels.Channel6 == (channels & MidiChannels.Channel6))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 7:
								if (MidiChannels.Channel7 == (channels & MidiChannels.Channel7))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 8:
								if (MidiChannels.Channel8 == (channels & MidiChannels.Channel8))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 9:
								if (MidiChannels.Channel9 == (channels & MidiChannels.Channel9))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 10:
								if (MidiChannels.Channel10 == (channels & MidiChannels.Channel10))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 11:
								if (MidiChannels.Channel11 == (channels & MidiChannels.Channel11))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 12:
								if (MidiChannels.Channel12 == (channels & MidiChannels.Channel12))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 13:
								if (MidiChannels.Channel13 == (channels & MidiChannels.Channel13))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 14:
								if (MidiChannels.Channel14 == (channels & MidiChannels.Channel14))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
							case 15:
								if (MidiChannels.Channel15 == (channels & MidiChannels.Channel15))
									yield return new MidiEvent(ev.Position + ofs, ev.Message);
								ofs = 0;
								break;
						}
						break;
					default:
						ofs += ev.Position;
						break;

				}
			}
		}
		/// <summary>
		/// Retrieves the next events in the sequence from the position specified by <paramref name="position"/>. The event's delta is modified such that it reflects the difference between the requested position and the time the note should be played.
		/// </summary>
		/// <param name="position">The position within the sequence</param>
		/// <param name="loop">True if the sequence should be treated as a loop, and the position logically wrapped if it's past the end of the track, otherwise false</param>
		/// <returns>A series of <see cref="MidiEvent"/> objects, the first with an adjusted delta, or empty if the sequence is empty or if <paramref name="loop"/> is false and there are no more events after <paramref name="position"/></returns>
		public IEnumerable<MidiEvent> GetNextEventsAtPosition(int position, bool loop = false)
		{
			if (0 == Events.Count)
				yield break;
			var pos = 0;
			for(var i = 0;i<Events.Count;++i)
			{
				var e = Events[i];
				var delta = e.Position;
				if (delta + pos >= position)
				{
					yield return new MidiEvent(delta + pos - position, e.Message);
					for (var j = i + i; j < Events.Count; ++j)
						yield return Events[j];
					yield break;
				}
				pos += delta;
			}
			if (!loop)
				yield break;
			position %= (pos + 1);
			foreach (var e in GetNextEventsAtPosition(position))
				yield return e;
		}
		/// <summary>
		/// Gets the MIDI tick position for the current sequence at the current time
		/// </summary>
		/// <param name="time">The span of time that has elapsed</param>
		/// <param name="timeBase">The time base to use</param>
		/// <returns>The number of ticks <paramref name="time"/> corresponds to for this sequence</returns>
		public int GetPositionAtTime(TimeSpan time, short timeBase = 24)
			=> GetPositionAtTime(time.Ticks,timeBase);
		/// <summary>
		/// Gets a range of MIDI events as a new sequence
		/// </summary>
		/// <param name="start">The start of the range to retrieve in pulses/ticks</param>
		/// <param name="length">The length of the range to retrieve in pulses/ticks</param>
		/// <param name="copyTimingAndPatchInfo">True to copy the current patch and timing info to the range, otherwise false</param>
		/// <returns>A new MIDI sequence with the specified range of events</returns>
		public MidiSequence GetRange(int start,int length, bool copyTimingAndPatchInfo=false)
		{
			var result = new MidiSequence();
			var last = start;
			if (copyTimingAndPatchInfo && 0 != start)
			{
				var ctx = GetContext(start - 1);
				if(0!=ctx.MicroTempo)
				{
					result.Events.Add(new MidiEvent(0, new MidiMessageMetaTempo(ctx.MicroTempo)));
				}
				if(0!=ctx.TimeSignature.Numerator)
				{
					result.Events.Add(new MidiEvent(0, new MidiMessageMetaTimeSignature(ctx.TimeSignature)));
				}
				for (var i = 0; i < ctx.Channels.Length; i++) {
					var ch = ctx.Channels[i];
					if (0x80 > ch.Program)
					{
						result.Events.Add(new MidiEvent(0, new MidiMessagePatchChange(ch.Program, unchecked((byte)i))));
					}
				}

			}

			foreach (var e in AbsoluteEvents)
			{
				if(e.Position>=start)
				{
					if (e.Position >= start + length)
						break;
					if (0xFF == e.Message.Status && -1 == e.Message.PayloadLength)
					{
						var mbs = e.Message as MidiMessageMeta;
						// filter the midi end track sequences out, note if we found at least one
						if (0x2F == mbs.Data1)
						{
							continue;
						}
					}
					result.Events.Add(new MidiEvent(e.Position - last, e.Message));
					last = e.Position;
				}
			}
			var seq = new MidiSequence();
			seq.Events.Add(new MidiEvent(length, new MidiMessageMetaEndOfTrack()));
			return Merge(result,seq);
		}
		/// <summary>
		/// Indicates the name of the sequence, or null if no name is present
		/// </summary>
		public string Name { get {
				foreach(var e in AbsoluteEvents)
				{
					if(e.Message.Status==0xFF)
					{
						var mm = e.Message as MidiMessageMeta;
						if(3==mm.Data1)
						{
							return mm.Text;
						}
					}
				}
				return null;
			}
		}
		/// <summary>
		/// Indicates the name of the instrument in the MIDI sequence or null if not known
		/// </summary>
		public string Instrument {
			get {
				foreach (var e in AbsoluteEvents)
				{
					if (e.Message.Status == 0xFF)
					{
						var mm = e.Message as MidiMessageMeta;
						if (4 == mm.Data1)
						{
							return mm.Text;
						}
					}
				}
				return null;
			}
		}
		/// <summary>
		/// Indicates the copyright of the MIDI sequence or null if unspecified
		/// </summary>
		public string Copyright {
			get {
				foreach (var e in AbsoluteEvents)
				{

					if (e.Message.Status == 0xFF)
					{
						var mm = e.Message as MidiMessageMeta;
						if (2 == mm.Data1)
						{
							return mm.Text;
						}
					}
				}
				return null;
			}
		}
		/// <summary>
		/// Retrieves a note map for the sequence
		/// </summary>
		/// <returns>A list of <see cref="MidiNote"/> instances representing the note map</returns>
		public IList<MidiNote> ToNoteMap() {
			var result = new List<MidiNote>();
			var dic = new Dictionary<(byte NoteId, byte Channel), IList<(int Position, byte Velocity)>>();
			var pos = 0;
			for(int ic=Events.Count,i=0;i<ic;++i)
			{
				var ev = Events[i];
				var m = ev.Message;
				if(0x90 == (m.Status&0xF0))
				{
					var mw = m as MidiMessageWord;
					var key = (NoteId: mw.Data1, Channel: mw.Channel);
					IList<(int Position, byte Velocity)> l;
					if(!dic.TryGetValue(key,out l))
					{
						l = new List<(int Position, byte Velocity)>();
						dic.Add(key, l);
					}
					l.Add((pos+ev.Position,mw.Data2));

				} else if(0x80==(m.Status&0xF0)) 
				{
					
					var mw = m as MidiMessageWord;
					var key = (NoteId: mw.Data1, Channel: mw.Channel);
					IList<(int Position, byte Velocity)> l;
					if (dic.TryGetValue(key,out l))
					{
						for(int jc = l.Count,j=0;j<jc;++j)
						{
							var mn = l[j];
							result.Add(new MidiNote(mn.Position, mw.Channel, mw.Data1, mn.Velocity, pos - mn.Position+ev.Position));
							l.RemoveAt(j);
							--jc;
							--j;
							if (0 == l.Count)
								dic.Remove(key);
						}
					}
				}
				pos += ev.Position;
			}
			result.Sort((x, y) => { return x.Position.CompareTo(y.Position); });
			return result;
		}
		/// <summary>
		/// Retrieves a new MIDI sequence given the specified note map
		/// </summary>
		/// <param name="noteMap">The MIDI note map</param>
		/// <returns>A new MIDI sequence from the specified note map</returns>
		public static MidiSequence FromNoteMap(IEnumerable<MidiNote> noteMap)
		{
			var l = new List<MidiEvent>();
			MidiNote oldNote=default(MidiNote);
			var first = true;
			foreach(var note in noteMap)
			{
				if (!first)
				{
					if (oldNote.Position + oldNote.Length < note.Position || (oldNote.NoteId!=note.NoteId || oldNote.Channel!=note.Channel))
						l.Add(new MidiEvent(oldNote.Position + oldNote.Length, new MidiMessageNoteOff(oldNote.NoteId, 0, oldNote.Channel)));
				}
				else
					first = false;
				l.Add(new MidiEvent(note.Position, new MidiMessageNoteOn(note.NoteId, note.Velocity, note.Channel)));
				oldNote = note;
			}
			if(!first)
			{
				l.Add(new MidiEvent(oldNote.Position + oldNote.Length, new MidiMessageNoteOff(oldNote.NoteId, 0, oldNote.Channel)));
			}
			l.Sort((x, y) => { return x.Position.CompareTo(y.Position); });
			var result = new MidiSequence();
			var oldPos = 0;
			for(int ic=l.Count,i=0;i<ic;++i)
			{
				var ev = l[i];
				result.Events.Add(new MidiEvent(ev.Position - oldPos, ev.Message));
				oldPos = ev.Position;
			}
			result.Events.Add(new MidiEvent(0, new MidiMessageMetaEndOfTrack()));
			return result;
		}
		internal static MidiSequence ReadFrom(Stream stream)
		{
			MidiSequence result = new MidiSequence();
			var rs = (byte)0;
			var delta = _ReadVarlen(stream);
			if (BitConverter.IsLittleEndian)
				delta = MidiUtility.Swap(delta);
			var i = stream.ReadByte();
			while (-1 != i)
			{
				var hasStatus = false;
				var b = (byte)i;
				if (0x7F < b)
				{
					hasStatus = true;
					rs = b;
					i = stream.ReadByte();
					if (-1 != i)
						b = (byte)i;
					else
						b = 0;

				}
				var st = hasStatus ? rs : (byte)0;
				var m = (MidiMessage)null;
				switch (rs & 0xF0)
				{
					case 0x80:
						if (i == -1) throw new EndOfStreamException();
						m = new MidiMessageNoteOff(b, (byte)stream.ReadByte(), unchecked((byte)(st & 0x0F)));
						break;
					case 0x90:
						if (i == -1) throw new EndOfStreamException();
						m = new MidiMessageNoteOn(b, (byte)stream.ReadByte(), unchecked((byte)(st & 0x0F)));
						break;
					case 0xA0:
						if (i == -1) throw new EndOfStreamException();
						m = new MidiMessageKeyPressure(b, (byte)stream.ReadByte(),  unchecked((byte)(st & 0x0F)));
						break;
					case 0xB0:
						if (i == -1) throw new EndOfStreamException();
						m = new MidiMessageCC(b, (byte)stream.ReadByte(), unchecked((byte)(st & 0x0F)));
						break;
					case 0xC0:
						if (i == -1) throw new EndOfStreamException();
						m = new MidiMessagePatchChange(b,unchecked((byte)(st & 0x0F)));
						break;
					case 0xD0:
						if (i == -1) throw new EndOfStreamException();
						m = new MidiMessageChannelPressure(b, unchecked((byte)(st & 0x0F)));
						break;
					case 0xE0:
						if (i == -1) throw new EndOfStreamException();
						m = new MidiMessageChannelPitch(b, (byte)stream.ReadByte(), unchecked((byte)(st & 0x0F)));
						break;
					case 0xF0:
						switch (rs & 0xF)
						{
							case 0xF:
								if (i == -1) throw new EndOfStreamException();
								var l = _ReadVarlen(stream);
								var ba = new byte[l];
								if (l != stream.Read(ba, 0, ba.Length))
									throw new EndOfStreamException();
								switch (b)
								{
									case 0:
										m = new MidiMessageMetaSequenceNumber(ba);
										break;
									case 1:
										m = new MidiMessageMetaText(ba);
										break;
									case 2:
										m = new MidiMessageMetaCopyright(ba);
										break;
									case 3:
										m = new MidiMessageMetaSequenceOrTrackName(ba);
										break;
									case 4:
										m = new MidiMessageMetaInstrumentName(ba);
										break;
									case 5:
										m = new MidiMessageMetaLyric(ba);
										break;
									case 6:
										m = new MidiMessageMetaMarker(ba);
										break;
									case 7:
										m = new MidiMessageMetaCuePoint(ba);
										break;
									case 8:
										m = new MidiMessageMetaProgramName(ba);
										break;
									case 9:
										m = new MidiMessageMetaDevicePortName(ba);
										break;
									case 0x20:
										m = new MidiMessageMetaChannelPrefix(ba);
										break;
									case 0x21:
										m = new MidiMessageMetaPort(ba);
										break;
									case 0x2F:
										m = new MidiMessageMetaEndOfTrack(ba);
										break;
									case 0x51:
										m = new MidiMessageMetaTempo(ba);
										break;
									case 0x58:
										m = new MidiMessageMetaTimeSignature(ba);
										break;
									case 0x59:
										m = new MidiMessageMetaKeySignature(ba);
										break;
									default:
										m = new MidiMessageMeta(b, ba);
										break;

								}
								
								break;
							case 0x0:
								if (i == -1) throw new EndOfStreamException();
								var bb = b;
								var d = new List<byte>(128);
								if(0xF7==bb)
								{
									m = new MidiMessageSysex(new byte[0]);
									break;
								}
								d.Add(bb);
								while(0xF7!=bb)
								{
									var rb = stream.ReadByte();
									if(0>rb)
										throw new EndOfStreamException("Unterminated MIDI sysex message in file.");
									if (0xF7 == rb)
										break;
									bb= unchecked((byte)rb);
									d.Add(bb);
									
								}
								d.Add(0xF7);
								ba = d.ToArray();
								m = new MidiMessageSysex(ba);
								break;
							case 0x2:
								if (i == -1) throw new EndOfStreamException();
								m = new MidiMessageSongPosition(b, (byte)stream.ReadByte());
								break;
							case 0x3:
								if (i == -1) throw new EndOfStreamException();
								m = new MidiMessageSongSelect(b);
								break;
							case 0x6:
								if (i == -1) throw new EndOfStreamException();
								m = new MidiMessageTuneRequest();
								break;
							// system reatime messages follow. Shouldn't be present in a file but we handle them anyway
							case 0x8:
								if (i == -1) throw new EndOfStreamException();
								m = new MidiMessageRealTimeTimingClock();
								break;
							case 0xA:
								if (i == -1) throw new EndOfStreamException();
								m = new MidiMessageRealTimeStart();
								break;
							case 0xB:
								if (i == -1) throw new EndOfStreamException();
								m = new MidiMessageRealTimeContinue();
								break;
							case 0xC:
								if (i == -1) throw new EndOfStreamException();
								m = new MidiMessageRealTimeStop();
								break;
							case 0xE:
								if (i == -1) throw new EndOfStreamException();
								m = new MidiMessageRealTimeActiveSensing();
								break;
							default:
								throw new NotSupportedException("The MIDI message is not recognized.");
						}
						break;
				}
				
				result.Events.Add(new MidiEvent(delta,m));
				i = _ReadVarlen(stream);
				if (-1 == i)
					break;
				delta = i;
				i = stream.ReadByte();
			}
			return result;
		}
		internal void WriteTo(Stream stream)
		{
			foreach(var e in Events)
			{
				var pos = e.Position;
				_WriteVarlen(stream, pos);
				switch(e.Message.PayloadLength)
				{
					case 0:
						stream.WriteByte(e.Message.Status);
						break;
					case 1:
						if (0 != e.Message.Status) stream.WriteByte(e.Message.Status);
						var mb = e.Message as MidiMessageByte;
						stream.WriteByte(mb.Data1);
						break;
					case 2:
						if (0 != e.Message.Status) stream.WriteByte(e.Message.Status);
						var mw = e.Message as MidiMessageWord;
						stream.WriteByte(mw.Data1);
						stream.WriteByte(mw.Data2);
						break;
					case -1:
						if (0 != e.Message.Status) stream.WriteByte(e.Message.Status);
						var mma = e.Message as MidiMessageMeta;
						if (null != mma)
						{
							stream.WriteByte(mma.Data1);
							int v = mma.Data.Length;
							_WriteVarlen(stream, v);
							stream.Write(mma.Data, 0, mma.Data.Length);
							break; 
						}
						var msx = e.Message as MidiMessageSysex;
						if(null!=msx)
						{
							int v = msx.Data.Length;
							stream.Write(msx.Data, 0, v);
						}
						break;
				}
			}
		}
		/// <summary>
		/// Concatenates this sequence with another MIDI sequence
		/// </summary>
		/// <param name="right">The sequence to concatenate this sequence with</param>
		/// <returns>A new MIDI sequence that is the concatenation of this sequence and <paramref name="right"/></returns>
		public MidiSequence Concat(MidiSequence right) => Concat(this, right);
		/// <summary>
		/// Concatenates this sequence with other MIDI sequences
		/// </summary>
		/// <param name="sequences">The sequences to concatenate this sequence with</param>
		/// <returns>A new MIDI sequence that is the concatenation <paramref name="sequences"/></returns>
		public static MidiSequence Concat(params MidiSequence[] sequences) => Concat((IEnumerable<MidiSequence>)sequences);
		/// <summary>
		/// Concatenates this sequence with other MIDI sequences
		/// </summary>
		/// <param name="sequences">The sequences to concatenate this sequence with</param>
		/// <returns>A new MIDI sequence that is the concatenation of <paramref name="sequences"/></returns>
		public static MidiSequence Concat(IEnumerable<MidiSequence> sequences)
		{
			var result = new MidiSequence();
			var endDelta = 0;
			//var sawEnd = false;
			foreach (var seq in sequences)
			{
				int rs = 0;
				foreach (var e in seq.Events)
				{
					var m = e.Message;
					if (0 != m.Status) rs = m.Status;
					if(0xFF==rs)
					{
						var mbs = m as MidiMessageMeta;
						if (0x2F == mbs.Data1)
						{
							//sawEnd = true;
							endDelta = e.Position;
							break;
						}
					}
					result.Events.Add(new MidiEvent(e.Position+endDelta, m));
					endDelta = 0;
				}
			}
			//if(sawEnd) // add an end marker back to the track
			result.Events.Add(new MidiEvent(endDelta, new MidiMessageMetaEndOfTrack()));
			return result;
		}
		/// <summary>
		/// Merges this sequence with other MIDI sequences
		/// </summary>
		/// <param name="right">The sequence to merge this sequence with</param>
		/// <returns>A new MIDI sequence that is a merge of this sequence and <paramref name="right"/></returns>
		public MidiSequence Merge(MidiSequence right) => Merge(this, right);
		/// <summary>
		/// Merges this sequence with other MIDI sequences
		/// </summary>
		/// <param name="sequences">The sequences to merge this sequence with</param>
		/// <returns>A new MIDI sequence that is a merge <paramref name="sequences"/></returns>
		public static MidiSequence Merge(params MidiSequence[] sequences) => Merge((IEnumerable<MidiSequence>)sequences);
		/// <summary>
		/// Merges this sequence with other MIDI sequences
		/// </summary>
		/// <param name="sequences">The sequences to merge this sequence with</param>
		/// <returns>A new MIDI sequence that is a merge <paramref name="sequences"/></returns>
		public static MidiSequence Merge(IEnumerable<MidiSequence> sequences)
		{
			var result = new MidiSequence();
			var l = new List<MidiEvent>();
			foreach(var seq in sequences)
				l.AddRange(seq.AbsoluteEvents);
			l.Sort(delegate (MidiEvent x, MidiEvent y) { return x.Position.CompareTo(y.Position); });
			int midiEnd = -1;
			var last = 0;
			foreach (var e in l)
			{
				if(0xFF==e.Message.Status && -1==e.Message.PayloadLength)
				{
					var mbs = e.Message as MidiMessageMeta;
					// filter the midi end track sequences out, note where if we found one
					if (0x2F == mbs.Data1)
					{
						midiEnd = e.Position;
						continue;
					}
				}
				result.Events.Add(new MidiEvent(e.Position - last, e.Message));
				last = e.Position;
			}
			if(-1<midiEnd) // if we found a midi end track, then add one back after all is done
				result.Events.Add(new MidiEvent(Math.Max(0,midiEnd-last), new MidiMessageMetaEndOfTrack()));
			
			return result;
		}
		/// <summary>
		/// Transposes the notes in a sequence, optionally wrapping the note values
		/// </summary>
		/// <param name="noteAdjust">The number of MIDI notes to add or subtract</param>
		/// <param name="wrap">True if out of range notes are wrapped, false if they are to be clipped</param>
		/// <param name="noDrums">True if drums are to be skipped, otherwise false</param>
		/// <returns>A new MIDI sequence with the notes transposed</returns>
		public MidiSequence Transpose(sbyte noteAdjust,bool wrap = false,bool noDrums=true)
		{
			var events = new List<MidiEvent>(Events.Count);
			foreach(var ev in AbsoluteEvents)
			{
				int n;
				switch(ev.Message.Status & 0xF0)
				{
					case 0x80:
						if (noDrums && 9 == ev.Message.Channel)
							goto default;
						var no = ev.Message as MidiMessageWord;
						n = no.Data1 + noteAdjust;
						if(0>n || 127<n)
						{
							if (!wrap)
								continue;
							if (0 > n)
								n += 127;
							else
								n -= 127;
						}
						no = new MidiMessageNoteOff(unchecked((byte)n), no.Data2, no.Channel);
						events.Add(new MidiEvent(ev.Position, no));
						break;
					case 0x90:
						if (noDrums && 9 == ev.Message.Channel)
							goto default;
						no = ev.Message as MidiMessageWord;
						n = no.Data1 + noteAdjust;
						if (0 > n || 127 < n)
						{
							if (!wrap)
								continue;
							if (0 > n)
								n += 127;
							else
								n -= 127;
						}
						no = new MidiMessageNoteOn(unchecked((byte)n), no.Data2, no.Channel);
						events.Add(new MidiEvent(ev.Position, no));
						break;
					default:
						events.Add(ev.Clone());
						break;
				}
			}
			var result = new MidiSequence();
			var last = 0;
			foreach(var ev in events)
			{
				result.Events.Add(new MidiEvent(ev.Position - last, ev.Message));
				last = ev.Position;
			}
			return result;
		}
		/// <summary>
		/// Stretches or compresses the MIDI sequence events
		/// </summary>
		/// <remarks>If <paramref name="adjustTempo"/> is false this will change the playback speed of the MIDI</remarks>
		/// <param name="diff">The differential for the size. 1 is the same length, .5 would be half the length and 2 would be twice the length</param>
		/// <param name="adjustTempo">Indicates whether or not the tempo should be adjusted to compensate</param>
		/// <returns>A new MIDI sequence that is stretched the specified amount</returns>
		public MidiSequence Stretch(double diff,bool adjustTempo=false)
		{
			var result = new MidiSequence();
			if (!adjustTempo)
				foreach (var e in Events)
					result.Events.Add(new MidiEvent((int)Math.Round(e.Position * diff, MidpointRounding.AwayFromZero), e.Message.Clone()));
			else
			{
				byte runningStatus = 0;
				foreach (var e in Events)
				{
					if (0 != e.Message.Status)
						runningStatus = e.Message.Status;
					var m = e.Message;
					if (-1 == m.PayloadLength)
					{
						if (0xFF == runningStatus)
						{
							var mbs = m as MidiMessageMeta;
							if(0x51==mbs.Data1)
							{
								var mt = 0;
								if (BitConverter.IsLittleEndian)
									mt = (mbs.Data[0] << 16) | (mbs.Data[1] << 8) | mbs.Data[2];
								else
									mt = (mbs.Data[2] << 16) | (mbs.Data[1] << 8) | mbs.Data[0];
								mt = (int)Math.Round(mt / diff, MidpointRounding.AwayFromZero);
								
								m= new MidiMessageMetaTempo(mt);
							}
						}
					}
					result.Events.Add(new MidiEvent((int)Math.Round(e.Position * diff, MidpointRounding.AwayFromZero), m));
				}
			}
			return result;
		}
		/// <summary>
		/// Adjusts the tempo of a sequence
		/// </summary>
		/// <param name="tempo">The new tempo</param>
		/// <returns>A new sequence with an adjusted tempo. All other tempo messages are adjusted relatively to the first one</returns>
		public MidiSequence AdjustTempo(double tempo)
			=> AdjustTempo(MidiUtility.TempoToMicroTempo(tempo));
		/// <summary>
		/// Adjusts the tempo of a sequence
		/// </summary>
		/// <param name="microTempo">The new microtempo</param>
		/// <returns>A new sequence with an adjusted tempo. All other tempo messages are adjusted relatively to the first one</returns>
		public MidiSequence AdjustTempo(int microTempo)
		{
			var smt = MicroTempo;
			if (smt == microTempo)
				return Clone();
			var diff = microTempo/smt;
			return ScaleTempo(diff);
		}
		/// <summary>
		/// Adjusts the tempo by the specified difference
		/// </summary>
		/// <param name="diff">A value indicating the multiplier. Larger makes the tempo higher.</param>
		/// <returns></returns>
		public MidiSequence ScaleTempo(double diff)
		{
			var result = new MidiSequence();
			byte runningStatus = 0;
			foreach (var e in Events)
			{
				if (0 != e.Message.Status)
					runningStatus = e.Message.Status;
				var m = e.Message;
				if (-1 == m.PayloadLength)
				{
					if (0xFF == runningStatus)
					{
						var mbs = m as MidiMessageMeta;
						if (0x51 == mbs.Data1)
						{
							var mt = 0;
							if (BitConverter.IsLittleEndian)
								mt = (mbs.Data[0] << 16) | (mbs.Data[1] << 8) | mbs.Data[2];
							else
								mt = (mbs.Data[2] << 16) | (mbs.Data[1] << 8) | mbs.Data[0];
							mt = (int)Math.Round(mt * diff, MidpointRounding.AwayFromZero);

							m = new MidiMessageMetaTempo(mt);
						}
					}
				}
				result.Events.Add(new MidiEvent(e.Position, m));
			}
			return result;
		}
		internal int GetMaxVelocity()
		{
			var maxvel = 0;
			foreach (var e in Events)
			{
				switch (e.Message.Status & 0xF0)
				{
					//case 0x80:
					case 0x90:
						var non = e.Message as MidiMessageWord;
						if (maxvel < non.Data2)
							maxvel = non.Data2;
						break;
				}
			}
			return maxvel;
		}
		/// <summary>
		/// Scales all note velocities such that the highest velocity is now 127
		/// </summary>
		/// <returns>A new MIDI sequence with the velocities scaled</returns>
		public MidiSequence NormalizeVelocities()
		{
			var maxvel = GetMaxVelocity();
			return ScaleVelocities(0 != maxvel ? 128d / maxvel : 0);
		}
		/// <summary>
		/// Scales note velocities by the specified value
		/// </summary>
		/// <param name="multiplier">The multiplier. 1 = no change</param>
		/// <returns>A new sequence with the velocities adjusted</returns>
		public MidiSequence ScaleVelocities(double multiplier)
		{
			var result = new MidiSequence();
			foreach(var e in Events)
			{
				switch(e.Message.Status & 0xF0)
				{
					case 0x80:
						var noff = e.Message as MidiMessageWord;
						var d = noff.Data2 * multiplier;
						d = Math.Round(Math.Min(Math.Max(0, d), 127), MidpointRounding.AwayFromZero);
						var nnoff = new MidiMessageNoteOff(noff.Data1, unchecked((byte)d), unchecked((byte)(noff.Status & 0xF)));
						result.Events.Add(new MidiEvent(e.Position, nnoff));
						break;
					case 0x90:
						var non = e.Message as MidiMessageWord;
						d = non.Data2 * multiplier;
						d = Math.Round(Math.Min(Math.Max(0, d), 127), MidpointRounding.AwayFromZero);
						var nnon = new MidiMessageNoteOn(non.Data1, unchecked((byte)d), unchecked((byte)(non.Status & 0xF)));
						result.Events.Add(new MidiEvent(e.Position, nnon));
						break;
					default:
						result.Events.Add(e.Clone());
						break;
				}
			}
			return result;
		}
		/// <summary>
		/// Indicates the MicroTempo of the MIDI sequence
		/// </summary>
		public int MicroTempo {
			get {
				foreach (var e in AbsoluteEvents)
				{
					switch (e.Message.Status & 0xF0)
					{
						case 0x80:
						case 0x90:
							return 500000;
					}
					if (e.Message.Status == 0xFF)
					{
						var mm = e.Message as MidiMessageMeta;
						if (0x51 == mm.Data1)
						{
							return BitConverter.IsLittleEndian ?
								(mm.Data[0] << 16) | (mm.Data[1] << 8) | mm.Data[2] :
								(mm.Data[2] << 16) | (mm.Data[1] << 8) | mm.Data[0];
						}
					}
				}
				return 500000;
			}
		}
		/// <summary>
		/// Indicates all of the MicroTempos in the sequence
		/// </summary>
		public IEnumerable<KeyValuePair<int,int>> MicroTempos { get {
				foreach (var e in AbsoluteEvents)
				{
					if (e.Message.Status == 0xFF)
					{
						var mm = e.Message as MidiMessageMeta;
						if (0x51 == mm.Data1)
						{
							if (BitConverter.IsLittleEndian)
								yield return new KeyValuePair<int, int>(e.Position, (mm.Data[0] << 16) | (mm.Data[1] << 8) | mm.Data[2]);
							else
								yield return new KeyValuePair<int, int>(e.Position,(mm.Data[2] << 16) | (mm.Data[1] << 8) | mm.Data[0]);
						}
					}
				}
			}
		}
		/// <summary>
		/// Indicates the tempo of the sequence
		/// </summary>
		public double Tempo {
			get {
				return MidiUtility.MicroTempoToTempo(MicroTempo);
			}
			
		}
		/// <summary>
		/// Indicates all the tempos in the sequence
		/// </summary>
		public IEnumerable<KeyValuePair<int, double>> Tempos {
			get {
				foreach (var mt in MicroTempos)
					yield return new KeyValuePair<int, double>(mt.Key, MidiUtility.MicroTempoToTempo(mt.Value));
			}
		}
		/// <summary>
		/// Indicates the time signature of the MIDI sequence
		/// </summary>
		public MidiTimeSignature TimeSignature {
			get {
				foreach (var e in AbsoluteEvents)
				{
					switch (e.Message.Status & 0xF0)
					{
						case 0x80:
						case 0x90:
							return MidiTimeSignature.Default;
					}
					if (e.Message.Status == 0xFF)
					{
						var mm = e.Message as MidiMessageMeta;
						if (0x58 == mm.Data1)
						{
							var num = mm.Data[0];
							var den = mm.Data[1];
							var met = mm.Data[2];
							var q32 = mm.Data[3];
							return new MidiTimeSignature(num, (short)Math.Pow(2,den), met, q32);
						}
					}
				}
				return MidiTimeSignature.Default;
			}
		}
		/// <summary>
		/// Indicates all of the TimeSignatures in the sequence
		/// </summary>
		public IEnumerable<KeyValuePair<int, MidiTimeSignature>> TimeSignatures {
			get {
				foreach (var e in AbsoluteEvents)
				{
					if (e.Message.Status == 0xFF)
					{
						var mm = e.Message as MidiMessageMeta;
						if (0x58 == mm.Data1)
						{
							var num = mm.Data[0];
							var den = mm.Data[1];
							var met = mm.Data[2];
							var q32 = mm.Data[3];
							yield return new KeyValuePair<int, MidiTimeSignature>(e.Position, new MidiTimeSignature(num, (short)Math.Pow(den, 2), met, q32));
						}
					}
				}
			}
		}
		/// <summary>
		/// Indicates the key signature of the MIDI sequence
		/// </summary>
		public MidiKeySignature KeySignature {
			get {
				foreach (var e in AbsoluteEvents)
				{
					switch (e.Message.Status & 0xF0)
					{
						case 0x80:
						case 0x90:
							return MidiKeySignature.Default;
					}
					if (e.Message.Status == 0xFF)
					{
						var mm = e.Message as MidiMessageMeta;
						if (0x59 == mm.Data1)
						{
							return new MidiKeySignature(unchecked((sbyte)mm.Data[0]), 0 != mm.Data[1]);
						}
					}
				}
				return MidiKeySignature.Default;
			}
		}
		/// <summary>
		/// Indicates all of the MIDI key signatures in the sequence
		/// </summary>
		public IEnumerable<KeyValuePair<int, MidiKeySignature>> KeySignatures {
			get {
				foreach (var e in AbsoluteEvents)
				{
					if (e.Message.Status == 0xFF)
					{
						var mm = e.Message as MidiMessageMeta;
						if (0x59 == mm.Data1)
						{
							yield return new KeyValuePair<int, MidiKeySignature>(e.Position, new MidiKeySignature(unchecked((sbyte)mm.Data[0]), 0 != mm.Data[1]));							
						}
					}
				}
			}
		}
		/// <summary>
		/// Indicates the length of the MIDI sequence
		/// </summary>
		public int Length {
			get {
				var l= 0;
				byte r = 0;
				foreach (var e in Events) {
					var m = e.Message;
					if (0 != m.Status)
						r = m.Status;
					l += e.Position;
					if(0xFF==r && 0x2F==(m as MidiMessageMeta).Data1)
					{
						break;
					}
				}
				return l + 1;
			}
		}
		/// <summary>
		/// Indicates the lyrics of the MIDI sequence
		/// </summary>
		public IEnumerable<KeyValuePair<int,string>> Lyrics {
			get {
				foreach(var e in AbsoluteEvents)
				{
					if(0xFF == e.Message.Status)
					{
						var mm = e.Message as MidiMessageMeta;
						if(5==mm.Data1)
							yield return new KeyValuePair<int, string>(e.Position, mm.Text);
					}
				}
			}
		}
		/// <summary>
		/// Indicates the markers in the MIDI sequence
		/// </summary>
		public IEnumerable<KeyValuePair<int, string>> Markers {
			get {
				foreach (var e in AbsoluteEvents)
				{
					if (0xFF == e.Message.Status)
					{
						var mm = e.Message as MidiMessageMeta;
						if (6 == mm.Data1)
							yield return new KeyValuePair<int, string>(e.Position, mm.Text);
					}
				}
			}
		}
		/// <summary>
		/// Indicates the comments in the MIDI sequence
		/// </summary>
		public IEnumerable<KeyValuePair<int, string>> Comments {
			get {
				foreach (var e in AbsoluteEvents)
				{
					if (0xFF == e.Message.Status)
					{
						var mm = e.Message as MidiMessageMeta;
						if (1 == mm.Data1)
							yield return new KeyValuePair<int, string>(e.Position, mm.Text);
					}
				}
			}
		}
		/// <summary>
		/// Indicates the cue points in the MIDI sequence
		/// </summary>
		public IEnumerable<KeyValuePair<int, string>> CuePoints {
			get {
				foreach (var e in AbsoluteEvents)
				{
					if (0xFF == e.Message.Status)
					{
						var mm = e.Message as MidiMessageMeta;
						if (7 == mm.Data1)
							yield return new KeyValuePair<int, string>(e.Position, mm.Text);
					}
				}
			}
		}
		/// <summary>
		/// Indicates the events as absolutely positioned events
		/// </summary>
		public IEnumerable<MidiEvent> AbsoluteEvents {
			get {
				var runningStatus = default(byte);
				var channelPrefix = (byte)0xFF;
				var r = runningStatus;
				var pos=0;
				foreach(var e in Events)
				{
					pos += e.Position;
					var hs = true;
					if (0 != e.Message.Status)
						runningStatus = e.Message.Status;
					else
						hs = false;
					r = runningStatus;
					if(!hs && r<0xF0 && 0xFF!=channelPrefix)
						r = unchecked((byte)((r & 0xF0) | channelPrefix));
					
					yield return new MidiEvent(pos, e.Message.Clone());
				}
			}
		}
		/// <summary>
		/// Adds an event at the specified absolute position
		/// </summary>
		/// <param name="position">The absolute position in ticks</param>
		/// <param name="message">The message to insert</param>
		public void AddAbsoluteEvent(int position,MidiMessage message)
		{
			var pos = 0;
			for(var i =0;i<Events.Count;++i)
			{
				var ev = Events[i];
				var delta = ev.Position;
				if(delta+pos>=position)
				{
					var ndelta = position - pos;
					var nev = new MidiEvent(ndelta, message);
					Events[i] = new MidiEvent(delta - ndelta, ev.Message);
					Events.Insert(i, nev);
					return;
				}
				pos += delta;
			}
			Events.Add(new MidiEvent(position - pos, message));
		}
		/// <summary>
		/// Plays the sequence to the specified MIDI device using the specified timebase
		/// </summary>
		/// <param name="timeBase">The timebase to use, in pulses/ticks per quarter note</param>
		/// <param name="device">The MIDI output device to use</param>
		/// <param name="loop">Indicates whether to loop playback or not</param>
		public void Preview(short timeBase = 24, MidiOutputDevice device = null,bool loop=false)
		{
			var isOpen = false;
			if (null == device)
				device = MidiDevice.Outputs[0];
			if (device.IsOpen)
				isOpen = true;
			else
				device.Open();
			var ppq = timeBase;
			var mt = MidiUtility.TempoToMicroTempo(120d);
			var first = true;
			try
			{
				while (loop || first)
				{
					first = false;
					var ticksusec = mt / (double)timeBase;
					var tickspertick = ticksusec / (TimeSpan.TicksPerMillisecond / 1000) * 100;
					var tickStart = _PreciseUtcNowTicks;
					var tickCurrent = tickStart;

					var end = (long)(Length * tickspertick + tickStart);
					var tpm = TimeSpan.TicksPerMillisecond;

					using (var e = AbsoluteEvents.GetEnumerator())
					{
						if (!e.MoveNext())
							return;
						var done = false;
						while (!done && tickCurrent <= end)
						{
							tickCurrent = _PreciseUtcNowTicks;
							var ce = (long)((tickCurrent - tickStart) / tickspertick);
							while (!done && e.Current.Position <= ce)
							{
								if (0xFF == e.Current.Message.Status)
								{
									var mbs = e.Current.Message as MidiMessageMeta;
									if (0x51 == mbs.Data1)
									{
										if (BitConverter.IsLittleEndian)
											mt = (mbs.Data[0] << 16) | (mbs.Data[1] << 8) | mbs.Data[2];
										else
											mt = (mbs.Data[2] << 16) | (mbs.Data[1] << 8) | mbs.Data[0];
										ticksusec = mt / (double)ppq;
										tickspertick = ticksusec / (tpm / 1000) * 100;
										end = (long)(Length * tickspertick + tickStart);
									}
									// we ignore the end marker. it's possible this will render invalid with an invalid sequence
									// such that there are notes after the end marker. this will play them instead of exiting
								}
								else
								{
									device.Send(e.Current.Message);
								}
								if (!e.MoveNext())
									done = true;
								
							}
						}
					}
				}
			}
			finally
			{
				if (!isOpen)
					device.Close();
			}
		}
		/// <summary>
		/// Creates a deep copy of the sequence
		/// </summary>
		/// <returns></returns>
		public MidiSequence Clone()
		{
			var result = new MidiSequence();
			for(int ic=Events.Count,i=0;i<ic;++i)
				result.Events.Add(Events[i].Clone());
			return result;
		}
		object ICloneable.Clone()
		{
			return Clone();
		}
		
		private static int _ReadVarlen(Stream stream)
		{
			var b = stream.ReadByte();
			var result =-1;
			if (-1 == b) return result;
			if (0x80 > b) // single value
			{
				result = b;
				return result;
			}
			else // short or int
			{
				result = b & 0x7F;
				result <<= 7;
				if (-1 == (b = stream.ReadByte())) throw new EndOfStreamException();
				result |= b & 0x7F;
				if (0x80 > b)
				{
					if (BitConverter.IsLittleEndian)
						return result;
					else
						return MidiUtility.Swap(result);
				}
				// int
				result <<= 7;
				if (-1 == (b = stream.ReadByte())) throw new EndOfStreamException();
				result |= b & 0x7F;
				if (0x80 > b)
				{
					if (BitConverter.IsLittleEndian)
						return result;
					else
						return MidiUtility.Swap(result << 7);
				}
				// int (4 len)
				result <<= 7;
				if (-1 == (b = stream.ReadByte())) throw new EndOfStreamException();
				result |= b & 0x7F;
				if (0x80 > b)
				{
					if (BitConverter.IsLittleEndian)
						return result;
					else
						return MidiUtility.Swap(result << 7);
				}
				throw new OverflowException("MIDI Variable length quantity can't be greater than 28 bits.");
			}
		}
		private static int _ReadVarlen(byte firstByte,Stream stream)
		{
			var b = (int)firstByte;
			int result;
			if (0x80 > b) // single value
			{
				result = b;
				return result;
			}
			else // short or int
			{
				result = b & 0x7F;
				result <<= 7;
				if (-1 == (b = stream.ReadByte())) throw new EndOfStreamException();
				result |= b & 0x7F;
				if (0x80 > b)
				{
					if (BitConverter.IsLittleEndian)
						return result;
					else
						return MidiUtility.Swap(result);
				}
				// int
				result <<= 7;
				if (-1 == (b = stream.ReadByte())) throw new EndOfStreamException();
				result |= b & 0x7F;
				if (0x80 > b)
				{
					if (BitConverter.IsLittleEndian)
						return result;
					else
						return MidiUtility.Swap(result << 7);
				}
				// int (4 len)
				result <<= 7;
				if (-1 == (b = stream.ReadByte())) throw new EndOfStreamException();
				result |= b & 0x7F;
				if (0x80 > b)
				{
					if (BitConverter.IsLittleEndian)
						return result;
					else
						return MidiUtility.Swap(result << 7);
				}
				throw new OverflowException("MIDI Variable length quantity can't be greater than 28 bits.");
			}
		}
		private static void _WriteVarlen(Stream stream, int value)
		{
			int buffer;
			buffer = value & 0x7f;
			while ((value >>= 7) > 0)
			{
				buffer <<= 8;
				buffer |= 0x80;
				buffer += (value & 0x7f);
			}

			while (true)
			{
				stream.WriteByte(unchecked((byte)buffer));
				if (0 < unchecked((byte)(buffer & 0x80))) buffer >>= 8;
				else
					break;
			}
		}
	}
}
