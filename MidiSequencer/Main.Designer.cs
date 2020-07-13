namespace MidiSequencer
{
	partial class Main
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.KeysPanel = new System.Windows.Forms.Panel();
			this.MidiGroupBox = new System.Windows.Forms.GroupBox();
			this.MidiRefreshDeviceListButton = new System.Windows.Forms.Button();
			this.MidiOutputComboBox = new System.Windows.Forms.ComboBox();
			this.MidiOutputLabel = new System.Windows.Forms.Label();
			this.MidiInputComboBox = new System.Windows.Forms.ComboBox();
			this.MidiInputLabel = new System.Windows.Forms.Label();
			this.Visualizer = new M.MidiVisualizer();
			this.Piano = new M.PianoBox();
			this.KeysPanel.SuspendLayout();
			this.MidiGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// KeysPanel
			// 
			this.KeysPanel.AutoScroll = true;
			this.KeysPanel.Controls.Add(this.Piano);
			this.KeysPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.KeysPanel.Location = new System.Drawing.Point(0, 325);
			this.KeysPanel.Name = "KeysPanel";
			this.KeysPanel.Size = new System.Drawing.Size(800, 125);
			this.KeysPanel.TabIndex = 1;
			// 
			// MidiGroupBox
			// 
			this.MidiGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MidiGroupBox.Controls.Add(this.MidiRefreshDeviceListButton);
			this.MidiGroupBox.Controls.Add(this.MidiOutputComboBox);
			this.MidiGroupBox.Controls.Add(this.MidiOutputLabel);
			this.MidiGroupBox.Controls.Add(this.MidiInputComboBox);
			this.MidiGroupBox.Controls.Add(this.MidiInputLabel);
			this.MidiGroupBox.Location = new System.Drawing.Point(595, 1);
			this.MidiGroupBox.Name = "MidiGroupBox";
			this.MidiGroupBox.Size = new System.Drawing.Size(200, 100);
			this.MidiGroupBox.TabIndex = 2;
			this.MidiGroupBox.TabStop = false;
			this.MidiGroupBox.Text = "MIDI";
			// 
			// MidiRefreshDeviceListButton
			// 
			this.MidiRefreshDeviceListButton.Location = new System.Drawing.Point(41, 69);
			this.MidiRefreshDeviceListButton.Name = "MidiRefreshDeviceListButton";
			this.MidiRefreshDeviceListButton.Size = new System.Drawing.Size(151, 23);
			this.MidiRefreshDeviceListButton.TabIndex = 4;
			this.MidiRefreshDeviceListButton.Text = "Refresh Device List";
			this.MidiRefreshDeviceListButton.UseVisualStyleBackColor = true;
			this.MidiRefreshDeviceListButton.Click += new System.EventHandler(this.MidiRefreshDeviceListButton_Click);
			// 
			// MidiOutputComboBox
			// 
			this.MidiOutputComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.MidiOutputComboBox.FormattingEnabled = true;
			this.MidiOutputComboBox.Location = new System.Drawing.Point(42, 42);
			this.MidiOutputComboBox.Name = "MidiOutputComboBox";
			this.MidiOutputComboBox.Size = new System.Drawing.Size(149, 21);
			this.MidiOutputComboBox.TabIndex = 3;
			this.MidiOutputComboBox.SelectedIndexChanged += new System.EventHandler(this.MidiOutputComboBox_SelectedIndexChanged);
			// 
			// MidiOutputLabel
			// 
			this.MidiOutputLabel.AutoSize = true;
			this.MidiOutputLabel.Location = new System.Drawing.Point(7, 45);
			this.MidiOutputLabel.Name = "MidiOutputLabel";
			this.MidiOutputLabel.Size = new System.Drawing.Size(39, 13);
			this.MidiOutputLabel.TabIndex = 2;
			this.MidiOutputLabel.Text = "Output";
			// 
			// MidiInputComboBox
			// 
			this.MidiInputComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.MidiInputComboBox.FormattingEnabled = true;
			this.MidiInputComboBox.Location = new System.Drawing.Point(42, 17);
			this.MidiInputComboBox.Name = "MidiInputComboBox";
			this.MidiInputComboBox.Size = new System.Drawing.Size(149, 21);
			this.MidiInputComboBox.TabIndex = 1;
			// 
			// MidiInputLabel
			// 
			this.MidiInputLabel.AutoSize = true;
			this.MidiInputLabel.Location = new System.Drawing.Point(7, 20);
			this.MidiInputLabel.Name = "MidiInputLabel";
			this.MidiInputLabel.Size = new System.Drawing.Size(31, 13);
			this.MidiInputLabel.TabIndex = 0;
			this.MidiInputLabel.Text = "Input";
			// 
			// Visualizer
			// 
			this.Visualizer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Visualizer.ChannelColors = new System.Drawing.Color[] {
        System.Drawing.Color.LightGreen,
        System.Drawing.Color.LightGoldenrodYellow,
        System.Drawing.Color.LightBlue,
        System.Drawing.Color.LightCyan,
        System.Drawing.Color.LightPink,
        System.Drawing.Color.LightGray,
        System.Drawing.Color.Magenta,
        System.Drawing.Color.Orange,
        System.Drawing.Color.DarkGreen,
        System.Drawing.Color.Brown,
        System.Drawing.Color.DarkBlue,
        System.Drawing.Color.DarkCyan,
        System.Drawing.Color.HotPink,
        System.Drawing.Color.DarkGray,
        System.Drawing.Color.DarkMagenta,
        System.Drawing.Color.DarkOrange};
			this.Visualizer.Location = new System.Drawing.Point(9, 219);
			this.Visualizer.Name = "Visualizer";
			this.Visualizer.Sequence = null;
			this.Visualizer.Size = new System.Drawing.Size(786, 100);
			this.Visualizer.TabIndex = 3;
			// 
			// Piano
			// 
			this.Piano.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.Piano.BlackKeyColor = System.Drawing.Color.Black;
			this.Piano.BorderColor = System.Drawing.Color.Black;
			this.Piano.Location = new System.Drawing.Point(0, 0);
			this.Piano.Name = "Piano";
			this.Piano.NoteHighlightColor= System.Drawing.Color.Orange;
			this.Piano.Octaves = 12;
			this.Piano.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.Piano.Size = new System.Drawing.Size(1600, 120);
			this.Piano.TabIndex = 1;
			this.Piano.WhiteKeyColor = System.Drawing.Color.White;
			this.Piano.PianoKeyDown += new M.PianoKeyEventHandler(this.Piano_PianoKeyDown);
			this.Piano.PianoKeyUp += new M.PianoKeyEventHandler(this.Piano_PianoKeyUp);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.Visualizer);
			this.Controls.Add(this.MidiGroupBox);
			this.Controls.Add(this.KeysPanel);
			this.Name = "Main";
			this.Text = "MIDI Sequencer";
			this.KeysPanel.ResumeLayout(false);
			this.MidiGroupBox.ResumeLayout(false);
			this.MidiGroupBox.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel KeysPanel;
		private M.PianoBox Piano;
		private System.Windows.Forms.GroupBox MidiGroupBox;
		private System.Windows.Forms.Button MidiRefreshDeviceListButton;
		private System.Windows.Forms.ComboBox MidiOutputComboBox;
		private System.Windows.Forms.Label MidiOutputLabel;
		private System.Windows.Forms.ComboBox MidiInputComboBox;
		private System.Windows.Forms.Label MidiInputLabel;
		private M.MidiVisualizer Visualizer;
	}
}

