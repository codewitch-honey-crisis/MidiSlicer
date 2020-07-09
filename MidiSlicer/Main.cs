using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using M;
namespace MidiSlicer
{
	public partial class Main : Form
	{
		MidiStream _play;
		MidiFile _file;
		MidiFile _processedFile;
		string _tracksLabelFormat;
		bool _dirty;
		bool _reseekDirty;
		public Main()
		{
			InitializeComponent();
			var devs = MidiDevice.Outputs;
			OutputComboBox.DisplayMember = "Name";
			foreach (var dev in devs)
				OutputComboBox.Items.Add(dev);
			OutputComboBox.SelectedIndex = 0;
			_tracksLabelFormat = TracksLabel.Text;
			UnitsCombo.SelectedIndex = 0;
			StartCombo.SelectedIndex = 0;
			_processedFile = null;
			_dirty = true;
			_reseekDirty = true;
			_UpdateMidiFile();

		}

		private void BrowseButton_Click(object sender, EventArgs e)
		{
			var res = OpenMidiFile.ShowDialog(this);
			if (DialogResult.OK == res)
			{
				MidiFileBox.Text = OpenMidiFile.FileName;
				_UpdateMidiFile();
			}
		}
		void _UpdateMidiFile()
		{
			var exists = false;
			try
			{
				if (File.Exists(MidiFileBox.Text))
					exists = true;
			}
			catch { }
			TrackList.Items.Clear();
			if (!exists)
			{
				TracksLabel.Text = "";
				MidiFileBox.ForeColor = Color.Red;
				_file = null;
				TrackList.Enabled = false;
				PreviewButton.Enabled = false;
				UnitsCombo.Enabled = false;
				StartCombo.Enabled = false;
				OffsetUpDown.Enabled = false;
				LengthUpDown.Enabled = false;
				StretchUpDown.Enabled = false;
				MergeTracksCheckBox.Enabled = false;
				CopyTimingPatchCheckBox.Enabled = false;
				AdjustTempoCheckBox.Enabled = false;
				ResampleUpDown.Enabled = false;
				NormalizeCheckBox.Enabled = false;
				LevelsUpDown.Enabled = false;
				TransposeUpDown.Enabled = false;
				WrapCheckBox.Enabled = false;
				DrumsCheckBox.Enabled = false;
				SaveAsButton.Enabled = false;
				TempoUpDown.Enabled = false;
				Visualizer.Sequence = null;
				Visualizer.Size = VisualizerPanel.Size;
			}
			else
			{
				MidiFileBox.ForeColor = SystemColors.WindowText;
				using (Stream stm = File.OpenRead(MidiFileBox.Text))
					 _file = MidiFile.ReadFrom(stm);
				var i = 0;
				foreach (var trk in _file.Tracks)
				{
					var s = trk.Name;
					if (string.IsNullOrEmpty(s))
						s = "Track #" + i.ToString();
					TrackList.Items.Add(s, true);
					++i;
				}
				var sig = _file.TimeSignature;
				var key = _file.KeySignature;
				TracksLabel.Text = string.Format(_tracksLabelFormat, sig.Numerator,sig.Denominator,key);
				TrackList.Enabled = true;
				PreviewButton.Enabled = true;
				UnitsCombo.Enabled = true;
				StartCombo.Enabled = true;
				OffsetUpDown.Enabled = true;
				LengthUpDown.Enabled = true;
				StretchUpDown.Enabled = true;
				MergeTracksCheckBox.Enabled = true;
				CopyTimingPatchCheckBox.Enabled = true;
				AdjustTempoCheckBox.Enabled = true;
				ResampleUpDown.Enabled = true;
				NormalizeCheckBox.Enabled = true;
				LevelsUpDown.Enabled = true;
				TransposeUpDown.Enabled = true;
				WrapCheckBox.Enabled = true;
				DrumsCheckBox.Enabled = true;
				SaveAsButton.Enabled = true;
				TempoUpDown.Enabled = true;
				
				StretchUpDown.Value = 1;
				UnitsCombo.SelectedIndex = 0;
				StartCombo.SelectedIndex = 0;
				ResampleUpDown.Value = _file.TimeBase;
				UnitsCombo.SelectedIndex = 0;
				LengthUpDown.Maximum = _file.Length / (decimal)_file.TimeBase;
				OffsetUpDown.Maximum = LengthUpDown.Maximum - 1;
				LengthUpDown.Value = LengthUpDown.Maximum;
				OffsetUpDown.Value = 0;
				AdjustTempoCheckBox.Checked = false;
				MergeTracksCheckBox.Checked = false;
				NormalizeCheckBox.Checked = false;
				CopyTimingPatchCheckBox.Checked = true;
				LevelsUpDown.Value = 1;
				TransposeUpDown.Value = 0;
				WrapCheckBox.Checked = false;
				DrumsCheckBox.Checked = false;
				TempoUpDown.Value = (decimal)_file.Tempo;
				_dirty = true;
				_processedFile = null;
				Visualizer.Sequence = MidiSequence.Merge(_file.Tracks);
				Visualizer.Width = Math.Max(VisualizerPanel.Width,Visualizer.Sequence.Length/4);
			}
		}

