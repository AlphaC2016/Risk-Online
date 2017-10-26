using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace risk_server
{
    class Database
    {
        SQLiteConnection con;
        public Database()
        {
            SQLiteConnection con = new SQLiteConnection("Data Source=" + Helper.DB_PATH +";Version=3;");
            con.Open();
        }

        private SQLiteDataReader Execute(string query)
        {
            SQLiteCommand cmd = new SQLiteCommand(query, con);
            return cmd.ExecuteReader();
        }

        public bool IsUserAndPassMatch(string username, string password)
        {
            SQLiteDataReader reader = Execute("SELECT * FROM User WHERE username=\""+username+"\" AND password=\""+password+';');
            return reader.Read();
        }

        public bool DoesUserExist(string username)
        {
            SQLiteDataReader reader= Execute("SELECT * FROM User WHERE username=\"" + username + "\";");
            return reader.Read();
        }

        public void AddNewUser(string username, string password)
        {
            Execute("INSERT INTO User ('username', 'password', 'victories') VALUES ('" + username + "', '" + password + "', 0);");
        }

        public string[,] GetLeaderboards()
        {
            SQLiteDataReader reader = Execute("SELECT username,victories FROM User ORDER BY vicories DESC");
            string[,] ans = new string[10, 2];

            for (int i = 0; i < ans.Length && reader.Read(); i++)
            {
                ans[i, 0] = (string)reader[0];
                ans[i, 1] = (string)reader[1];
            }

            return ans;
        }

        public void AddVictory(string username)
        {
            Execute("UPDATE User SET victories = victories + 1 WHERE username=" + username + ';');
        }
    }
}
