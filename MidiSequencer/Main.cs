using M;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MidiSequencer
{
	public partial class Main : Form
	{
		MidiInputDevice _midiInputDevice=null;
		MidiOutputDevice _midiOutputDevice=null;
		public Main()
		{
			MinimumSize = new Size(200, 130);
			InitializeComponent();
			MidiInputComboBox.DisplayMember = "Name";
			MidiOutputComboBox.DisplayMember = "Name";
			_RefreshDeviceList();
			Visualizer.Sequence = MidiSequence.Merge(MidiFile.ReadFrom(@"..\..\GORILLAZ_-_Feel_Good_Inc.mid").Tracks);
		}
		void _RefreshDeviceList()
		{
			var isi = MidiInputComboBox.SelectedIndex;
			var osi = MidiOutputComboBox.SelectedIndex;
			MidiInputComboBox.Enabled = false;
			MidiOutputComboBox.Enabled = false;
			MidiInputComboBox.Items.Clear();
			MidiOutputComboBox.Items.Clear();
			foreach (var dev in MidiDevice.Inputs)
			{
				MidiInputComboBox.Enabled = true;
				MidiInputComboBox.Items.Add(dev);
			}
			foreach (var dev in MidiDevice.Outputs)
			{
				MidiOutputComboBox.Enabled = true;
				MidiOutputComboBox.Items.Add(dev);
			}
			if (-1==isi)
				isi = 0;
			if (MidiInputComboBox.Enabled)
				MidiInputComboBox.SelectedIndex = isi;
			if (-1==osi )
				osi = 0;
			if (MidiOutputComboBox.Enabled)
				MidiOutputComboBox.SelectedIndex = osi;

		}

		private void MidiRefreshDeviceListButton_Click(object sender, EventArgs e)
		{
			_RefreshDeviceList();
		}

		private void MidiOutputComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (null != _midiOutputDevice)
				_midiOutputDevice.Close();
			_midiOutputDevice = MidiOutputComboBox.SelectedItem as MidiOutputDevice;
			if (null != _midiOutputDevice)
				_midiOutputDevice.Open();
		}

		private void Piano_PianoKeyUp(object sender, PianoKeyEventArgs args)
		{
			if (null != _midiOutputDevice && _midiOutputDevice.IsOpen)
			{
				if (args.Key < 128)
				{
					_midiOutputDevice.Send(new MidiMessageNoteOff(unchecked((byte)args.Key), 127, 0));
				}
			}
		}

		private void Piano_PianoKeyDown(object sender, PianoKeyEventArgs args)
		{
			if (null != _midiOutputDevice && _midiOutputDevice.IsOpen)
			{
				if (args.Key < 128)
				{
					_midiOutputDevice.Send(new MidiMessageNoteOn(unchecked((byte)args.Key), 127, 0));
				}
			}
		}
	}
}
