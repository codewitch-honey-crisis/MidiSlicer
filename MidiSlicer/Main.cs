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
				OffsetUpDown.Enabled = false;
				LengthUpDown.Enabled = false;
				StretchUpDown.Enabled = false;
				MergeTracksCheckBox.Enabled = false;
				AdjustTempoCheckBox.Enabled = false;
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
				OffsetUpDown.Enabled = true;
				LengthUpDown.Enabled = true;
				StretchUpDown.Enabled = true;
				MergeTracksCheckBox.Enabled = true;
				AdjustTempoCheckBox.Enabled = true;
				SaveAsButton.Enabled = true;
				StretchUpDown.Value = 1;
				UnitsCombo.SelectedIndex = 0;
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
			var trks = new List<MidiSequence>(_file.Tracks.Count);
			for (int ic = TrackList.Items.Count, i = 0; i < ic; ++i)
				if (TrackList.CheckedItems.Contains(TrackList.Items[i]))
					trks.Add(_file.Tracks[i]);
			var trk = MidiSequence.Merge(trks);
			var ofs = (int)OffsetUpDown.Value;
			var len = (int)LengthUpDown.Value;
			if (0 == UnitsCombo.SelectedIndex) // beats
			{
				len = (int)Math.Min(Math.Ceiling(len * (decimal)_file.TimeBase), _file.Length);
				ofs = (int)Math.Min(Math.Ceiling(ofs * (decimal)_file.TimeBase), _file.Length);
			}
			if(trk.Length!=len || 0!=ofs)
				trk = trk.GetRange(ofs, len);
			if (1m != StretchUpDown.Value)
				trk = trk.Stretch((double)StretchUpDown.Value, AdjustTempoCheckBox.Checked);

			if (null != _previewThread)
			{
				_previewThread.Abort();
				_previewThread.Join();
				_previewThread = null;
			}

			_previewThread = new Thread(() => { trk.Preview(); });
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
				var trks = new List<MidiSequence>(_file.Tracks.Count);
				for (int ic = TrackList.Items.Count, i = 0; i < ic; ++i)
					if (TrackList.CheckedItems.Contains(TrackList.Items[i]))
						trks.Add(_file.Tracks[i]);

				var mf = new MidiFile(1,_file.TimeBase);
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
			var ofs = (int)OffsetUpDown.Value;
			var len = (int)LengthUpDown.Value;
			if (0 == UnitsCombo.SelectedIndex) // beats
			{
				len = (int)Math.Min(Math.Ceiling(len * (decimal)_file.TimeBase), _file.Length);
				ofs = (int)Math.Min(Math.Ceiling(ofs * (decimal)_file.TimeBase), _file.Length);
			}
			if (trk.Length != len || 0 != ofs)
				trk = trk.GetRange(ofs, len);
			if (1m != StretchUpDown.Value)
				trk = trk.Stretch((double)StretchUpDown.Value, AdjustTempoCheckBox.Checked);
			return trk;
		}
	}
}
