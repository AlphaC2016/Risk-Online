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

namespace risk_project
{
    static class Helper
    {
        private static MediaElement player;
        public static bool musicPlaying;
        public static bool soundPlaying;
        public static bool fullScreen;

        public static double red;
        public static double green;
        public static double blue;

        public static string Username { get; set; }

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

        public static bool IsMusicPlaying() { return musicPlaying; }

        /// <summary>
        /// Initializes the Game's music.
        /// </summary>
        public static async void InitMusic()
        {
            player = new MediaElement();
            StorageFolder Folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            Folder = await Folder.GetFolderAsync(@"Assets\\Data");
            StorageFile sf = await Folder.GetFileAsync("music.mp3");
            player.SetSource(await sf.OpenAsync(FileAccessMode.Read), sf.ContentType);
            player.AutoPlay = false;
        }

        
        /// <summary>
        /// This function Initializes the static class - settings and music included.
        /// </summary>
        public async static void Init()
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

            await Task.Delay(250);
            if (!musicPlaying)
                player.Pause();
        }

        public static async void UpdateConfig()
        {
            string[] rawData = File.ReadAllLines(@"Assets/Data/config.txt");

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile dataFile = await storageFolder.GetFileAsync(@"Assets/Data/config.txt");
            var stream = await dataFile.OpenAsync(FileAccessMode.ReadWrite);

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
