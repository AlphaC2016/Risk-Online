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
using Windows.UI.Core;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace risk_project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoomPage : Page
    {
        List<TextBlock> users;
        List<Button> buttons;
        string roomName;
        bool isAdmin;
        string id;
        bool done;

        Task handler;
        CoreDispatcher dispatcher;
        public RoomPage()
        {
            done = false;
            this.InitializeComponent();
            users = new List<TextBlock>();
            buttons = new List<Button>();
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            handler = new Task(() => ServerHandler());
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
            buttons.Add(BtnPlay);
            buttons.Add(BtnReturn);

            LblTitle.Text = roomName;
            if (!isAdmin)
            {
                BtnPlay.Visibility = Visibility.Collapsed;
            }
            FitSize(sender, null);
            handler.Start();
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            if (isAdmin)
            {
                Comms.SendData(Comms.CLOSE_ROOM);
            }
            else
            {
                Comms.SendData(Comms.LEAVE_ROOM);
            }
        }

        private void FitSize(object sender, SizeChangedEventArgs e)
        {
            LblTitle.FontSize = (ActualHeight + ActualWidth) / 41.6;

            foreach (TextBlock user in users)
            {
                user.FontSize = (ActualHeight + ActualWidth) / 71.4;
            }

            foreach (Button button in buttons)
            {
                button.FontSize = (ActualHeight + ActualWidth) / 62.5;
                button.Width = ActualWidth / 6.4;
            }
        }

        private async void ServerHandler()
        {
            ReceivedMessage msg;
            string code;
            while (!done)
            {
                msg = new ReceivedMessage(1);
                code = msg.GetCode();

                switch (code)
                {
                    case Comms.GET_USERS_RES:
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HandleUsersUpdate(msg));
                        break;

                    case Comms.CLOSE_ROOM_RES:
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HandleRoomClosed(msg));
                        break;

                    case Comms.LEAVE_ROOM_RES:
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HandleLeaveRoom(msg));
                        break;

                    case Comms.START_GAME_RES:
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HandleGameStart(msg));
                        break;

                    default:
                        throw new Exception();
                }
            }
        }

        private void HandleUsersUpdate(ReceivedMessage msg)
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

        private async void HandleRoomClosed(ReceivedMessage msg)
        {
            done = true;
            MessageDialog dialog = new MessageDialog("This room has been closed by the admin.");
            await dialog.ShowAsync();
            Frame.Navigate(typeof(MainMenu));
        }

        private async void HandleLeaveRoom(ReceivedMessage msg)
        {
            done = true;
            MessageDialog dialog = new MessageDialog("You have left the room.");
            await dialog.ShowAsync();
            Frame.Navigate(typeof(MainMenu));
        }

        private async void HandleGameStart(ReceivedMessage msg)
        {
            done = true;
            MessageDialog dialog = new MessageDialog("The game will begin now!");
            await dialog.ShowAsync();
            Frame.Navigate(typeof(GamePage));
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            Comms.SendData(Comms.START_GAME);
        }
    }
}
