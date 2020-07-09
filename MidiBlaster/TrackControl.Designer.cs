namespace MidiBlaster
{
	partial class TrackControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OpenButton = new System.Windows.Forms.Button();
			this.TransposeComboBox = new System.Windows.Forms.ComboBox();
			this.RootNoteLabel = new System.Windows.Forms.Label();
			this.AutoTransposeCheckBox = new System.Windows.Forms.CheckBox();
			this.VelocityHScrollBar = new System.Windows.Forms.HScrollBar();
			this.SuspendLayout();
			// 
			// OpenButton
			// 
			this.OpenButton.Location = new System.Drawing.Point(3, 3);
			this.OpenButton.Name = "OpenButton";
			this.OpenButton.Size = new System.Drawing.Size(25, 23);
			this.OpenButton.TabIndex = 0;
			this.OpenButton.Text = "...";
			this.OpenButton.UseVisualStyleBackColor = true;
			// 
			// TransposeComboBox
			// 
			this.TransposeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TransposeComboBox.FormattingEnabled = true;
			this.TransposeComboBox.Location = new System.Drawing.Point(90, 4);
			this.TransposeComboBox.Name = "TransposeComboBox";
			this.TransposeComboBox.Size = new System.Drawing.Size(93, 21);
			this.TransposeComboBox.TabIndex = 1;
			// 
			// RootNoteLabel
			// 
			this.RootNoteLabel.AutoSize = true;
			this.RootNoteLabel.Location = new System.Drawing.Point(34, 8);
			this.RootNoteLabel.Name = "RootNoteLabel";
			this.RootNoteLabel.Size = new System.Drawing.Size(56, 13);
			this.RootNoteLabel.TabIndex = 2;
			this.RootNoteLabel.Text = "Root Note";
			// 
			// AutoTransposeCheckBox
			// 
			this.AutoTransposeCheckBox.AutoSize = true;
			this.AutoTransposeCheckBox.Checked = true;
			this.AutoTransposeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.AutoTransposeCheckBox.Location = new System.Drawing.Point(190, 7);
			this.AutoTransposeCheckBox.Name = "AutoTransposeCheckBox";
			this.AutoTransposeCheckBox.Size = new System.Drawing.Size(48, 17);
			this.AutoTransposeCheckBox.TabIndex = 3;
			this.AutoTransposeCheckBox.Text = "Auto";
			this.AutoTransposeCheckBox.UseVisualStyleBackColor = true;
			// 
			// VelocityHScrollBar
			// 
			this.VelocityHScrollBar.Location = new System.Drawing.Point(241, 5);
			this.VelocityHScrollBar.Maximum = 10;
			this.VelocityHScrollBar.Name = "VelocityHScrollBar";
			this.VelocityHScrollBar.Size = new System.Drawing.Size(251, 17);
			this.VelocityHScrollBar.TabIndex = 4;
			// 
			// TrackControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.VelocityHScrollBar);
			this.Controls.Add(this.AutoTransposeCheckBox);
			this.Controls.Add(this.RootNoteLabel);
			this.Controls.Add(this.TransposeComboBox);
			this.Controls.Add(this.OpenButton);
			this.Name = "TrackControl";
			this.Size = new System.Drawing.Size(763, 29);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button OpenButton;
		private System.Windows.Forms.ComboBox TransposeComboBox;
		private System.Windows.Forms.Label RootNoteLabel;
		private System.Windows.Forms.CheckBox AutoTransposeCheckBox;
		private System.Windows.Forms.HScrollBar VelocityHScrollBar;
	}
}
