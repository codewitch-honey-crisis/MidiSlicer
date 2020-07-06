namespace M
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Represents a MIDI file
	/// </summary>
#if MIDILIB
	public
#else
	internal
#endif
	sealed partial class MidiFile : ICloneable
 	{
		/// <summary>
		/// Constructs a MIDI file of the specified type with the specified timebase
		/// </summary>
		/// <param name="type">The MIDI file type - either 0, 1 or 2</param>
		/// <param name="timeBase">The timebase of the MIDI file in ticks/pulses per quarter note</param>
		/// <remarks>Normally the default timebase in the API is 24, but that's undesirable for most MIDI files, so this defaults to 480, which is a much more realistic resolution</remarks>
		public MidiFile(short type=1,short timeBase=480)
		{
			Type = type;
			TimeBase = timeBase;
			Tracks = new List<MidiSequence>();
			FilePath = null;
		}
		/// <summary>
		/// Plays the file over the specified device
		/// </summary>
		/// <param name="device">The MIDI output device to use</param>
		/// <param name="loop">Indicates whether to loop playback or not</param>
		public void Preview(MidiOutputDevice device=null,bool loop=false)
		{
			MidiSequence.Merge(Tracks).Preview(TimeBase, device,loop);
		}
		/// <summary>
		/// Changes the timebase of the MIDI file
		/// </summary>
		/// <param name="timeBase">The new timebase, in ticks/pulses per quarter note</param>
		/// <returns></returns>
		public MidiFile Resample(short timeBase)
		{
			var diff = timeBase / (double)TimeBase;
			var result = new MidiFile(Type, timeBase);
			foreach(var trk in Tracks)
				result.Tracks.Add(trk.Stretch(diff, false));
			return result;
		}
		/// <summary>
		/// Indicates the name of the file this was loaded from or saved to, if available, otherwise null
		/// </summary>
		public string FilePath { get; private set; }
		/// <summary>
		/// Returns the internal name of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's name</remarks>
		public string Name {  get {
				if(1==Type && 0<Tracks.Count)
					return Tracks[0].Name;
				return null;
			} 
		}
		/// <summary>
		/// Returns the copyright information of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's copyright metadata</remarks>
		public string Copyright { get {
				if (1 == Type && 0 < Tracks.Count)
					return Tracks[0].Copyright;
				return null;

			} 
		}
		/// <summary>
		/// Indicates the MicroTempo of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's microtempo</remarks>
		public int MicroTempo { get {
				if (0 == Tracks.Count) return 500000;
				return Tracks[0].MicroTempo;
			}
			
		}
		/// <summary>
		/// Indicates all of the MicroTempos of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's microtempos</remarks>
		public IEnumerable<KeyValuePair<int,int>> MicroTempos {
			get {
				if (0 == Tracks.Count) return new KeyValuePair<int,int>[0];
				return Tracks[0].MicroTempos;
			}
		}
		/// <summary>
		/// Indicates the Tempo of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's tempo</remarks>
		public double Tempo {
			get {
				return MidiUtility.MicroTempoToTempo(MicroTempo);
			}
		}
		/// <summary>
		/// Indicates all of the Tempos of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's tempos</remarks>
		public IEnumerable<KeyValuePair<int,double>> Tempos {
			get {
				foreach(var mt in MicroTempos)
					yield return new KeyValuePair<int, double>(mt.Key, MidiUtility.MicroTempoToTempo(mt.Value));
			}
		}
		/// <summary>
		/// Indicates the time signature of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's time signature</remarks>
		public MidiTimeSignature TimeSignature {
			get {
				if (0 == Tracks.Count)
					return MidiTimeSignature.Default;
				return Tracks[0].TimeSignature;
			}
		}
		/// <summary>
		/// Indicates all the time signatures of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's time signatures</remarks>
		public IEnumerable<KeyValuePair<int,MidiTimeSignature>> TimeSignatures {
			get {
				if (0 == Tracks.Count)
					return new KeyValuePair<int, MidiTimeSignature>[0];
				return Tracks[0].TimeSignatures;
			}
		}
		/// <summary>
		/// Indicates the key signature of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's key signature</remarks>
		public MidiKeySignature KeySignature {
			get {
				if (0 == Tracks.Count)
					return MidiKeySignature.Default;
				return Tracks[0].KeySignature;
			}
		}
		/// <summary>
		/// Indicates all the key signatures of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's key signatures</remarks>
		public IEnumerable<KeyValuePair<int, MidiKeySignature>> KeySignatures {
			get {
				if (0 == Tracks.Count)
					return new KeyValuePair<int, MidiKeySignature>[0];
				return Tracks[0].KeySignatures;
			}
		}

		/// <summary>
		/// Indicates the duration of the MIDI file as a <see cref="TimeSpan"/>
		/// </summary>
		public TimeSpan Duration {
			get {
				if (0 == Tracks.Count)
					return TimeSpan.Zero;
				var seq = MidiSequence.Merge(Tracks);
				return seq.GetContext(seq.Length, TimeBase).Time;
			}
		}
		/// <summary>
		/// Indicates the duration of the MIDI file in system ticks
		/// </summary>
		public long DurationSystemTicks {
			get {
				if (0 == Tracks.Count)
					return 0L;
				var seq = MidiSequence.Merge(Tracks);
				return seq.GetContext(seq.Length, TimeBase).SystemTicks;
			}
		}

		/// <summary>
		/// Indicates the type of the MIDI file
		/// </summary>
		/// <remarks>This can be 0, 1 or 2</remarks>
		public short Type { get; private set; }
		/// <summary>
		/// Indicates the timebase of the MIDI file as ticks/pulses per quarter note
		/// </summary>
		public short TimeBase { get; private set; }
		/// <summary>
		/// Retrieves a list of all MIDI tracks in the file
		/// </summary>
		public IList<MidiSequence> Tracks { get; private set; }
		/// <summary>
		/// Indicates the total length of the MIDI file, in ticks/pulses
		/// </summary>
		public int Length {  get {
				int result = 0;
				foreach(var trk in Tracks)
				{
					int l = trk.Length;
					if (l > result) result = l;
				}
				return result;
			}
		}
		/// <summary>
		/// Indicates the comments of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's comments</remarks>
		public IEnumerable<KeyValuePair<int, string>> Comments {
			get {
				if (0 < Tracks.Count)
					return Tracks[0].Comments;
				return new KeyValuePair<int, string>[0];
			}
		}
		/// <summary>
		/// Indicates the MIDI Markers of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's markers</remarks>
		public IEnumerable<KeyValuePair<int,string>> Markers {
			get {
				if(0<Tracks.Count)
					return Tracks[0].Markers;
				return new KeyValuePair<int, string>[0];
			}
		}
		/// <summary>
		/// Indicates the lyrics of the MIDI file
		/// </summary>
		/// <remarks>This is derived from Track #0's lyrics</remarks>
		public IEnumerable<KeyValuePair<int, string>> Lyrics {
			get {
				if (0 < Tracks.Count)
					return Tracks[0].Lyrics;
				return new KeyValuePair<int, string>[0];
			}
		}
		/// <summary>
		/// Indicates the position of the first beat of the MIDI file in pulses/ticks
		/// </summary>
		public int FirstDownBeat { get {
				var result = -1;
				foreach (var trk in Tracks)
				{
					var d = trk.FirstDownBeat;
					if (-1!=d && (-1==result || result > d))
						result = d;
				}
				return result;
			}
		}
		/// <summary>
		/// Indicates the position of the first note on message in the MIDI file in pulses/ticks
		/// </summary>
		public int FirstNoteOn {
			get {
				var result = -1;
				foreach (var trk in Tracks)
				{
					var d = trk.FirstNoteOn;
					if (-1 != d && (-1 == result || result > d))
						result = d;
				}
				return result;
			}
		}
		/// <summary>
		/// Transposes the notes in a file, optionally wrapping the note values
		/// </summary>
		/// <param name="noteAdjust">The number of MIDI notes to add or subtract</param>
		/// <param name="wrap">True if out of range notes are wrapped, false if they are to be clipped</param>
		/// <param name="noDrums">True if drum/percussion notes are to be left alone, otherwise false</param>
		/// <returns>A new MIDI file with the notes transposed</returns>
		public MidiFile Transpose(sbyte noteAdjust, bool wrap = false,bool noDrums=true)
		{
			var result = new MidiFile(Type, TimeBase);
			foreach(var track in Tracks)
				result.Tracks.Add(track.Transpose(noteAdjust, wrap,noDrums));
			return result;
		}
		/// <summary>
		/// Stretches or compresses the MIDI file events
		/// </summary>
		/// <remarks>If <paramref name="adjustTempo"/> is false this will change the playback speed of the MIDI</remarks>
		/// <param name="diff">The differential for the size. 1 is the same length, .5 would be half the length and 2 would be twice the length</param>
		/// <param name="adjustTempo">Indicates whether or not the tempo should be adjusted to compensate</param>
		/// <returns>A new MIDI file that is stretched the specified amount</returns>
		public MidiFile Stretch(double diff,bool adjustTempo=false)
		{
			var result = new MidiFile(Type, TimeBase);
			foreach(var trk in Tracks)
				result.Tracks.Add(trk.Stretch(diff,adjustTempo));
			return result;
		}
		/// <summary>
		/// Adjusts the tempo of a MIDI file
		/// </summary>
		/// <param name="tempo">The new tempo</param>
		/// <returns>A new file with an adjusted tempo. All other tempo messages are adjusted relatively to the first one</returns>
		public MidiFile AdjustTempo(double tempo)
			=> AdjustTempo(MidiUtility.TempoToMicroTempo(tempo));
		/// <summary>
		/// Adjusts the tempo of a MIDI file
		/// </summary>
		/// <param name="microTempo">The new microtempo</param>
		/// <returns>A new file with an adjusted tempo. All other tempo messages are adjusted relatively to the first one</returns>
		public MidiFile AdjustTempo(int microTempo)
		{
			var mt = MicroTempo;
			var diff = microTempo / (double)mt;
			var result = new MidiFile(Type, TimeBase);
			for (var i = 0; i < Tracks.Count; ++i)
			{
				result.Tracks.Add(Tracks[i].ScaleTempo(diff));
			}
			return result;
		}
		/// <summary>
		/// Scales the tempo of a file to a new relative tempo
		/// </summary>
		/// <param name="diff">The relative difference in the tempo. Higher makes the tempo higher</param>
		/// <returns>A new file with the adjusted tempo. All tempo messages are adjusted relatively</returns>
		public MidiFile ScaleTempo(double diff)
		{
			var result = new MidiFile(Type, TimeBase);
			for (var i = 0; i < Tracks.Count; ++i)
				result.Tracks.Add(Tracks[i].ScaleTempo(diff));
			return result;
		}
		/// <summary>
		/// Gets a range of events as a new MidiFile
		/// </summary>
		/// <param name="start">The start in ticks</param>
		/// <param name="length">The length, in ticks</param>
		/// <param name="copyTimingAndPatchInfo">True to copy the current timing and patch info, otherwise false</param>
		/// <param name="eliminateEmptyTracks">True to eliminate tracks that end up with no events, otherwise false</param>
		/// <returns></returns>
		public MidiFile GetRange(int start,int length,bool copyTimingAndPatchInfo=false,bool eliminateEmptyTracks = true)
		{
			var result = new MidiFile(Type, TimeBase);
			foreach (var trk in Tracks)
			{
				var t = trk.GetRange(start, length,copyTimingAndPatchInfo);
				if(!eliminateEmptyTracks || 0<t.Events.Count)
					result.Tracks.Add(t);
			}
			return result;
		}
		/// <summary>
		/// Scales the velocity levels
		/// </summary>
		/// <param name="multiplier">The multiplier</param>
		/// <returns>A new MIDI file with the levels scaled</returns>
		public MidiFile ScaleVelocities(double multiplier)
		{
			var result = new MidiFile(Type, TimeBase);
			foreach (var trk in Tracks)
				result.Tracks.Add(trk.ScaleVelocities(multiplier));
			return result;
		}
		/// <summary>
		/// Normalizes the velocity levels
		/// </summary>
		/// <returns>A new MIDI file with the levels normalized</returns>
		public MidiFile NormalizeVelocities()
		{
			var result = new MidiFile(Type, TimeBase);
			var maxvel = 0;
			foreach (var trk in Tracks)
			{
				var v = trk.GetMaxVelocity();
				if (maxvel < v)
					maxvel = v;
			}
			foreach(var trk in Tracks)
				result.Tracks.Add(trk.ScaleVelocities(0 != maxvel ? 128d / maxvel : 0));
			return result;
		}
		/// <summary>
		/// Creates a deep copy of the MIDI file
		/// </summary>
		/// <returns>A new MIDI file that is equivelent to this MIDI file</returns>
		public MidiFile Clone()
		{
			var result = new MidiFile(Type, TimeBase);
			for(int ic=Tracks.Count,i=0;i<ic;++i)
				result.Tracks.Add(Tracks[i].Clone());
			return result;
		}
		object ICloneable.Clone()
		{
			return Clone();
		}
		/// <summary>
		/// Reads a MIDI file from the specified stream
		/// </summary>
		/// <param name="stream">The stream to read from</param>
		/// <returns>A MIDI file object representing the MIDI data in the stream</returns>
		public static MidiFile ReadFrom(Stream stream)
		{
			KeyValuePair<string, byte[]> chunk;
					
			if (!_TryReadChunk(stream, out chunk) || "MThd" != chunk.Key || 6>chunk.Value.Length)
				throw new InvalidDataException("The stream is not a MIDI file format.");
			var type = BitConverter.ToInt16(chunk.Value, 0);
			var trackCount = BitConverter.ToInt16(chunk.Value, 2);
			var timeBase = BitConverter.ToInt16(chunk.Value, 4);
			if(BitConverter.IsLittleEndian)
			{
				type = MidiUtility.Swap(type);
				trackCount = MidiUtility.Swap(trackCount);
				timeBase = MidiUtility.Swap(timeBase);
			}
			var result = new MidiFile(type,timeBase);
			var fileStream = stream as FileStream;
			if (null != fileStream)
				result.FilePath = fileStream.Name;	
			while (_TryReadChunk(stream,out chunk))
			{
				if("MTrk"==chunk.Key)
				{
					var tstm = new MemoryStream(chunk.Value, false);
					var trk = MidiSequence.ReadFrom(tstm);
					result.Tracks.Add(trk);
				}
			}
			return result;
		}
		/// <summary>
		/// Reads a MIDI file from the specified file
		/// </summary>
		/// <param name="filename">The filename</param>
		/// <returns>A new instance representing the MIDI file</returns>
		public static MidiFile ReadFrom(string filename)
		{
			using (var stream = File.OpenRead(filename))
				return ReadFrom(stream);
		}
		/// <summary>
		/// Writes the MIDI file to the specified stream
		/// </summary>
		/// <param name="stream">The stream to write to</param>
		public void WriteTo(Stream stream)
		{
			var fileStream = stream as FileStream;
			if (null != fileStream)
				FilePath = fileStream.Name;
			try
			{
				stream.SetLength(0);
			}
			catch { }
			var buf = new byte[4];
			buf[0] = (byte)'M'; buf[1] = (byte)'T'; buf[2] = (byte)'h';buf[3] = (byte)'d';
			stream.Write(buf, 0, 4);
			var len = 6;
			if (BitConverter.IsLittleEndian)
				len=MidiUtility.Swap(len);
			stream.Write(BitConverter.GetBytes(len),0,4);
			var type = Type;
			var trackCount = (short)Tracks.Count;
			var timeBase = TimeBase;
			if (BitConverter.IsLittleEndian)
			{
				type = MidiUtility.Swap(type);
				trackCount = MidiUtility.Swap(trackCount);
				timeBase = MidiUtility.Swap(timeBase);
			}
			stream.Write(BitConverter.GetBytes(type), 0, 2);
			stream.Write(BitConverter.GetBytes(trackCount), 0, 2);
			stream.Write(BitConverter.GetBytes(timeBase), 0, 2);
			foreach(var trk in Tracks)
			{
				buf[0] = (byte)'M'; buf[1] = (byte)'T'; buf[2] = (byte)'r'; buf[3] = (byte)'k';
				stream.Write(buf, 0, 4);
				var tstm = new MemoryStream();
				trk.WriteTo(tstm);
				tstm.Position = 0;
				len = (int)tstm.Length;
				if (BitConverter.IsLittleEndian)
					len = MidiUtility.Swap(len);
				stream.Write(BitConverter.GetBytes(len), 0, 4);
				tstm.CopyTo(stream); 
				tstm.Close();
			}
		}
		/// <summary>
		/// Writes the MIDI file to the specified file
		/// </summary>
		/// <param name="filename">The filename to write</param>
		public void WriteTo(string filename)
		{
			using (var stream = File.OpenWrite(filename))
				WriteTo(stream);
		}
		private static bool _TryReadChunk(Stream stream,out KeyValuePair<string, byte[]> chunk)
		{
			chunk = default(KeyValuePair<string, byte[]>);
			var buf = new byte[4];
			if (4 != stream.Read(buf, 0, 4)) return false;
			var name = Encoding.ASCII.GetString(buf);
			if (4 != stream.Read(buf, 0, 4)) throw new EndOfStreamException();
			var len = BitConverter.ToInt32(buf, 0);
			if (BitConverter.IsLittleEndian)
				len = MidiUtility.Swap(len);
			buf = new byte[len];
			if(len!=stream.Read(buf,0,len)) throw new EndOfStreamException();
			chunk = new KeyValuePair<string, byte[]>(name, buf);
			return true;
		}
		
	}
}
