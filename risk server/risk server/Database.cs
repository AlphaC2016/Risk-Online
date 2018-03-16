using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace risk_server
{
    /// <summary>
    /// this class handles all connections with the server's database.
    /// </summary>
    class Database
    {
        SQLiteConnection con;
        /// <summary>
        /// the standard constructor.
        /// </summary>
        public Database()
        {
            con = new SQLiteConnection("Data Source=" + Helper.DB_PATH +";Version=3;");
            con.Open();
        }


        /// <summary>
        /// this function executes a SQL query and returns a reader.
        /// </summary>
        /// <param name="query">the query.</param>
        /// <returns>the DataReader</returns>
        private SQLiteDataReader Execute(string query)
        {
            SQLiteCommand cmd = new SQLiteCommand(query, con);
            return cmd.ExecuteReader();
        }

        /// <summary>
        /// this function checks whether a username fits a password in the database.
        /// </summary>
        /// <param name="username">the username.</param>
        /// <param name="password">the password.</param>
        /// <returns>returns true if the pair exists, false otherwise.</returns>
        public bool IsUserAndPassMatch(string username, string password)
        {
            SQLiteDataReader reader = Execute("SELECT * FROM User WHERE username=\""+username+"\" AND password=\""+password+"\";");
            return reader.Read();
        }

        /// <summary>
        /// this function checks if a user exists in the database.
        /// </summary>
        /// <param name="username">the username to be checked.</param>
        /// <returns>true if the user exists, false otherwise.</returns>
        public bool DoesUserExist(string username)
        {
            SQLiteDataReader reader= Execute("SELECT * FROM User WHERE username='" + username + "';");
            return reader.Read();
        }

        /// <summary>
        /// this function adds a new username to the database.
        /// </summary>
        /// <param name="username">the new username.</param>
        /// <param name="password">the new password.</param>
        public void AddNewUser(string username, string password)
        {
            Execute("INSERT INTO User (username, password) VALUES ('" + username + "', '" + password + "');");
        }

        /// <summary>
        /// this function gets a matrix that contains the top users and their victories.
        /// </summary>
        /// <returns>the matrix.</returns>
        public string[,] GetLeaderboards()
        {
            SQLiteDataReader reader = Execute("SELECT username,victories FROM User ORDER BY victories DESC LIMIT 8;");
            string[,] ans = new string[8, 2];

            for (int i = 0; i < ans.Length && reader.Read(); i++)
            {
                ans[i, 0] = (string)reader[0];
                ans[i, 1] = reader[1].ToString();
            }

            return ans;
        }

        /// <summary>
        /// this function adds a victory to the winnig user.
        /// </summary>
        /// <param name="username">the name of the winner.</param>
        public void AddVictory(string username)
        {
            Execute("UPDATE User SET victories = victories + 1 WHERE username=\"" + username + "\";");
        }
    }
}
