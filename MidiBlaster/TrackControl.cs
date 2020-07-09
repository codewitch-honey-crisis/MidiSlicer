using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using M;

namespace MidiBlaster
{
	public partial class TrackControl : UserControl
	{
		public TrackControl()
		{
			InitializeComponent();
			TransposeComboBox.Items.Add("(None)");
			for(byte i = 0;i<128;++i)
				TransposeComboBox.Items.Add(MidiUtility.NoteIdToNote(i));
			TransposeComboBox.SelectedIndex = 0;
			
		}
	}
}
