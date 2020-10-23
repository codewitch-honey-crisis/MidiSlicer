namespace MidiDisplay
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
			this.components = new System.ComponentModel.Container();
			this.MidiGroupBox = new System.Windows.Forms.GroupBox();
			this.MidiRefreshButton = new System.Windows.Forms.Button();
			this.MidiOutComboBox = new System.Windows.Forms.ComboBox();
			this.MidiOutLabel = new System.Windows.Forms.Label();
			this.VisualizerPanel = new System.Windows.Forms.Panel();
			this.MidiOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.FileTextBox = new System.Windows.Forms.TextBox();
			this.FileLabel = new System.Windows.Forms.Label();
			this.FileBrowseButton = new System.Windows.Forms.Button();
			this.FileTimer = new System.Windows.Forms.Timer(this.components);
			this.PlayButton = new System.Windows.Forms.Button();
			this.VolumeLabel = new System.Windows.Forms.Label();
			this.VolumeKnob = new M.Knob();
			this.Visualizer = new M.MidiVisualizer();
			this.MidiGroupBox.SuspendLayout();
			this.VisualizerPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// MidiGroupBox
			// 
			this.MidiGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MidiGroupBox.Controls.Add(this.MidiRefreshButton);
			this.MidiGroupBox.Controls.Add(this.MidiOutComboBox);
			this.MidiGroupBox.Controls.Add(this.MidiOutLabel);
			this.MidiGroupBox.Location = new System.Drawing.Point(636, 5);
			this.MidiGroupBox.Name = "MidiGroupBox";
			this.MidiGroupBox.Size = new System.Drawing.Size(158, 75);
			this.MidiGroupBox.TabIndex = 3;
			this.MidiGroupBox.TabStop = false;
			this.MidiGroupBox.Text = "MIDI";
			// 
			// MidiRefreshButton
			// 
			this.MidiRefreshButton.Location = new System.Drawing.Point(76, 43);
			this.MidiRefreshButton.Name = "MidiRefreshButton";
			this.MidiRefreshButton.Size = new System.Drawing.Size(75, 23);
			this.MidiRefreshButton.TabIndex = 4;
			this.MidiRefreshButton.Text = "Refresh";
			this.MidiRefreshButton.UseVisualStyleBackColor = true;
			// 
			// MidiOutComboBox
			// 
			this.MidiOutComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.MidiOutComboBox.FormattingEnabled = true;
			this.MidiOutComboBox.Location = new System.Drawing.Point(30, 18);
			this.MidiOutComboBox.Name = "MidiOutComboBox";
			this.MidiOutComboBox.Size = new System.Drawing.Size(121, 21);
			this.MidiOutComboBox.TabIndex = 3;
			this.MidiOutComboBox.SelectedIndexChanged += new System.EventHandler(this.MidiOutComboBox_SelectedIndexChanged);
			// 
			// MidiOutLabel
			// 
			this.MidiOutLabel.AutoSize = true;
			this.MidiOutLabel.Location = new System.Drawing.Point(7, 21);
			this.MidiOutLabel.Name = "MidiOutLabel";
			this.MidiOutLabel.Size = new System.Drawing.Size(24, 13);
			this.MidiOutLabel.TabIndex = 2;
			this.MidiOutLabel.Text = "Out";
			// 
			// VisualizerPanel
			// 
			this.VisualizerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.VisualizerPanel.AutoScroll = true;
			this.VisualizerPanel.BackColor = System.Drawing.Color.Black;
			this.VisualizerPanel.Controls.Add(this.Visualizer);
			this.VisualizerPanel.Location = new System.Drawing.Point(5, 84);
			this.VisualizerPanel.Name = "VisualizerPanel";
			this.VisualizerPanel.Size = new System.Drawing.Size(789, 100);
			this.VisualizerPanel.TabIndex = 4;
			// 
			// MidiOpenFileDialog
			// 
			this.MidiOpenFileDialog.DefaultExt = "mid";
			this.MidiOpenFileDialog.Filter = "MIDI Files|*.mid|All Files|*.*";
			// 
			// FileTextBox
			// 
			this.FileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.FileTextBox.ForeColor = System.Drawing.Color.Red;
			this.FileTextBox.Location = new System.Drawing.Point(28, 5);
			this.FileTextBox.Name = "FileTextBox";
			this.FileTextBox.Size = new System.Drawing.Size(572, 20);
			this.FileTextBox.TabIndex = 5;
			this.FileTextBox.TextChanged += new System.EventHandler(this.FileTextBox_TextChanged);
			// 
			// FileLabel
			// 
			this.FileLabel.AutoSize = true;
			this.FileLabel.Location = new System.Drawing.Point(2, 8);
			this.FileLabel.Name = "FileLabel";
			this.FileLabel.Size = new System.Drawing.Size(23, 13);
			this.FileLabel.TabIndex = 6;
			this.FileLabel.Text = "File";
			// 
			// FileBrowseButton
			// 
			this.FileBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.FileBrowseButton.Location = new System.Drawing.Point(604, 4);
			this.FileBrowseButton.Name = "FileBrowseButton";
			this.FileBrowseButton.Size = new System.Drawing.Size(24, 22);
			this.FileBrowseButton.TabIndex = 7;
			this.FileBrowseButton.Text = "...";
			this.FileBrowseButton.UseVisualStyleBackColor = true;
			this.FileBrowseButton.Click += new System.EventHandler(this.FileBrowseButton_Click);
			// 
			// FileTimer
			// 
			this.FileTimer.Enabled = true;
			this.FileTimer.Interval = 50;
			this.FileTimer.Tick += new System.EventHandler(this.FileTimer_Tick);
			// 
			// PlayButton
			// 
			this.PlayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PlayButton.Location = new System.Drawing.Point(567, 31);
			this.PlayButton.Name = "PlayButton";
			this.PlayButton.Size = new System.Drawing.Size(61, 23);
			this.PlayButton.TabIndex = 8;
			this.PlayButton.Text = "Play";
			this.PlayButton.UseVisualStyleBackColor = true;
			this.PlayButton.Click += new System.EventHandler(this.PlayButton_Click);
			// 
			// VolumeLabel
			// 
			this.VolumeLabel.AutoSize = true;
			this.VolumeLabel.Location = new System.Drawing.Point(492, 37);
			this.VolumeLabel.Name = "VolumeLabel";
			this.VolumeLabel.Size = new System.Drawing.Size(42, 13);
			this.VolumeLabel.TabIndex = 11;
			this.VolumeLabel.Text = "Volume";
			// 
			// VolumeKnob
			// 
			this.VolumeKnob.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
			this.VolumeKnob.BorderWidth = 2;
			this.VolumeKnob.HasTicks = true;
			this.VolumeKnob.KnobColor = System.Drawing.SystemColors.Control;
			this.VolumeKnob.LargeChange = 32;
			this.VolumeKnob.Location = new System.Drawing.Point(530, 29);
			this.VolumeKnob.Maximum = 255;
			this.VolumeKnob.Minimum = 0;
			this.VolumeKnob.Name = "VolumeKnob";
			this.VolumeKnob.PointerColor = System.Drawing.SystemColors.ControlText;
			this.VolumeKnob.PointerWidth = 2;
			this.VolumeKnob.Size = new System.Drawing.Size(31, 31);
			this.VolumeKnob.TabIndex = 10;
			this.VolumeKnob.TickColor = System.Drawing.Color.Black;
			this.VolumeKnob.TickWidth = 2;
			this.VolumeKnob.Value = 255;
			this.VolumeKnob.ValueChanged += new System.EventHandler(this.VolumeKnob_ValueChanged);
			// 
			// Visualizer
			// 
			this.Visualizer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.Visualizer.BackColor = System.Drawing.Color.Black;
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
			this.Visualizer.CursorColor = System.Drawing.Color.DarkGoldenrod;
			this.Visualizer.Location = new System.Drawing.Point(0, 0);
			this.Visualizer.Name = "Visualizer";
			this.Visualizer.Sequence = null;
			this.Visualizer.Size = new System.Drawing.Size(75, 100);
			this.Visualizer.TabIndex = 0;
			this.Visualizer.Text = "Visualizer";
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 190);
			this.Controls.Add(this.VolumeKnob);
			this.Controls.Add(this.PlayButton);
			this.Controls.Add(this.FileBrowseButton);
			this.Controls.Add(this.FileLabel);
			this.Controls.Add(this.FileTextBox);
			this.Controls.Add(this.VisualizerPanel);
			this.Controls.Add(this.MidiGroupBox);
			this.Controls.Add(this.VolumeLabel);
			this.Name = "Main";
			this.Text = "MIDI Display";
			this.MidiGroupBox.ResumeLayout(false);
			this.MidiGroupBox.PerformLayout();
			this.VisualizerPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox MidiGroupBox;
		private System.Windows.Forms.Button MidiRefreshButton;
		private System.Windows.Forms.ComboBox MidiOutComboBox;
		private System.Windows.Forms.Label MidiOutLabel;
		private System.Windows.Forms.Panel VisualizerPanel;
		private M.MidiVisualizer Visualizer;
		private System.Windows.Forms.OpenFileDialog MidiOpenFileDialog;
		private System.Windows.Forms.TextBox FileTextBox;
		private System.Windows.Forms.Label FileLabel;
		private System.Windows.Forms.Button FileBrowseButton;
		private System.Windows.Forms.Timer FileTimer;
		private System.Windows.Forms.Button PlayButton;
		private M.Knob VolumeKnob;
		private System.Windows.Forms.Label VolumeLabel;
	}
}

