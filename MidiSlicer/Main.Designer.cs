namespace MidiSlicer
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
			this.TrackList = new System.Windows.Forms.CheckedListBox();
			this.TracksLabel = new System.Windows.Forms.Label();
			this.MidiFileBox = new System.Windows.Forms.TextBox();
			this.FileLabel = new System.Windows.Forms.Label();
			this.BrowseButton = new System.Windows.Forms.Button();
			this.OpenMidiFile = new System.Windows.Forms.OpenFileDialog();
			this.PreviewButton = new System.Windows.Forms.Button();
			this.UnitsLabel = new System.Windows.Forms.Label();
			this.UnitsCombo = new System.Windows.Forms.ComboBox();
			this.OffsetUpDown = new System.Windows.Forms.NumericUpDown();
			this.OffsetLabel = new System.Windows.Forms.Label();
			this.LengthLabel = new System.Windows.Forms.Label();
			this.LengthUpDown = new System.Windows.Forms.NumericUpDown();
			this.SaveAsButton = new System.Windows.Forms.Button();
			this.SaveMidiFile = new System.Windows.Forms.SaveFileDialog();
			this.MergeTracksCheckBox = new System.Windows.Forms.CheckBox();
			this.StretchLabel = new System.Windows.Forms.Label();
			this.StretchUpDown = new System.Windows.Forms.NumericUpDown();
			this.AdjustTempoCheckBox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.OffsetUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.LengthUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.StretchUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// TrackList
			// 
			this.TrackList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TrackList.Enabled = false;
			this.TrackList.FormattingEnabled = true;
			this.TrackList.Location = new System.Drawing.Point(1, 52);
			this.TrackList.Name = "TrackList";
			this.TrackList.Size = new System.Drawing.Size(79, 94);
			this.TrackList.TabIndex = 0;
			// 
			// TracksLabel
			// 
			this.TracksLabel.AutoSize = true;
			this.TracksLabel.Location = new System.Drawing.Point(1, 30);
			this.TracksLabel.Name = "TracksLabel";
			this.TracksLabel.Size = new System.Drawing.Size(225, 13);
			this.TracksLabel.TabIndex = 1;
			this.TracksLabel.Text = "Tracks:    Tempo {0}     Time signature {1}/{2}";
			// 
			// MidiFileBox
			// 
			this.MidiFileBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MidiFileBox.Location = new System.Drawing.Point(30, 2);
			this.MidiFileBox.Name = "MidiFileBox";
			this.MidiFileBox.Size = new System.Drawing.Size(251, 20);
			this.MidiFileBox.TabIndex = 2;
			this.MidiFileBox.Leave += new System.EventHandler(this.MidiFileBox_Leave);
			// 
			// FileLabel
			// 
			this.FileLabel.AutoSize = true;
			this.FileLabel.Location = new System.Drawing.Point(1, 5);
			this.FileLabel.Name = "FileLabel";
			this.FileLabel.Size = new System.Drawing.Size(23, 13);
			this.FileLabel.TabIndex = 3;
			this.FileLabel.Text = "File";
			// 
			// BrowseButton
			// 
			this.BrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BrowseButton.Location = new System.Drawing.Point(287, 1);
			this.BrowseButton.Name = "BrowseButton";
			this.BrowseButton.Size = new System.Drawing.Size(25, 22);
			this.BrowseButton.TabIndex = 4;
			this.BrowseButton.Text = "...";
			this.BrowseButton.UseVisualStyleBackColor = true;
			this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
			// 
			// PreviewButton
			// 
			this.PreviewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.PreviewButton.Location = new System.Drawing.Point(237, 125);
			this.PreviewButton.Name = "PreviewButton";
			this.PreviewButton.Size = new System.Drawing.Size(75, 23);
			this.PreviewButton.TabIndex = 5;
			this.PreviewButton.Text = "Preview";
			this.PreviewButton.UseVisualStyleBackColor = true;
			this.PreviewButton.Click += new System.EventHandler(this.PreviewButton_Click);
			// 
			// UnitsLabel
			// 
			this.UnitsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.UnitsLabel.AutoSize = true;
			this.UnitsLabel.Location = new System.Drawing.Point(86, 55);
			this.UnitsLabel.Name = "UnitsLabel";
			this.UnitsLabel.Size = new System.Drawing.Size(31, 13);
			this.UnitsLabel.TabIndex = 9;
			this.UnitsLabel.Text = "Units";
			// 
			// UnitsCombo
			// 
			this.UnitsCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.UnitsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.UnitsCombo.FormattingEnabled = true;
			this.UnitsCombo.Items.AddRange(new object[] {
            "Beats",
            "Ticks"});
			this.UnitsCombo.Location = new System.Drawing.Point(121, 52);
			this.UnitsCombo.Name = "UnitsCombo";
			this.UnitsCombo.Size = new System.Drawing.Size(75, 21);
			this.UnitsCombo.TabIndex = 8;
			this.UnitsCombo.SelectedIndexChanged += new System.EventHandler(this.UnitsCombo_SelectedIndexChanged);
			// 
			// OffsetUpDown
			// 
			this.OffsetUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.OffsetUpDown.Location = new System.Drawing.Point(121, 79);
			this.OffsetUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.OffsetUpDown.Name = "OffsetUpDown";
			this.OffsetUpDown.Size = new System.Drawing.Size(75, 20);
			this.OffsetUpDown.TabIndex = 10;
			// 
			// OffsetLabel
			// 
			this.OffsetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.OffsetLabel.AutoSize = true;
			this.OffsetLabel.Location = new System.Drawing.Point(86, 82);
			this.OffsetLabel.Name = "OffsetLabel";
			this.OffsetLabel.Size = new System.Drawing.Size(35, 13);
			this.OffsetLabel.TabIndex = 11;
			this.OffsetLabel.Text = "Offset";
			// 
			// LengthLabel
			// 
			this.LengthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LengthLabel.AutoSize = true;
			this.LengthLabel.Location = new System.Drawing.Point(197, 82);
			this.LengthLabel.Name = "LengthLabel";
			this.LengthLabel.Size = new System.Drawing.Size(40, 13);
			this.LengthLabel.TabIndex = 13;
			this.LengthLabel.Text = "Length";
			// 
			// LengthUpDown
			// 
			this.LengthUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LengthUpDown.Location = new System.Drawing.Point(237, 79);
			this.LengthUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.LengthUpDown.Name = "LengthUpDown";
			this.LengthUpDown.Size = new System.Drawing.Size(75, 20);
			this.LengthUpDown.TabIndex = 12;
			// 
			// SaveAsButton
			// 
			this.SaveAsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.SaveAsButton.Location = new System.Drawing.Point(156, 125);
			this.SaveAsButton.Name = "SaveAsButton";
			this.SaveAsButton.Size = new System.Drawing.Size(75, 23);
			this.SaveAsButton.TabIndex = 14;
			this.SaveAsButton.Text = "Save As...";
			this.SaveAsButton.UseVisualStyleBackColor = true;
			this.SaveAsButton.Click += new System.EventHandler(this.SaveAsButton_Click);
			// 
			// MergeTracksCheckBox
			// 
			this.MergeTracksCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MergeTracksCheckBox.AutoSize = true;
			this.MergeTracksCheckBox.Location = new System.Drawing.Point(202, 55);
			this.MergeTracksCheckBox.Name = "MergeTracksCheckBox";
			this.MergeTracksCheckBox.Size = new System.Drawing.Size(92, 17);
			this.MergeTracksCheckBox.TabIndex = 15;
			this.MergeTracksCheckBox.Text = "Merge Tracks";
			this.MergeTracksCheckBox.UseVisualStyleBackColor = true;
			// 
			// StretchLabel
			// 
			this.StretchLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.StretchLabel.AutoSize = true;
			this.StretchLabel.Location = new System.Drawing.Point(81, 106);
			this.StretchLabel.Name = "StretchLabel";
			this.StretchLabel.Size = new System.Drawing.Size(41, 13);
			this.StretchLabel.TabIndex = 19;
			this.StretchLabel.Text = "Stretch";
			// 
			// StretchUpDown
			// 
			this.StretchUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.StretchUpDown.DecimalPlaces = 2;
			this.StretchUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.StretchUpDown.Location = new System.Drawing.Point(121, 103);
			this.StretchUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.StretchUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.StretchUpDown.Name = "StretchUpDown";
			this.StretchUpDown.Size = new System.Drawing.Size(75, 20);
			this.StretchUpDown.TabIndex = 18;
			this.StretchUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// AdjustTempoCheckBox
			// 
			this.AdjustTempoCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AdjustTempoCheckBox.AutoSize = true;
			this.AdjustTempoCheckBox.Location = new System.Drawing.Point(200, 105);
			this.AdjustTempoCheckBox.Name = "AdjustTempoCheckBox";
			this.AdjustTempoCheckBox.Size = new System.Drawing.Size(91, 17);
			this.AdjustTempoCheckBox.TabIndex = 20;
			this.AdjustTempoCheckBox.Text = "Adjust Tempo";
			this.AdjustTempoCheckBox.UseVisualStyleBackColor = true;
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(320, 150);
			this.Controls.Add(this.AdjustTempoCheckBox);
			this.Controls.Add(this.StretchUpDown);
			this.Controls.Add(this.MergeTracksCheckBox);
			this.Controls.Add(this.SaveAsButton);
			this.Controls.Add(this.LengthLabel);
			this.Controls.Add(this.LengthUpDown);
			this.Controls.Add(this.OffsetLabel);
			this.Controls.Add(this.OffsetUpDown);
			this.Controls.Add(this.UnitsLabel);
			this.Controls.Add(this.UnitsCombo);
			this.Controls.Add(this.PreviewButton);
			this.Controls.Add(this.BrowseButton);
			this.Controls.Add(this.FileLabel);
			this.Controls.Add(this.MidiFileBox);
			this.Controls.Add(this.TracksLabel);
			this.Controls.Add(this.TrackList);
			this.Controls.Add(this.StretchLabel);
			this.MinimumSize = new System.Drawing.Size(336, 189);
			this.Name = "Main";
			this.Text = "Midi Slicer";
			((System.ComponentModel.ISupportInitialize)(this.OffsetUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.LengthUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.StretchUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckedListBox TrackList;
		private System.Windows.Forms.Label TracksLabel;
		private System.Windows.Forms.TextBox MidiFileBox;
		private System.Windows.Forms.Label FileLabel;
		private System.Windows.Forms.Button BrowseButton;
		private System.Windows.Forms.OpenFileDialog OpenMidiFile;
		private System.Windows.Forms.Button PreviewButton;
		private System.Windows.Forms.Label UnitsLabel;
		private System.Windows.Forms.ComboBox UnitsCombo;
		private System.Windows.Forms.NumericUpDown OffsetUpDown;
		private System.Windows.Forms.Label OffsetLabel;
		private System.Windows.Forms.Label LengthLabel;
		private System.Windows.Forms.NumericUpDown LengthUpDown;
		private System.Windows.Forms.Button SaveAsButton;
		private System.Windows.Forms.SaveFileDialog SaveMidiFile;
		private System.Windows.Forms.CheckBox MergeTracksCheckBox;
		private System.Windows.Forms.Label StretchLabel;
		private System.Windows.Forms.NumericUpDown StretchUpDown;
		private System.Windows.Forms.CheckBox AdjustTempoCheckBox;
	}
}