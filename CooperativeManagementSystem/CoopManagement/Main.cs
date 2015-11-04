using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CoopManagement.Classes;
using CoopManagement.Models;

using SwingWERX.Controls;

namespace CoopManagement
{
    public partial class Main : Form
    {
        private List<StripButton> navButtons;
        public Main()
        {
            InitializeComponent();
            navButtons = new List<StripButton>();
            navButtons.Add(stripButton0);
            navButtons.Add(stripButton1);
            navButtons.Add(stripButton2);
            navButtons.Add(stripButton3);
            navButtons.Add(stripButton4);
            navButtons.Add(stripButton5);
            navButtons.Add(stripButton6);
            navButtons.Add(stripButton7);
            navButtons.Add(stripButton8);
        }

        private void LoadEvent(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.WindowState = FormWindowState.Maximized;

            User user = User.Find<User>("shit");

            User usr = new User();
            /*usr.UserID = "2012-1030F";
            usr.Username = "ajamiscosa";
            usr.Password = "9951354a";
            usr.LastName = "Amiscosa";
            usr.FirstName = "Aron Jhed";
            usr.MiddleName = "Bautista";*/
            usr.DateOfBirth = DateTime.Parse("1992-04-24");
            usr.Age = 10;
            usr.Update();
        }

        private void ClickEvent(object sender, EventArgs e)
        {
            StripButton btn = (StripButton)sender;
            for(int i=0;i<navButtons.Count;i++)
            {
                if(navButtons[i]==btn)
                {
                    navButtons[i].Checked = true;
                }
                else
                {
                    navButtons[i].Checked = false;
                }
            }
        }
    }
}
