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
		MidiFile _file;
		Thread _previewThread;
		string _tracksLabelFormat;
		public Main()
		{
			InitializeComponent();
			_tracksLabelFormat = TracksLabel.Text;
			UnitsCombo.SelectedIndex = 0;
			StartCombo.SelectedIndex = 0;
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
				TracksLabel.Text = string.Format(_tracksLabelFormat, _file.Tempo,sig.Numerator,sig.Denominator);
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
				if (null != _previewThread)
				{
					_previewThread.Abort();
					_previewThread.Join();
					_previewThread = null;
				}
				PreviewButton.Text = "Preview";
				return;
			}
			
			if (null != _previewThread)
			{
				_previewThread.Abort();
				_previewThread.Join();
				_previewThread = null;
			}
			PreviewButton.Text = "Stop";
			var f = _ProcessFile();
#if DEBUG
			if(System.Diagnostics.Debugger.IsAttached)
				_DumpFile(f);
#endif

			_previewThread = new Thread(() => { f.Preview(0,true); });
			_previewThread.Start();
		}
#if DEBUG
		void _DumpFile(MidiFile f)
		{
			for(var i = 0;i<f.Tracks.Count;++i)
			{
				System.Diagnostics.Debug.WriteLine("Track #" + i.ToString());
				foreach(var ev in f.Tracks[i].AbsoluteEvents)
				{
					System.Diagnostics.Debug.WriteLine(ev.Position.ToString() + ": " + ev.Message.ToString());
				}
				System.Diagnostics.Debug.WriteLine("");
			}
		}
#endif
		protected override void OnClosing(CancelEventArgs e)
		{
			if (null != _previewThread)
			{
				_previewThread.Abort();
				_previewThread.Join();
				_previewThread = null;
			}

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
			// stretch the result. we do this
			// here so the track lengths are
			// correct and we don't need ofs
			// or len anymore
			if (1m != StretchUpDown.Value)
				result = result.Stretch((double)StretchUpDown.Value, AdjustTempoCheckBox.Checked);

			// if merge is checked merge the
			// tracks
			if (MergeTracksCheckBox.Checked)
			{
				var trk = MidiSequence.Merge(result.Tracks);
				result.Tracks.Clear();
				result.Tracks.Add(trk);
			}
			return result;
		}
	}
}
