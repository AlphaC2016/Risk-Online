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
using Windows.UI.Xaml.Media.Imaging;

namespace risk_project
{
    class Territory : StackPanel
    {
        string owner;

        int _amount;
        int _prevAmount;


        /// <summary>
        /// The default constructor. Builds the element and the appropriate content.
        /// </summary>
        /// <param name="name">The territory's name.</param>
        public Territory(string name) : base()
        {
            TextBlock lbl = new TextBlock();
            lbl.FontFamily = new FontFamily("Papyrus");
            lbl.Foreground = new SolidColorBrush(Colors.Black);
            lbl.Text = name;
            Children.Add(lbl);
            Name = name;


            lbl = new TextBlock();
            lbl.FontFamily = new FontFamily("Papyrus");
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            lbl.VerticalAlignment = VerticalAlignment.Center;
            //Color to be changed accordingly to player holding the point.
            lbl.Foreground = new SolidColorBrush(Colors.Black);
            lbl.Text = "?";
            _amount = _prevAmount = 0;
            lbl.FontWeight = FontWeights.Bold;
            Children.Add(lbl);
            Orientation = Orientation.Vertical;


            BitmapImage bmp = new BitmapImage(new Uri("ms-appx:///Assets/Icons/shine.png"));
            Background = new ImageBrush
            {
                ImageSource = bmp, Stretch = Stretch.Fill, Opacity=0
            };
            
        }

        /// <summary>
        /// sets the territory's owner.
        /// </summary>
        /// <param name="owner">The new user.</param>
        public void SetOwner(string owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Returns the name of the owner.
        /// </summary>
        /// <returns></returns>
        public string GetOwner()
        {
            return this.owner;
        }

        /// <summary>
        /// Returns the amount of units in the territory.
        /// </summary>
        /// <returns></returns>
        public int GetAmount()
        {
            return this._amount;
        }

        /// <summary>
        /// Sets a new amount of units in the territory.
        /// WARNING: UNSAFE FUNCTION: SETS BOTH PREV AND CURR VALUES TO THE PARAMETER.
        /// </summary>
        /// <param name="amount">The new amount of units.</param>
        public void SetAmount(int amount)
        {
            _prevAmount = _amount = amount;
            ((TextBlock)Children[1]).Text = amount.ToString();
        }

        /// <summary>
        /// Changes the territory's color.
        /// </summary>
        /// <param name="color">The new color.</param>
        public void SetColor(Color color)
        {
            foreach(TextBlock element in Children)
            {
                element.Foreground = new SolidColorBrush(color);
            }
        }


        /// <summary>
        /// This function does a controlled increase in the territory's amount of units.
        /// </summary>
        /// <param name="total">The total amount of units available (relevant in reinforcement mode).</param>
        /// <param name="state">The game's current state (different rules apply in different states). defaults to Initial Reinforcements.</param>
        /// <returns>Returns whether the increase was valid and successful.</returns>
        public bool Inc(int total = 0, GameState state = GameState.InitialReinforcments)
        {
            if (state == GameState.InitialReinforcments && total == 0)
            {
                return false;
            }
            else
            {
                ((TextBlock)Children[1]).Text = (++_amount).ToString();
                return true;
            }
        }


        /// <summary>
        /// This function does a controlled decrease in the territory's  amount of units.
        /// </summary>
        /// <param name="state">The game's current state (different rules apply in different states). defaults to Initial Reinforcements.</param>
        /// <returns>Returns whether the decrease was valid and successful.</returns>
        public bool Dec(GameState state = GameState.InitialReinforcments)
        {

            if ((state == GameState.InitialReinforcments && _amount == _prevAmount) || _amount == 0)
            {
                return false;
            }
            else if (_amount > 1)
            {
                ((TextBlock)Children[1]).Text = (--_amount).ToString();
                return true;
            }
            return false;
        }


        /// <summary>
        /// Confirms that the current amount is valid and sets the previous amount to it.
        /// </summary>
        public void Confirm()
        {
            _prevAmount = _amount;
        }

        /// <summary>
        /// Reverts the current amount to the previous one.
        /// </summary>
        public void Revert()
        {
            _amount = _prevAmount;
        }


        /// <summary>
        /// Checks if the current value is equal to the previous value.
        /// </summary>
        /// <returns>Returns true if the two are equal, false otherwise.</returns>
        public bool Compare()
        {
            return _amount == _prevAmount;
        }
    }
}
