using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoopManagement.Classes
{
    public class Queries : Dictionary<String, String>
    {

        private static Dictionary<String, String> Qrys = new Dictionary<String, String>()
        {
            {"SESSION_START",                "INSERT INTO tbl_sessions(SessionID, UserID, DateLogged, LoginTime) VALUES('{0}', {1}, NOW(), NOW());" },
            {"SESSION_DESTROY",              "UPDATE tbl_sessions SET LogoutTime = NOW() WHERE SessionID = '{0}';" },
        };

        public static Dictionary<String, String> Data { get { return Qrys; } }
    }
}
