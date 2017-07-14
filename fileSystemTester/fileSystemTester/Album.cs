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

        public Album(string albumName, string artist, int year=-1)
        {
            this.albumName = albumName;
            this.artist = artist;

            if (year != -1) this.year = year;

            songs = new List<Song>();
            InitSongs();
        }

        private void InitSongs()
        {
            List<List<string>> data = DataBase.GetSongsFromAlbum(albumName);

            for (int i = 0; i < data.Count; i++)
            {
                songs.Add(new Song(data[i][0], data[i][4], int.Parse(data[i][3]), data[i][5], data[i][1], data[i][2]));
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

        public List<Song> GetSongs()
        {
            return songs;
        }

        public void SetYear(int year)
        {
            this.year = year;
        }

        public void AddSong(string name, int index, string length, string path)
        {

        }
    }
}
