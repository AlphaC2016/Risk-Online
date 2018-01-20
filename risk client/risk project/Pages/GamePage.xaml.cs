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
    enum GameState
    {
        InitialReinforcments,
        Attacker,
        Spectator,
        Reinforcements,
        Defender
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        List<string[]> labelData;
        Dictionary<string, Territory> territories;

        Dictionary<string, Color> colors;
        List<Rectangle> colorRects;
        List<TextBlock> messageLabels;
        List<TextBlock> nameLabels;

        bool done;
        Task handler;
        CoreDispatcher dispatcher;

        GameState currState;
        int temp;
        int territoryCount;

        //---------------------------------------PAGE HANDLING CODE---------------------------------------------------

        /// <summary>
        /// The regular constructor. Initializes all the containers.
        /// </summary>
        public GamePage()
        {
            this.InitializeComponent();

            labelData = new List<string[]>();
            territories = new Dictionary<string, Territory>();
            colors = new Dictionary<string, Color>();
            colorRects = new List<Rectangle>();
            messageLabels = new List<TextBlock>();
            nameLabels = new List<TextBlock>();

            dispatcher = Window.Current.Dispatcher;
            done = false;
            territoryCount = 0;
        }


        /// <summary>
        /// This function starts the board building and starts up the comms task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BuildBoard();

            handler = new Task(() =>
            {
                ServerHandler();
            });
            handler.Start();
        }


        /// <summary>
        /// This function builds the basic board for the game according to mapdata.csv
        /// </summary>
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

            Territory t;
            foreach (string[] line in labelData)
            {
                string name = line[0].Replace('#', '\n');
                t = new Territory(name);
                t.PointerPressed += T_PointerPressed;
                territories.Add(name, t);
            }

            FitSize(null, null);

            foreach (Territory curr in territories.Values)
            {
                Arena.Children.Add(curr);
            }
        }


        /// <summary>
        /// This function fits all elements to the new screen size, thus making everything adaptive. (yaaaaaay.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FitSize(object sender, SizeChangedEventArgs e)
        {
            IEnumerable<TextBlock> content;
            IEnumerable<Territory> vals = territories.Values;

            for (int i = 0; i < territories.Count; i++)
            {
                Canvas.SetLeft(vals.ElementAt(i), this.ActualWidth / double.Parse(labelData[i][1]));
                Canvas.SetTop(vals.ElementAt(i), this.ActualHeight / double.Parse(labelData[i][2]));

                content = vals.ElementAt(i).Children.Cast<TextBlock>();
                foreach (TextBlock lbl in content)
                {
                    lbl.FontSize = (ActualHeight + ActualWidth) / 150;
                }    
            }

            Canvas.SetTop(GrdUsers, ActualHeight / 1.558);
            Canvas.SetLeft(GrdUsers, ActualWidth / 42.6667);
            GrdUsers.Height = ActualHeight / 4.32;
            GrdUsers.Width = ActualWidth / 6.4;
            foreach (Rectangle rect in colorRects)
            {
                rect.Height = rect.Width = ActualWidth / 64;
            }
        }


        /// <summary>
        /// This function is a thread-like task that handles all communication with the server.
        /// </summary>
        private async void ServerHandler()
        {
            ReceivedMessage msg;
            string code;
            while (!done)
            {
                msg = new ReceivedMessage();
                code = msg.GetCode();

                if (code == Comms.INIT_MAP)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        HandleInitMap(msg);
                        
                    });
                }
                else if (code == Comms.RECEIVE_MESSAGE)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HandleUserMessage(msg));
                }
            }
        }


        /// <summary>
        /// This function sends a message to all the other players in the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendUserMessage(object sender, RoutedEventArgs e)
        {
            string content = Comms.SEND_MESSAGE;
            content += TxbMessage.Text;
            Comms.SendData(content);
        }

        
        private void PresentError(string message)
        {
            string currContent = LblSecondary.Text;
            Task t = new Task(async() =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    LblSecondary.Text = message;
                    LblSecondary.Foreground = new SolidColorBrush(Colors.Red);
                });
                await Task.Delay(5000);
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (LblSecondary.Text == message)
                    {
                        LblSecondary.Text = currContent;
                        LblSecondary.Foreground = new SolidColorBrush(Colors.White);
                    }
                });
            });
            t.Start();
        }

        //-----------------------------GAME HANDLING CODE------------------------------------------


        /// <summary>
        /// This function sets the territories' ownership according to the users.
        /// </summary>
        /// <param name="msg">The server message which contains the ownership data.</param>
        private void HandleInitMap(ReceivedMessage msg)
        {
            int count = int.Parse(msg[0]);
            int i = 1;

            TextBlock txb;
            Rectangle rect;
            Color color;
            for (i = 1; i <= count; i++)
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
                nameLabels.Add(txb);

                rect = new Rectangle();
                if (msg[i] == Helper.username)
                {
                    color = Helper.GetPlayerColor();
                    territoryCount++;
                }
                else
                {
                    color = Helper.GetRandomColor();
                }
                rect.Fill = new SolidColorBrush(color);
                Grid.SetColumn(rect, 0);
                Grid.SetRow(rect, i - 1);
                GrdUsers.Children.Add(rect);
                colorRects.Add(rect);
                colors.Add(msg[i], color);
            }

            foreach (Territory t in territories.Values)
            {
                t.SetOwner(msg[i]);
                t.SetColor(colors[msg[i]]);
                i++;
            }
        }


        /// <summary>
        /// This function handles a user message and displays it in the message log.
        /// </summary>
        /// <param name="msg">The server message.</param>
        private void HandleUserMessage(ReceivedMessage msg)
        {
            TextBlock txb = new TextBlock();
            string user = msg[0], content = msg[1];
            txb.Text = user + ": " + content;
            txb.HorizontalAlignment = HorizontalAlignment.Center;
            txb.VerticalAlignment = VerticalAlignment.Center;
            txb.FontFamily = new FontFamily("Papyrus");
            txb.Foreground = new SolidColorBrush(Colors.DarkRed);
            messageLabels.Add(txb);
            StkMessages.Children.Add(txb);
            FitSize(null, null);
        }


        private void HandleStartTurn(ReceivedMessage msg)
        {
            if (Helper.username == msg[0])
            {
                currState = GameState.Reinforcements;
                LblInstructions.Text = "IT'S YOUR TURN!\nSET YOUR FORCES";
                temp = territoryCount / 3;
                LblSecondary.Text = "Units Remaining: " + temp;
            }
            else
            {
                currState = GameState.Spectator;
            }
        }

        private void SetReinforcements()
        {
            currState = GameState.InitialReinforcments;
            LblInstructions.Text = "SET YOUR FORCES IN PLACE";

            temp = 50 - (5*colorRects.Count());
            LblSecondary.Text = "amount Left: " + temp + " units";
            
        }

        private void T_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (currState == GameState.InitialReinforcments || currState == GameState.Reinforcements)
            {
                Territory curr = sender as Territory;
                var clickType = e.GetCurrentPoint(null).Properties;

                if (clickType.IsLeftButtonPressed)
                {
                    if (curr.Inc(temp))
                    {
                        LblSecondary.Text = "amount Left: " + --temp + " units";
                    }
                }
                else if (clickType.IsRightButtonPressed)
                {
                    if (curr.Dec())
                    {
                        LblSecondary.Text = "amount Left: " + ++temp + " units";
                    }
                }
            }
            
        }

        private void ElpYes_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            switch (currState)
            {
                case GameState.InitialReinforcments:
                    string message = Comms.FORCES_INIT;
                    bool ok = true;

                    foreach (Territory t in territories.Values)
                    {
                        if (t.GetAmount() != 0 || t.GetOwner() != Helper.username)
                        {
                            
                            message += Comms.GetPaddedNumber(t.GetAmount(), 2);
                        }
                        else
                        {
                            PresentError("all your territories must contain at least one unit.");
                            ok = false;
                            break;
                        }
                    }
                    if (ok)
                    {
                        Comms.SendData(message);
                        foreach (Territory t in territories.Values)
                        {
                            t.Confirm();
                        }
                    }
                    break;

                case GameState.Reinforcements:
                    message = Comms.SEND_REINFORCEMENTS;
                    foreach (Territory t in territories.Values)
                    {
                        message += Comms.GetPaddedNumber(t.GetAmount(), 2);
                    }
                    Comms.SendData(message);
                    break;
            }
        }

        private void ElpNo_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Comms.SendData(Comms.QUIT_GAME);
            Frame.Navigate(typeof(MainMenu));
        }
    }
}
