namespace CoopManagement.Controls
{
    partial class MemberAccounts
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
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn5 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn6 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn7 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            this.separator1 = new SwingWERX.Controls.Separator();
            this.gridView = new Telerik.WinControls.UI.RadGridView();
            this.txtSearch = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.w8 = new Telerik.WinControls.Themes.Windows8Theme();
            this.btnCreateAcct = new MaterialSkin.Controls.MaterialRaisedButton();
            this.gridPanel = new SwingWERX.Controls.SwxPanel();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView.MasterTemplate)).BeginInit();
            this.gridPanel.SuspendLayout();
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
            this.separator1.Text = "Subsidiary Ledger — Members Maintenance";
            // 
            // gridView
            // 
            this.gridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridView.BackColor = System.Drawing.Color.Snow;
            this.gridView.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridView.EnableHotTracking = false;
            this.gridView.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.gridView.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gridView.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gridView.Location = new System.Drawing.Point(0, 1);
            // 
            // gridView
            // 
            this.gridView.MasterTemplate.AllowAddNewRow = false;
            this.gridView.MasterTemplate.AllowColumnReorder = false;
            gridViewTextBoxColumn1.HeaderText = "#";
            gridViewTextBoxColumn1.Name = "ID";
            gridViewTextBoxColumn2.EnableExpressionEditor = false;
            gridViewTextBoxColumn2.HeaderText = "Member ID";
            gridViewTextBoxColumn2.Name = "MemberID";
            gridViewTextBoxColumn2.Width = 103;
            gridViewTextBoxColumn3.EnableExpressionEditor = false;
            gridViewTextBoxColumn3.HeaderText = "Last Name";
            gridViewTextBoxColumn3.IsVisible = false;
            gridViewTextBoxColumn3.Name = "LastName";
            gridViewTextBoxColumn4.EnableExpressionEditor = false;
            gridViewTextBoxColumn4.HeaderText = "First Name";
            gridViewTextBoxColumn4.IsVisible = false;
            gridViewTextBoxColumn4.Name = "FirstName";
            gridViewTextBoxColumn5.EnableExpressionEditor = false;
            gridViewTextBoxColumn5.HeaderText = "Middle Name";
            gridViewTextBoxColumn5.IsVisible = false;
            gridViewTextBoxColumn5.Name = "MiddleName";
            gridViewTextBoxColumn6.EnableExpressionEditor = false;
            gridViewTextBoxColumn6.HeaderText = "Name";
            gridViewTextBoxColumn6.Name = "FullName";
            gridViewTextBoxColumn6.Width = 298;
            gridViewTextBoxColumn7.EnableExpressionEditor = false;
            gridViewTextBoxColumn7.HeaderText = "Type";
            gridViewTextBoxColumn7.Name = "Type";
            gridViewTextBoxColumn7.Width = 43;
            this.gridView.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4,
            gridViewTextBoxColumn5,
            gridViewTextBoxColumn6,
            gridViewTextBoxColumn7});
            this.gridView.Name = "gridView";
            this.gridView.ReadOnly = true;
            this.gridView.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gridView.ShowGroupPanel = false;
            this.gridView.Size = new System.Drawing.Size(580, 403);
            this.gridView.TabIndex = 11;
            this.gridView.Text = "radGridView1";
            this.gridView.ThemeName = "Windows8";
            this.gridView.CellFormatting += new Telerik.WinControls.UI.CellFormattingEventHandler(this.CellFormattingEvent);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Depth = 0;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.txtSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.txtSearch.Hint = "Search";
            this.txtSearch.Location = new System.Drawing.Point(434, 47);
            this.txtSearch.MaxLength = 32767;
            this.txtSearch.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PasswordChar = '\0';
            this.txtSearch.SelectedText = "";
            this.txtSearch.SelectionLength = 0;
            this.txtSearch.SelectionStart = 0;
            this.txtSearch.Size = new System.Drawing.Size(156, 27);
            this.txtSearch.TabIndex = 32;
            this.txtSearch.TabStop = false;
            this.txtSearch.UseSystemPasswordChar = false;
            // 
            // btnCreateAcct
            // 
            this.btnCreateAcct.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCreateAcct.Depth = 0;
            this.btnCreateAcct.Font = new System.Drawing.Font("Segoe UI Semilight", 14.25F);
            this.btnCreateAcct.ForeColor = System.Drawing.Color.White;
            this.btnCreateAcct.Location = new System.Drawing.Point(216, 493);
            this.btnCreateAcct.Margin = new System.Windows.Forms.Padding(6, 7, 48, 7);
            this.btnCreateAcct.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnCreateAcct.Name = "btnCreateAcct";
            this.btnCreateAcct.Primary = true;
            this.btnCreateAcct.Size = new System.Drawing.Size(168, 35);
            this.btnCreateAcct.TabIndex = 33;
            this.btnCreateAcct.Text = "Create Account";
            this.btnCreateAcct.UseVisualStyleBackColor = true;
            this.btnCreateAcct.Click += new System.EventHandler(this.CreateAccount_Action);
            // 
            // gridPanel
            // 
            this.gridPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridPanel.BackColor = System.Drawing.Color.Transparent;
            this.gridPanel.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.gridPanel.BorderWidth = 1;
            this.gridPanel.Controls.Add(this.gridView);
            this.gridPanel.Location = new System.Drawing.Point(10, 80);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(580, 404);
            this.gridPanel.TabIndex = 34;
            this.gridPanel.Text = "swxPanel1";
            // 
            // MemberAccounts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Snow;
            this.Controls.Add(this.gridPanel);
            this.Controls.Add(this.btnCreateAcct);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.separator1);
            this.Name = "MemberAccounts";
            this.Size = new System.Drawing.Size(600, 540);
            this.Load += new System.EventHandler(this.LoadEvent);
            ((System.ComponentModel.ISupportInitialize)(this.gridView.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.gridPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SwingWERX.Controls.Separator separator1;
        private Telerik.WinControls.UI.RadGridView gridView;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtSearch;
        private Telerik.WinControls.Themes.Windows8Theme w8;
        private MaterialSkin.Controls.MaterialRaisedButton btnCreateAcct;
        private SwingWERX.Controls.SwxPanel gridPanel;
    }
}
