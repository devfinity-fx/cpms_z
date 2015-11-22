using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoopManagement.Core;

namespace CoopManagement.Models
{
    public class AccountCategory : Model
    {
        public int CategoryID;
        public string Title;
        public string Description;

        public AccountCategory()
        {

        }

        protected override string[] Fillable
        {
            get
            {
                return new string[] {
                    "Title",
                    "Description"
                };
            }
        }

        protected override string TableName
        {
            get
            {
                return "tbl_acctcat";
            }
        }

        protected override string PrimaryKey
        {
            get
            {
                return "CategoryID";
            }
        }
    }
}
