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

using MySql.Data.MySqlClient;

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
            Reload();
        }

        public void Reload()
        {
            // Combobox
            AccountCategory[] categories = AccountCategory.All<AccountCategory>().Get<AccountCategory>();
            cmbCategories.DropDownHeight = ((categories.Length >= 4 ? 4 : categories.Length) * 20) + 2;
            cmbCategories.Items.Add(new RadListDataItem() { Text = "All Categories", Value = '*', Font = new Font("Segoe UI Light", 12, FontStyle.Regular), Height = 20 });

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
            cmbCategories.Text = "- Select -";
            cmbCategories.SelectedIndex = 0;

            // grid
            this.gridView.Rows.Clear(); // clear shit first.
            using (MySqlDataReader reader = LedgerAccount.All<LedgerAccount>().Get())
            {
                this.gridView.MasterTemplate.AllowAddNewRow = false;
                this.gridView.MasterTemplate.AutoGenerateColumns = false;
                this.gridView.TableElement.BeginUpdate();
                this.gridView.MasterTemplate.LoadFrom(reader);
                this.gridView.MasterTemplate.Columns["ID"].Width = 35;
                this.gridView.MasterTemplate.Columns["ID"].MaxWidth = 40;
                this.gridView.MasterTemplate.Columns["ID"].HeaderText = "Item";
                this.gridView.MasterTemplate.Columns["ID"].TextAlignment = ContentAlignment.MiddleRight;

                this.gridView.MasterTemplate.Columns["AccountID"].Width = 75;
                this.gridView.MasterTemplate.Columns["AccountID"].MaxWidth = 75;
                this.gridView.MasterTemplate.Columns["AccountID"].HeaderText = "Code";
                this.gridView.MasterTemplate.Columns["AccountID"].TextAlignment = ContentAlignment.MiddleCenter;

                this.gridView.MasterTemplate.Columns["Title"].Width = 200;
                this.gridView.MasterTemplate.Columns["Title"].MaxWidth = 250;
                this.gridView.MasterTemplate.Columns["Title"].HeaderText = "Title";
                this.gridView.MasterTemplate.Columns["Title"].TextAlignment = ContentAlignment.MiddleLeft;

                this.gridView.MasterTemplate.Columns["Description"].Width = 300;
                this.gridView.MasterTemplate.Columns["Description"].HeaderText = "Description";
                this.gridView.MasterTemplate.Columns["Description"].TextAlignment = ContentAlignment.MiddleLeft;

                this.gridView.MasterTemplate.Columns["Balance"].Width = 120;
                this.gridView.MasterTemplate.Columns["Balance"].MaxWidth = 120;
                this.gridView.MasterTemplate.Columns["Balance"].HeaderText = "Balance";
                this.gridView.MasterTemplate.Columns["Balance"].TextAlignment = ContentAlignment.MiddleRight;

                this.gridView.MasterTemplate.Columns["Type"].Width = 80;
                this.gridView.MasterTemplate.Columns["Type"].MaxWidth = 80;
                this.gridView.MasterTemplate.Columns["Type"].HeaderText = "Type";
                this.gridView.MasterTemplate.Columns["Type"].TextAlignment = ContentAlignment.MiddleCenter;


                this.gridView.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                this.gridView.TableElement.EndUpdate();
                this.gridView.CurrentRow = null;
            }
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

        private void CellFormatEvent(object sender, CellFormattingEventArgs e)
        {
            String ColumnName = e.Column.Name;
            Object Value = e.Row.Cells[ColumnName].Value;

            Console.WriteLine("Column: {0}\nValue: {1}\n\n", e.Column.Name, e.CellElement.Text);
            if (ColumnName == "Type")
            {
                e.CellElement.Text = object.Equals(Value, "D") ? "Debit" : "Credit";
            }
            if (ColumnName == "Balance")
            {
                e.CellElement.Text = ((decimal)Value).ToString("₱# ##0.00");
            }
            if (ColumnName == "ID")
            {
                e.CellElement.Text = String.Format("{0}", e.CellElement.RowIndex + 1);
            }

        }
    }
}
