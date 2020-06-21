namespace FourByFour
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
			this.TempoLabel = new System.Windows.Forms.Label();
			this.TempoUpDown = new System.Windows.Forms.NumericUpDown();
			this.BeatsPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.AddButton = new System.Windows.Forms.Button();
			this.PlayButton = new System.Windows.Forms.Button();
			this.SaveAsButton = new System.Windows.Forms.Button();
			this.SaveMidiFile = new System.Windows.Forms.SaveFileDialog();
			this.BarsLabel = new System.Windows.Forms.Label();
			this.BarsUpDown = new System.Windows.Forms.NumericUpDown();
			this.PatternLabel = new System.Windows.Forms.Label();
			this.PatternComboBox = new System.Windows.Forms.ComboBox();
			this.OutputComboBox = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.TempoUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.BarsUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// TempoLabel
			// 
			this.TempoLabel.AutoSize = true;
			this.TempoLabel.Location = new System.Drawing.Point(3, 9);
			this.TempoLabel.Name = "TempoLabel";
			this.TempoLabel.Size = new System.Drawing.Size(40, 13);
			this.TempoLabel.TabIndex = 0;
			this.TempoLabel.Text = "Tempo";
			// 
			// TempoUpDown
			// 
			this.TempoUpDown.DecimalPlaces = 2;
			this.TempoUpDown.Location = new System.Drawing.Point(42, 7);
			this.TempoUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this.TempoUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.TempoUpDown.Name = "TempoUpDown";
			this.TempoUpDown.Size = new System.Drawing.Size(60, 20);
			this.TempoUpDown.TabIndex = 1;
			this.TempoUpDown.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
			// 
			// BeatsPanel
			// 
			this.BeatsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.BeatsPanel.AutoScroll = true;
			this.BeatsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.BeatsPanel.Location = new System.Drawing.Point(1, 33);
			this.BeatsPanel.Name = "BeatsPanel";
			this.BeatsPanel.Size = new System.Drawing.Size(612, 276);
			this.BeatsPanel.TabIndex = 2;
			// 
			// AddButton
			// 
			this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AddButton.Location = new System.Drawing.Point(312, 6);
			this.AddButton.Name = "AddButton";
			this.AddButton.Size = new System.Drawing.Size(44, 23);
			this.AddButton.TabIndex = 3;
			this.AddButton.Text = "Add";
			this.AddButton.UseVisualStyleBackColor = true;
			this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
			// 
			// PlayButton
			// 
			this.PlayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PlayButton.Location = new System.Drawing.Point(362, 6);
			this.PlayButton.Name = "PlayButton";
			this.PlayButton.Size = new System.Drawing.Size(44, 23);
			this.PlayButton.TabIndex = 4;
			this.PlayButton.Text = "Play";
			this.PlayButton.UseVisualStyleBackColor = true;
			this.PlayButton.Click += new System.EventHandler(this.PlayButton_Click);
			// 
			// SaveAsButton
			// 
			this.SaveAsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SaveAsButton.Location = new System.Drawing.Point(412, 6);
			this.SaveAsButton.Name = "SaveAsButton";
			this.SaveAsButton.Size = new System.Drawing.Size(68, 23);
			this.SaveAsButton.TabIndex = 5;
			this.SaveAsButton.Text = "Save As...";
			this.SaveAsButton.UseVisualStyleBackColor = true;
			this.SaveAsButton.Click += new System.EventHandler(this.SaveAsButton_Click);
			// 
			// SaveMidiFile
			// 
			this.SaveMidiFile.Filter = "MIDI files|*.mid|All files|*.*";
			// 
			// BarsLabel
			// 
			this.BarsLabel.AutoSize = true;
			this.BarsLabel.Location = new System.Drawing.Point(109, 10);
			this.BarsLabel.Name = "BarsLabel";
			this.BarsLabel.Size = new System.Drawing.Size(28, 13);
			this.BarsLabel.TabIndex = 6;
			this.BarsLabel.Text = "Bars";
			// 
			// BarsUpDown
			// 
			this.BarsUpDown.Location = new System.Drawing.Point(141, 7);
			this.BarsUpDown.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.BarsUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.BarsUpDown.Name = "BarsUpDown";
			this.BarsUpDown.Size = new System.Drawing.Size(45, 20);
			this.BarsUpDown.TabIndex = 7;
			this.BarsUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.BarsUpDown.ValueChanged += new System.EventHandler(this.BarsUpDown_ValueChanged);
			// 
			// PatternLabel
			// 
			this.PatternLabel.AutoSize = true;
			this.PatternLabel.Location = new System.Drawing.Point(193, 10);
			this.PatternLabel.Name = "PatternLabel";
			this.PatternLabel.Size = new System.Drawing.Size(41, 13);
			this.PatternLabel.TabIndex = 8;
			this.PatternLabel.Text = "Pattern";
			// 
			// PatternComboBox
			// 
			this.PatternComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.PatternComboBox.FormattingEnabled = true;
			this.PatternComboBox.Items.AddRange(new object[] {
            "(None)",
            "Basic Empty",
            "Break",
            "House"});
			this.PatternComboBox.Location = new System.Drawing.Point(232, 7);
			this.PatternComboBox.Name = "PatternComboBox";
			this.PatternComboBox.Size = new System.Drawing.Size(72, 21);
			this.PatternComboBox.TabIndex = 9;
			this.PatternComboBox.SelectedIndexChanged += new System.EventHandler(this.PatternComboBox_SelectedIndexChanged);
			// 
			// OutputComboBox
			// 
			this.OutputComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.OutputComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.OutputComboBox.FormattingEnabled = true;
			this.OutputComboBox.Location = new System.Drawing.Point(484, 7);
			this.OutputComboBox.Name = "OutputComboBox";
			this.OutputComboBox.Size = new System.Drawing.Size(127, 21);
			this.OutputComboBox.TabIndex = 10;
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(615, 310);
			this.Controls.Add(this.OutputComboBox);
			this.Controls.Add(this.PatternComboBox);
			this.Controls.Add(this.PatternLabel);
			this.Controls.Add(this.BarsUpDown);
			this.Controls.Add(this.BarsLabel);
			this.Controls.Add(this.SaveAsButton);
			this.Controls.Add(this.PlayButton);
			this.Controls.Add(this.AddButton);
			this.Controls.Add(this.BeatsPanel);
			this.Controls.Add(this.TempoUpDown);
			this.Controls.Add(this.TempoLabel);
			this.MinimumSize = new System.Drawing.Size(631, 140);
			this.Name = "Main";
			this.Text = "4x4 Beats";
			((System.ComponentModel.ISupportInitialize)(this.TempoUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.BarsUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label TempoLabel;
		private System.Windows.Forms.NumericUpDown TempoUpDown;
		private System.Windows.Forms.FlowLayoutPanel BeatsPanel;
		private System.Windows.Forms.Button AddButton;
		private System.Windows.Forms.Button PlayButton;
		private System.Windows.Forms.Button SaveAsButton;
		private System.Windows.Forms.SaveFileDialog SaveMidiFile;
		private System.Windows.Forms.Label BarsLabel;
		private System.Windows.Forms.NumericUpDown BarsUpDown;
		private System.Windows.Forms.Label PatternLabel;
		private System.Windows.Forms.ComboBox PatternComboBox;
		private System.Windows.Forms.ComboBox OutputComboBox;
	}
}

