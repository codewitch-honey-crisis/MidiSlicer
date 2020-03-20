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
				SaveAsButton.Enabled = true;
				StretchUpDown.Value = 1;
				UnitsCombo.SelectedIndex = 0;
				StartCombo.SelectedIndex = 0;
				ResampleUpDown.Value = _file.TimeBase;
				if (0 == UnitsCombo.SelectedIndex) // beats
				{
					LengthUpDown.Maximum = Math.Ceiling(_file.Length / (decimal)_file.TimeBase);
					OffsetUpDown.Maximum = LengthUpDown.Maximum - 1;
				}
				else // ticks
				{
					LengthUpDown.Maximum = _file.Length;
					OffsetUpDown.Maximum = LengthUpDown.Maximum - 1;
				}
				LengthUpDown.Value = LengthUpDown.Maximum;
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
			var f = _file;
			if (ResampleUpDown.Value != _file.TimeBase)
				f = _file.Resample(unchecked((short)ResampleUpDown.Value));
			if (NormalizeCheckBox.Checked)
				f = _file.NormalizeVelocities();
			if (1m != LevelsUpDown.Value)
				f = _file.ScaleVelocities((double)LevelsUpDown.Value);
			var trks = new List<MidiSequence>(f.Tracks.Count);
			for (int ic = TrackList.Items.Count, i = 0; i < ic; ++i)
				if (TrackList.CheckedItems.Contains(TrackList.Items[i]))
					trks.Add(f.Tracks[i]);
			var trk = MidiSequence.Merge(trks);
			var ofs = OffsetUpDown.Value;
			var len = LengthUpDown.Value;
			if (0 == UnitsCombo.SelectedIndex) // beats
			{
				len = Math.Min(len * f.TimeBase, f.Length);
				ofs = Math.Min(ofs * f.TimeBase, f.Length);
			}
			switch(StartCombo.SelectedIndex)
			{
				case 1:
					ofs += f.FirstDownBeat;
					break;
				case 2:
					ofs += f.FirstNoteOn;
					break;
			}
			if (0 != ofs && CopyTimingPatchCheckBox.Checked)
			{
				var end = trk.FirstNoteOn;
				if (0 == end)
					end = trk.Length;
				var trk2= trk.GetRange((int)ofs, (int)len);
				var ins = 0;
				for(int ic = trk.Events.Count,i=0;i<ic;++i)
				{
					var ev = trk.Events[i];
					if (ev.Position >= end)
						break;
					var m = ev.Message;
					switch(m.Status)
					{
						case 0xFF:
							var mm = m as MidiMessageMeta;
							switch(mm.Data1) 
							{
								case 0x51:
								case 0x54:
									trk2.Events.Insert(ins, ev.Clone());
									++ins;
									break;
							}
							break;
						default:
							if (0xC0 == (ev.Message.Status & 0xF0))
							{
								trk2.Events.Insert(ins, ev.Clone());
								++ins;
							}
							break;
					}
				}
				trk = trk2;
			} else {
				if (trk.Length != len || 0 != ofs)
					trk = trk.GetRange((int)ofs, (int)len);
			}
			if (1m != StretchUpDown.Value)
				trk = trk.Stretch((double)StretchUpDown.Value, AdjustTempoCheckBox.Checked);

			if (null != _previewThread)
			{
				_previewThread.Abort();
				_previewThread.Join();
				_previewThread = null;
			}
			PreviewButton.Text = "Stop";
			_previewThread = new Thread(() => { trk.Preview(f.TimeBase,0,true); });
			_previewThread.Start();
		}
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
				var f = _file;
				if (ResampleUpDown.Value != _file.TimeBase)
				{
					f = f.Resample(unchecked((short)ResampleUpDown.Value));
				}
				var trks = new List<MidiSequence>(f.Tracks.Count);
				for (int ic = TrackList.Items.Count, i = 0; i < ic; ++i)
					if (TrackList.CheckedItems.Contains(TrackList.Items[i]))
						trks.Add(f.Tracks[i]);

				var mf = new MidiFile(1,f.TimeBase);
				if (!MergeTracksCheckBox.Checked)
				{
					foreach (var tr in trks)
					{
						mf.Tracks.Add(_ProcessTrack(tr));
					}
				}
				else
				{
					mf.Tracks.Add(_ProcessTrack(MidiSequence.Merge(trks)));
				}
				using (var stm = File.OpenWrite(SaveMidiFile.FileName))
				{
					mf.WriteTo(stm);
				}
					
			}
		}
		MidiSequence _ProcessTrack(MidiSequence trk)
		{
			if (NormalizeCheckBox.Checked)
				trk.NormalizeVelocities();
			if (1m != LevelsUpDown.Value)
				trk.ScaleVelocities((double)LevelsUpDown.Value);
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
					ofs += _file.FirstDownBeat;
					break;
				case 2:
					ofs += _file.FirstNoteOn;
					break;
			}
			switch (StartCombo.SelectedIndex)
			{
				case 1:
					ofs += _file.FirstDownBeat;
					break;
				case 2:
					ofs += _file.FirstNoteOn;
					break;
			}
			if (0 != ofs && CopyTimingPatchCheckBox.Checked)
			{
				var end = trk.FirstNoteOn;
				if (0 == end)
					end = trk.Length;
				var trk2 = trk.GetRange((int)ofs, (int)len);
				var ins = 0;
				for (int ic = trk.Events.Count, i = 0; i < ic; ++i)
				{
					var ev = trk.Events[i];
					if (ev.Position >= end)
						break;
					var m = ev.Message;
					switch (m.Status)
					{
						case 0xFF:
							var mm = m as MidiMessageMeta;
							switch (mm.Data1)
							{
								case 0x51:
								case 0x54:
									trk2.Events.Insert(ins, ev.Clone());
									++ins;
									break;
							}
							break;
						default:
							if (0xC0 == (ev.Message.Status & 0xF0))
							{
								trk2.Events.Insert(ins, ev.Clone());
								++ins;
							}
							break;
					}
				}
				trk = trk2;
			}
			else
			{
				if (trk.Length != len || 0 != ofs)
					trk = trk.GetRange((int)ofs, (int)len);
			}
			if (1m != StretchUpDown.Value)
				trk = trk.Stretch((double)StretchUpDown.Value, AdjustTempoCheckBox.Checked);
			return trk;
		}
	}
}
