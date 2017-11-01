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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace risk_project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        List<string[]> labelData;
        List<StackPanel> names;

        public GamePage()
        {
            this.InitializeComponent();
            labelData = new List<string[]>();
            names = new List<StackPanel>();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string[] readFile = File.ReadAllLines("mapdata.csv");
            foreach (var line in readFile)
            {
                labelData.Add(line.Split(','));
            }

            foreach (string[] line in labelData)
            {
                TextBlock lbl = new TextBlock();
                StackPanel stp = new StackPanel();

                lbl.FontFamily = new FontFamily("Papyrus");
                lbl.Foreground = new SolidColorBrush(Colors.Black);
                lbl.Text = line[0].Replace('#','\n');
                stp.Children.Add(lbl);

                lbl = new TextBlock();
                lbl.FontFamily = new FontFamily("Papyrus");
                lbl.HorizontalAlignment = HorizontalAlignment.Center;
                lbl.VerticalAlignment = VerticalAlignment.Center;
                //Color to be changed accordingly to player holding the point.
                lbl.Foreground = new SolidColorBrush(Colors.Black);
                lbl.Text = "0";
                lbl.FontWeight = FontWeights.Bold;
                stp.Children.Add(lbl);

                stp.Orientation = Orientation.Vertical;
                stp.PointerPressed += Panel_Click;
                stp.PointerEntered += Panel_PointerEntered;
                names.Add(stp);
            }

            FitSize(sender, null);

            foreach (StackPanel stp in names)
            {
                MainCanvas.Children.Add(stp);
            }
        }

        private void FitSize(object sender, SizeChangedEventArgs e)
        {
            IEnumerable<TextBlock> content;

            for (int i = 0; i < names.Count; i++)
            {
                Canvas.SetLeft(names[i], ActualWidth / double.Parse(labelData[i][1]));
                Canvas.SetTop(names[i], ActualHeight / double.Parse(labelData[i][2]));

                content = names[i].Children.Cast<TextBlock>();
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
    }
}
