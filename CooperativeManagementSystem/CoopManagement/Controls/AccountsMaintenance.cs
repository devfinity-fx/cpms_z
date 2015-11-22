using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoopManagement.Controls
{
    public partial class AccountsMaintenance : UserControl
    {
        public AccountsMaintenance()
        {
            InitializeComponent();
        }

        private void LoadEvent(object sender, EventArgs e)
        {

        }

        private void CreateAccount_Action(object sender, EventArgs e)
        {
            new Forms.CreateAccount().ShowDialog();
        }

        private void CreateCat_Event(object sender, EventArgs e)
        {
            new Forms.CreateCategory().ShowDialog();
        }
    }
}
