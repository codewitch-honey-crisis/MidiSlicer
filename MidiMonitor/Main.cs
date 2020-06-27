using M;
using System;
using System.Windows.Forms;

namespace MidiMonitor
{
	public partial class Main : Form
	{
		MidiInputDevice _device = null;
		public Main()
		{
			InitializeComponent();
			InputsComboBox.DisplayMember = "Name";
			var inputs = MidiDevice.Inputs;

			foreach(var input in inputs)
				InputsComboBox.Items.Add(input);
			
			InputsComboBox.SelectedIndex = 0;
		}

		private void InputsComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (null != _device)
				_device.Close();
			_device = InputsComboBox.SelectedItem as MidiInputDevice;
			_device.Input +=device_Input;
			_device.Open();
			_device.Start();
		}

		private void device_Input(object sender, MidiEventArgs args)
		{
			MessagesTextBox.AppendText(args.Message.ToString()+Environment.NewLine);
		}
		protected override void OnClosed(EventArgs e)
		{
			if (null != _device)
				_device.Close();
			base.OnClosed(e);
		}
	}
}
