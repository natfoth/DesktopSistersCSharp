namespace DesktopSistersCSharpForm
{
    partial class Form1
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
            this.useNewArtCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // useNewArtCheckbox
            // 
            this.useNewArtCheckbox.AutoSize = true;
            this.useNewArtCheckbox.Checked = true;
            this.useNewArtCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useNewArtCheckbox.Location = new System.Drawing.Point(12, 12);
            this.useNewArtCheckbox.Name = "useNewArtCheckbox";
            this.useNewArtCheckbox.Size = new System.Drawing.Size(112, 17);
            this.useNewArtCheckbox.TabIndex = 0;
            this.useNewArtCheckbox.Text = "Use New Style Art";
            this.useNewArtCheckbox.UseVisualStyleBackColor = true;
            this.useNewArtCheckbox.CheckedChanged += new System.EventHandler(this.useNewArtCheckbox_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.useNewArtCheckbox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox useNewArtCheckbox;
    }
}

