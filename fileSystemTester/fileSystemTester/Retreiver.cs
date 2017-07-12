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
    class Retreiver
    {
        SqlConnection sqn;
        string connectionString;
        SqlDataAdapter sda;
        DataTable dt;
        /*DataSet ds;
        SqlCommand cmd;
        SqlDataReader reader;
        DataRow dr;*/

        public Retreiver()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            sqn = new SqlConnection(connectionString);
        }

        public void setCommand(string query)
        {
            sda = new SqlDataAdapter(query, sqn);
            dt = new DataTable();
            sda.Fill(dt);
            var reader = dt.CreateDataReader();

            while (reader.Read())
            {
                Console.Write("|");
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write("\t" + reader[i] + "\t\t|");
                }
                Console.WriteLine();
            }
        }
    }
}
