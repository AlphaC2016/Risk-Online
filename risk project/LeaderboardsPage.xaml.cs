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
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace risk_project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class LeaderboardsPage : Page
    {
        List<TextBlock> labels = new List<TextBlock>();


        public LeaderboardsPage()
        {
            this.InitializeComponent();
        }

        private void Return(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainMenu));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string[,] data = GetData();

            for (int i=0; i<data.GetLength(0); i++)
            {
                for (int j=0; j<data.GetLength(1); j++)
                {
                    TextBlock txb = new TextBlock();
                    txb.Text = data[i, j];
                    Grid.SetRow(txb, i + 2);
                    Grid.SetColumn(txb, j + 1);
                    txb.HorizontalAlignment = HorizontalAlignment.Center;
                    txb.VerticalAlignment = VerticalAlignment.Center;
                    txb.FontFamily = new FontFamily("Papyrus");
                    txb.Foreground = new SolidColorBrush(Colors.DarkRed);
                    labels.Add(txb);
                    MainGrid.Children.Add(txb);
                }
            }

            FitSize(sender, null);
        }

        private string[,] GetData()
        {
            string[,] data = new string[8, 2];

            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    data[i, j] = "   -----   ";

            return data;
        }

        private void FitSize(object sender, SizeChangedEventArgs e)
        {
            LblTitle.FontSize = (ActualHeight + ActualWidth) / 41;
            LblName.FontSize = (ActualHeight + ActualWidth) / 62.5;
            LblGamesWon.FontSize = (ActualHeight + ActualWidth) / 62.5;

            foreach (TextBlock txb in labels)
            {
                txb.FontSize = LblName.FontSize = (ActualHeight + ActualWidth) / 65;
            }

            BtnReturn.FontSize = (ActualHeight + ActualWidth) / 62.5;
            BtnReturn.Width = (ActualHeight + ActualWidth) / 10;
        }
    }
}
