using System;
using System.Drawing;
using System.Windows.Forms;
using M;
namespace MidiKeys
{
	public partial class Main : Form
	{
		MidiInputDevice _inputDevice;
		MidiOutputDevice _outputDevice;
		public Main()
		{
			InitializeComponent();
			
			// for some reason the designer
			// screws this up:
			Piano.Location = new Point(0, 0);
			_RefreshDeviceList();
		}
		
		void _RefreshDeviceList()
		{
			string inp = null;
			string outp = null;
			// store the old input, if there is one
			if(-1<MidiInComboBox.SelectedIndex)
			{
				inp = MidiInComboBox.Text;
			}
			// repopulate the input
			MidiInComboBox.Items.Clear();
			MidiInComboBox.DisplayMember = "Name";
			foreach (var dev in MidiDevice.Inputs)
				MidiInComboBox.Items.Add(dev);
			// restore the old input
			if(!string.IsNullOrEmpty(inp))
			{
				for (var i = 0; i < MidiInComboBox.Items.Count; ++i)
				{
					if (inp == (MidiInComboBox.Items[i] as MidiInputDevice).Name)
					{
						MidiInComboBox.SelectedIndex = i;
						break;
					}
				}
			}
			if (0 < MidiInComboBox.Items.Count && 0 > MidiInComboBox.SelectedIndex)
				MidiInComboBox.SelectedIndex = 0;
			// store the old output, if there is one
			if (-1 < MidiOutComboBox.SelectedIndex)
			{
				outp = MidiOutComboBox.Text;
			}
			// repopulate the output
			MidiOutComboBox.Items.Clear();
			MidiOutComboBox.DisplayMember = "Name";
			foreach (var dev in MidiDevice.Outputs)
				MidiOutComboBox.Items.Add(dev);
			// restore the old output
			if (!string.IsNullOrEmpty(outp))
			{
				for (var i = 0; i < MidiOutComboBox.Items.Count; ++i)
				{
					if (outp == (MidiOutComboBox.Items[i] as MidiOutputDevice).Name)
					{
						MidiOutComboBox.SelectedIndex = i;
						break;
					}
				}
			}
			if (0 < MidiOutComboBox.Items.Count && 0 > MidiOutComboBox.SelectedIndex)
				MidiOutComboBox.SelectedIndex = 0;
		}

		private void MidiRefreshButton_Click(object sender, System.EventArgs e)
		{
			_RefreshDeviceList();
		}

		private void MidiInComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (null != _inputDevice)
				_inputDevice.Close();
			_inputDevice = MidiInComboBox.SelectedItem as MidiInputDevice;
			if(null!=_inputDevice)
			{
				_inputDevice.Input += _inputDevice_Input;
			}
			_inputDevice.Open();
			_inputDevice.Start();
		}
		private void MidiOutComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (null != _outputDevice)
				_outputDevice.Close();
			_outputDevice = MidiOutComboBox.SelectedItem as MidiOutputDevice;
			
			_outputDevice.Open();
			
		}
		private void _inputDevice_Input(object sender, MidiInputEventArgs args)
		{
			if (IsHandleCreated)
			{
				BeginInvoke(new Action(() =>
				{
					if (null != _outputDevice && _outputDevice.IsOpen)
					{
						_outputDevice.Send(args.Message);
					}
					// when we hit a note on, or note off below
					// we set or release the corresponding piano
					// key. We must suppress raising events or
					// this would cause a circular codepath
					switch (args.Message.Status & 0xF0)
					{
						case 0x80: // key up
							var msw = args.Message as MidiMessageWord;
							Piano.SetKey(msw.Data1, false, true);
							break;
						case 0x90: // key down
							msw = args.Message as MidiMessageWord;
							Piano.SetKey(msw.Data1, true, true);
							break;
					}
				}));
			}
		}
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (null != _inputDevice && _inputDevice.IsOpen) {
				// attempt to prevent any further
				// events from firing
				_inputDevice.Stop();
				_inputDevice.Reset();

				_inputDevice.Close();
				_inputDevice = null;
			}
			if(null!=_outputDevice && _outputDevice.IsOpen)
			{
				_outputDevice.Close();
				_outputDevice = null;
			}

		}

		private void Piano_PianoKeyDown(object sender, PianoKeyEventArgs args)
		{
			if(null!=_outputDevice && _outputDevice.IsOpen && args.Key < 128)
			{
				_outputDevice.Send(new MidiMessageNoteOn((byte)args.Key, 127, (byte)ChannelUpDown.Value));
			}
		}

		private void Piano_PianoKeyUp(object sender, PianoKeyEventArgs args)
		{
			if (null != _outputDevice && _outputDevice.IsOpen && args.Key < 128)
			{
				_outputDevice.Send(new MidiMessageNoteOff((byte)args.Key, 127, (byte)ChannelUpDown.Value));
			}
		}
	}
}
