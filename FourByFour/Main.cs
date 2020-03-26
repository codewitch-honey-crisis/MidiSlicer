using M;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FourByFour
{
	public partial class Main : Form
	{
		Thread _previewThread;
		public Main()
		{
			InitializeComponent();
		}

		private void AddButton_Click(object sender, EventArgs e)
		{
			var beats = new BeatControl();
			BeatsPanel.Controls.Add(beats);
			beats.Dock = DockStyle.Top;
			beats.Delete += Beats_Delete;
		}

		private void Beats_Delete(object sender, EventArgs e)
		{
			BeatsPanel.Controls.Remove(sender as Control);
		}

		private void PlayButton_Click(object sender, EventArgs e)
		{
			if("Stop"==PlayButton.Text)
			{
				if(null!=_previewThread)
				{
					_previewThread.Abort();
					_previewThread.Join();
					_previewThread = null;
				}
				PlayButton.Text = "Play";
				return;
			}
			var file = _CreateMidiFile();
			PlayButton.Text = "Stop";
			_previewThread = new Thread(() => file.Preview(0, true));
			_previewThread.Start();
		}
		MidiFile _CreateMidiFile()
		{
			var file = new MidiFile();
			var track0 = new MidiSequence();
			track0.Events.Add(new MidiEvent(0, new MidiMessageMetaTempo((double)TempoUpDown.Value)));
			var len = 4 * file.TimeBase;
			track0.Events.Add(new MidiEvent(len, new MidiMessageMetaEndOfTrack()));
			var trackEnd = new MidiSequence();
			trackEnd.Events.Add(new MidiEvent(len, new MidiMessageMetaEndOfTrack()));
			file.Tracks.Add(track0);
			var track1 = new MidiSequence();
			track1.Events.Add(new MidiEvent(0, new MidiMessagePatchChange(0, 9)));
			var trks = new List<MidiSequence>(BeatsPanel.Controls.Count);
			foreach (var ctl in BeatsPanel.Controls)
			{
				var beat = ctl as BeatControl;
				var note = beat.Note;
				var noteMap = new List<MidiNote>();
				for (int ic = beat.Steps.Count, i = 0; i < ic; ++i)
				{
					if (beat.Steps[i])
						noteMap.Add(new MidiNote(i * file.TimeBase / 4, 9, note, 127, file.TimeBase / 4));
				}
				trks.Add(MidiSequence.FromNoteMap(noteMap));
			}
			var t = MidiSequence.Merge(trks);
			track1 = MidiSequence.Merge(track1, t, trackEnd);
			file.Tracks.Add(track1);
			return file;
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			if(null!=_previewThread)
			{
				_previewThread.Abort();
				_previewThread.Join();
				_previewThread = null;
			}
		}

		private void SaveAsButton_Click(object sender, EventArgs e)
		{
			if(DialogResult.OK==SaveMidiFile.ShowDialog())
			{
				var file = _CreateMidiFile();
				file.WriteTo(SaveMidiFile.FileName);
			}
		}
	}
}
