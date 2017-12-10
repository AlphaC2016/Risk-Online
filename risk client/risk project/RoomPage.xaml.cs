using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class RoomPage : Page
    {
        List<TextBlock> users;
        string roomName;
        bool isAdmin;
        string id;

        Task getUpdates;
        public RoomPage()
        {
            this.InitializeComponent();
            users = new List<TextBlock>();

            getUpdates = new Task(async () =>
            {
                try
                {
                    while (true)
                    {
                        RecievedMessage msg = new RecievedMessage();
                        var dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
                        await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => HandleUpdate(msg));
                    }
                }
                catch (Exception) { }
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            object[] arr = (object[])e.Parameter;
            roomName = (string)arr[0];
            id = (string)arr[1];
            isAdmin = (bool)arr[2];
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RecievedMessage msg = new RecievedMessage();
            
            LblTitle.Text = roomName;
            HandleUpdate(msg);
            FitSize(sender, null);
            getUpdates.Start();
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            //getUpdates.
            Comms.SendData(Comms.LEAVE_ROOM);
            Frame.Navigate(typeof(MainMenu));
        }

        private void FitSize(object sender, SizeChangedEventArgs e)
        {
            LblTitle.FontSize = (ActualHeight + ActualWidth) / 41.6;

            foreach (TextBlock user in users)
            {
                user.FontSize = (ActualHeight + ActualWidth) / 71.4;
            }
        }

        private void HandleUpdate(RecievedMessage msg)
        {
            if (msg.GetCode() == Comms.GET_USERS_RES)
            {
                UsersGrid.Children.Clear();
                users.Clear();
                TextBlock name;
                for (int i = 0; i < msg.GetArgs().Count; i++)
                {
                    name = new TextBlock();
                    name.Text = msg[i];
                    name.FontFamily = new FontFamily("Papyrus");
                    name.FontSize = 48;
                    name.Foreground = new SolidColorBrush(Colors.DarkRed);
                    name.HorizontalAlignment = HorizontalAlignment.Center;
                    name.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(name, i);
                    UsersGrid.Children.Add(name);
                    users.Add(name);
                }
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
