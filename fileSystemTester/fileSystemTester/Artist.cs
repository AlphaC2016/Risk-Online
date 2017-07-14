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

        public Artist(string artistName, string genre="")
        {
            this.artistName = artistName;
            this.genre = genre;
            albums = new List<Album>();
            InitAlbums();
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

        private void InitAlbums()
        {
            List<List<string>> data = DataBase.GetAlbumsFromArtist(artistName);
            for (int i=0; i<data.Count; i++)
            {
                if (data[i].Count == 3)
                {
                    albums.Add(new Album(data[i][0], data[i][1], int.Parse(data[i][2])));
                }
                else
                {
                    albums.Add(new Album(data[i][0], data[i][1]));
                }
            }

        }

        public void addAlbum(string albumName)
        {

        }
    }
}
