namespace MidiMonitor
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
			this.InputsComboBox = new System.Windows.Forms.ComboBox();
			this.InputsLabel = new System.Windows.Forms.Label();
			this.MessagesTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// InputsComboBox
			// 
			this.InputsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.InputsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.InputsComboBox.FormattingEnabled = true;
			this.InputsComboBox.Location = new System.Drawing.Point(39, 3);
			this.InputsComboBox.Name = "InputsComboBox";
			this.InputsComboBox.Size = new System.Drawing.Size(254, 21);
			this.InputsComboBox.TabIndex = 0;
			this.InputsComboBox.SelectedIndexChanged += new System.EventHandler(this.InputsComboBox_SelectedIndexChanged);
			// 
			// InputsLabel
			// 
			this.InputsLabel.AutoSize = true;
			this.InputsLabel.Location = new System.Drawing.Point(3, 6);
			this.InputsLabel.Name = "InputsLabel";
			this.InputsLabel.Size = new System.Drawing.Size(31, 13);
			this.InputsLabel.TabIndex = 1;
			this.InputsLabel.Text = "Input";
			// 
			// MessagesTextBox
			// 
			this.MessagesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MessagesTextBox.Location = new System.Drawing.Point(6, 30);
			this.MessagesTextBox.Multiline = true;
			this.MessagesTextBox.Name = "MessagesTextBox";
			this.MessagesTextBox.ReadOnly = true;
			this.MessagesTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.MessagesTextBox.Size = new System.Drawing.Size(287, 416);
			this.MessagesTextBox.TabIndex = 2;
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(301, 450);
			this.Controls.Add(this.MessagesTextBox);
			this.Controls.Add(this.InputsLabel);
			this.Controls.Add(this.InputsComboBox);
			this.Name = "Main";
			this.Text = "MIDI Monitor";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox InputsComboBox;
		private System.Windows.Forms.Label InputsLabel;
		private System.Windows.Forms.TextBox MessagesTextBox;
	}
}

