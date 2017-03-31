using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace fileSystemTester
{
    class Artist
    {
        string path;
        string artistName;
        string[] members;
        List<Album> albums;

        public Artist(string path)
        {
            albums = new List<Album>();
            this.path = path;

            string dataPath = path + "data.manager";

            if (File.Exists(dataPath))
            {
                string[] data = File.ReadAllLines(dataPath);

                if (data.Length > 0)
                    artistName = data[0];

                if (data.Length >  1)
                    members = data[1].Split('#');

                for (int i = 2; i < data.Length; i++)
                {
                    string newDir = path + '\\' + data[i];
                    Directory.CreateDirectory(newDir);
                    FileStream newData = File.Open(newDir + "data.manager", FileMode.CreateNew, FileAccess.Write);
                    newData.Write(Encoding.ASCII.GetBytes(artistName), 0, artistName.Length);
                    newData.Close();
                    albums.Add(new Album(newDir));
                }
            }
            else
            {
                File.Create(dataPath);
            }
        }

        public string GetName()
        {
            return artistName;
        }

        public string[] GetMembers()
        {
            return members;
        }

        public List<Album> GetAlbums()
        {
            return albums;
        }

        public void addAlbum(string albumName)
        {
            string newDir = path + '\\' + artistName;
            Directory.CreateDirectory(newDir);

            FileStream newData = File.Open(newDir + "data.manager", FileMode.CreateNew, FileAccess.Write);

            newData.Write(Encoding.ASCII.GetBytes(name), 0, name.Length);
            newData.Close();

            File.AppendAllLines(newDir+"data.manager", new IEnumerable<string>())
        }
    }
}
