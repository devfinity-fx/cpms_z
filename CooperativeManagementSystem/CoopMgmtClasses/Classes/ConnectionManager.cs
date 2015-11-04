using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using MySql.Data.MySqlClient;

namespace CoopManagement.Classes
{
    public class ConnectionManager
    {
        private static MySqlConnection _MySqlConnection;

        public static String ConnectionString = @"Server=localhost;Database=enr;Uid=root;Pwd=mindwerx;";
        private ConnectionManager()
        {

            if (_MySqlConnection == null || _MySqlConnection.State == System.Data.ConnectionState.Closed)
            {
                _MySqlConnection = new MySqlConnection(@"Server=localhost;Database=enr;Uid=root;Pwd=mindwerx;");
                // replace [mindwerx] with blank or null if you don't have password on your mysql server.
                _MySqlConnection.Open();
            }
        }

        public static MySqlConnection GetConnection()
        {
            new ConnectionManager();
            return _MySqlConnection;
        }

        public static MySqlDataReader ExecuteQuery(String Query)
        {
            try
            {
                MySqlCommand command = new MySqlCommand()
                {
                    Connection = GetConnection(),
                    CommandType = System.Data.CommandType.Text,
                    CommandText = Query
                };
                Console.WriteLine("QueryNoParam: {0}", Query);
                return command.ExecuteReader();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "SQL Error");
                Console.Error.WriteLine(e.StackTrace);
                Console.Error.WriteLine(e.Data);
                Console.Error.WriteLine(e.Message);
                return null;
            }
        }

        public static MySqlDataReader ExecuteQuery(String Query, params object[] args)
        {
            try
            {
                MySqlCommand command = new MySqlCommand()
                {
                    Connection = GetConnection(),
                    CommandType = System.Data.CommandType.Text,
                    CommandText = String.Format(Query, args)
                };
                Console.WriteLine("QueryParam: {0}", command.CommandText);
                return command.ExecuteReader();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "SQL Error");
                Console.Error.WriteLine(e.StackTrace);
                Console.Error.WriteLine(e.Data);
                Console.Error.WriteLine(e.Message);
                return null;
            }
        }

        public static bool ExecuteCommand(String Command)
        {
            try
            {
                MySqlCommand command = new MySqlCommand()
                {
                    Connection = GetConnection(),
                    CommandType = System.Data.CommandType.Text,
                    CommandText = Command
                };
                Console.WriteLine("CommandNoParam: {0}", command.CommandText);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "SQL Error");
                return false;
            }

        }

        public static bool ExecuteCommand(String Command, params object[] args)
        {
            try
            {
                MySqlCommand command = new MySqlCommand()
                {
                    Connection = GetConnection(),
                    CommandType = System.Data.CommandType.Text,
                    CommandText = String.Format(Command, args)
                };
                Console.WriteLine("QueryWtParam: {0}", command.CommandText);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "SQL Error");
                return false;
            }
        }
        public static string MySQLEscape(string str)
        {
            return Regex.Replace(str, @"[\x00'""\b\n\r\t\cZ\\%_]",
                delegate (Match match)
                {
                    string v = match.Value;
                    switch (v)
                    {
                        case "\x00":            // ASCII NUL (0x00) character
                            return "\\0";
                        case "\b":              // BACKSPACE character
                            return "\\b";
                        case "\n":              // NEWLINE (linefeed) character
                            return "\\n";
                        case "\r":              // CARRIAGE RETURN character
                            return "\\r";
                        case "\t":              // TAB
                            return "\\t";
                        case "\u001A":          // Ctrl-Z
                            return "\\Z";
                        default:
                            return "\\" + v;
                    }
                });
        }
    }
}
