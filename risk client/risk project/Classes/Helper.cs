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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;


namespace risk_project
{
    static class Helper
    {
        private static MediaPlayer player;
        public static bool musicPlaying;
        public static bool soundPlaying;
        public static bool fullScreen;

        

        public static Color UserColor { get; set; }

        public static string Username { get; set; }

        static ApplicationView view = ApplicationView.GetForCurrentView();

        public static int TERRITORY_AMOUNT = 42;

        public static Color[] ColorChoices { get; } = 
           { Colors.Navy,
            Colors.Maroon,
            Colors.Blue,
            Colors.DarkMagenta,
            Colors.Cyan,
            Colors.MediumVioletRed,
            Colors.LightBlue,
            Colors.Black
        };



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

        public static bool IsMusicPlaying() { return musicPlaying; }

        /// <summary>
        /// Initializes the Game's music.
        /// </summary>
        public static async void InitMusic()
        {
            //player = new MediaElement();
            //player.MediaOpened += Player_MediaOpened;

            

            StorageFolder Folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            Folder = await Folder.GetFolderAsync(@"Assets\\Data");
            StorageFile sf = await Folder.GetFileAsync("music.mp3");

            player = BackgroundMediaPlayer.Current;
            player.Source = MediaSource.CreateFromStorageFile(sf);

            if (musicPlaying)
                player.Play();
            else
                player.Pause();
        }


        /// <summary>
        /// This function Initializes the static class - settings and music included.
        /// </summary>
        public static void Init()
        {
            var localSettings = ApplicationData.Current.LocalSettings.Values;

            if (localSettings["musicPlaying"] == null)
            {
                localSettings["musicPlaying"] = musicPlaying = false;
                localSettings["soundPlaying"] = soundPlaying = false;
                localSettings["fullscreen"] = fullScreen = false;
                localSettings["color"] = 0;
                UserColor = ColorChoices[0];
            }
            else
            {
                musicPlaying = (bool)localSettings["musicPlaying"];
                soundPlaying = (bool)localSettings["soundPlaying"];
                fullScreen = (bool)localSettings["fullscreen"];
                UserColor = ColorChoices[(int)localSettings["color"]];
            }

            if (fullScreen)
            {
                GoFullscreen();
            }

            InitMusic();
        }

        public static void UpdateConfig()
        {
            var localSettings = ApplicationData.Current.LocalSettings.Values;

            localSettings["musicPlaying"] = musicPlaying;
            localSettings["soundPlaying"] = soundPlaying;
            localSettings["fullscreen"] = fullScreen;
            localSettings["color"] = Array.IndexOf(ColorChoices, UserColor);
        }

        /// <summary>
        /// This function returns the index of a territory in a string-Territory dictionary.
        /// </summary>
        /// <param name="dict">The dictionary to be searched.</param>
        /// <param name="val">The seeked value.</param>
        /// <returns>Returns the index of the said territory. If not found, returns -1.</returns>
        public static int GetIndex(Dictionary<string, Territory> dict, Territory val)
        {
            int i = 0;
            foreach (Territory t in dict.Values)
            {
                if (t == val)
                    return i;
                i++;
            }
            return -1;
        }
    }
}
