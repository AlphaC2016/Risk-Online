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

        public static string username { get; set; }

        static ApplicationView view = ApplicationView.GetForCurrentView();

        public static int TERRITORY_AMOUNT = 42;
        

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
            musicPlaying = bool.Parse(rawData[2]);
            soundPlaying = bool.Parse(rawData[3]);
            fullScreen = bool.Parse(rawData[4]);

            string[] colors = rawData[5].Split(',');
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
            //string[] rawData = File.ReadAllLines(@"Assets/Data/config.txt");
            //File.Delete(@"Assets/Data/config.txt");
            //rawData[2] = musicPlaying.ToString();
            //rawData[3] = soundPlaying.ToString();
            //rawData[4] = fullScreen.ToString();
            //rawData[5] = red.ToString() + ',' + green + ',' + blue;
            //File.WriteAllLines(@"Assets/Data/config.txt", rawData);
        }

        public static Color GetRandomColor()
        {
            Random r = new Random();
            return Color.FromArgb((byte)255, (byte)r.Next(256), (byte)r.Next(256), (byte)r.Next(256));
        }

        public static Color GetPlayerColor()
        {
            return Color.FromArgb((byte)255, (byte)red, (byte)green, (byte)blue);
        }
    }
}
