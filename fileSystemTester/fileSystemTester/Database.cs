using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;

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
    /* This is the biggest data structure in the file system - the complete database.
     * data-wise, It contains a list of all albums
     * function-wise, it contains all search functions, and the 
     */
    class Database
    {
        List<Artist> artists;
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
                    artists.Add(new Artist(newDir));
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
            artists.Add(new Artist(newDir));
        }

        public List<Artist> getArtists()
        {
            return artists;
        }

        public Artist GetArtist(string name)
        {
            Artist ans = artists.SingleOrDefault<Artist>(x => x.GetName().Equals(name));

            if (ans.Equals(default(Artist)))
                throw new Exception("ARTIST DOES NOT EXIST (or exists multiple times)");

            return ans;
        }

        public Album GetAlbum(string artist, string name)
        {
            Artist a = GetArtist(artist);
            Album ans = a.GetAlbums().SingleOrDefault<Album>(x => x.GetName().Equals(name));

            if (ans.Equals(default(Album)))
                throw new Exception("ALBUM DOES NOT EXIST (or exists multiple times)");

            return ans;
        }

        public Album GetAlbum(string name)
        {
            foreach (Artist artist in artists)
            {
                Album ans = artist.GetAlbums().SingleOrDefault<Album>(x => x.GetName().Equals(name));
                if (!ans.Equals(default(Album)))
                    return ans;
            }
            throw new Exception("ALBUM DOES NOT EXIST (or exists multiple times)");
        }
    }
}
