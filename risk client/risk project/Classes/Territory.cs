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
namespace risk_project
{
    class Territory : StackPanel
    {
        string owner;

        int _amount;
        int _prevAmount;

        public Territory(string name) : base()
        {
            TextBlock lbl = new TextBlock();
            lbl.FontFamily = new FontFamily("Papyrus");
            lbl.Foreground = new SolidColorBrush(Colors.Black);
            lbl.Text = name;
            Children.Add(lbl);

            lbl = new TextBlock();
            lbl.FontFamily = new FontFamily("Papyrus");
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            lbl.VerticalAlignment = VerticalAlignment.Center;
            //Color to be changed accordingly to player holding the point.
            lbl.Foreground = new SolidColorBrush(Colors.Black);
            lbl.Text = "0";
            lbl.FontWeight = FontWeights.Bold;
            Children.Add(lbl);
            Orientation = Orientation.Vertical;
        }

        public void SetOwner(string owner)
        {
            this.owner = owner;
        }

        public string GetOwner()
        {
            return this.owner;
        }

        public int GetAmount()
        {
            return this._amount;
        }

        public void SetAmount(int amount)
        {
            _prevAmount = _amount = amount;
            ((TextBlock)Children[1]).Text = amount.ToString();
        }

        public void SetColor(Color color)
        {
            foreach(TextBlock element in Children)
            {
                element.Foreground = new SolidColorBrush(color);
            }
        }
        
        public bool Inc(int total)
        {
            if (total == 0)
            {
                return false;
            }
            else
            {
                ((TextBlock)Children[1]).Text = (++_amount).ToString();
                return true;
            }
        }

        public bool Dec()
        {
            if (_amount == _prevAmount)
            {
                return false;
            }
            else
            {
                ((TextBlock)Children[1]).Text = (--_amount).ToString();
                return true;
            }
        }

        public void Confirm()
        {
            _prevAmount = _amount;
        }
    }
}