		private void MidiFileBox_Leave(object sender, EventArgs e)
		{
			_UpdateMidiFile();
		}

		private void PreviewButton_Click(object sender, EventArgs e)
		{
			if("Stop"==PreviewButton.Text)
			{
				if (null != _play)
				{
					_play.Close();
				}
				MidiFileBox.Enabled = true;
				BrowseButton.Enabled = true;
				OutputComboBox.Enabled = true;
				PreviewButton.Text = "Preview";
				return;
			}
			
			if (null != _play)
			{
				_play.Close();
			}
			MidiFileBox.Enabled = false;
			BrowseButton.Enabled = false;
			OutputComboBox.Enabled = false;
			PreviewButton.Text = "Stop";
			if(_dirty)
				_processedFile=_ProcessFile();
			var mf = _processedFile;
			
			_play = (OutputComboBox.SelectedItem as MidiOutputDevice).Stream;
			var stm = _play;
			// we use 100 events, which should be safe and allow
			// for some measure of SYSEX messages in the stream
			// without bypassing the 64kb limit
			const int MAX_EVENT_COUNT = 100;
			const int RATE_TICKS = 500;
			// our current cursor pos
			var pos = 0;
			// for tracking deltas
			var ofs = 0;
			// for tracking the song position
			var songPos = 0;
			// merge our file for playback
			var seq = MidiSequence.Merge(mf.Tracks);
			var events = seq.Events;
			// the number of events in the seq
			var len = events.Count;
			// stores the next set of events
			var eventList = new List<MidiEvent>(MAX_EVENT_COUNT);
			
			// open the stream
			stm.Open();
			// start it
			stm.Start();

			// first set the timebase
			stm.TimeBase = mf.TimeBase;
			// set up our send complete handler
			stm.SendComplete += delegate (object s, EventArgs ea)
			{

				try
				{
					BeginInvoke(new Action(delegate ()
					{
						// clear the list	
						eventList.Clear();
						mf = _processedFile;
						if (_dirty)
						{
							if (_reseekDirty)
							{
								var time = _processedFile.Tracks[0].GetContext(songPos, _processedFile.TimeBase).Time;
								_processedFile = _ProcessFile();
								songPos = _processedFile.Tracks[0].GetPositionAtTime(time, _processedFile.TimeBase);
								mf = _processedFile;
								seq = MidiSequence.Merge(mf.Tracks);
								events = new List<MidiEvent>(seq.GetNextEventsAtPosition(songPos, true));
								len = events.Count;
								pos = 0;
							}
							else
							{
								_processedFile = _ProcessFile();
								mf = _processedFile;
								seq = MidiSequence.Merge(mf.Tracks);
								events = seq.Events;
							}
							Visualizer.Sequence = seq;
							Visualizer.Width = Math.Max(VisualizerPanel.Width, Visualizer.Sequence.Length/4);
						}
						

						ofs = 0;
						len = events.Count;
						// iterate through the next events
						var next = pos + MAX_EVENT_COUNT;
						for (; pos < next && ofs <= RATE_TICKS; ++pos)

						{
							// if it's past the end, loop it
							if (len <= pos)
							{
								pos = 0;
								songPos = 0;
								events = seq.Events;
								break;
							}
							var ev = events[pos];
							ofs += ev.Position;
							songPos += pos;
							if (ev.Position < RATE_TICKS && RATE_TICKS < ofs)
								break;
							// otherwise add the next event
							eventList.Add(ev);
						}
						// send the list of events
						if (MidiStreamState.Closed != stm.State && 0 != eventList.Count)
							stm.SendDirect(eventList);
					}));
				}
				catch { }
				
			};
			// add the first events
			for (pos = 0; pos < MAX_EVENT_COUNT && ofs<=RATE_TICKS; ++pos)
			{
				// if it's past the end, loop it
				if (len <= pos )
				{
					pos = 0;
					songPos = 0;
					events = seq.Events;
					break;
				}
				var ev = events[pos];
				ofs += ev.Position;
				if (ev.Position<RATE_TICKS && RATE_TICKS < ofs)
					break;
				// otherwise add the next event
				eventList.Add(ev);
			}
			// send the list of events
			if (0 != eventList.Count)
				stm.SendDirect(eventList);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			

		}

