using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CoopManagement.Models;

namespace CoopManagement.Controls
{
    public partial class MemberCreate : UserControl
    {
        private DateTime DefaultDoB;
        public MemberCreate()
        {
            InitializeComponent();
            DefaultDoB = pckBirthDate.Value;
        }

        private void LoadEvent(object sender, EventArgs e)
        {
            DoReset();
        }

        private void Save_Action(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtLastName.Text) &&
                !String.IsNullOrEmpty(txtFirstName.Text) &&
                !String.IsNullOrEmpty(txtMiddleName.Text) &&
                cmbGender.SelectedIndex >= 0 &&
                pckBirthDate.Value != DefaultDoB && 
                cmbMemberType.SelectedIndex >=0
                )
            {
                if(new Member()
                {
                    LastName = txtLastName.Text,
                    FirstName = txtFirstName.Text,
                    MiddleName = txtMiddleName.Text,
                    Gender = cmbGender.SelectedItem.ToString()[0],
                    BirthDate = pckBirthDate.Value,
                    Joined = pckJoinDate.Value,
                    Type = cmbMemberType.SelectedItem.ToString()[0]
                }.Save())
                {
                    MessageBox.Show(this, "You have successfully added a new member.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show(this, "Please fill up all required fields.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Reset_Action(object sender, EventArgs e)
        {
            DoReset();
        }

        public void DoReset()
        {
            this.ActiveControl = lblMemberNo;
            pckJoinDate.Value = DateTime.Now;

            int LastID = Member.Last<Member>().Get<Member>()[0].MemberID;

            lblMemberNo.Text = String.Format("{0}", LastID + 1).PadLeft(5, '0');
            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtLastName.Clear();
            cmbGender.SelectedIndex = -1;
            cmbGender.Text = "- Select -";
            cmbMemberType.SelectedIndex = -1;
            cmbMemberType.Text = "- Select -";
            pckJoinDate.Value = DateTime.Today;
            pckBirthDate.Value = DefaultDoB;
        }
    }
}