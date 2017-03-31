using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/*Structure of a database data file:
 * 
 * artist name $ member # member # member ...
 * artist name $ member # member # member ...
 * artist name $ member # member # member ...
 * .
 * .
 * .
 */
namespace fileSystemTester
{
    class Database
    {
        SortedDictionary<string, Artist> artists;
        string base_path;

        public Database(string path)
        {
            base_path = path;

            string dataPath = base_path + "\\data.manager";
            if (!File.Exists(dataPath))
            {
                File.Create(dataPath);
            }
            else
            {
                string[] data = File.ReadAllLines(dataPath);

                for (int i=0; i<data.Length; i++)
                {
                    int firstDelimeter = data[i].IndexOf('$');
                    string name = data[i].Substring(0, firstDelimeter-1);
                    string members = data[i].Substring(firstDelimeter + 1, data.Length - firstDelimeter - 1);

                    string newDir = path + '\\' + name;
                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                        FileStream newData = File.Open(newDir + "\\data.manager", FileMode.CreateNew, FileAccess.Write);
                        newData.Write(Encoding.ASCII.GetBytes(name+'\n'), 0, name.Length+1);
                        newData.Write(Encoding.ASCII.GetBytes(members), 0, members.Length);
                        artists.Add(name, new Artist(newDir));
                    }
                }
            }
        }

        public void addArtist(string name, string[] members)
        {}
    }
}
