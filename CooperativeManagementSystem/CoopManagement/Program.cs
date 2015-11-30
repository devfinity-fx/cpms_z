using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.VisualBasic.ApplicationServices;  // Add reference to Microsoft.VisualBasic!!


// remove after
using CoopManagement.Core;
using CoopManagement.Models;

namespace CoopManagement
{
    class Program : WindowsFormsApplicationBase
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var App = new Program();

            App.EnableVisualStyles = true;
            App.ShutdownStyle = ShutdownMode.AfterAllFormsClose;
            App.MainForm = new Login();
            App.Run(args);
            


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
