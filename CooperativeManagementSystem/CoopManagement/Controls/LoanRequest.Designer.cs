namespace CoopManagement.Controls
{
    partial class LoanRequest
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
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.lblRequestNo = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.materialSingleLineTextField2 = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.SuspendLayout();
            // 
            // separator1
            // 
            this.separator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator1.Font = new System.Drawing.Font("Segoe UI Semilight", 14F, System.Drawing.FontStyle.Bold);
            this.separator1.Location = new System.Drawing.Point(10, 14);
            this.separator1.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.separator1.Name = "separator1";
            this.separator1.Size = new System.Drawing.Size(580, 23);
            this.separator1.TabIndex = 7;
            this.separator1.Text = "Loan Request Form";
            // 
            // materialLabel2
            // 
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.materialLabel2.Location = new System.Drawing.Point(20, 84);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(72, 21);
            this.materialLabel2.TabIndex = 9;
            this.materialLabel2.Text = "Member:";
            // 
            // lblRequestNo
            // 
            this.lblRequestNo.AutoSize = true;
            this.lblRequestNo.Depth = 0;
            this.lblRequestNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblRequestNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblRequestNo.Location = new System.Drawing.Point(310, 54);
            this.lblRequestNo.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblRequestNo.Name = "lblRequestNo";
            this.lblRequestNo.Size = new System.Drawing.Size(56, 18);
            this.lblRequestNo.TabIndex = 8;
            this.lblRequestNo.Text = "100001";
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.materialLabel1.Location = new System.Drawing.Point(20, 53);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(169, 21);
            this.materialLabel1.TabIndex = 6;
            this.materialLabel1.Text = "Loan Request Number:";
            // 
            // materialSingleLineTextField2
            // 
            this.materialSingleLineTextField2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.materialSingleLineTextField2.Depth = 0;
            this.materialSingleLineTextField2.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.materialSingleLineTextField2.Hint = "";
            this.materialSingleLineTextField2.Location = new System.Drawing.Point(310, 76);
            this.materialSingleLineTextField2.MaxLength = 32767;
            this.materialSingleLineTextField2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialSingleLineTextField2.Name = "materialSingleLineTextField2";
            this.materialSingleLineTextField2.PasswordChar = '\0';
            this.materialSingleLineTextField2.SelectedText = "";
            this.materialSingleLineTextField2.SelectionLength = 0;
            this.materialSingleLineTextField2.SelectionStart = 0;
            this.materialSingleLineTextField2.Size = new System.Drawing.Size(195, 27);
            this.materialSingleLineTextField2.TabIndex = 4;
            this.materialSingleLineTextField2.TabStop = false;
            this.materialSingleLineTextField2.UseSystemPasswordChar = false;
            // 
            // LoanRequest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Snow;
            this.Controls.Add(this.materialLabel2);
            this.Controls.Add(this.lblRequestNo);
            this.Controls.Add(this.separator1);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.materialSingleLineTextField2);
            this.Name = "LoanRequest";
            this.Size = new System.Drawing.Size(600, 540);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MaterialSkin.Controls.MaterialSingleLineTextField materialSingleLineTextField2;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private SwingWERX.Controls.Separator separator1;
        private MaterialSkin.Controls.MaterialLabel lblRequestNo;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
    }
}
