using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


// remove after
using CoopManagement.Core;
using CoopManagement.Models;

namespace CoopManagement
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new testform());


            /*
            User[] result = User.All<User>().Get<User>("Username", "Password");

            for(int i=0;i<result.Length;i++)
            {
                User usr = result[i];
                Console.WriteLine("Username: {0}", usr.Username);
            }
            */

        }
    }
}
