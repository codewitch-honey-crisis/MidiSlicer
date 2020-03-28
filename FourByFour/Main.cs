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
			// we'll need a track 0 for our tempo map
			var track0 = new MidiSequence();
			// set the tempo at the first position
			track0.Events.Add(new MidiEvent(0, new MidiMessageMetaTempo((double)TempoUpDown.Value)));
			// compute the length of our loop
			var len = ((int)BarsUpDown.Value) * 4 * file.TimeBase;
			// add an end of track marker just so all
			// of our tracks will be the loop length
			track0.Events.Add(new MidiEvent(len, new MidiMessageMetaEndOfTrack()));
			
			// here we need a track end with an 
			// absolute position for the MIDI end
			// of track meta message. We'll use this
			// later to set the length of the track
			var trackEnd = new MidiSequence();
			trackEnd.Events.Add(new MidiEvent(len, new MidiMessageMetaEndOfTrack()));
			
			// add track 0 (our tempo map)
			file.Tracks.Add(track0);

			// create track 1 (our drum track)
			var track1 = new MidiSequence();
			
			// we're going to create a new sequence for
			// each one of the drum sequencer tracks in
			// the UI
			var trks = new List<MidiSequence>(BeatsPanel.Controls.Count);
			foreach (var ctl in BeatsPanel.Controls)
			{
				var beat = ctl as BeatControl;
				// get the note for the drum
				var note = beat.NoteId;
				// it's easier to use a note map
				// to build the drum sequence
				var noteMap = new List<MidiNote>();
				for (int ic = beat.Steps.Count, i = 0; i < ic; ++i)
				{
					// if the step is pressed create 
					// a note for it
					if (beat.Steps[i])
						noteMap.Add(new MidiNote(i * (file.TimeBase / 4), 9, note, 127, file.TimeBase / 4-1));
				}
				// convert the note map to a sequence
				// and add it to our working tracks
				trks.Add(MidiSequence.FromNoteMap(noteMap));
			}
			// now we merge the sequences into one
			var t = MidiSequence.Merge(trks);
			// we merge everything down to track 1
			track1 = MidiSequence.Merge(track1, t, trackEnd);
			// .. and add it to the file
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

		private void BarsUpDown_ValueChanged(object sender, EventArgs e)
		{
			foreach(var ctl in BeatsPanel.Controls)
			{
				var beat = ctl as BeatControl;
				if (null != beat) // sanity
					beat.Bars = (int)BarsUpDown.Value;
			}
		}
	}
}
