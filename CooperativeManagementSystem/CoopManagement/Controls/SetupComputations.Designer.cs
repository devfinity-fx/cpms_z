namespace CoopManagement.Controls
{
    partial class SetupComputations
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
            this.separator1 = new SwingWERX.Controls.Separator();
            this.SuspendLayout();
            // 
            // separator1
            // 
            this.separator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator1.Font = new System.Drawing.Font("Segoe UI Semilight", 14F, System.Drawing.FontStyle.Bold);
            this.separator1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.separator1.Location = new System.Drawing.Point(10, 14);
            this.separator1.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.separator1.Name = "separator1";
            this.separator1.Size = new System.Drawing.Size(580, 23);
            this.separator1.TabIndex = 10;
            this.separator1.Text = "Setup and Computations";
            // 
            // SetupComputations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Snow;
            this.Controls.Add(this.separator1);
            this.Name = "SetupComputations";
            this.Size = new System.Drawing.Size(600, 540);
            this.ResumeLayout(false);

        }

        #endregion

        private SwingWERX.Controls.Separator separator1;
    }
}
