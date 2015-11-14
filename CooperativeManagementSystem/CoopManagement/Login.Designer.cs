namespace CoopManagement
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.txtUsername = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.txtPassword = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.btnLogin = new MaterialSkin.Controls.MaterialRaisedButton();
            this.btnClear = new MaterialSkin.Controls.MaterialFlatButton();
            this.SuspendLayout();
            // 
            // txtUsername
            // 
            this.txtUsername.Depth = 0;
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.txtUsername.Hint = "Username";
            this.txtUsername.Location = new System.Drawing.Point(41, 110);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(32, 3, 32, 10);
            this.txtUsername.MaxLength = 32767;
            this.txtUsername.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.PasswordChar = '\0';
            this.txtUsername.SelectedText = "";
            this.txtUsername.SelectionLength = 0;
            this.txtUsername.SelectionStart = 0;
            this.txtUsername.Size = new System.Drawing.Size(305, 27);
            this.txtUsername.TabIndex = 1;
            this.txtUsername.TabStop = false;
            this.txtUsername.UseSystemPasswordChar = false;
            // 
            // txtPassword
            // 
            this.txtPassword.Depth = 0;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.txtPassword.Hint = "Password";
            this.txtPassword.Location = new System.Drawing.Point(41, 155);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.txtPassword.MaxLength = 32767;
            this.txtPassword.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '\0';
            this.txtPassword.SelectedText = "";
            this.txtPassword.SelectionLength = 0;
            this.txtPassword.SelectionStart = 0;
            this.txtPassword.Size = new System.Drawing.Size(305, 27);
            this.txtPassword.TabIndex = 2;
            this.txtPassword.TabStop = false;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // btnLogin
            // 
            this.btnLogin.Depth = 0;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI Semilight", 14.25F);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(198, 203);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnLogin.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Primary = true;
            this.btnLogin.Size = new System.Drawing.Size(148, 35);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClear.Depth = 0;
            this.btnClear.Location = new System.Drawing.Point(41, 203);
            this.btnClear.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnClear.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnClear.Name = "btnClear";
            this.btnClear.Primary = false;
            this.btnClear.Size = new System.Drawing.Size(148, 35);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // Login
            // 
            this.AccentColor = System.Drawing.Color.Gray;
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 276);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsername);
            this.DarkPrimaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.Font = new System.Drawing.Font("Segoe UI Semilight", 15F);
            this.ForeColor = System.Drawing.Color.Snow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.PrimaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(37)))), ((int)(((byte)(48)))));
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Login_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialSkin.Controls.MaterialSingleLineTextField txtUsername;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtPassword;
        private MaterialSkin.Controls.MaterialRaisedButton btnLogin;
        private MaterialSkin.Controls.MaterialFlatButton btnClear;
    }
}