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
    public sealed partial class MainMenu : Page
    {
        List<Button> buttons = new List<Button>();

        public MainMenu()
        {
            this.InitializeComponent();

            buttons.Add(BtnHelp);
            buttons.Add(BtnSettings);
            buttons.Add(BtnJoinRoom);
        }

        private void FitSize(object sender, RoutedEventArgs e)
        {
            foreach (Button b in buttons)
            {
                b.Width = ActualWidth / 12.8;
                b.Height = ActualHeight / 7.2;
            }
        }
    }
}
