using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MaterialSkin;
using MaterialSkin.Controls;

using CoopManagement.Models;
using CoopManagement.Classes;

namespace CoopManagement
{
    public partial class Login : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public Login()
        {
            InitializeComponent();

            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

        }

        private async void Login_Action(object sender, EventArgs e)
        {
            await Task.Delay(50);
            DoLogin();
        }


        private String GenerateID()
        {
            const String str = "ABCDEFGHIJKLMNOPRQRSTUVWXUZ0123456789";
            Random rand = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 15; i++)
            {
                sb.Append(str[rand.Next(str.Length)]);
            }
            return sb.ToString();
        }

        private async void KeyUpEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                this.ActiveControl = btnLogin;
                await Task.Delay(50);
                DoLogin();
            }
        }

        private void DoLogin()
        {
            String Username = txtUsername.Text;
            String Password = txtPassword.Text;
            if (!String.IsNullOrEmpty(Username) && !String.IsNullOrEmpty(Password))
            {
                try
                {
                    User user = User.Find<User>(txtUsername.Text, "Username");
                    if (user.Password == txtPassword.Text)
                    {
                        MessageBox.Show(this, "You are now logged in.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        new Session()
                        {
                            SessionID = GenerateID(),
                            UserID = user.UserID
                        }.Start();

                        Close();
                    }
                    else
                    {
                        // incorrect password
                        MessageBox.Show(this, "You have entered an incorrect password.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txtPassword.Clear();
                        this.ActiveControl = btnClear;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "User does not exist.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtPassword.Clear();
                    txtUsername.Clear();
                    this.ActiveControl = btnClear;
                }
            }
            else
            {
                MessageBox.Show(this, "Please enter your credentials.", "Input Required!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void FormClosingEvent(object sender, FormClosingEventArgs e)
        {
            new Main().Show();
        }

        private void LoadEvent(object sender, EventArgs e)
        {
            this.ActiveControl = btnLogin;
        }
    }
}
