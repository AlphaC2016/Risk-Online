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
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace risk_project
{
    enum GameState
    {
        InitialReinforcments,
        Reinforcements,
        StopOrAttack,
        Attacker,
        Spectator,
        Defender,
        MoveForces
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

        Territory src;
        Territory dst;

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
            LblInstructions.Text = "SET YOUR FORCES IN PLACE";
            handler = new Task(() =>
            {
                ServerHandler();
            });
            handler.Start();
            FitSize();
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
                t.PointerEntered += T_PointerEntered;
                t.PointerExited += T_PointerExited;
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
        private void FitSize(object sender = null, SizeChangedEventArgs e = null)
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

            Canvas.SetLeft(GrdUsers, ActualWidth / 42.6667);
            Canvas.SetTop(GrdUsers, ActualHeight / 1.558);
            GrdUsers.Height = ActualHeight / 4.32;
            GrdUsers.Width = ActualWidth / 6.4;
            foreach (Rectangle rect in colorRects)
            {
                rect.Height = rect.Width = ActualWidth / 64;
            }
            foreach (TextBlock txb in nameLabels)
            {
                txb.FontSize = (ActualHeight + ActualWidth) / 83.333;
            }
            
            foreach (TextBlock txb in StkMessages.Children)
            {
                txb.FontSize = (ActualHeight * ActualWidth) / 80000;
            }

            LblInstructions.FontSize = (ActualHeight + ActualWidth) / 83.333;

            Canvas.SetLeft(GrdChat, ActualWidth / 1.2);
            Canvas.SetTop(GrdChat, ActualHeight / 2.634);
            GrdChat.Height = GrdChat.Width = (ActualHeight + ActualWidth) / 10;

            Canvas.SetLeft(ElpNo, ActualWidth / 1.066);
            Canvas.SetTop(ElpNo, ActualHeight / 1.384);
            ElpNo.Height = ElpNo.Width = (ActualHeight + ActualWidth) / 40;

            Canvas.SetLeft(ElpYes, ActualWidth / 1.164);
            Canvas.SetTop(ElpYes, ActualHeight / 1.384);
            ElpYes.Height = ElpYes.Width = (ActualHeight + ActualWidth) / 40;

            Canvas.SetLeft(RectQuit, ActualWidth / 1.052);
            Canvas.SetTop(RectQuit, ActualHeight / 43.2);
            RectQuit.Height = RectQuit.Width = (ActualHeight + ActualWidth) / 40;

            Canvas.SetLeft(LblSecondary, ActualWidth / 3.84);
            Canvas.SetTop(LblSecondary, ActualHeight / 1.091);
            LblSecondary.FontSize = (ActualHeight + ActualWidth) / 100;

            TxbMessage.FontSize = (ActualHeight * ActualWidth) / 80000;
            BtnSend.FontSize = (ActualHeight * ActualWidth) / 120000;
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
                msg = new ReceivedMessage(1);
                code = msg.GetCode();

                switch (code)
                {
                    case Comms.INIT_MAP:
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            HandleInitMap(msg);
                            SetReinforcements();
                        });
                        break;

                    case Comms.RECEIVE_MESSAGE:
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HandleUserMessage(msg));
                        break;

                    case Comms.UPDATE_MAP:
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HandleUpdate(msg));
                        break;

                    case Comms.START_TURN:
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HandleStartTurn(msg));
                        break;

                    case Comms.MOVE_FORCES_RES:
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HandleMoveForcesRes(msg));
                        break;
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
            content += Comms.GetPaddedNumber(TxbMessage.Text.Length, 2);
            content += TxbMessage.Text;
            TxbMessage.Text = "";
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

        //---------------------------------------- MESSAGE HANDLERS -----------------------------------------------


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
                if (msg[i] == Helper.Username)
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
                colorRects.Add(rect);
                colors.Add(msg[i], color);
            }

            foreach (Territory t in territories.Values)
            {
                t.SetOwner(msg[i]);
                t.SetColor(colors[msg[i]]);
                if (msg[i] == Helper.Username)
                {
                    t.SetAmount(0);
                    territoryCount++;
                }
                i++;
            }
            FitSize();
            SetReinforcements();
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
            txb.HorizontalAlignment = HorizontalAlignment.Left;
            txb.VerticalAlignment = VerticalAlignment.Center;
            txb.FontFamily = new FontFamily("Papyrus");
            txb.Foreground = new SolidColorBrush(colors[user]);
            messageLabels.Add(txb);
            StkMessages.Children.Add(txb);
            FitSize(null, null);
        }


        private void HandleStartTurn(ReceivedMessage msg)
        {
            if (Helper.Username == msg[0])
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

        private void HandleMoveForcesRes(ReceivedMessage msg)
        {
            if (msg[0] == "1")
            {
                PresentError("The territories must be connected.");
                src.Revert();
                dst.Revert();
            }
            src = dst = null;
        }

        private void SetReinforcements()
        {
            currState = GameState.InitialReinforcments;
            LblInstructions.Text = "SET YOUR FORCES IN PLACE";

            temp = 50 - (5*colorRects.Count());
            LblSecondary.Text = "amount Left: " + temp + " units";
            
        }

        private void HandleUpdate(ReceivedMessage msg)
        {
            for (int i=0; i<Helper.TERRITORY_AMOUNT; i++)
            {
                territories.ElementAt(i).Value.SetOwner(msg[i * 2]);
                territories.ElementAt(i).Value.SetAmount(int.Parse(msg[i * 2 + 1]));
            }

            switch (currState)
            {
                case GameState.Reinforcements:
                    LblInstructions.Text = "Would you like to attack?";
                    LblSecondary.Text = "click ✓ to attack, X to start moving forces.";
                    currState = GameState.StopOrAttack;
                    break;
            }
        }


        //---------------------------------------- PRACTICAL BUTTON FUNCTIONS -------------------------------------

        private void T_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Territory curr = sender as Territory;
            if (currState == GameState.InitialReinforcments || currState == GameState.Reinforcements)
            {
                if (curr.GetOwner() == Helper.Username)
                {
                    var clickType = e.GetCurrentPoint(null).Properties;

                    if (clickType.IsLeftButtonPressed)
                    {
                        if (curr.Inc(temp))
                        {
                            temp--;
                            LblSecondary.Text = "amount Left: " + temp + " units";
                        }
                    }
                    else if (clickType.IsRightButtonPressed)
                    {
                        if (curr.Dec())
                        {
                            temp++;
                            LblSecondary.Text = "amount Left: " + temp + " units";
                        }
                    }
                }
            }

            else if (currState == GameState.MoveForces)
            {
                if (curr.GetOwner() == Helper.Username)
                {
                    if (src == null)
                    {
                        src = curr;
                    }
                    else if (dst == null)
                    {
                        dst = curr;
                    }
                    else if (curr == src)
                    {
                        if (dst.Dec(currState))
                        {
                            src.Inc(state: currState);
                            temp--;
                        }
                    }
                    else if (curr == dst)
                    {
                        if (src.Dec(currState))
                        {
                            dst.Inc(state: currState);
                            temp++;
                        }
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

                    foreach (Territory curr in territories.Values)
                    {
                        if (curr.GetAmount() != 0 || curr.GetOwner() != Helper.Username)
                        {
                            
                            message += Comms.GetPaddedNumber(curr.GetAmount(), 2);
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
                        foreach (Territory curr in territories.Values)
                        {
                            curr.Confirm();
                        }
                        LblInstructions.Text = "Waiting for other players...";
                    }
                    break;

                case GameState.Reinforcements:

                    int count = 0;
                    message = Comms.SEND_REINFORCEMENTS;
                    Territory t;
                    for (int i=0; i<Helper.TERRITORY_AMOUNT; i++)
                    {
                        t = territories.ElementAt(i).Value;
                        if (!t.Compare())
                        {
                            message += Comms.GetPaddedNumber(i, 2);
                            message += Comms.GetPaddedNumber(t.GetAmount(), 2);
                            t.Confirm();
                            count++;
                        }
                    }
                    message = message.Insert(3, Comms.GetPaddedNumber(count, 2));
                    Comms.SendData(message);
                    break;

                case GameState.StopOrAttack:
                    currState = GameState.Attacker;
                    LblInstructions.Text = "ATTACK MODE // DEBUG";
                    LblSecondary.Text = "ATTACK MODE // DEBUG";
                    break;

                case GameState.MoveForces:
                    if (src != null && dst != null)
                    {
                        message = Comms.MOVE_FORCES;
                        message += Comms.GetPaddedNumber(Helper.GetIndex(territories, src), 2);
                        message += Comms.GetPaddedNumber(Helper.GetIndex(territories, dst), 2);
                        message += Comms.GetPaddedNumber(temp, 2);
                        Comms.SendData(message);

                        temp = 0;
                    }
                    break;
            }
        }

        private void ElpNo_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            string message;
            switch (currState)
            {
                case GameState.InitialReinforcments | GameState.Reinforcements:
                    foreach (Territory t in territories.Values)
                    {
                        if (t.GetOwner() == Helper.Username)
                        {
                            t.Revert();
                        }
                    }
                    break;

                case GameState.StopOrAttack:
                    currState = GameState.MoveForces;
                    LblInstructions.Text = "MOVE YOUR FORCES!";
                    LblSecondary.Text = "pick the forrce movement pair.";
                    break;

                case GameState.MoveForces:
                    if (src != null)
                    {
                        src.Revert();
                        if (dst != null)
                        {
                            dst.Revert();
                        }
                        src = dst = null;
                    }

                    else
                    {
                        message = Comms.END_TURN;
                        Comms.SendData(message);
                    }
                    break;
            }
        }

        private void RectQuit_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Comms.SendData(Comms.QUIT_GAME);
            Frame.Navigate(typeof(MainMenu));
        }


        //------------------------------------------- COSMETIC FUNCTIONS ------------------------------------------

        private void T_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Territory t = (Territory)sender;
            t.Background.Opacity = 0.8;
        }

        private void T_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Territory t = (Territory)sender;

            if (t != src && t != dst)
                t.Background.Opacity = 0;
        }

        private void ElpYes_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            BitmapImage bmp = new BitmapImage(new Uri("ms-appx:///Assets/Icons/Yes2.png"));
            ElpYes.Fill = new ImageBrush
            {
                ImageSource = bmp,
                Stretch = Stretch.Fill,
            };
        }

        private void ElpYes_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            BitmapImage bmp = new BitmapImage(new Uri("ms-appx:///Assets/Icons/Yes1.png"));
            ElpYes.Fill = new ImageBrush
            {
                ImageSource = bmp,
                Stretch = Stretch.Fill,
            };
        }

        private void ElpNo_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            BitmapImage bmp = new BitmapImage(new Uri("ms-appx:///Assets/Icons/No2.png"));
            ElpNo.Fill = new ImageBrush
            {
                ImageSource = bmp,
                Stretch = Stretch.Fill,
            };
        }

        private void ElpNo_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            BitmapImage bmp = new BitmapImage(new Uri("ms-appx:///Assets/Icons/No1.png"));
            ElpNo.Fill = new ImageBrush
            {
                ImageSource = bmp,
                Stretch = Stretch.Fill,
            };
        }
    }
}
