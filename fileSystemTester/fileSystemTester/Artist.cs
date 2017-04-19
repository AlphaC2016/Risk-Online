using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;


/* Structure of Artist data file:
 * 
 * Artist name
 * genre
 * album name
 * album name
 * album name
 * .
 * .
 * .
 */

namespace fileSystemTester
{
    class Artist
    {
        string path;
        string artistName;
        string genre;
        List<Album> albums;

        public Artist(string path)
        {
            albums = new List<Album>();
            this.path = path;

            string dataPath = path + "\\data.manager";

            if (File.Exists(dataPath))
            {
                string[] data = File.ReadAllLines(dataPath);

                if (data.Length > 2)
                {
                    throw new Exception("DATA FILE WRITTEN INCORRECTLY!");
                }

                artistName = data[0];

                genre = data[1];


                for (int i = 2; i < data.Length; i++)
                {
                    string newDir = path + '\\' + data[i];
                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                        FileStream newData = File.Open(newDir + "\\data.manager", FileMode.CreateNew, FileAccess.Write);
                        newData.Write(Encoding.ASCII.GetBytes(data[i]), 0, data[i].Length);

                        newData.Write(Encoding.ASCII.GetBytes("#" + artistName), 0, artistName.Length);
                        newData.Close();
                        albums.Add(new Album(newDir));
                    }
                }
            }
            else
            {
                throw new Exception("NO DATA FILE!");
            }
        }

        public string GetName()
        {
            return artistName;
        }

        public string GetGenre()
        {
            return genre;
        }

        public List<Album> GetAlbums()
        {
            return albums;
        }

        public void addAlbum(string albumName)
        {
            if (File.ReadAllLines(path).Length < 2)
                throw new Exception("no members - can't insert albums.");


            string newDir = path + '\\' + artistName; //making a new directory for the album
            Directory.CreateDirectory(newDir);

            FileStream newData = File.Open(newDir + "\\data.manager", FileMode.CreateNew, FileAccess.Write); //make the new data file

            newData.Write(Encoding.ASCII.GetBytes(artistName), 0, artistName.Length);

            newData.Write(Encoding.ASCII.GetBytes("#" + albumName), 0, artistName.Length);
            newData.Close();
            albums.Add(new Album(newDir));

            FileStream artistDataFile = File.Open(path + "\\data.manager", FileMode.Append, FileAccess.Write);
            artistDataFile.Write(Encoding.ASCII.GetBytes("\n"+albumName), 0, albumName.Length);
            artistDataFile.Close();
        }
    }
}
