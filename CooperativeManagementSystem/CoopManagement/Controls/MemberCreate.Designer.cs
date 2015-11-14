namespace CoopManagement.Controls
{
    partial class MemberCreate
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
            Telerik.WinControls.UI.RadListDataItem radListDataItem1 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem2 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem3 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem4 = new Telerik.WinControls.UI.RadListDataItem();
            this.separator1 = new SwingWERX.Controls.Separator();
            this.cmbGender = new Telerik.WinControls.UI.RadDropDownList();
            this.w8 = new Telerik.WinControls.Themes.Windows8Theme();
            this.pckBirthDate = new Telerik.WinControls.UI.RadDateTimePicker();
            this.pckJoinDate = new Telerik.WinControls.UI.RadDateTimePicker();
            this.cmbMemberType = new Telerik.WinControls.UI.RadDropDownList();
            this.btnReset = new MaterialSkin.Controls.MaterialFlatButton();
            this.btnSave = new MaterialSkin.Controls.MaterialRaisedButton();
            this.materialLabel8 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel7 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel6 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel5 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel4 = new MaterialSkin.Controls.MaterialLabel();
            this.txtMiddleName = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.txtFirstName = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.txtLastName = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.lblMemberNo = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            ((System.ComponentModel.ISupportInitialize)(this.cmbGender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pckBirthDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pckJoinDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbMemberType)).BeginInit();
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
            this.separator1.TabIndex = 8;
            this.separator1.Text = "Member Registration Form";
            // 
            // cmbGender
            // 
            this.cmbGender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbGender.DropDownHeight = 46;
            this.cmbGender.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.cmbGender.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.cmbGender.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            radListDataItem1.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            radListDataItem1.Height = 22;
            radListDataItem1.Text = "Male";
            radListDataItem2.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            radListDataItem2.Height = 22;
            radListDataItem2.Text = "Female";
            this.cmbGender.Items.Add(radListDataItem1);
            this.cmbGender.Items.Add(radListDataItem2);
            this.cmbGender.Location = new System.Drawing.Point(337, 208);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.NullText = "- Select -";
            this.cmbGender.Size = new System.Drawing.Size(215, 31);
            this.cmbGender.TabIndex = 16;
            this.cmbGender.Text = "- Select -";
            this.cmbGender.ThemeName = "Windows8";
            // 
            // pckBirthDate
            // 
            this.pckBirthDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pckBirthDate.CustomFormat = "yyyy-MM-dd";
            this.pckBirthDate.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.pckBirthDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.pckBirthDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.pckBirthDate.Location = new System.Drawing.Point(337, 250);
            this.pckBirthDate.Name = "pckBirthDate";
            this.pckBirthDate.Size = new System.Drawing.Size(215, 30);
            this.pckBirthDate.TabIndex = 17;
            this.pckBirthDate.TabStop = false;
            this.pckBirthDate.Text = "1970-01-01";
            this.pckBirthDate.ThemeName = "Windows8";
            this.pckBirthDate.Value = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            // 
            // pckJoinDate
            // 
            this.pckJoinDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pckJoinDate.CustomFormat = "yyyy-MM-dd";
            this.pckJoinDate.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.pckJoinDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.pckJoinDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.pckJoinDate.Location = new System.Drawing.Point(337, 291);
            this.pckJoinDate.Name = "pckJoinDate";
            this.pckJoinDate.Size = new System.Drawing.Size(215, 30);
            this.pckJoinDate.TabIndex = 18;
            this.pckJoinDate.TabStop = false;
            this.pckJoinDate.Text = "2015-11-12";
            this.pckJoinDate.ThemeName = "Windows8";
            this.pckJoinDate.Value = new System.DateTime(2015, 11, 12, 12, 28, 49, 646);
            // 
            // cmbMemberType
            // 
            this.cmbMemberType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMemberType.DropDownHeight = 46;
            this.cmbMemberType.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.cmbMemberType.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.cmbMemberType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            radListDataItem3.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            radListDataItem3.Height = 22;
            radListDataItem3.Text = "Regular";
            radListDataItem4.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            radListDataItem4.Height = 22;
            radListDataItem4.Text = "Associate";
            this.cmbMemberType.Items.Add(radListDataItem3);
            this.cmbMemberType.Items.Add(radListDataItem4);
            this.cmbMemberType.Location = new System.Drawing.Point(337, 332);
            this.cmbMemberType.Name = "cmbMemberType";
            this.cmbMemberType.NullText = "- Select -";
            this.cmbMemberType.Size = new System.Drawing.Size(215, 31);
            this.cmbMemberType.TabIndex = 25;
            this.cmbMemberType.Text = "- Select -";
            this.cmbMemberType.ThemeName = "Windows8";
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnReset.Depth = 0;
            this.btnReset.Font = new System.Drawing.Font("Segoe UI Semilight", 14.25F);
            this.btnReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.btnReset.Location = new System.Drawing.Point(278, 485);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnReset.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnReset.Name = "btnReset";
            this.btnReset.Primary = false;
            this.btnReset.Size = new System.Drawing.Size(132, 35);
            this.btnReset.TabIndex = 27;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.Reset_Action);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Depth = 0;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI Semilight", 14.25F);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(420, 485);
            this.btnSave.Margin = new System.Windows.Forms.Padding(6, 7, 48, 7);
            this.btnSave.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSave.Name = "btnSave";
            this.btnSave.Primary = true;
            this.btnSave.Size = new System.Drawing.Size(132, 35);
            this.btnSave.TabIndex = 26;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.Save_Action);
            // 
            // materialLabel8
            // 
            this.materialLabel8.AutoSize = true;
            this.materialLabel8.Depth = 0;
            this.materialLabel8.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.materialLabel8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.materialLabel8.Location = new System.Drawing.Point(48, 335);
            this.materialLabel8.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel8.Name = "materialLabel8";
            this.materialLabel8.Size = new System.Drawing.Size(51, 25);
            this.materialLabel8.TabIndex = 24;
            this.materialLabel8.Text = "Type";
            // 
            // materialLabel7
            // 
            this.materialLabel7.AutoSize = true;
            this.materialLabel7.Depth = 0;
            this.materialLabel7.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.materialLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.materialLabel7.Location = new System.Drawing.Point(48, 294);
            this.materialLabel7.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel7.Name = "materialLabel7";
            this.materialLabel7.Size = new System.Drawing.Size(108, 25);
            this.materialLabel7.TabIndex = 23;
            this.materialLabel7.Text = "Date Joined";
            // 
            // materialLabel6
            // 
            this.materialLabel6.AutoSize = true;
            this.materialLabel6.Depth = 0;
            this.materialLabel6.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.materialLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.materialLabel6.Location = new System.Drawing.Point(48, 253);
            this.materialLabel6.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel6.Name = "materialLabel6";
            this.materialLabel6.Size = new System.Drawing.Size(116, 25);
            this.materialLabel6.TabIndex = 22;
            this.materialLabel6.Text = "Date of Birth";
            // 
            // materialLabel5
            // 
            this.materialLabel5.AutoSize = true;
            this.materialLabel5.Depth = 0;
            this.materialLabel5.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.materialLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.materialLabel5.Location = new System.Drawing.Point(48, 211);
            this.materialLabel5.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel5.Name = "materialLabel5";
            this.materialLabel5.Size = new System.Drawing.Size(72, 25);
            this.materialLabel5.TabIndex = 21;
            this.materialLabel5.Text = "Gender";
            // 
            // materialLabel4
            // 
            this.materialLabel4.AutoSize = true;
            this.materialLabel4.Depth = 0;
            this.materialLabel4.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.materialLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.materialLabel4.Location = new System.Drawing.Point(48, 171);
            this.materialLabel4.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel4.Name = "materialLabel4";
            this.materialLabel4.Size = new System.Drawing.Size(123, 25);
            this.materialLabel4.TabIndex = 20;
            this.materialLabel4.Text = "Middle Name";
            // 
            // txtMiddleName
            // 
            this.txtMiddleName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMiddleName.Depth = 0;
            this.txtMiddleName.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.txtMiddleName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.txtMiddleName.Hint = "";
            this.txtMiddleName.Location = new System.Drawing.Point(337, 168);
            this.txtMiddleName.MaxLength = 32767;
            this.txtMiddleName.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtMiddleName.Name = "txtMiddleName";
            this.txtMiddleName.PasswordChar = '\0';
            this.txtMiddleName.SelectedText = "";
            this.txtMiddleName.SelectionLength = 0;
            this.txtMiddleName.SelectionStart = 0;
            this.txtMiddleName.Size = new System.Drawing.Size(215, 30);
            this.txtMiddleName.TabIndex = 15;
            this.txtMiddleName.TabStop = false;
            this.txtMiddleName.UseSystemPasswordChar = false;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFirstName.Depth = 0;
            this.txtFirstName.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.txtFirstName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.txtFirstName.Hint = "";
            this.txtFirstName.Location = new System.Drawing.Point(337, 130);
            this.txtFirstName.MaxLength = 32767;
            this.txtFirstName.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.PasswordChar = '\0';
            this.txtFirstName.SelectedText = "";
            this.txtFirstName.SelectionLength = 0;
            this.txtFirstName.SelectionStart = 0;
            this.txtFirstName.Size = new System.Drawing.Size(215, 30);
            this.txtFirstName.TabIndex = 14;
            this.txtFirstName.TabStop = false;
            this.txtFirstName.UseSystemPasswordChar = false;
            // 
            // materialLabel3
            // 
            this.materialLabel3.AutoSize = true;
            this.materialLabel3.Depth = 0;
            this.materialLabel3.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.materialLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.materialLabel3.Location = new System.Drawing.Point(48, 133);
            this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel3.Name = "materialLabel3";
            this.materialLabel3.Size = new System.Drawing.Size(100, 25);
            this.materialLabel3.TabIndex = 13;
            this.materialLabel3.Text = "First Name";
            // 
            // txtLastName
            // 
            this.txtLastName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLastName.Depth = 0;
            this.txtLastName.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.txtLastName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.txtLastName.Hint = "";
            this.txtLastName.Location = new System.Drawing.Point(337, 92);
            this.txtLastName.MaxLength = 32767;
            this.txtLastName.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.PasswordChar = '\0';
            this.txtLastName.SelectedText = "";
            this.txtLastName.SelectionLength = 0;
            this.txtLastName.SelectionStart = 0;
            this.txtLastName.Size = new System.Drawing.Size(215, 30);
            this.txtLastName.TabIndex = 12;
            this.txtLastName.TabStop = false;
            this.txtLastName.UseSystemPasswordChar = false;
            // 
            // materialLabel2
            // 
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.materialLabel2.Location = new System.Drawing.Point(48, 95);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(100, 25);
            this.materialLabel2.TabIndex = 11;
            this.materialLabel2.Text = "Last Name";
            // 
            // lblMemberNo
            // 
            this.lblMemberNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMemberNo.AutoSize = true;
            this.lblMemberNo.Depth = 0;
            this.lblMemberNo.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.lblMemberNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.lblMemberNo.Location = new System.Drawing.Point(331, 58);
            this.lblMemberNo.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblMemberNo.Name = "lblMemberNo";
            this.lblMemberNo.Size = new System.Drawing.Size(66, 25);
            this.lblMemberNo.TabIndex = 10;
            this.lblMemberNo.Text = "100001";
            this.lblMemberNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.materialLabel1.Location = new System.Drawing.Point(48, 55);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(104, 25);
            this.materialLabel1.TabIndex = 9;
            this.materialLabel1.Text = "Member ID";
            // 
            // MemberCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Snow;
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbMemberType);
            this.Controls.Add(this.materialLabel8);
            this.Controls.Add(this.materialLabel7);
            this.Controls.Add(this.materialLabel6);
            this.Controls.Add(this.materialLabel5);
            this.Controls.Add(this.materialLabel4);
            this.Controls.Add(this.pckJoinDate);
            this.Controls.Add(this.pckBirthDate);
            this.Controls.Add(this.cmbGender);
            this.Controls.Add(this.txtMiddleName);
            this.Controls.Add(this.txtFirstName);
            this.Controls.Add(this.materialLabel3);
            this.Controls.Add(this.txtLastName);
            this.Controls.Add(this.materialLabel2);
            this.Controls.Add(this.lblMemberNo);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.separator1);
            this.Name = "MemberCreate";
            this.Size = new System.Drawing.Size(600, 540);
            this.Load += new System.EventHandler(this.LoadEvent);
            ((System.ComponentModel.ISupportInitialize)(this.cmbGender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pckBirthDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pckJoinDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbMemberType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SwingWERX.Controls.Separator separator1;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialLabel lblMemberNo;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtLastName;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtFirstName;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtMiddleName;
        private Telerik.WinControls.UI.RadDropDownList cmbGender;
        private Telerik.WinControls.Themes.Windows8Theme w8;
        private Telerik.WinControls.UI.RadDateTimePicker pckBirthDate;
        private Telerik.WinControls.UI.RadDateTimePicker pckJoinDate;
        private MaterialSkin.Controls.MaterialLabel materialLabel4;
        private MaterialSkin.Controls.MaterialLabel materialLabel5;
        private MaterialSkin.Controls.MaterialLabel materialLabel6;
        private MaterialSkin.Controls.MaterialLabel materialLabel7;
        private MaterialSkin.Controls.MaterialLabel materialLabel8;
        private Telerik.WinControls.UI.RadDropDownList cmbMemberType;
        private MaterialSkin.Controls.MaterialRaisedButton btnSave;
        private MaterialSkin.Controls.MaterialFlatButton btnReset;
    }
}
