using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;

/*Structure of album data file:
 * 
 * album name # artist name # year of release
 * song name # length # index
 * song name # length # index
 * song name # length # index
 * .
 * .
 * .
 */

namespace fileSystemTester
{
    class Album
    {
        private string path;
        private string albumName;
        private string artist;
        private int year;
        private List<Song> songs;

        public Album(string path)
        {
            year = -1;
            songs = new List<Song>();
            this.path = path;

            string dataPath = path + "\\data.manager";

            if (File.Exists(dataPath))
            {
                string[] data = File.ReadAllLines(dataPath);
                string[] albumData = data[0].Split('#');
                
                if (albumData.Length < 2 || albumData.Length > 3)
                {
                    throw new Exception("DATA FILE WRITTEN INCORRECTLY");
                }


                albumName = albumData[0];
                artist = albumData[1];
                if (albumData.Length == 3)
                {
                    year = int.Parse(albumData[2]);
                }

                for (int i=1; i<data.Length; i++)
                {
                    string[] songData = data[i].Split('#');
                    songs.Add(new Song(songData[0], songData[1], int.Parse(songData[2]), path));
                }
            }
            else
            {
                throw new Exception("NO DATA FILE!");
            }
        }

        public string GetName()
        {
            return albumName;
        }

        public string GetArtist()
        {
            return artist;
        }

        public int GetYear()
        {
            return year;
        }

        public List<Song> GetSogs()
        {
            return songs;
        }

        public void SetYear(int year)
        {
            this.year = year;
        }

        public void AddSong(string songName, string length, int index)
        {
            if (year == -1)
            {
                throw new Exception("ALBUM METADATA MUST BE INSERTED BEFORE SONGS");
            }
            else
            {
                FileStream dataFile = File.Open(path + "\\data.manager", FileMode.Append, FileAccess.Write);
                string newEntry = "\n" + songName + "#" + length + "#" + index.ToString();
                dataFile.Write(Encoding.ASCII.GetBytes(newEntry), 0, newEntry.Length);
                dataFile.Close();
                songs.Add(new Song(songName, length, index, path));
            }
        }
    }
}
