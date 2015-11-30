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

using Telerik.WinControls.UI;
namespace CoopManagement.Forms
{
    public partial class CreateAccount : MaterialForm
    {
        public CreateAccount()
        {
            InitializeComponent();
        }

        private void LoadEvent(object sender, EventArgs e)
        {
            // types
            cmbType.Items.Clear();
            String[] Types = {"Debit", "Credit"};
            cmbCategories.DropDownHeight = 42;
            for (int i=0;i<Types.Length;i++)
            {
                RadListDataItem item = new RadListDataItem()
                {
                    Text = Types[i],
                    Value = Types[i][0],
                    Font = new Font("Segoe UI Light", 12, FontStyle.Regular),
                    Height = 20
                };
                cmbType.Items.Add(item);
            }
            cmbType.Text = "- Select -";

            // category
            cmbCategories.Items.Clear();
            AccountCategory[] categories = AccountCategory.All<AccountCategory>().Get<AccountCategory>();
            for (int i = 0; i < categories.Length; i++)
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
            cmbCategories.Items.Add(new RadListDataItem()
            {
                Text = "Create New...",
                Value = -1,
                Font = new Font("Segoe UI Light", 12, FontStyle.Italic),
                Height = 20
            });
            cmbCategories.DropDownHeight = ((categories.Length >= 4 ? 4 : categories.Length) * 20) + 2;
            cmbCategories.Text = "- Select -";


        }

        private void Category_Changed(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if(e.Position==cmbCategories.Items.Count-1)
            {
                Hide();
                new CreateCategory(1).ShowDialog();
                Close();
            }
        }

        private void Save_Action(object sender, EventArgs e)
        {
            if(cmbCategories.SelectedIndex>=0 &&
                !String.IsNullOrEmpty(txtTitle.Text) &&
                !String.IsNullOrEmpty(txtDescription.Text) &&
                cmbType.SelectedIndex>=0 &&
                !String.IsNullOrEmpty(txtInitialBalance.Text)
                )
            {
                int CategoryID          = int.Parse(cmbCategories.SelectedItem.Value.ToString());
                String Title            = txtTitle.Text;
                String Description      = txtDescription.Text;
                char Type               = cmbType.SelectedItem.Text[0];
                decimal InitialBalance  = decimal.Parse(txtInitialBalance.Text);

                if(new LedgerAccount()
                {
                    Title = Title,
                    Category = CategoryID,
                    Description = Description,
                    Type = Type,
                    Balance = InitialBalance
                }.Save())
                {
                    MessageBox.Show(this, "You have successfully added a new ledger account.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            Console.WriteLine("Create Account Closing");
        }
    }
}
