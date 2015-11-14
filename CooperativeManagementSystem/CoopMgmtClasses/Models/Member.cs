using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoopManagement.Core;
namespace CoopManagement.Models
{
    public class Member : Model
    {
		public int MemberID;
		public int Pin;
		public string LastName;
		public string FirstName;
		public string MiddleName;
		public char Gender;
		public DateTime BirthDate;
		public DateTime Joined;
		public char Type;


        public Member()
        {

        }

        protected override string[] Fillable
        {
            get
            {
                return new string[] {
					"LastName",
					"FirstName",
					"MiddleName",
					"Gender",
					"BirthDate",
					"Joined",
					"Type"
                };
            }
        }

        protected override string TableName
        {
            get
            {
                return "tbl_members";
            }
        }

        protected override string PrimaryKey
        {
            get
            {
                return "MemberID";
            }
        }

        protected override string[] Dates
        {
            get
            {
                return new string[] {
                    "BirthDate", "Joined"
                };
            }
        }

    }
    
}
