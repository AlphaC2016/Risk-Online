using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace fileSystemTester
{
    static class DataBase
    {
        static SqlConnection sqn;
        static string connectionString;
        static SqlDataAdapter sda;
        static DataTable dt;

        static DataBase()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            sqn = new SqlConnection(connectionString);
        }

        private static List<List<string>> SetCommand(string query)
        {
            List<List<string>> ans = new List<List<string>>();
            List<string> curr = null;

            sda = new SqlDataAdapter(query, sqn);
            dt = new DataTable();
            sda.Fill(dt);
            var reader = dt.CreateDataReader();

            while (reader.Read())
            {
                curr = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    curr.Add(reader[i].ToString());
                }
                ans.Add(curr);
            }
            return ans;
        }

        public static List<List<string>> GetArtistsFromDB()
        {
            return SetCommand("SELECT * FROM Artists");
        }

        public static List<List<string>> GetAlbumsFromArtist(string artist)
        {
            return SetCommand("SELECT * FROM Albums WHERE Artist_name=\'" + artist + "\'");
        }

        public static List<List<string>> GetSongsFromAlbum(string Album)
        {
            return SetCommand("SELECT * FROM Songs WHERE Album_name =\'" + Album + "\'");
        }

        public static List<List<string>> GetSongsFromArtist(string artist)
        {
            return SetCommand("SELECT * FROM Songs WHERE Artist_name=\'" + artist + "\' ORDER BY index");
        }

        public static void InsertAlbum(string albumName, string artistName)
        {
            SetCommand("INSERT INTO Albums (Name, Artist_name) VALUES (" + albumName + ", " + artistName + ")");
        }
    }
}
