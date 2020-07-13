using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using M;
namespace MidiDisplay
{
	public partial class Main : Form
	{
		MidiStream _outputStream;
		public Main()
		{
			InitializeComponent();
			_RefreshDeviceList();
			_UpdateMidiFile();
		}
		void _RefreshDeviceList()
		{
			string outp = null;
			// store the old output, if there is one
			if (-1 < MidiOutComboBox.SelectedIndex)
			{
				outp = MidiOutComboBox.Text;
			}
			// repopulate the output
			MidiOutComboBox.Items.Clear();
			MidiOutComboBox.DisplayMember = "Name";
			foreach (var dev in MidiDevice.Streams)
				MidiOutComboBox.Items.Add(dev);
			// restore the old output
			if (!string.IsNullOrEmpty(outp))
			{
				for (var i = 0; i < MidiOutComboBox.Items.Count; ++i)
				{
					if (outp == (MidiOutComboBox.Items[i] as MidiStream).Name)
					{
						MidiOutComboBox.SelectedIndex = i;
						break;
					}
				}
			}
			if (0 < MidiOutComboBox.Items.Count && 0 > MidiOutComboBox.SelectedIndex)
				MidiOutComboBox.SelectedIndex = 0;
		}
		private void MidiOutComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(null!=_outputStream && _outputStream.IsOpen)
			{
				_outputStream.Close();
				_outputStream = null;
			}
			_outputStream = MidiOutComboBox.SelectedItem as MidiStream;
		}

		private void FileBrowseButton_Click(object sender, EventArgs e)
		{
			if(DialogResult.OK==MidiOpenFileDialog.ShowDialog())
			{
				FileTextBox.Text = MidiOpenFileDialog.FileName;
			}
		}

		private void FileTimer_Tick(object sender, EventArgs e)
		{
			FileTimer.Enabled = false;
			_UpdateMidiFile();
		}
		private void _UpdateMidiFile()
		{
			MidiFile file;
			try { file = MidiFile.ReadFrom(FileTextBox.Text); }
			catch { file = null; }
			if(null==file)
			{
				PlayButton.Enabled = false;
				FileTextBox.ForeColor = Color.Red;
				Visualizer.Sequence = null;
				return;
			}
			PlayButton.Enabled = true;
			FileTextBox.ForeColor = SystemColors.WindowText;
			var seq = MidiSequence.Merge(file.Tracks);
			Visualizer.Sequence = seq;
			Visualizer.Width = seq.Length;
		}

		private void FileTextBox_TextChanged(object sender, EventArgs e)
		{
			FileTimer.Enabled = true;
		}

		private void PlayButton_Click(object sender, EventArgs e)
		{
			if("Stop"==PlayButton.Text)
			{
				PlayButton.Text = "Play";
				if(null!=_outputStream )
				{
					_outputStream.Close();
				}
				MidiOutComboBox.Enabled = true;
				FileTextBox.Enabled = true;
				FileBrowseButton.Enabled = true;
				Visualizer.ShowCursor = false;
				return;
			}
			PlayButton.Text = "Stop";
			Visualizer.CursorPosition = 0;
			Visualizer.ShowCursor = true;
			MidiOutComboBox.Enabled = false;
			FileTextBox.Enabled = false;
			FileBrowseButton.Enabled = false;
			if(null!=_outputStream)
			{
				var stm = _outputStream;
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
				var mf = MidiFile.ReadFrom(FileTextBox.Text);
				var seq = MidiSequence.Merge(mf.Tracks);
				var events = seq.Events;
				// the number of events in the seq
				var len = events.Count;
				// stores the next set of events
				var eventList = new List<MidiEvent>(MAX_EVENT_COUNT);
				var songLen = seq.Length;
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
							try
							{
								// send the list of events
								if (MidiStreamState.Closed != stm.State && 0 != eventList.Count)
									stm.SendDirect(eventList);
							}
							catch { }
							Visualizer.CursorPosition = stm.PositionTicks % songLen;
						}));
					}
					catch { }

				};
				// add the first events
				for (pos = 0; pos < MAX_EVENT_COUNT && ofs <= RATE_TICKS; ++pos)
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
					if (ev.Position < RATE_TICKS && RATE_TICKS < ofs)
						break;
					// otherwise add the next event
					eventList.Add(ev);
				}
				// send the list of events
				if (0 != eventList.Count)
					stm.SendDirect(eventList);
			}
		}
	}
}
