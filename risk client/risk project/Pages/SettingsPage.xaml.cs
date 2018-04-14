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
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace risk_project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        List<Button> buttons;
        List<TextBlock> labels;
        List<Rectangle> colors;

        public SettingsPage()
        {
            this.InitializeComponent();

            buttons = new List<Button>();
            labels = new List<TextBlock>();
            colors = new List<Rectangle>();

            buttons.Add(BtnMusic);
            buttons.Add(BtnSound);
            buttons.Add(BtnReturn);

            labels.Add(LblColor);
            labels.Add(LblMode);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            Rectangle rect;

            for (int i=0; i<Helper.ColorChoices.Length; i++)
            {
                rect = new Rectangle();
                rect.HorizontalAlignment = HorizontalAlignment.Center;
                rect.VerticalAlignment = VerticalAlignment.Center;
                rect.Fill = new SolidColorBrush(Helper.ColorChoices[i]);
                rect.Stroke = new SolidColorBrush(Colors.White);
                rect.StrokeThickness = 0;
                rect.PointerPressed += ChangeColor;

                Grid.SetRow(rect, i / 4);
                Grid.SetColumn(rect, i % 4);
                ColorsGrid.Children.Add(rect);
                colors.Add(rect);

                if (Helper.ColorChoices[i] == Helper.UserColor)
                    ChangeColor(rect, null);
            }

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
            FitSize(null, null);
        }

        private void Return(object sender, RoutedEventArgs e)
        {
            Helper.PlayConfirmSound();
            Helper.UpdateConfig();
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

        private void ChangeColor(object sender, RoutedEventArgs e)
        {
            foreach (Rectangle curr in colors)
                curr.StrokeThickness = 0;

            Rectangle rect = sender as Rectangle;
            Helper.UserColor = (rect.Fill as SolidColorBrush).Color;
            rect.StrokeThickness = 3;
        }

        private void CbxModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();

            if (((ComboBoxItem)CbxModes.SelectedValue).Content.ToString() == "Fullscreen")
            {
                Helper.GoFullscreen();
            }
               

            else if (view.IsFullScreenMode)
            {
                Helper.ExitFullScreen();
            }
                
        }

        private void FitSize(object sender, SizeChangedEventArgs e)
        {

            foreach (TextBlock lbl in labels)
            {
                lbl.FontSize = (ActualHeight + ActualWidth) / 62.5;
            }

            foreach (Button btn in buttons)
            {
                btn.FontSize = (ActualHeight + ActualWidth) / 70;
                btn.Width = ActualWidth / 6.4;
                btn.Height = ActualHeight / 12;
            }

            foreach (Rectangle rect in colors)
            {
                rect.Height = rect.Width = (ActualHeight + ActualWidth) / 30;
            }

            LblTitle.FontSize = (ActualHeight + ActualWidth) / 41.67;

            CbxModes.Width = ActualWidth / 4.8;
            CbxModes.Height = ActualHeight / 14;
            CbxModes.FontSize = (ActualHeight + ActualWidth) / 62.5;
        }
    }
}
