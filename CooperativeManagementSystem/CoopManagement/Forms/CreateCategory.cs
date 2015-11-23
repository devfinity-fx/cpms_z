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

namespace CoopManagement.Forms
{
    public partial class CreateCategory : MaterialForm
    {
        private int ExitCode = 0;

        public CreateCategory()
        {
            InitializeComponent();
        }

        public CreateCategory(int ExitCode)
        {
            InitializeComponent();
            this.ExitCode = ExitCode;
        }

        private void LoadEvent(object sender, EventArgs e)
        {
            this.ActiveControl = separator2;
        }

        private void Create_Action(object sender, EventArgs e)
        {
            String Title = txtTitle.Text;
            String Description = txtDescription.Text;

            if(!String.IsNullOrEmpty(Title) && !String.IsNullOrEmpty(Description))
            {
                if (new AccountCategory()
                {
                    Title = Title,
                    Description = Description
                }.Save())
                {
                    MessageBox.Show(this, "You have successfully added a new category.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
            }
            else
            {
                MessageBox.Show(this, "Please fill up all required fields.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FormClosingEvent(object sender, FormClosingEventArgs e)
        {
            if(ExitCode==1)
            {
                new CreateAccount().ShowDialog();
            }
        }
    }
}
