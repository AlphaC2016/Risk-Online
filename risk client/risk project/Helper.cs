using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.Media.Playback;

namespace risk_project
{
    static class Helper
    {
        private static MediaPlayer player;
        public static bool musicPlaying;
        public static bool soundPlaying;
        public static bool fullScreen;

        public static double red;
        public static double green;
        public static double blue;

        //public static object RecordPlayer { get; private set; }

        public static void PlayMusic()
        {
            player.Play();
        }
        public static void PauseMusic()
        {
            player.Pause();
        }

        public static void InitMusic()
        {
            player = new MediaPlayer();
            player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/Data/music.mp3"));
            if (musicPlaying)
            {
                player.Play();
            }
        }

        public static bool IsMusicPlaying() { return musicPlaying; }

        public static void Init()
        {
            musicPlaying = true;
            soundPlaying = true;
            fullScreen = false;

            red = 127;
            green = 127;
            blue = 127;
            InitMusic();
        }
    }
}
