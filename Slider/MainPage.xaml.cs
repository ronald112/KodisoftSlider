using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI;
using System.Numerics;
using Windows.UI.Text;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;


namespace Slider
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            // Make almost reusable switch
            this.InitializeComponent();
        }

        private void Page_Unload(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }
    }
}
