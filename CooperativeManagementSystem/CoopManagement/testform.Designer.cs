namespace CoopManagement
{
    partial class testform
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
            this.swxPanel1 = new SwingWERX.Controls.SwxPanel();
            this.memberCreate1 = new CoopManagement.Controls.MemberCreate();
            this.swxPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // swxPanel1
            // 
            this.swxPanel1.BackColor = System.Drawing.Color.Transparent;
            this.swxPanel1.Controls.Add(this.memberCreate1);
            this.swxPanel1.GradientColor1 = System.Drawing.Color.Snow;
            this.swxPanel1.GradientColor2 = System.Drawing.Color.Snow;
            this.swxPanel1.Location = new System.Drawing.Point(2, 64);
            this.swxPanel1.Name = "swxPanel1";
            this.swxPanel1.Size = new System.Drawing.Size(716, 473);
            this.swxPanel1.TabIndex = 0;
            // 
            // memberCreate1
            // 
            this.memberCreate1.BackColor = System.Drawing.Color.Snow;
            this.memberCreate1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memberCreate1.Location = new System.Drawing.Point(0, 0);
            this.memberCreate1.Name = "memberCreate1";
            this.memberCreate1.Size = new System.Drawing.Size(716, 473);
            this.memberCreate1.TabIndex = 0;
            // 
            // testform
            // 
            this.AccentColor = System.Drawing.Color.Gray;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 540);
            this.Controls.Add(this.swxPanel1);
            this.DarkPrimaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.ForeColor = System.Drawing.Color.Snow;
            this.Name = "testform";
            this.PrimaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(37)))), ((int)(((byte)(48)))));
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "testform";
            this.swxPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SwingWERX.Controls.SwxPanel swxPanel1;
        private Controls.MemberCreate memberCreate1;
    }
}