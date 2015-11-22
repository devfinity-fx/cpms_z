using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MaterialSkin;
using MaterialSkin.Controls;

namespace CoopManagement.Controls
{
    public partial class LoanRequest : UserControl
    {
        public LoanRequest()
        {
            InitializeComponent();
        }

        private void materialLabel7_Click(object sender, EventArgs e)
        {

        }

        private void LoanRequest_Load(object sender, EventArgs e)
        {

        }
        /*
        public void DoReset()
        {
            this.ActiveControl = lblMemberNo;

            int LastID = LoanRequest.Last<Member>().Get<Member>()[0].MemberID;

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
        */
    }

}
