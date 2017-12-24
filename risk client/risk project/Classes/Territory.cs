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

        public Territory() : base()
        {

        }

        public void SetOwner(string owner) { this.owner = owner; }
        public string GetOwner() { return this.owner; }

        public void SetColor(Color color)
        {
            foreach(TextBlock element in Children)
            {
                element.Foreground = new SolidColorBrush(color);
            }
        }

        public TextBlock this[int index]
        {
            get { return (TextBlock)Children.ElementAt(index); }
        }
    }
}