		private void UnitsCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			OffsetUpDown.Value = 0;

			if(null!=_file) // sanity
			{
				if(0==UnitsCombo.SelectedIndex) // beats
				{
					LengthUpDown.Maximum = Math.Ceiling(_file.Length / (decimal)_file.TimeBase);
					OffsetUpDown.Maximum = LengthUpDown.Maximum - 1;
				} else // ticks
				{
					LengthUpDown.Maximum = _file.Length;
					OffsetUpDown.Maximum = LengthUpDown.Maximum - 1;
				}
				LengthUpDown.Value = LengthUpDown.Maximum;
				_dirty = true;
			}
		}

		private void SaveAsButton_Click(object sender, EventArgs e)
		{
			var res = SaveMidiFile.ShowDialog(this);
			if (DialogResult.OK == res)
			{
				var mf = _ProcessFile();
				using (var stm = File.OpenWrite(SaveMidiFile.FileName))
				{
					stm.SetLength(0);
					mf.WriteTo(stm);
				}
			}
		}
		MidiFile _ProcessFile()
		{
			// first we clone the file to be safe
			// that way in case there's no modifications
			// specified in the UI we'll still return 
			// a copy.
			var result = _file.Clone();
			// transpose it if specified
			if(0!=TransposeUpDown.Value)
				result = result.Transpose((sbyte)TransposeUpDown.Value, WrapCheckBox.Checked,!DrumsCheckBox.Checked);
			// resample if specified
			if (ResampleUpDown.Value != _file.TimeBase)
				result = result.Resample(unchecked((short)ResampleUpDown.Value));
			// compute our offset and length in ticks or beats/quarter-notes
			var ofs = OffsetUpDown.Value;
			var len = LengthUpDown.Value;
			if (0 == UnitsCombo.SelectedIndex) // beats
			{
				len = Math.Min(len * _file.TimeBase, _file.Length);
				ofs = Math.Min(ofs * _file.TimeBase, _file.Length);
			}
			switch (StartCombo.SelectedIndex)
			{
				case 1:
					ofs += result.FirstDownBeat;
					break;
				case 2:
					ofs += result.FirstNoteOn;
					break;
			}
			// nseq holds our patch and timing info
			var nseq = new MidiSequence();
			if(0!=ofs && CopyTimingPatchCheckBox.Checked)
			{
				// we only want to scan until the
				// first note on
				// we need to check all tracks so
				// we merge them into mtrk and scan
				// that
				var mtrk = MidiSequence.Merge(result.Tracks);
				var end = mtrk.FirstNoteOn;
				if (0 == end) // break later:
					end = mtrk.Length;
				var ins = 0;
				for (int ic = mtrk.Events.Count, i = 0; i < ic; ++i)
				{
					var ev = mtrk.Events[i];
					if (ev.Position >= end)
						break;
					var m = ev.Message;
					switch (m.Status)
					{
						// the reason we don't check for MidiMessageMetaTempo
						// is a user might have specified MidiMessageMeta for
						// it instead. we want to handle both
						case 0xFF:
							var mm = m as MidiMessageMeta;
							switch (mm.Data1)
							{

								case 0x51: // tempo
								case 0x54: // smpte
									if (0 == nseq.Events.Count)
										nseq.Events.Add(new MidiEvent(0,ev.Message.Clone()));
									else
										nseq.Events.Insert(ins, new MidiEvent(0,ev.Message.Clone()));
									++ins;
									break;
							}
							break;
						default:
							// check if it's a patch change
							if (0xC0 == (ev.Message.Status & 0xF0))
							{
								if (0 == nseq.Events.Count)
									nseq.Events.Add(new MidiEvent(0, ev.Message.Clone()));
								else
									nseq.Events.Insert(ins, new MidiEvent(0, ev.Message.Clone()));
								// increment the insert count
								++ins;
							}
							break;
					}
				}
				// set the track to the loop length
				nseq.Events.Add(new MidiEvent((int)len, new MidiMessageMetaEndOfTrack()));
			}
			// see if track 0 is checked
			var hasTrack0 = TrackList.GetItemChecked(0);
			// slice our loop out of it
			if (0!=ofs || result.Length!=len)
				result = result.GetRange((int)ofs, (int)len,CopyTimingPatchCheckBox.Checked,false);
			// normalize it!
			if (NormalizeCheckBox.Checked)
				result = result.NormalizeVelocities();
			// scale levels
			if (1m != LevelsUpDown.Value)
				result = result.ScaleVelocities((double)LevelsUpDown.Value);

			// create a temporary copy of our
			// track list
			var l = new List<MidiSequence>(result.Tracks);
			// now clear the result
			result.Tracks.Clear();
			for(int ic=l.Count,i=0;i<ic;++i)
			{
				// if the track is checked in the list
				// add it back to result
				if(TrackList.GetItemChecked(i))
				{
					result.Tracks.Add(l[i]);
				}
			}
			if (0 < nseq.Events.Count)
			{
				// if we don't have track zero we insert
				// one.
				if(!hasTrack0)
					result.Tracks.Insert(0,nseq);
				else
				{
					// otherwise we merge with track 0
					result.Tracks[0] = MidiSequence.Merge(nseq, result.Tracks[0]);
					
				}
			}
			// next adjust the tempo
			if (_file.Tempo != (double)TempoUpDown.Value)
			{
				result = result.AdjustTempo((double)TempoUpDown.Value);
			}
			// stretch the result. we do this
			// here so the track lengths are
			// correct and we don't need ofs
			// or len anymore
			if (1m != StretchUpDown.Value)
			{
				result = result.Stretch((double)StretchUpDown.Value, AdjustTempoCheckBox.Checked);
			}
			
			// if merge is checked merge the
			// tracks
			if (MergeTracksCheckBox.Checked)
			{
				var trk = MidiSequence.Merge(result.Tracks);
				result.Tracks.Clear();
				result.Tracks.Add(trk);
			}
			_dirty = false;
			_reseekDirty = false;
			return result;
		}

		private void OutputComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void TrackList_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			_dirty = true;
			_reseekDirty = true;
			
			
		}

		private void _SetDirtyHandler(object sender, EventArgs e)
		{
			_dirty = true;
		}

		private void LengthUpDown_ValueChanged(object sender, EventArgs e)
		{
			_dirty = true;
			_reseekDirty = true;
		}

		private void VisualizerPanel_Resize(object sender, EventArgs e)
		{
			Visualizer.Height = VisualizerPanel.Height;
			if (null != Visualizer.Sequence)
			{
				Visualizer.Width = Math.Max(VisualizerPanel.Width, Visualizer.Sequence.Length/4);
			}
			else
				Visualizer.Width = VisualizerPanel.Width;

		}
	}
}
