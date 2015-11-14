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
        private DateTime DefaultJoin;
        public MemberCreate()
        {
            InitializeComponent();
            DefaultDoB = pckBirthDate.Value;
            //DefaultJoin = pckJoinDate.Value;
        }

        private void LoadEvent(object sender, EventArgs e)
        {
            this.ActiveControl = lblMemberNo;
            pckJoinDate.Value = DateTime.Now;

            lblMemberNo.Text = String.Format("{0}",Member.Last<Member>().Get<Member>()[0].MemberID).PadLeft(5, '0');
        }

        private void Save_Action(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtLastName.Text) &&
                !String.IsNullOrEmpty(txtFirstName.Text) &&
                !String.IsNullOrEmpty(txtMiddleName.Text) &&
                cmbGender.SelectedIndex >= 0 &&
                pckBirthDate.Value != DefaultDoB && 
                // pckJoinDate.Value != DefaultJoin && // checking removed.
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

            
        }
    }
}
