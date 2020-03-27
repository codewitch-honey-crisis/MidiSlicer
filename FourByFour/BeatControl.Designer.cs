namespace FourByFour
{
	partial class BeatControl
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
			this.Instrument = new System.Windows.Forms.ComboBox();
			this.DeleteButton = new System.Windows.Forms.Button();
			this.StepControl = new FourByFour.StepControl();
			this.SuspendLayout();
			// 
			// Instrument
			// 
			this.Instrument.DisplayMember = "Key";
			this.Instrument.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Instrument.FormattingEnabled = true;
			this.Instrument.Location = new System.Drawing.Point(28, 6);
			this.Instrument.Name = "Instrument";
			this.Instrument.Size = new System.Drawing.Size(127, 21);
			this.Instrument.TabIndex = 0;
			// 
			// DeleteButton
			// 
			this.DeleteButton.Location = new System.Drawing.Point(3, 5);
			this.DeleteButton.Name = "DeleteButton";
			this.DeleteButton.Size = new System.Drawing.Size(19, 23);
			this.DeleteButton.TabIndex = 2;
			this.DeleteButton.Text = "X";
			this.DeleteButton.UseVisualStyleBackColor = true;
			this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
			// 
			// StepControl
			// 
			this.StepControl.AutoSize = true;
			this.StepControl.Bars = 1;
			this.StepControl.Location = new System.Drawing.Point(161, 4);
			this.StepControl.MinimumSize = new System.Drawing.Size(272, 16);
			this.StepControl.Name = "StepControl";
			this.StepControl.Size = new System.Drawing.Size(272, 29);
			this.StepControl.TabIndex = 1;
			// 
			// BeatControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.DeleteButton);
			this.Controls.Add(this.StepControl);
			this.Controls.Add(this.Instrument);
			this.MinimumSize = new System.Drawing.Size(0, 36);
			this.Name = "BeatControl";
			this.Size = new System.Drawing.Size(437, 36);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox Instrument;
		private StepControl StepControl;
		private System.Windows.Forms.Button DeleteButton;
	}
}
