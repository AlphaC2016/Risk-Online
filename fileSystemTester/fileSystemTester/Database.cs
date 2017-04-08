using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/*Structure of a database data file:
 * 
 * artist name # genre
 * artist name # genre
 * artist name # genre
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
                    string[] thisLine = data[i].Split('#');
                    string name = thisLine[0];
                    string genre = thisLine[1];

                    string newDir = path + '\\' + name;
                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                        FileStream newData = File.Open(newDir + "\\data.manager", FileMode.CreateNew, FileAccess.Write);
                        newData.Write(Encoding.ASCII.GetBytes(name+'\n'), 0, name.Length+1);
                        newData.Write(Encoding.ASCII.GetBytes(genre), 0, genre.Length);
                    }
                    artists.Add(name, new Artist(newDir));
                }
            }
        }

        public void addArtist(string name, string genre)
        {
            string newDir = Path.Combine(base_path, name);
            if (!Directory.Exists(newDir))
            {
                Directory.CreateDirectory(newDir);
                FileStream newData = File.Open(newDir + "\\data.manager", FileMode.CreateNew, FileAccess.Write);
                newData.Write(Encoding.ASCII.GetBytes(name + '\n'), 0, name.Length + 1);
                newData.Write(Encoding.ASCII.GetBytes(genre), 0, genre.Length);
            }
            artists.Add(name, new Artist(newDir));
        }

        public SortedDictionary<string, Artist> getArtists()
        {
            return artists;
        }
    }
}
