using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoopManagement.Core;
namespace CoopManagement.Models
{
    public class User : Model
    {
        public String UserID;
        public String Username;
        public String Password;
        public String LastName;
        public String FirstName;
        public String MiddleName;
        public DateTime DateOfBirth;
        public int Age;

        public User()
        {

        }

        protected override string[] Fillable
        {
            get
            {
                return new string[] {
                    "Username", "Password",
                    "LastName", "FirstName", "MiddleName",
                    "DateOfBirth", "Age"
                };
            }
        }

        protected override string TableName
        {
            get
            {
                return "tbl_users";
            }
        }

        protected override string PrimaryKey
        {
            get
            {
                return "UserID";
            }
        }

        protected override string[] Dates
        {
            get
            {
                return new string[] {
                    "DateOfBirth"
                };
            }
        }
    }
}
