using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace risk_project
{
    static class Helper
    {
        private static MediaPlayer player;
        public static bool musicPlaying;
        public static bool soundPlaying;
        public static bool fullScreen;
        static SolidColorBrush playerColor;

        static Helper()
        {
            //InitMusic();
            musicPlaying = true;
            soundPlaying = true;
            fullScreen = false;
            playerColor = new SolidColorBrush(Colors.Gray);

            player.Play();

        }

        private async static void InitMusic()
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
            StorageFile file = await folder.GetFileAsync("music.mp3");

            player.AutoPlay = false;
            player.Source = MediaSource.CreateFromStorageFile(file);
        }

        public static void ToggleMusic()
        {
            musicPlaying = !musicPlaying;

            /*if (musicPlaying)
                player.Play();
            else
                player.Pause();*/
        }

        public static bool IsMusicPlaying() { return musicPlaying; }

        public static void Init() { }
    }
}
