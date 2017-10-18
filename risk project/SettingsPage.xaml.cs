using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace risk_project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        bool music = true;
        bool sound = false;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void Return(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainMenu));
        }

        private void ToggleMusic(object sender, RoutedEventArgs e)
        {
            Auxiliary.ToggleMusic();

            if (!Auxiliary.IsMusicPlaying())
            {
                BtnMusic.Content = "Music: OFF";
                BtnMusic.Background = new SolidColorBrush(Colors.DarkRed);
            }
            else
            {
                BtnMusic.Content = "Music: ON";
                BtnMusic.Background = new SolidColorBrush(Colors.DarkGreen);
            }
        }

        private void ToggleSound(object sender, RoutedEventArgs e)
        {
            sound = !sound;

            if (!sound)
            {
                BtnSound.Content = "Sound: OFF";
                BtnSound.Background = new SolidColorBrush(Colors.DarkRed);
            }
            else
            {
                BtnSound.Content = "Sound: ON";
                BtnSound.Background = new SolidColorBrush(Colors.DarkGreen);
            }
        }

        private void ChangeColor(object sender, RangeBaseValueChangedEventArgs e)
        {
            LblColor.Foreground = new SolidColorBrush(Color.FromArgb(255, (byte)SldRed.Value, (byte)SldGreen.Value, (byte)SldBlue.Value));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SldRed.Value = 127;
            SldGreen.Value = 127;
            SldBlue.Value = 127;
            ChangeColor(sender, null);
        }

        private void CbxModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();

            if (((ComboBoxItem)CbxModes.SelectedValue).Content.ToString() == "Fullscreen")
                view.TryEnterFullScreenMode();

            else if (view.IsFullScreenMode)
                view.ExitFullScreenMode();
        }
    }
}
