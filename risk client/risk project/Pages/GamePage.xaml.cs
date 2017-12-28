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
using System.Windows;
using Windows.UI.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.Storage;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace risk_project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        List<string[]> labelData;
        Dictionary<string, Territory> territories;

        Dictionary<string, Color> colors;

        CoreDispatcher dispatcher;

        public GamePage()
        {
            this.InitializeComponent();
            labelData = new List<string[]>();
            territories = new Dictionary<string, Territory>();
            colors = new Dictionary<string, Color>();
            dispatcher = Window.Current.Dispatcher;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BuildBoard();

            Task wait = new Task(async() =>
            {
                ReceivedMessage msg = new ReceivedMessage(0);

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => InitBoard(msg));
            });
            wait.Start();
        }

        private void FitSize(object sender, SizeChangedEventArgs e)
        {
            IEnumerable<TextBlock> content;
            IEnumerable<Territory> vals = territories.Values;

            for (int i = 0; i < territories.Count; i++)
            {
                Canvas.SetLeft(vals.ElementAt(i), ActualWidth / double.Parse(labelData[i][1]));
                Canvas.SetTop(vals.ElementAt(i), ActualHeight / double.Parse(labelData[i][2]));

                content = vals.ElementAt(i).Children.Cast<TextBlock>();
                foreach (TextBlock lbl in content)
                {
                    lbl.FontSize = (ActualHeight + ActualWidth) / 150;
                }
            }
        }

        private async void readStuffAsync()
        {
            var path = @"../mapdata.csv";
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            // acquire file
            var file = await folder.GetFileAsync(path);
            var readFile = await Windows.Storage.FileIO.ReadLinesAsync(file);
        
        }

        private void Panel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Panel_PointerEntered(object sender, RoutedEventArgs e)
        {
            //StackPanel obj = (StackPanel)sender;
            //obj.Background = new SolidColorBrush(Colors.Gold);
        }

        private void BuildBoard()
        {
            Task read = new Task(() =>
            {
                string[] readFile = File.ReadAllLines(@"Assets/Data/mapdata.csv");
                foreach (var line in readFile)
                {
                    labelData.Add(line.Split(','));
                }
            });
            read.Start();
            read.Wait();


            foreach (string[] line in labelData)
            {
                TextBlock lbl = new TextBlock();
                Territory t = new Territory();
                string name = line[0].Replace('#', '\n');


                lbl.FontFamily = new FontFamily("Papyrus");
                lbl.Foreground = new SolidColorBrush(Colors.Black);
                lbl.Text = name;
                t.Children.Add(lbl);

                lbl = new TextBlock();
                lbl.FontFamily = new FontFamily("Papyrus");
                lbl.HorizontalAlignment = HorizontalAlignment.Center;
                lbl.VerticalAlignment = VerticalAlignment.Center;
                //Color to be changed accordingly to player holding the point.
                lbl.Foreground = new SolidColorBrush(Colors.Black);
                lbl.Text = "0";
                lbl.FontWeight = FontWeights.Bold;
                t.Children.Add(lbl);

                t.Orientation = Orientation.Vertical;
                t.PointerPressed += Panel_Click;
                t.PointerEntered += Panel_PointerEntered;
                territories.Add(name, t);
            }

            FitSize(null, null);

            foreach (Territory t in territories.Values)
            {
                MainCanvas.Children.Add(t);
            }
        }

        private void InitBoard(ReceivedMessage msg)
        {
            int count = int.Parse(msg[0]);
            int i = 1;

            TextBlock txb;
            Rectangle rect;
            Color color;
            for (i=1; i<=count; i++)
            {
                txb = new TextBlock();
                txb.Text = msg[i];
                txb.VerticalAlignment = VerticalAlignment.Center;
                txb.HorizontalAlignment = HorizontalAlignment.Left;

                txb.FontFamily = new FontFamily("Papyrus");
                txb.FontSize = 36;
                txb.Foreground = new SolidColorBrush(Colors.Black);
                Grid.SetColumn(txb, 1);
                Grid.SetRow(txb, i - 1);
                GrdUsers.Children.Add(txb);

                rect = new Rectangle();
                if (msg[i] == Helper.username)
                {
                    color = Helper.GetPlayerColor();
                }
                else
                {
                    color = Helper.GetRandomColor();
                }
                rect.Fill = new SolidColorBrush(color);
                Grid.SetColumn(rect, 0);
                Grid.SetRow(rect, i - 1);
                GrdUsers.Children.Add(rect);

                colors.Add(msg[i], color);
            }

            foreach (Territory t in territories.Values)
            {
                t.SetOwner(msg[i]);
                t.SetColor(colors[msg[i]]);
                i++;
            }
        }
    }
}
