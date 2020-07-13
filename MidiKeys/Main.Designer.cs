namespace MidiKeys
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
			this.PianoPanel = new System.Windows.Forms.Panel();
			this.Piano = new M.PianoBox();
			this.MidiGroupBox = new System.Windows.Forms.GroupBox();
			this.MidiRefreshButton = new System.Windows.Forms.Button();
			this.MidiOutComboBox = new System.Windows.Forms.ComboBox();
			this.MidiOutLabel = new System.Windows.Forms.Label();
			this.MidiInComboBox = new System.Windows.Forms.ComboBox();
			this.MidiInLabel = new System.Windows.Forms.Label();
			this.ChannelLabel = new System.Windows.Forms.Label();
			this.ChannelUpDown = new System.Windows.Forms.NumericUpDown();
			this.PianoPanel.SuspendLayout();
			this.MidiGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ChannelUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// PianoPanel
			// 
			this.PianoPanel.AutoScroll = true;
			this.PianoPanel.Controls.Add(this.Piano);
			this.PianoPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.PianoPanel.Location = new System.Drawing.Point(0, 105);
			this.PianoPanel.Name = "PianoPanel";
			this.PianoPanel.Size = new System.Drawing.Size(429, 100);
			this.PianoPanel.TabIndex = 1;
			// 
			// Piano
			// 
			this.Piano.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.Piano.BlackKeyColor = System.Drawing.Color.Black;
			this.Piano.BorderColor = System.Drawing.Color.Black;
			this.Piano.HotKeys = new System.Windows.Forms.Keys[] {
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.Q,
        System.Windows.Forms.Keys.D2,
        System.Windows.Forms.Keys.W,
        System.Windows.Forms.Keys.D3,
        System.Windows.Forms.Keys.E,
        System.Windows.Forms.Keys.R,
        System.Windows.Forms.Keys.D5,
        System.Windows.Forms.Keys.T,
        System.Windows.Forms.Keys.D6,
        System.Windows.Forms.Keys.Y,
        System.Windows.Forms.Keys.D7,
        System.Windows.Forms.Keys.U,
        System.Windows.Forms.Keys.Z,
        System.Windows.Forms.Keys.S,
        System.Windows.Forms.Keys.X,
        System.Windows.Forms.Keys.D,
        System.Windows.Forms.Keys.C,
        System.Windows.Forms.Keys.V,
        System.Windows.Forms.Keys.G,
        System.Windows.Forms.Keys.B,
        System.Windows.Forms.Keys.H,
        System.Windows.Forms.Keys.N,
        System.Windows.Forms.Keys.J,
        System.Windows.Forms.Keys.M};
			this.Piano.Location = new System.Drawing.Point(0, 0);
			this.Piano.Name = "Piano";
			this.Piano.NoteHighlightColor = System.Drawing.Color.Orange;
			this.Piano.Octaves = 11;
			this.Piano.Size = new System.Drawing.Size(1200, 100);
			this.Piano.TabIndex = 1;
			this.Piano.Text = "Piano";
			this.Piano.WhiteKeyColor = System.Drawing.Color.White;
			this.Piano.PianoKeyDown += new M.PianoKeyEventHandler(this.Piano_PianoKeyDown);
			this.Piano.PianoKeyUp += new M.PianoKeyEventHandler(this.Piano_PianoKeyUp);
			// 
			// MidiGroupBox
			// 
			this.MidiGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MidiGroupBox.Controls.Add(this.MidiRefreshButton);
			this.MidiGroupBox.Controls.Add(this.MidiOutComboBox);
			this.MidiGroupBox.Controls.Add(this.MidiOutLabel);
			this.MidiGroupBox.Controls.Add(this.MidiInComboBox);
			this.MidiGroupBox.Controls.Add(this.MidiInLabel);
			this.MidiGroupBox.Location = new System.Drawing.Point(263, 2);
			this.MidiGroupBox.Name = "MidiGroupBox";
			this.MidiGroupBox.Size = new System.Drawing.Size(158, 97);
			this.MidiGroupBox.TabIndex = 2;
			this.MidiGroupBox.TabStop = false;
			this.MidiGroupBox.Text = "MIDI";
			// 
			// MidiRefreshButton
			// 
			this.MidiRefreshButton.Location = new System.Drawing.Point(76, 67);
			this.MidiRefreshButton.Name = "MidiRefreshButton";
			this.MidiRefreshButton.Size = new System.Drawing.Size(75, 23);
			this.MidiRefreshButton.TabIndex = 4;
			this.MidiRefreshButton.Text = "Refresh";
			this.MidiRefreshButton.UseVisualStyleBackColor = true;
			this.MidiRefreshButton.Click += new System.EventHandler(this.MidiRefreshButton_Click);
			// 
			// MidiOutComboBox
			// 
			this.MidiOutComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.MidiOutComboBox.FormattingEnabled = true;
			this.MidiOutComboBox.Location = new System.Drawing.Point(30, 42);
			this.MidiOutComboBox.Name = "MidiOutComboBox";
			this.MidiOutComboBox.Size = new System.Drawing.Size(121, 21);
			this.MidiOutComboBox.TabIndex = 3;
			this.MidiOutComboBox.SelectedIndexChanged += new System.EventHandler(this.MidiOutComboBox_SelectedIndexChanged);
			// 
			// MidiOutLabel
			// 
			this.MidiOutLabel.AutoSize = true;
			this.MidiOutLabel.Location = new System.Drawing.Point(7, 45);
			this.MidiOutLabel.Name = "MidiOutLabel";
			this.MidiOutLabel.Size = new System.Drawing.Size(24, 13);
			this.MidiOutLabel.TabIndex = 2;
			this.MidiOutLabel.Text = "Out";
			// 
			// MidiInComboBox
			// 
			this.MidiInComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.MidiInComboBox.FormattingEnabled = true;
			this.MidiInComboBox.Location = new System.Drawing.Point(30, 17);
			this.MidiInComboBox.Name = "MidiInComboBox";
			this.MidiInComboBox.Size = new System.Drawing.Size(121, 21);
			this.MidiInComboBox.TabIndex = 1;
			this.MidiInComboBox.SelectedIndexChanged += new System.EventHandler(this.MidiInComboBox_SelectedIndexChanged);
			// 
			// MidiInLabel
			// 
			this.MidiInLabel.AutoSize = true;
			this.MidiInLabel.Location = new System.Drawing.Point(7, 20);
			this.MidiInLabel.Name = "MidiInLabel";
			this.MidiInLabel.Size = new System.Drawing.Size(16, 13);
			this.MidiInLabel.TabIndex = 0;
			this.MidiInLabel.Text = "In";
			// 
			// ChannelLabel
			// 
			this.ChannelLabel.AutoSize = true;
			this.ChannelLabel.Location = new System.Drawing.Point(4, 86);
			this.ChannelLabel.Name = "ChannelLabel";
			this.ChannelLabel.Size = new System.Drawing.Size(46, 13);
			this.ChannelLabel.TabIndex = 3;
			this.ChannelLabel.Text = "Channel";
			// 
			// ChannelUpDown
			// 
			this.ChannelUpDown.Location = new System.Drawing.Point(49, 83);
			this.ChannelUpDown.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.ChannelUpDown.Name = "ChannelUpDown";
			this.ChannelUpDown.Size = new System.Drawing.Size(34, 20);
			this.ChannelUpDown.TabIndex = 4;
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(429, 205);
			this.Controls.Add(this.ChannelUpDown);
			this.Controls.Add(this.ChannelLabel);
			this.Controls.Add(this.MidiGroupBox);
			this.Controls.Add(this.PianoPanel);
			this.MinimumSize = new System.Drawing.Size(445, 244);
			this.Name = "Main";
			this.Text = "MIDI Keyboard";
			this.PianoPanel.ResumeLayout(false);
			this.MidiGroupBox.ResumeLayout(false);
			this.MidiGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ChannelUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel PianoPanel;
		private M.PianoBox Piano;
		private System.Windows.Forms.GroupBox MidiGroupBox;
		private System.Windows.Forms.Button MidiRefreshButton;
		private System.Windows.Forms.ComboBox MidiOutComboBox;
		private System.Windows.Forms.Label MidiOutLabel;
		private System.Windows.Forms.ComboBox MidiInComboBox;
		private System.Windows.Forms.Label MidiInLabel;
		private System.Windows.Forms.Label ChannelLabel;
		private System.Windows.Forms.NumericUpDown ChannelUpDown;
	}
}

