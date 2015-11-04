namespace test
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
            this.fancyPanel1 = new SwingWERX.Controls.FancyPanel();
            this.SuspendLayout();
            // 
            // fancyPanel1
            // 
            this.fancyPanel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.fancyPanel1.BorderColor = System.Drawing.Color.Red;
            this.fancyPanel1.BorderWidth = 2;
            this.fancyPanel1.GradientEndColor = System.Drawing.Color.Gray;
            this.fancyPanel1.GradientStartColor = System.Drawing.Color.White;
            this.fancyPanel1.Location = new System.Drawing.Point(103, 98);
            this.fancyPanel1.Name = "fancyPanel1";
            this.fancyPanel1.RoundCornerRadius = 10;
            this.fancyPanel1.ShadowOffSet = 1;
            this.fancyPanel1.Size = new System.Drawing.Size(461, 317);
            this.fancyPanel1.TabIndex = 0;
            this.fancyPanel1.Text = "fancyPanel1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 494);
            this.Controls.Add(this.fancyPanel1);
            this.ForeColor = System.Drawing.Color.Red;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private SwingWERX.Controls.FancyPanel fancyPanel1;



    }
}

