namespace CoopManagement.Forms
{
    partial class MemberRegistration
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
            Telerik.WinControls.UI.RadListDataItem radListDataItem5 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem6 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem1 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem2 = new Telerik.WinControls.UI.RadListDataItem();
            this.separator2 = new SwingWERX.Controls.Separator();
            this.lblMemberNo = new Telerik.WinControls.UI.RadTextBox();
            this.w8 = new Telerik.WinControls.Themes.Windows8Theme();
            this.txtLastName = new Telerik.WinControls.UI.RadTextBox();
            this.separator1 = new SwingWERX.Controls.Separator();
            this.txtFirstName = new Telerik.WinControls.UI.RadTextBox();
            this.txtMiddleName = new Telerik.WinControls.UI.RadTextBox();
            this.separator3 = new SwingWERX.Controls.Separator();
            this.cmbGender = new Telerik.WinControls.UI.RadDropDownList();
            this.separator4 = new SwingWERX.Controls.Separator();
            this.pckBirthDate = new Telerik.WinControls.UI.RadDateTimePicker();
            this.pckJoinDate = new Telerik.WinControls.UI.RadDateTimePicker();
            this.separator5 = new SwingWERX.Controls.Separator();
            this.separator6 = new SwingWERX.Controls.Separator();
            this.cmbMemberType = new Telerik.WinControls.UI.RadDropDownList();
            this.btnCreateAcct = new MaterialSkin.Controls.MaterialRaisedButton();
            this.radSeparator1 = new Telerik.WinControls.UI.RadSeparator();
            this.btnReset = new MaterialSkin.Controls.MaterialFlatButton();
            ((System.ComponentModel.ISupportInitialize)(this.lblMemberNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLastName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFirstName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMiddleName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbGender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pckBirthDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pckJoinDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbMemberType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radSeparator1)).BeginInit();
            this.SuspendLayout();
            // 
            // separator2
            // 
            this.separator2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator2.BackColor = System.Drawing.Color.Transparent;
            this.separator2.Font = new System.Drawing.Font("Segoe UI Semilight", 13F);
            this.separator2.Location = new System.Drawing.Point(12, 77);
            this.separator2.Name = "separator2";
            this.separator2.Size = new System.Drawing.Size(308, 23);
            this.separator2.TabIndex = 1;
            this.separator2.Text = "Member ID";
            // 
            // lblMemberNo
            // 
            this.lblMemberNo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMemberNo.BackColor = System.Drawing.SystemColors.Window;
            this.lblMemberNo.Enabled = false;
            this.lblMemberNo.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.lblMemberNo.Location = new System.Drawing.Point(12, 103);
            this.lblMemberNo.Name = "lblMemberNo";
            this.lblMemberNo.ReadOnly = true;
            this.lblMemberNo.Size = new System.Drawing.Size(308, 27);
            this.lblMemberNo.TabIndex = 2;
            this.lblMemberNo.ThemeName = "Windows8";
            this.lblMemberNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownEvent);
            // 
            // txtLastName
            // 
            this.txtLastName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLastName.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.txtLastName.Location = new System.Drawing.Point(12, 159);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.NullText = "Last Name";
            this.txtLastName.Size = new System.Drawing.Size(308, 27);
            this.txtLastName.TabIndex = 4;
            this.txtLastName.ThemeName = "Windows8";
            this.txtLastName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownEvent);
            // 
            // separator1
            // 
            this.separator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator1.BackColor = System.Drawing.Color.Transparent;
            this.separator1.Font = new System.Drawing.Font("Segoe UI Semilight", 13F);
            this.separator1.Location = new System.Drawing.Point(12, 133);
            this.separator1.Name = "separator1";
            this.separator1.Size = new System.Drawing.Size(308, 23);
            this.separator1.TabIndex = 3;
            this.separator1.Text = "Name";
            // 
            // txtFirstName
            // 
            this.txtFirstName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFirstName.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.txtFirstName.Location = new System.Drawing.Point(12, 192);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.NullText = "First Name";
            this.txtFirstName.Size = new System.Drawing.Size(308, 27);
            this.txtFirstName.TabIndex = 5;
            this.txtFirstName.ThemeName = "Windows8";
            this.txtFirstName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownEvent);
            // 
            // txtMiddleName
            // 
            this.txtMiddleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMiddleName.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.txtMiddleName.Location = new System.Drawing.Point(12, 225);
            this.txtMiddleName.Name = "txtMiddleName";
            this.txtMiddleName.NullText = "Middle Name (Optional)";
            this.txtMiddleName.Size = new System.Drawing.Size(308, 27);
            this.txtMiddleName.TabIndex = 6;
            this.txtMiddleName.ThemeName = "Windows8";
            this.txtMiddleName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownEvent);
            // 
            // separator3
            // 
            this.separator3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator3.BackColor = System.Drawing.Color.Transparent;
            this.separator3.Font = new System.Drawing.Font("Segoe UI Semilight", 13F);
            this.separator3.Location = new System.Drawing.Point(12, 255);
            this.separator3.Name = "separator3";
            this.separator3.Size = new System.Drawing.Size(308, 23);
            this.separator3.TabIndex = 7;
            this.separator3.Text = "Gender";
            // 
            // cmbGender
            // 
            this.cmbGender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbGender.DropDownHeight = 46;
            this.cmbGender.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.cmbGender.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.cmbGender.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            radListDataItem5.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            radListDataItem5.Height = 22;
            radListDataItem5.Text = "Male";
            radListDataItem6.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            radListDataItem6.Height = 22;
            radListDataItem6.Text = "Female";
            this.cmbGender.Items.Add(radListDataItem5);
            this.cmbGender.Items.Add(radListDataItem6);
            this.cmbGender.Location = new System.Drawing.Point(12, 281);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.NullText = "- Select -";
            this.cmbGender.Size = new System.Drawing.Size(308, 27);
            this.cmbGender.TabIndex = 17;
            this.cmbGender.Text = "- Select -";
            this.cmbGender.ThemeName = "Windows8";
            this.cmbGender.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownEvent);
            ((Telerik.WinControls.UI.RadDropDownListElement)(this.cmbGender.GetChildAt(0))).DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            ((Telerik.WinControls.UI.RadDropDownTextBoxElement)(this.cmbGender.GetChildAt(0).GetChildAt(2).GetChildAt(0).GetChildAt(0))).Text = "- Select -";
            ((Telerik.WinControls.UI.RadDropDownTextBoxElement)(this.cmbGender.GetChildAt(0).GetChildAt(2).GetChildAt(0).GetChildAt(0))).Visibility = Telerik.WinControls.ElementVisibility.Hidden;
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.cmbGender.GetChildAt(0).GetChildAt(2).GetChildAt(0).GetChildAt(0).GetChildAt(0))).NullText = "- Select -";
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.cmbGender.GetChildAt(0).GetChildAt(2).GetChildAt(0).GetChildAt(0).GetChildAt(0))).ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            // 
            // separator4
            // 
            this.separator4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator4.BackColor = System.Drawing.Color.Transparent;
            this.separator4.Font = new System.Drawing.Font("Segoe UI Semilight", 13F);
            this.separator4.Location = new System.Drawing.Point(12, 311);
            this.separator4.Name = "separator4";
            this.separator4.Size = new System.Drawing.Size(308, 23);
            this.separator4.TabIndex = 18;
            this.separator4.Text = "Date of Birth";
            // 
            // pckBirthDate
            // 
            this.pckBirthDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pckBirthDate.CustomFormat = "yyyy-MM-dd";
            this.pckBirthDate.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.pckBirthDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.pckBirthDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.pckBirthDate.Location = new System.Drawing.Point(12, 337);
            this.pckBirthDate.Name = "pckBirthDate";
            this.pckBirthDate.Size = new System.Drawing.Size(308, 27);
            this.pckBirthDate.TabIndex = 19;
            this.pckBirthDate.TabStop = false;
            this.pckBirthDate.Text = "1970-01-01";
            this.pckBirthDate.ThemeName = "Windows8";
            this.pckBirthDate.Value = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            this.pckBirthDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownEvent);
            // 
            // pckJoinDate
            // 
            this.pckJoinDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pckJoinDate.CustomFormat = "yyyy-MM-dd";
            this.pckJoinDate.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.pckJoinDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.pckJoinDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.pckJoinDate.Location = new System.Drawing.Point(12, 393);
            this.pckJoinDate.Name = "pckJoinDate";
            this.pckJoinDate.Size = new System.Drawing.Size(308, 27);
            this.pckJoinDate.TabIndex = 21;
            this.pckJoinDate.TabStop = false;
            this.pckJoinDate.Text = "1970-01-01";
            this.pckJoinDate.ThemeName = "Windows8";
            this.pckJoinDate.Value = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            this.pckJoinDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownEvent);
            // 
            // separator5
            // 
            this.separator5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator5.BackColor = System.Drawing.Color.Transparent;
            this.separator5.Font = new System.Drawing.Font("Segoe UI Semilight", 13F);
            this.separator5.Location = new System.Drawing.Point(12, 367);
            this.separator5.Name = "separator5";
            this.separator5.Size = new System.Drawing.Size(308, 23);
            this.separator5.TabIndex = 20;
            this.separator5.Text = "Date Joined";
            // 
            // separator6
            // 
            this.separator6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator6.BackColor = System.Drawing.Color.Transparent;
            this.separator6.Font = new System.Drawing.Font("Segoe UI Semilight", 13F);
            this.separator6.Location = new System.Drawing.Point(12, 423);
            this.separator6.Name = "separator6";
            this.separator6.Size = new System.Drawing.Size(308, 23);
            this.separator6.TabIndex = 22;
            this.separator6.Text = "Member Type";
            // 
            // cmbMemberType
            // 
            this.cmbMemberType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMemberType.DropDownHeight = 46;
            this.cmbMemberType.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.cmbMemberType.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.cmbMemberType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            radListDataItem1.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            radListDataItem1.Height = 22;
            radListDataItem1.Text = "Regular";
            radListDataItem2.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            radListDataItem2.Height = 22;
            radListDataItem2.Text = "Associate";
            this.cmbMemberType.Items.Add(radListDataItem1);
            this.cmbMemberType.Items.Add(radListDataItem2);
            this.cmbMemberType.Location = new System.Drawing.Point(12, 449);
            this.cmbMemberType.Name = "cmbMemberType";
            this.cmbMemberType.NullText = "- Select -";
            this.cmbMemberType.Size = new System.Drawing.Size(308, 27);
            this.cmbMemberType.TabIndex = 26;
            this.cmbMemberType.Text = "- Select -";
            this.cmbMemberType.ThemeName = "Windows8";
            this.cmbMemberType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownEvent);
            // 
            // btnCreateAcct
            // 
            this.btnCreateAcct.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCreateAcct.Depth = 0;
            this.btnCreateAcct.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.btnCreateAcct.ForeColor = System.Drawing.Color.White;
            this.btnCreateAcct.Location = new System.Drawing.Point(170, 497);
            this.btnCreateAcct.Margin = new System.Windows.Forms.Padding(6, 7, 48, 7);
            this.btnCreateAcct.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnCreateAcct.Name = "btnCreateAcct";
            this.btnCreateAcct.Primary = true;
            this.btnCreateAcct.Size = new System.Drawing.Size(150, 30);
            this.btnCreateAcct.TabIndex = 34;
            this.btnCreateAcct.Text = "Save";
            this.btnCreateAcct.UseVisualStyleBackColor = true;
            this.btnCreateAcct.Click += new System.EventHandler(this.Save_Action);
            // 
            // radSeparator1
            // 
            this.radSeparator1.BackColor = System.Drawing.Color.Transparent;
            this.radSeparator1.Location = new System.Drawing.Point(0, 483);
            this.radSeparator1.Name = "radSeparator1";
            this.radSeparator1.Size = new System.Drawing.Size(332, 10);
            this.radSeparator1.TabIndex = 35;
            this.radSeparator1.Text = "radSeparator1";
            this.radSeparator1.ThemeName = "Windows8";
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnReset.BackColor = System.Drawing.SystemColors.Window;
            this.btnReset.Depth = 0;
            this.btnReset.Font = new System.Drawing.Font("Segoe UI Semilight", 14.25F);
            this.btnReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.btnReset.Location = new System.Drawing.Point(12, 497);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnReset.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnReset.Name = "btnReset";
            this.btnReset.Primary = false;
            this.btnReset.Size = new System.Drawing.Size(150, 30);
            this.btnReset.TabIndex = 36;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.Reset_Action);
            // 
            // MemberRegistration
            // 
            this.AccentColor = System.Drawing.Color.Gray;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 538);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.radSeparator1);
            this.Controls.Add(this.btnCreateAcct);
            this.Controls.Add(this.cmbMemberType);
            this.Controls.Add(this.separator6);
            this.Controls.Add(this.pckJoinDate);
            this.Controls.Add(this.separator5);
            this.Controls.Add(this.pckBirthDate);
            this.Controls.Add(this.separator4);
            this.Controls.Add(this.cmbGender);
            this.Controls.Add(this.separator3);
            this.Controls.Add(this.txtMiddleName);
            this.Controls.Add(this.txtFirstName);
            this.Controls.Add(this.txtLastName);
            this.Controls.Add(this.separator1);
            this.Controls.Add(this.lblMemberNo);
            this.Controls.Add(this.separator2);
            this.DarkPrimaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.Font = new System.Drawing.Font("Segoe UI Light", 14F);
            this.ForeColor = System.Drawing.Color.Snow;
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MemberRegistration";
            this.PrimaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(37)))), ((int)(((byte)(48)))));
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Member Registration Form";
            this.Load += new System.EventHandler(this.LoadEvent);
            ((System.ComponentModel.ISupportInitialize)(this.lblMemberNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLastName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFirstName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMiddleName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbGender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pckBirthDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pckJoinDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbMemberType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radSeparator1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SwingWERX.Controls.Separator separator2;
        private Telerik.WinControls.UI.RadTextBox lblMemberNo;
        private Telerik.WinControls.Themes.Windows8Theme w8;
        private Telerik.WinControls.UI.RadTextBox txtLastName;
        private SwingWERX.Controls.Separator separator1;
        private Telerik.WinControls.UI.RadTextBox txtFirstName;
        private Telerik.WinControls.UI.RadTextBox txtMiddleName;
        private SwingWERX.Controls.Separator separator3;
        private Telerik.WinControls.UI.RadDropDownList cmbGender;
        private SwingWERX.Controls.Separator separator4;
        private Telerik.WinControls.UI.RadDateTimePicker pckBirthDate;
        private Telerik.WinControls.UI.RadDateTimePicker pckJoinDate;
        private SwingWERX.Controls.Separator separator5;
        private SwingWERX.Controls.Separator separator6;
        private Telerik.WinControls.UI.RadDropDownList cmbMemberType;
        private MaterialSkin.Controls.MaterialRaisedButton btnCreateAcct;
        private Telerik.WinControls.UI.RadSeparator radSeparator1;
        private MaterialSkin.Controls.MaterialFlatButton btnReset;
    }
}