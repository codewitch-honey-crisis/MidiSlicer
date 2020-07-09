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
			this.StartLabel = new System.Windows.Forms.Label();
			this.StartCombo = new System.Windows.Forms.ComboBox();
			this.CopyTimingPatchCheckBox = new System.Windows.Forms.CheckBox();
			this.ResampleLabel = new System.Windows.Forms.Label();
			this.ResampleUpDown = new System.Windows.Forms.NumericUpDown();
			this.NormalizeCheckBox = new System.Windows.Forms.CheckBox();
			this.LevelsLabel = new System.Windows.Forms.Label();
			this.LevelsUpDown = new System.Windows.Forms.NumericUpDown();
			this.TransposeUpDown = new System.Windows.Forms.NumericUpDown();
			this.TransposeLabel = new System.Windows.Forms.Label();
			this.WrapCheckBox = new System.Windows.Forms.CheckBox();
			this.DrumsCheckBox = new System.Windows.Forms.CheckBox();
			this.OutputComboBox = new System.Windows.Forms.ComboBox();
			this.TempoLabel = new System.Windows.Forms.Label();
			this.TempoUpDown = new System.Windows.Forms.NumericUpDown();
			this.VisualizerPanel = new System.Windows.Forms.Panel();
			this.Visualizer = new M.MidiVisualizer();
			((System.ComponentModel.ISupportInitialize)(this.OffsetUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.LengthUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.StretchUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ResampleUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.LevelsUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.TransposeUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.TempoUpDown)).BeginInit();
			this.VisualizerPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// TrackList
			// 
			this.TrackList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TrackList.CheckOnClick = true;
			this.TrackList.Enabled = false;
			this.TrackList.FormattingEnabled = true;
			this.TrackList.Location = new System.Drawing.Point(1, 52);
			this.TrackList.Name = "TrackList";
			this.TrackList.Size = new System.Drawing.Size(79, 184);
			this.TrackList.TabIndex = 0;
			this.TrackList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.TrackList_ItemCheck);
			// 
			// TracksLabel
			// 
			this.TracksLabel.AutoSize = true;
			this.TracksLabel.Location = new System.Drawing.Point(1, 30);
			this.TracksLabel.Name = "TracksLabel";
			this.TracksLabel.Size = new System.Drawing.Size(138, 13);
			this.TracksLabel.TabIndex = 1;
			this.TracksLabel.Text = "Tracks:    {0}/{1} time in {2}";
			// 
			// MidiFileBox
			// 
			this.MidiFileBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MidiFileBox.Location = new System.Drawing.Point(30, 2);
			this.MidiFileBox.Name = "MidiFileBox";
			this.MidiFileBox.Size = new System.Drawing.Size(147, 20);
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
			this.BrowseButton.Location = new System.Drawing.Point(179, 1);
			this.BrowseButton.Name = "BrowseButton";
			this.BrowseButton.Size = new System.Drawing.Size(25, 22);
			this.BrowseButton.TabIndex = 4;
			this.BrowseButton.Text = "...";
			this.BrowseButton.UseVisualStyleBackColor = true;
			this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
			// 
			// OpenMidiFile
			// 
			this.OpenMidiFile.DefaultExt = "mid";
			this.OpenMidiFile.Filter = "MIDI files|*.mid|All files|*.*";
			// 
			// PreviewButton
			// 
			this.PreviewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.PreviewButton.Location = new System.Drawing.Point(237, 355);
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
			this.OffsetUpDown.DecimalPlaces = 2;
			this.OffsetUpDown.Location = new System.Drawing.Point(121, 105);
			this.OffsetUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.OffsetUpDown.Name = "OffsetUpDown";
			this.OffsetUpDown.Size = new System.Drawing.Size(75, 20);
			this.OffsetUpDown.TabIndex = 10;
			this.OffsetUpDown.ValueChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// OffsetLabel
			// 
			this.OffsetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.OffsetLabel.AutoSize = true;
			this.OffsetLabel.Location = new System.Drawing.Point(86, 108);
			this.OffsetLabel.Name = "OffsetLabel";
			this.OffsetLabel.Size = new System.Drawing.Size(35, 13);
			this.OffsetLabel.TabIndex = 11;
			this.OffsetLabel.Text = "Offset";
			// 
			// LengthLabel
			// 
			this.LengthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LengthLabel.AutoSize = true;
			this.LengthLabel.Location = new System.Drawing.Point(197, 108);
			this.LengthLabel.Name = "LengthLabel";
			this.LengthLabel.Size = new System.Drawing.Size(40, 13);
			this.LengthLabel.TabIndex = 13;
			this.LengthLabel.Text = "Length";
			// 
			// LengthUpDown
			// 
			this.LengthUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LengthUpDown.DecimalPlaces = 2;
			this.LengthUpDown.Location = new System.Drawing.Point(237, 105);
			this.LengthUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.LengthUpDown.Name = "LengthUpDown";
			this.LengthUpDown.Size = new System.Drawing.Size(75, 20);
			this.LengthUpDown.TabIndex = 12;
			this.LengthUpDown.ValueChanged += new System.EventHandler(this.LengthUpDown_ValueChanged);
			// 
			// SaveAsButton
			// 
			this.SaveAsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.SaveAsButton.Location = new System.Drawing.Point(156, 355);
			this.SaveAsButton.Name = "SaveAsButton";
			this.SaveAsButton.Size = new System.Drawing.Size(75, 23);
			this.SaveAsButton.TabIndex = 14;
			this.SaveAsButton.Text = "Save As...";
			this.SaveAsButton.UseVisualStyleBackColor = true;
			this.SaveAsButton.Click += new System.EventHandler(this.SaveAsButton_Click);
			// 
			// SaveMidiFile
			// 
			this.SaveMidiFile.DefaultExt = "mid";
			this.SaveMidiFile.Filter = "MIDI files|*.mid|All files|*.*";
			// 
			// MergeTracksCheckBox
			// 
			this.MergeTracksCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MergeTracksCheckBox.AutoSize = true;
			this.MergeTracksCheckBox.Location = new System.Drawing.Point(200, 55);
			this.MergeTracksCheckBox.Name = "MergeTracksCheckBox";
			this.MergeTracksCheckBox.Size = new System.Drawing.Size(92, 17);
			this.MergeTracksCheckBox.TabIndex = 15;
			this.MergeTracksCheckBox.Text = "Merge Tracks";
			this.MergeTracksCheckBox.UseVisualStyleBackColor = true;
			this.MergeTracksCheckBox.CheckedChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// StretchLabel
			// 
			this.StretchLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.StretchLabel.AutoSize = true;
			this.StretchLabel.Location = new System.Drawing.Point(81, 132);
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
			this.StretchUpDown.Location = new System.Drawing.Point(121, 129);
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
			this.StretchUpDown.ValueChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// AdjustTempoCheckBox
			// 
			this.AdjustTempoCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AdjustTempoCheckBox.AutoSize = true;
			this.AdjustTempoCheckBox.Location = new System.Drawing.Point(203, 131);
			this.AdjustTempoCheckBox.Name = "AdjustTempoCheckBox";
			this.AdjustTempoCheckBox.Size = new System.Drawing.Size(91, 17);
			this.AdjustTempoCheckBox.TabIndex = 20;
			this.AdjustTempoCheckBox.Text = "Adjust Tempo";
			this.AdjustTempoCheckBox.UseVisualStyleBackColor = true;
			this.AdjustTempoCheckBox.CheckedChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// StartLabel
			// 
			this.StartLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.StartLabel.AutoSize = true;
			this.StartLabel.Location = new System.Drawing.Point(86, 81);
			this.StartLabel.Name = "StartLabel";
			this.StartLabel.Size = new System.Drawing.Size(29, 13);
			this.StartLabel.TabIndex = 22;
			this.StartLabel.Text = "Start";
			// 
			// StartCombo
			// 
			this.StartCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.StartCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.StartCombo.FormattingEnabled = true;
			this.StartCombo.Items.AddRange(new object[] {
            "Beginning",
            "First Downbeat",
            "First Note"});
			this.StartCombo.Location = new System.Drawing.Point(121, 78);
			this.StartCombo.Name = "StartCombo";
			this.StartCombo.Size = new System.Drawing.Size(75, 21);
			this.StartCombo.TabIndex = 21;
			this.StartCombo.SelectedIndexChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// CopyTimingPatchCheckBox
			// 
			this.CopyTimingPatchCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CopyTimingPatchCheckBox.AutoSize = true;
			this.CopyTimingPatchCheckBox.Checked = true;
			this.CopyTimingPatchCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CopyTimingPatchCheckBox.Location = new System.Drawing.Point(202, 77);
			this.CopyTimingPatchCheckBox.Name = "CopyTimingPatchCheckBox";
			this.CopyTimingPatchCheckBox.Size = new System.Drawing.Size(117, 17);
			this.CopyTimingPatchCheckBox.TabIndex = 23;
			this.CopyTimingPatchCheckBox.Text = "Copy Timing/Patch";
			this.CopyTimingPatchCheckBox.UseVisualStyleBackColor = true;
			this.CopyTimingPatchCheckBox.CheckedChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// ResampleLabel
			// 
			this.ResampleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ResampleLabel.AutoSize = true;
			this.ResampleLabel.Location = new System.Drawing.Point(84, 157);
			this.ResampleLabel.Name = "ResampleLabel";
			this.ResampleLabel.Size = new System.Drawing.Size(54, 13);
			this.ResampleLabel.TabIndex = 25;
			this.ResampleLabel.Text = "Resample";
			// 
			// ResampleUpDown
			// 
			this.ResampleUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ResampleUpDown.Location = new System.Drawing.Point(142, 154);
			this.ResampleUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.ResampleUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.ResampleUpDown.Name = "ResampleUpDown";
			this.ResampleUpDown.Size = new System.Drawing.Size(54, 20);
			this.ResampleUpDown.TabIndex = 24;
			this.ResampleUpDown.Value = new decimal(new int[] {
            480,
            0,
            0,
            0});
			this.ResampleUpDown.ValueChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// NormalizeCheckBox
			// 
			this.NormalizeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.NormalizeCheckBox.AutoSize = true;
			this.NormalizeCheckBox.Location = new System.Drawing.Point(205, 181);
			this.NormalizeCheckBox.Name = "NormalizeCheckBox";
			this.NormalizeCheckBox.Size = new System.Drawing.Size(72, 17);
			this.NormalizeCheckBox.TabIndex = 26;
			this.NormalizeCheckBox.Text = "Normalize";
			this.NormalizeCheckBox.UseVisualStyleBackColor = true;
			this.NormalizeCheckBox.CheckedChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// LevelsLabel
			// 
			this.LevelsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LevelsLabel.AutoSize = true;
			this.LevelsLabel.Location = new System.Drawing.Point(86, 182);
			this.LevelsLabel.Name = "LevelsLabel";
			this.LevelsLabel.Size = new System.Drawing.Size(38, 13);
			this.LevelsLabel.TabIndex = 27;
			this.LevelsLabel.Text = "Levels";
			// 
			// LevelsUpDown
			// 
			this.LevelsUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LevelsUpDown.DecimalPlaces = 2;
			this.LevelsUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.LevelsUpDown.Location = new System.Drawing.Point(130, 180);
			this.LevelsUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.LevelsUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.LevelsUpDown.Name = "LevelsUpDown";
			this.LevelsUpDown.Size = new System.Drawing.Size(66, 20);
			this.LevelsUpDown.TabIndex = 28;
			this.LevelsUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.LevelsUpDown.ValueChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// TransposeUpDown
			// 
			this.TransposeUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TransposeUpDown.Location = new System.Drawing.Point(142, 206);
			this.TransposeUpDown.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
			this.TransposeUpDown.Minimum = new decimal(new int[] {
            127,
            0,
            0,
            -2147483648});
			this.TransposeUpDown.Name = "TransposeUpDown";
			this.TransposeUpDown.Size = new System.Drawing.Size(54, 20);
			this.TransposeUpDown.TabIndex = 30;
			this.TransposeUpDown.ValueChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// TransposeLabel
			// 
			this.TransposeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TransposeLabel.AutoSize = true;
			this.TransposeLabel.Location = new System.Drawing.Point(86, 208);
			this.TransposeLabel.Name = "TransposeLabel";
			this.TransposeLabel.Size = new System.Drawing.Size(57, 13);
			this.TransposeLabel.TabIndex = 29;
			this.TransposeLabel.Text = "Transpose";
			// 
			// WrapCheckBox
			// 
			this.WrapCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.WrapCheckBox.AutoSize = true;
			this.WrapCheckBox.Location = new System.Drawing.Point(201, 207);
			this.WrapCheckBox.Name = "WrapCheckBox";
			this.WrapCheckBox.Size = new System.Drawing.Size(52, 17);
			this.WrapCheckBox.TabIndex = 31;
			this.WrapCheckBox.Text = "Wrap";
			this.WrapCheckBox.UseVisualStyleBackColor = true;
			this.WrapCheckBox.CheckedChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// DrumsCheckBox
			// 
			this.DrumsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.DrumsCheckBox.AutoSize = true;
			this.DrumsCheckBox.Location = new System.Drawing.Point(251, 207);
			this.DrumsCheckBox.Name = "DrumsCheckBox";
			this.DrumsCheckBox.Size = new System.Drawing.Size(56, 17);
			this.DrumsCheckBox.TabIndex = 32;
			this.DrumsCheckBox.Text = "Drums";
			this.DrumsCheckBox.UseVisualStyleBackColor = true;
			this.DrumsCheckBox.CheckedChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// OutputComboBox
			// 
			this.OutputComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.OutputComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.OutputComboBox.FormattingEnabled = true;
			this.OutputComboBox.Location = new System.Drawing.Point(210, 1);
			this.OutputComboBox.Name = "OutputComboBox";
			this.OutputComboBox.Size = new System.Drawing.Size(105, 21);
			this.OutputComboBox.TabIndex = 33;
			this.OutputComboBox.SelectedIndexChanged += new System.EventHandler(this.OutputComboBox_SelectedIndexChanged);
			// 
			// TempoLabel
			// 
			this.TempoLabel.AutoSize = true;
			this.TempoLabel.Location = new System.Drawing.Point(167, 30);
			this.TempoLabel.Name = "TempoLabel";
			this.TempoLabel.Size = new System.Drawing.Size(40, 13);
			this.TempoLabel.TabIndex = 34;
			this.TempoLabel.Text = "Tempo";
			// 
			// TempoUpDown
			// 
			this.TempoUpDown.DecimalPlaces = 3;
			this.TempoUpDown.Location = new System.Drawing.Point(209, 27);
			this.TempoUpDown.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            131072});
			this.TempoUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.TempoUpDown.Name = "TempoUpDown";
			this.TempoUpDown.Size = new System.Drawing.Size(74, 20);
			this.TempoUpDown.TabIndex = 35;
			this.TempoUpDown.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
			this.TempoUpDown.ValueChanged += new System.EventHandler(this._SetDirtyHandler);
			// 
			// VisualizerPanel
			// 
			this.VisualizerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.VisualizerPanel.AutoScroll = true;
			this.VisualizerPanel.Controls.Add(this.Visualizer);
			this.VisualizerPanel.Location = new System.Drawing.Point(4, 242);
			this.VisualizerPanel.Name = "VisualizerPanel";
			this.VisualizerPanel.Size = new System.Drawing.Size(311, 107);
			this.VisualizerPanel.TabIndex = 36;
			this.VisualizerPanel.Resize += new System.EventHandler(this.VisualizerPanel_Resize);
			// 
			// Visualizer
			// 
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
			this.Visualizer.Location = new System.Drawing.Point(0, 0);
			this.Visualizer.Name = "Visualizer";
			this.Visualizer.Sequence = null;
			this.Visualizer.Size = new System.Drawing.Size(311, 107);
			this.Visualizer.TabIndex = 0;
			this.Visualizer.Text = "Visualizer";
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(320, 380);
			this.Controls.Add(this.VisualizerPanel);
			this.Controls.Add(this.TempoUpDown);
			this.Controls.Add(this.TempoLabel);
			this.Controls.Add(this.OutputComboBox);
			this.Controls.Add(this.DrumsCheckBox);
			this.Controls.Add(this.WrapCheckBox);
			this.Controls.Add(this.TransposeUpDown);
			this.Controls.Add(this.TransposeLabel);
			this.Controls.Add(this.LevelsUpDown);
			this.Controls.Add(this.LevelsLabel);
			this.Controls.Add(this.NormalizeCheckBox);
			this.Controls.Add(this.ResampleUpDown);
			this.Controls.Add(this.CopyTimingPatchCheckBox);
			this.Controls.Add(this.StartLabel);
			this.Controls.Add(this.StartCombo);
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
			this.Controls.Add(this.ResampleLabel);
			this.MinimumSize = new System.Drawing.Size(336, 419);
			this.Name = "Main";
			this.Text = "MIDI Slicer";
			((System.ComponentModel.ISupportInitialize)(this.OffsetUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.LengthUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.StretchUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ResampleUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.LevelsUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.TransposeUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.TempoUpDown)).EndInit();
			this.VisualizerPanel.ResumeLayout(false);
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
		private System.Windows.Forms.Label StartLabel;
		private System.Windows.Forms.ComboBox StartCombo;
		private System.Windows.Forms.CheckBox CopyTimingPatchCheckBox;
		private System.Windows.Forms.Label ResampleLabel;
		private System.Windows.Forms.NumericUpDown ResampleUpDown;
		private System.Windows.Forms.CheckBox NormalizeCheckBox;
		private System.Windows.Forms.Label LevelsLabel;
		private System.Windows.Forms.NumericUpDown LevelsUpDown;
		private System.Windows.Forms.NumericUpDown TransposeUpDown;
		private System.Windows.Forms.Label TransposeLabel;
		private System.Windows.Forms.CheckBox WrapCheckBox;
		private System.Windows.Forms.CheckBox DrumsCheckBox;
		private System.Windows.Forms.ComboBox OutputComboBox;
		private System.Windows.Forms.Label TempoLabel;
		private System.Windows.Forms.NumericUpDown TempoUpDown;
		private System.Windows.Forms.Panel VisualizerPanel;
		private M.MidiVisualizer Visualizer;
	}
}