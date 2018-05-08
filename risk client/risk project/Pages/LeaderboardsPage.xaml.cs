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
using System.Threading.Tasks;
using Windows.UI.Text;

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
            Helper.PlayConfirmSound();
            Frame.Navigate(typeof(MainMenu));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

            Task fillTable = new Task(async () =>
           {
               string[,] data = GetData();
               for (int i = 0; i < data.GetLength(0); i++)
               {
                   for (int j = 0; j < data.GetLength(1); j++)
                   {
                       await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                       {
                           TextBlock txb = new TextBlock();
                           txb.Text = data[i, j];
                           txb.FontWeight = FontWeights.Bold;
                           Grid.SetRow(txb, i + 2);
                           Grid.SetColumn(txb, j + 1);
                           txb.HorizontalAlignment = HorizontalAlignment.Center;
                           txb.VerticalAlignment = VerticalAlignment.Center;
                           txb.FontFamily = new FontFamily("Papyrus");
                           txb.Foreground = new SolidColorBrush(Colors.DarkRed);
                           labels.Add(txb);
                           MainGrid.Children.Add(txb);
                       });
                   }
               }
               await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
               {
                   FitSize(sender, null);
               });
           });

            fillTable.Start();
            
        }

        private string[,] GetData()
        {

            Comms.SendData(Comms.LEADERBOARDS);

            ReceivedMessage msg = new ReceivedMessage();

            string[,] ans = new string[8, 2];
            int count = msg.GetArgs().Count;

            for (int i=0; i<count; i+=2)
            {
                ans[i / 2, 0] = msg[i];
                ans[i / 2, 1] = msg[i + 1];
            }

            return ans;
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
