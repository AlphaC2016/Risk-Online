﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
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
    public sealed partial class JoinRoomPage : Page
    {
        Dictionary<string, int> roomIDs;
        List<TextBlock> rooms;
        List<TextBlock> currRoomData;
        List<TextBlock> titles;
        List<Button> buttons;
        string currName;
        CoreDispatcher dispatcher = Window.Current.Dispatcher;

        public JoinRoomPage()
        {
            this.InitializeComponent();
            roomIDs = new Dictionary<string, int>();
            rooms = new List<TextBlock>();
            currRoomData = new List<TextBlock>();
            buttons = new List<Button>();
            titles = new List<TextBlock>();
            currName = "";
        }      

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            buttons.Add(BtnJoin);
            buttons.Add(BtnReturn);
            titles.Add(LblRooms);
            titles.Add(LblUsers);

            Task t = new Task(() => GetRooms());
            t.Start();

            FitSize(sender, null);
        }

        private void FitSize(object sender, SizeChangedEventArgs e)
        {
            foreach (Button button in buttons)
            {
                button.FontSize = (ActualHeight + ActualWidth) / 62.5;
                button.Width = ActualWidth / 6.4;
            }

            foreach (TextBlock text in rooms)
            {
                text.FontSize = (ActualHeight + ActualWidth) / 62.5;
            }
            foreach (TextBlock data in currRoomData)
            {
                data.FontSize = (ActualHeight + ActualWidth) / 62.5;
            }
            foreach (TextBlock title in titles)
            {
                title.FontSize = (ActualHeight + ActualWidth) / 62.5;
            }

            BtnRefresh.Height = BtnRefresh.Width = (ActualHeight + ActualWidth) / 40;
        }

        private async void GetRooms()
        {
            Comms.SendData(Comms.GET_ROOMS);
            ReceivedMessage msg = new ReceivedMessage();

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => StkRoomNames.Children.Clear());
            roomIDs = new Dictionary<string, int>();

            if (msg.GetCode() == Comms.GET_ROOMS_RES)
            {
                List<string> args = msg.GetArgs();

                for (int i = 0; i < args.Count; i += 2)
                {
                    roomIDs.Add(args[i + 1], int.Parse(args[i]));
                }
            }


            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TextBlock t;
                foreach (string name in roomIDs.Keys)
                {
                    t = new TextBlock();
                    t.Text = name;
                    t.HorizontalAlignment = HorizontalAlignment.Center;
                    t.VerticalAlignment = VerticalAlignment.Center;
                    t.FontFamily = new FontFamily("Papyrus");
                    t.Foreground = new SolidColorBrush(Colors.DarkRed);
                    t.PointerPressed += RoomSelected;
                    rooms.Add(t);
                    StkRoomNames.Children.Add(t);
                }
                FitSize(null, null);
            });
        }

        private void RoomSelected(object sender, RoutedEventArgs e)
        {
            currName = ((TextBlock)sender).Text;
            var dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            Task refresh = new Task(async () =>
            {
                Comms.SendData(Comms.GET_USERS + Comms.GetPaddedNumber(roomIDs[currName], 4));
                ReceivedMessage msg = new ReceivedMessage();
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    updateUsersList(msg);
                });
            });
            refresh.Start();
        }

        private void updateUsersList(ReceivedMessage msg)
        {
            StkRoomDetails.Children.Clear();
            currRoomData = new List<TextBlock>();
            TextBlock t;
            foreach (string name in msg.GetArgs())
            {
                t = new TextBlock();
                t.Text = name;
                t.HorizontalAlignment = HorizontalAlignment.Center;
                t.VerticalAlignment = VerticalAlignment.Center;
                t.FontFamily = new FontFamily("Papyrus");
                t.Foreground = new SolidColorBrush(Colors.DarkRed);
                currRoomData.Add(t);
                StkRoomDetails.Children.Add(t);
            }
            FitSize(null, null);
        }

        private void Return(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainMenu));
        }

        private void BtnJoin_Click(object sender, RoutedEventArgs e)
        {
            BtnJoin.IsEnabled = false;
            MessageDialog dialog;
            Helper.PlayConfirmSound();
            if (currName != "")
            {
                string id = Comms.GetPaddedNumber(roomIDs[currName], 4);
                Comms.SendData(Comms.JOIN_ROOM + id);
                Task response = new Task(async () =>
                {
                    ReceivedMessage msg = new ReceivedMessage();

                    if (msg.GetCode() == Comms.JOIN_ROOM_RES)
                    {
                        await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        {
                            if (msg[0] == "0")
                            {
                                object[] data = { currName, id, false };
                                Frame.Navigate(typeof(RoomPage), data);
                            }
                            else
                            {
                                dialog = new MessageDialog("An error occured. Please try again.");
                                await dialog.ShowAsync();
                            }
                        });
                    }
                });
                response.Start();
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Task t = new Task(() => GetRooms());
            t.Start();
        }
    }
}