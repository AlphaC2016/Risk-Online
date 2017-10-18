using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class CreateRoomPage : Page
    {
        List<Button> buttons;
        List<TextBlock> labels;

        public CreateRoomPage()
        {
            this.InitializeComponent();

            buttons = new List<Button>();
            labels = new List<TextBlock>();

            buttons.Add(BtnBack);
            buttons.Add(BtnCreate);

            labels.Add(LblPlayerAmount);
            labels.Add(LblRoomName);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainMenu));
        }

        private void FitSize(object sender, SizeChangedEventArgs e)
        {
            foreach (TextBlock label in labels)
            {
                label.FontSize = (ActualHeight + ActualWidth) / 41;
            }

            foreach (Button button in buttons)
            {
                button.FontSize = (ActualHeight+ActualWidth) / 41;
                button.Width = ActualWidth / 3;
            }

            LblTitle.FontSize = (ActualHeight + ActualWidth) / 27;

            CbxAmount.Height = ActualHeight / 10.8;
            CbxAmount.Width = ActualWidth / 4.8;

            CbxAmount.FontSize = (ActualHeight + ActualWidth) / 41;
        }
    }
}
