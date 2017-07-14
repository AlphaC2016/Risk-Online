using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileSystemTester
{
    class MusicData
    {
        List<Artist> artists;

        public MusicData()
        {
            artists = new List<Artist>();
            List<List<string>> data = DataBase.GetArtistsFromDB();

            for (int i=0; i<data.Count; i++)
            {
                if (data[i].Count == 1)
                    artists.Add(new Artist(data[i][0]));
                else
                    artists.Add(new Artist(data[i][0], data[i][1]));
            }
        }
    }
}
