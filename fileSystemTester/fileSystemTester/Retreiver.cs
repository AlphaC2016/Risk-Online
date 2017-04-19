using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace fileSystemTester
{
    class Retreiver
    {
        SqlConnection sqn;
        SqlCommand cmd;
        SqlDataReader reader;

        /*SqlDataAdapter sda;
        DataSet ds;
        DataTable dt;
        DataRow dr;*/

        public Retreiver()
        {
            sqn = new SqlConnection("Data Source=(LocalDB)\\v11.0;AttachDbFilename=\"F:\\2016-2017\\projects\\senior project\\fileSystemTester\\fileSystemTester\\MusicData.mdf\";Integrated Security=True");
            sqn.Open();
        }

        public void setCommand()
        {
            cmd = new SqlCommand("SELECT * FROM Artists", sqn);
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }

            while (reader.Read())
            {
                Console.Write("|");
                for (int i=0; i<reader.FieldCount; i++)
                {
                    Console.Write("\t" + reader[i] + "\t\t|");
                }
                Console.WriteLine();
            }
        }
    }
}
