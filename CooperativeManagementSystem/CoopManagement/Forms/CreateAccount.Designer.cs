namespace CoopManagement.Forms
{
    partial class CreateAccount
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
            this.separator2 = new SwingWERX.Controls.Separator();
            this.separator1 = new SwingWERX.Controls.Separator();
            this.txtDescription = new Telerik.WinControls.UI.RadTextBoxControl();
            this.txtTitle = new Telerik.WinControls.UI.RadTextBox();
            this.btnCreate = new MaterialSkin.Controls.MaterialRaisedButton();
            this.separator3 = new SwingWERX.Controls.Separator();
            this.cmbCategories = new Telerik.WinControls.UI.RadDropDownList();
            this.w8 = new Telerik.WinControls.Themes.Windows8Theme();
            this.cmbType = new Telerik.WinControls.UI.RadDropDownList();
            this.separator4 = new SwingWERX.Controls.Separator();
            this.separator5 = new SwingWERX.Controls.Separator();
            this.txtInitialBalance = new Telerik.WinControls.UI.RadIntTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCategories)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInitialBalance)).BeginInit();
            this.SuspendLayout();
            // 
            // separator2
            // 
            this.separator2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator2.BackColor = System.Drawing.Color.Transparent;
            this.separator2.Font = new System.Drawing.Font("Segoe UI Semilight", 13F);
            this.separator2.Location = new System.Drawing.Point(14, 128);
            this.separator2.Name = "separator2";
            this.separator2.Size = new System.Drawing.Size(291, 23);
            this.separator2.TabIndex = 4;
            this.separator2.Text = "Title";
            // 
            // separator1
            // 
            this.separator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator1.BackColor = System.Drawing.Color.Transparent;
            this.separator1.Font = new System.Drawing.Font("Segoe UI Semilight", 13F);
            this.separator1.Location = new System.Drawing.Point(14, 184);
            this.separator1.Name = "separator1";
            this.separator1.Size = new System.Drawing.Size(292, 23);
            this.separator1.TabIndex = 5;
            this.separator1.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.txtDescription.Location = new System.Drawing.Point(14, 210);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(291, 92);
            this.txtDescription.TabIndex = 3;
            this.txtDescription.ThemeName = "Windows8";
            // 
            // txtTitle
            // 
            this.txtTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTitle.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.txtTitle.Location = new System.Drawing.Point(14, 154);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(291, 27);
            this.txtTitle.TabIndex = 2;
            this.txtTitle.ThemeName = "Windows8";
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.Depth = 0;
            this.btnCreate.Font = new System.Drawing.Font("Segoe UI Semilight", 14.25F);
            this.btnCreate.ForeColor = System.Drawing.Color.White;
            this.btnCreate.Location = new System.Drawing.Point(12, 422);
            this.btnCreate.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.btnCreate.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Primary = true;
            this.btnCreate.Size = new System.Drawing.Size(293, 30);
            this.btnCreate.TabIndex = 8;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.Save_Action);
            // 
            // separator3
            // 
            this.separator3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator3.BackColor = System.Drawing.Color.Transparent;
            this.separator3.Font = new System.Drawing.Font("Segoe UI Semilight", 13F);
            this.separator3.Location = new System.Drawing.Point(14, 72);
            this.separator3.Name = "separator3";
            this.separator3.Size = new System.Drawing.Size(291, 23);
            this.separator3.TabIndex = 9;
            this.separator3.Text = "Account Category";
            // 
            // cmbCategories
            // 
            this.cmbCategories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCategories.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.cmbCategories.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.cmbCategories.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.cmbCategories.Location = new System.Drawing.Point(14, 98);
            this.cmbCategories.Name = "cmbCategories";
            this.cmbCategories.Size = new System.Drawing.Size(291, 27);
            this.cmbCategories.TabIndex = 1;
            this.cmbCategories.ThemeName = "Windows8";
            this.cmbCategories.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.Category_Changed);
            // 
            // cmbType
            // 
            this.cmbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbType.DropDownHeight = 42;
            this.cmbType.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.cmbType.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.cmbType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.cmbType.Location = new System.Drawing.Point(14, 331);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(291, 27);
            this.cmbType.TabIndex = 4;
            this.cmbType.ThemeName = "Windows8";
            // 
            // separator4
            // 
            this.separator4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator4.BackColor = System.Drawing.Color.Transparent;
            this.separator4.Font = new System.Drawing.Font("Segoe UI Semilight", 13F);
            this.separator4.Location = new System.Drawing.Point(14, 305);
            this.separator4.Name = "separator4";
            this.separator4.Size = new System.Drawing.Size(291, 23);
            this.separator4.TabIndex = 11;
            this.separator4.Text = "Type";
            // 
            // separator5
            // 
            this.separator5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator5.BackColor = System.Drawing.Color.Transparent;
            this.separator5.Font = new System.Drawing.Font("Segoe UI Semilight", 13F);
            this.separator5.Location = new System.Drawing.Point(14, 361);
            this.separator5.Name = "separator5";
            this.separator5.Size = new System.Drawing.Size(291, 23);
            this.separator5.TabIndex = 13;
            this.separator5.Text = "Initial Balance";
            // 
            // txtInitialBalance
            // 
            this.txtInitialBalance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInitialBalance.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.txtInitialBalance.Location = new System.Drawing.Point(14, 387);
            this.txtInitialBalance.Name = "txtInitialBalance";
            this.txtInitialBalance.Size = new System.Drawing.Size(291, 27);
            this.txtInitialBalance.TabIndex = 5;
            this.txtInitialBalance.TabStop = false;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.txtInitialBalance.GetChildAt(0).GetChildAt(2))).ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(172)))), ((int)(((byte)(172)))));
            // 
            // CreateAccount
            // 
            this.AccentColor = System.Drawing.Color.Gray;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 463);
            this.Controls.Add(this.txtInitialBalance);
            this.Controls.Add(this.separator5);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.separator4);
            this.Controls.Add(this.cmbCategories);
            this.Controls.Add(this.separator3);
            this.Controls.Add(this.separator2);
            this.Controls.Add(this.separator1);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.btnCreate);
            this.DarkPrimaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.ForeColor = System.Drawing.Color.Snow;
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateAccount";
            this.PrimaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(37)))), ((int)(((byte)(48)))));
            this.ShowInTaskbar = false;
            this.Sizable = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CreateAccount";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.LoadEvent);
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCategories)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInitialBalance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SwingWERX.Controls.Separator separator2;
        private SwingWERX.Controls.Separator separator1;
        private Telerik.WinControls.UI.RadTextBoxControl txtDescription;
        private Telerik.WinControls.UI.RadTextBox txtTitle;
        private MaterialSkin.Controls.MaterialRaisedButton btnCreate;
        private SwingWERX.Controls.Separator separator3;
        private Telerik.WinControls.UI.RadDropDownList cmbCategories;
        private Telerik.WinControls.Themes.Windows8Theme w8;
        private Telerik.WinControls.UI.RadDropDownList cmbType;
        private SwingWERX.Controls.Separator separator4;
        private SwingWERX.Controls.Separator separator5;
        private Telerik.WinControls.UI.RadIntTextBox txtInitialBalance;
    }
}