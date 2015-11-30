using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoopManagement.Core;
using CoopManagement.Classes;

namespace CoopManagement.Models
{
    public class Session : Model
    {
        public String SessionID;
        public int UserID;
        public DateTime DateLogged;
        public DateTime LoginTime;
        public DateTime LogoutTime;


        protected override string[] Fillable
        {
            get
            {
                return new string[] {
                    "SessionID", "UserID",
                    "DateLogged", "LoginTime", "LogoutTime"
                };
            }
        }

        protected override string TableName
        {
            get
            {
                return "tbl_sessions";
            }
        }

        protected override string PrimaryKey
        {
            get
            {
                return "SessionID";
            }
        }

        protected override string[] Dates
        {
            get
            {
                return new string[] {
                    "DateLogged",
                    // "LoginTime", "LogoutTime" are disregarded since NOW() will be the one used for it.
                };
            }
        }


        // damn special case when i am too lazy to create a separate shit for just this.
        public bool Start()
        {
            return ConnectionManager.ExecuteCommand(Queries.Data["SESSION_START"], SessionID, UserID);
        }
    }
}
