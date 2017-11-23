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
        bool init;
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            init = false;
            SldRed.Value = Helper.red;
            SldGreen.Value = Helper.green;
            SldBlue.Value = Helper.blue;
            init = true;
            ChangeColor(sender, null);

            if (!Helper.musicPlaying)
            {
                BtnMusic.Content = "Music: OFF";
                BtnMusic.Background = new SolidColorBrush(Colors.DarkRed);
            }
            if (!Helper.soundPlaying)
            {
                BtnSound.Content = "Sound: OFF";
                BtnSound.Background = new SolidColorBrush(Colors.DarkRed);
            }

            if (Helper.fullScreen)
            {
                CbxModes.SelectedIndex = 1;
            }
            else
            {
                CbxModes.SelectedIndex = 0;
            }
            
        }

        private void Return(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainMenu));
        }

        private void ToggleMusic(object sender, RoutedEventArgs e)
        {
            Helper.musicPlaying = !Helper.musicPlaying;
            if (!Helper.musicPlaying)
            {
                BtnMusic.Content = "Music: OFF";
                BtnMusic.Background = new SolidColorBrush(Colors.DarkRed);
                Helper.PauseMusic();
            }
            else
            {
                BtnMusic.Content = "Music: ON";
                BtnMusic.Background = new SolidColorBrush(Colors.DarkGreen);
                Helper.PlayMusic();
            }
        }

        private void ToggleSound(object sender, RoutedEventArgs e)
        {
            Helper.soundPlaying = !Helper.soundPlaying;

            if (!Helper.soundPlaying)
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
            if (init)
            {
                Helper.red = SldRed.Value;
                Helper.green = SldGreen.Value;
                Helper.blue = SldBlue.Value;
                RecColorSample.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)SldRed.Value, (byte)SldGreen.Value, (byte)SldBlue.Value));
            }
        }

        private void CbxModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();

            if (((ComboBoxItem)CbxModes.SelectedValue).Content.ToString() == "Fullscreen")
            {
                Helper.fullScreen = true;
                view.TryEnterFullScreenMode();
            }
               

            else if (view.IsFullScreenMode)
            {
                Helper.fullScreen = false;
                view.ExitFullScreenMode();
            }
                
        }

        private void FitSize(object sender, SizeChangedEventArgs e)
        {

        }
    }
}
