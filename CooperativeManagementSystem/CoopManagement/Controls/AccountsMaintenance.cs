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

using Telerik.WinControls.UI;

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
            // Combobox
            AccountCategory[] categories = AccountCategory.All<AccountCategory>().Get<AccountCategory>();
            cmbCategories.DropDownHeight = ((categories.Length>=4?4:categories.Length) * 20) + 2;
            cmbCategories.Items.Add (new RadListDataItem() { Text = "All Categories", Value = '*', Font = new Font("Segoe UI Light", 12, FontStyle.Regular), Height = 20 });

            for (int i=0;i<categories.Length;i++)
            {
                RadListDataItem item = new RadListDataItem()
                {
                    Text = categories[i].Title,
                    Value = categories[i].CategoryID,
                    Font = new Font("Segoe UI Light", 12, FontStyle.Regular),
                    Height = 20
                };
                cmbCategories.Items.Add(item);
            }
            cmbCategories.Text = "- Select -";

            // grid

        }

        private async void CreateAccount_Action(object sender, EventArgs e)
        {
            await Task.Delay(250);
            new Forms.CreateAccount().ShowDialog();
        }

        private async void CreateCat_Event(object sender, EventArgs e)
        {
            await Task.Delay(250);
            new Forms.CreateCategory().ShowDialog();
        }
    }
}
