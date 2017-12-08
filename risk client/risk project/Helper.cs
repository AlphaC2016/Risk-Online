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
using Windows.UI.ViewManagement;

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

        static ApplicationView view = ApplicationView.GetForCurrentView();
        

        //public static object RecordPlayer { get; private set; }

        public static void PlayMusic()
        {
            player.Play();
        }
        public static void PauseMusic()
        {
            player.Pause();
        }

        public static void GoFullscreen()
        {
            fullScreen = true;
            view.TryEnterFullScreenMode();
        }

        public static void ExitFullScreen()
        {
            fullScreen = false;
            view.ExitFullScreenMode();
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
            string[] rawData = File.ReadAllLines(@"Assets/Data/config.txt");
            musicPlaying = bool.Parse(rawData[2].Split(' ')[1]);
            soundPlaying = bool.Parse(rawData[3].Split(' ')[1]);
            fullScreen = bool.Parse(rawData[4].Split(' ')[1]);

            string[] colors = rawData[5].Split(' ')[1].Split(',');
            red = double.Parse(colors[0]);
            green = double.Parse(colors[1]);
            blue = double.Parse(colors[2]);

            if (fullScreen)
            {
                GoFullscreen();
            }

            InitMusic();
        }

        public static void UpdateConfig()
        {
            string[] rawData = File.ReadAllLines(@"Assets/Data/config.txt");

        }

        public static string[,] GetDataMap()
        {
            string[] rawData = File.ReadAllLines(@"Assets/Data/config.txt");
            string[,] ans = new string[6, 2];
            string[] temp;

            for (int i=0; i<rawData.Length; i++)
            {
                temp = rawData[i].Split(' ');
                ans[i,0] = temp[0];

            }
        }
    }
}
