using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace risk_project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        List<TextBlock> titles = new List<TextBlock>();
        List<TextBlock> labels = new List<TextBlock>();
        List<TextBox> boxes = new List<TextBox>();
        List<Button> buttons = new List<Button>();
        public LoginPage()
        {
            this.InitializeComponent();

            titles.Add(LblLogin);
            titles.Add(LblSignUp);

            labels.Add(LblLoginUsername);
            labels.Add(LblLoginPass);
            labels.Add(LblSignUpPass);
            labels.Add(LblSignUpUsername);
            labels.Add(LblSignUpRepass);

            boxes.Add(TxbLoginPass);
            boxes.Add(TxbLoginUsername);
            boxes.Add(TxbSignUpPass);
            boxes.Add(TxbSignUpRepass);
            boxes.Add(TxbSignUpUsername);

            buttons.Add(BtnLogin);
            buttons.Add(BtnSignUp);

            Task connect = new Task(() =>
            {
                while (!Comms.InitSocket())
                    Task.Delay(5000);
            });
            connect.Start();
            Helper.Init();
        }

        private void FitSize(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock label in labels)
            {
                label.FontSize = (ActualHeight + ActualWidth) / 62.5;
            }

            foreach (TextBlock title in titles)
            {
                title.FontSize = (ActualHeight + ActualWidth) / 42;
            }

            foreach (TextBox box in boxes)
            {
                box.FontSize = (ActualHeight + ActualWidth) / 62.5;
                box.Width = ActualWidth / 6.4;
            }

            foreach (Button button in buttons)
            {
                button.FontSize = (ActualHeight + ActualWidth) / 62.5;
                button.Width = ActualWidth / 6.4;
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string message = Comms.SIGN_IN.ToString();
            string username = TxbLoginUsername.Text;
            string password = TxbLoginPass.Text;

            message += Comms.GetPaddedNumber(username.Length, 2) + username;
            message += Comms.GetPaddedNumber(password.Length, 2) + password;

            Task send = new Task(() => { Comms.SendData(message); });
            send.Start();
            send.Wait();

            var dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            Task getAnswer = new Task(async () =>
            {
                ReceivedMessage msg = new ReceivedMessage();

                if (msg.GetCode() == Comms.SIGN_IN_RES)
                {
                   await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                   {
                       MessageDialog dialog;
                       switch (msg[0])
                       {
                           case Comms.SIGN_IN_SUCCESS:
                               Frame.Navigate(typeof(MainMenu));
                               break;

                           case Comms.SIGN_IN_WRONG_DETAILS:
                               dialog = new MessageDialog("Incorrect username or password.");
                               dialog.ShowAsync();

                               TxbLoginUsername.Text = "";
                               TxbLoginPass.Text = "";
                               break;

                           default:
                               dialog = new MessageDialog("The specified user is already connected - please try again.");
                               dialog.ShowAsync();

                               TxbLoginUsername.Text = "";
                               TxbLoginPass.Text = "";
                               break;
                       }
                   });
                    
                }
            });
            getAnswer.Start();
        }

        private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog;
            if (TxbSignUpPass.Text != TxbSignUpRepass.Text)
            {
                dialog = new MessageDialog("Passwords do not match. try again.");
                dialog.ShowAsync();
                TxbSignUpPass.Text = "";
                TxbSignUpRepass.Text = "";
            }
            else
            {
                string message = Comms.SIGN_UP.ToString();
                string username = TxbSignUpUsername.Text;
                string password = TxbSignUpPass.Text;

                message += Comms.GetPaddedNumber(username.Length, 2) + username;
                message += Comms.GetPaddedNumber(password.Length, 2) + password;

                Comms.SendData(message);

                var dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
                Task getAnswer = new Task(async () =>
                {
                    ReceivedMessage msg = new ReceivedMessage();

                    if (msg.GetCode() == Comms.SIGN_UP_RES)
                    {
                        await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            switch (msg[0])
                            {
                                case Comms.SIGN_UP_SUCCESS:
                                    Frame.Navigate(typeof(MainMenu));
                                    break;

                                case Comms.SIGN_UP_USERNAME_ALREADY_EXISTS:
                                    dialog = new MessageDialog("This username already exists - please choose something else!");
                                    dialog.ShowAsync();
                                    TxbSignUpUsername.Text = "";
                                    TxbSignUpPass.Text = "";
                                    TxbSignUpRepass.Text = "";
                                    break;

                                case Comms.SIGN_UP_OTHER:
                                    dialog = new MessageDialog("An error occured. please try again.");
                                    dialog.ShowAsync();
                                    TxbSignUpUsername.Text = "";
                                    TxbSignUpPass.Text = "";
                                    TxbSignUpRepass.Text = "";
                                    break;
                            }
                        });

                    }
                });
                getAnswer.Start();
            }
        }
    }
}
