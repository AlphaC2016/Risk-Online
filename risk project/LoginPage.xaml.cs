﻿using System;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace risk_project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<TextBlock> titles = new List<TextBlock>();
        List<TextBlock> labels = new List<TextBlock>();
        List<TextBox> boxes = new List<TextBox>();
        List<Button> buttons = new List<Button>();
        public MainPage()
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
            Frame.Navigate(typeof(MainMenu));
        }

        private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainMenu));
        }
    }
}