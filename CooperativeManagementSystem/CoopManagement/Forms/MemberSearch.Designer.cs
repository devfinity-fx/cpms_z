namespace CoopManagement.Forms
{
    partial class MemberSearch
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
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            this.txtSearch = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.gridView = new Telerik.WinControls.UI.RadGridView();
            this.w8 = new Telerik.WinControls.Themes.Windows8Theme();
            this.btnContinue = new MaterialSkin.Controls.MaterialRaisedButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView.MasterTemplate)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.SystemColors.Window;
            this.txtSearch.Depth = 0;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.txtSearch.Hint = "Enter Member Number or Name";
            this.txtSearch.Location = new System.Drawing.Point(12, 273);
            this.txtSearch.MaxLength = 32767;
            this.txtSearch.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PasswordChar = '\0';
            this.txtSearch.SelectedText = "";
            this.txtSearch.SelectionLength = 0;
            this.txtSearch.SelectionStart = 0;
            this.txtSearch.Size = new System.Drawing.Size(370, 27);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TabStop = false;
            this.txtSearch.UseSystemPasswordChar = false;
            // 
            // gridView
            // 
            this.gridView.Location = new System.Drawing.Point(12, 72);
            // 
            // gridView
            // 
            this.gridView.MasterTemplate.AllowColumnReorder = false;
            gridViewTextBoxColumn1.HeaderText = "column1";
            gridViewTextBoxColumn1.Name = "column1";
            gridViewTextBoxColumn2.HeaderText = "column2";
            gridViewTextBoxColumn2.Name = "column2";
            this.gridView.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2});
            this.gridView.Name = "gridView";
            this.gridView.ReadOnly = true;
            this.gridView.Size = new System.Drawing.Size(370, 195);
            this.gridView.TabIndex = 1;
            this.gridView.Text = "radGridView1";
            this.gridView.ThemeName = "Windows8";
            // 
            // btnContinue
            // 
            this.btnContinue.Depth = 0;
            this.btnContinue.Font = new System.Drawing.Font("Segoe UI Semilight", 14.25F);
            this.btnContinue.ForeColor = System.Drawing.Color.White;
            this.btnContinue.Location = new System.Drawing.Point(12, 313);
            this.btnContinue.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnContinue.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Primary = true;
            this.btnContinue.Size = new System.Drawing.Size(370, 30);
            this.btnContinue.TabIndex = 18;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = true;
            // 
            // MemberSearch
            // 
            this.AccentColor = System.Drawing.Color.Gray;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 357);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.gridView);
            this.Controls.Add(this.txtSearch);
            this.DarkPrimaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.Font = new System.Drawing.Font("Segoe UI Semilight", 14F);
            this.ForeColor = System.Drawing.Color.Snow;
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MemberSearch";
            this.PrimaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(37)))), ((int)(((byte)(48)))));
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MemberSearch";
            ((System.ComponentModel.ISupportInitialize)(this.gridView.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialSkin.Controls.MaterialSingleLineTextField txtSearch;
        private Telerik.WinControls.UI.RadGridView gridView;
        private Telerik.WinControls.Themes.Windows8Theme w8;
        private MaterialSkin.Controls.MaterialRaisedButton btnContinue;
    }
}