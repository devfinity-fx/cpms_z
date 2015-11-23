using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoopManagement.Core;

namespace CoopManagement.Models
{
    public class LedgerAccount : Model
    {
        public int AccountID;
        public string Title;
        public string Description;
        public char Type;
        public int Category;
        public decimal Balance;

        public LedgerAccount()
        {

        }

        protected override string[] Fillable
        {
            get
            {
                return new string[] {
                    "Title",
                    "Description",
                    "Type",
                    "Category",
                    "Balance"
                };
            }
        }

        protected override string TableName
        {
            get
            {
                return "tbl_accounts";
            }
        }

        protected override string PrimaryKey
        {
            get
            {
                return "AccountID";
            }
        }
    }
}
