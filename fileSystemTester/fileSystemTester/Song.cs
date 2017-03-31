using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace fileSystemTester
{
    class Song
    {
        string name;
        string length;
        int index;
        string album;
        string artist;
        string path;
        public Song(string name, string length, int index, string path)
        {
            this.name = name;
            this.length = length;
            this.index = index;
            string[] albumData = File.ReadAllLines(path + "\\data.manager")[0].Split('#');
            album = albumData[0];
            artist = albumData[1];
        }

        public string GetName()
        {
            return name;
        }

        public string GetLength()
        {
            return length;
        }

        public int GetIndex()
        {
            return index;
        }

        public string GetAlbumName()
        {
            return album;
        }

        public string GetArtistName()
        {
            return artist;
        }
    }
}
