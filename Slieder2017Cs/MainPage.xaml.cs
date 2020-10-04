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
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Text;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace Slieder2017Cs
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>

    public sealed partial class MainPage : Page
    {
        CanvasLinearGradientBrush brush;
        Rect rect;
        CompositionDrawingSurface drawingSurface;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Unload(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }

        async Task CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender)
        {

            brush = new CanvasLinearGradientBrush(sender, Colors.Orange, Colors.White);
            //brush.Image = await CanvasBitmap.LoadAsync(sender, "Assets/drone.jpg");
            //brush.StartPoint = new Vector2(sender.CenterPoint.X-75, sender.CenterPoint.Y);
            brush.StartPoint = new Vector2(sender.CenterPoint.X - 80, sender.CenterPoint.Y);
            brush.EndPoint = new Vector2(sender.CenterPoint.X - 74, sender.CenterPoint.Y);
            brush.StartPoint = new Vector2(sender.CenterPoint.X - 70, sender.CenterPoint.Y);
            rect = new Rect(sender.CenterPoint.X, sender.CenterPoint.Y, 150, 50);
        }

        private void Canvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateResources(sender).AsAsyncAction());
        }

        int i = 0;
        private void CanvasControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {

            //var myBitmap = new CanvasRenderTarget(sender, 300, 300);
            //using (var ds = myBitmap.CreateDrawingSession())
            //{
            //    ds.FillRoundedRectangle(rect, 25, 25, brush);
            //}

            //if (sender.Paused == true)
            //    sender.Paused = false;
            //else
            //    sender.Paused = true;

            //args.DrawingSession.DrawText($"Hello {i}", 50, 50, brush, format);


            using (args.DrawingSession.CreateLayer(brush))
            {
                args.DrawingSession.FillRoundedRectangle(rect, 25, 25, brush);
                
            }
        }

        private void Canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            i++;
        }

        private void canvas_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            
        }

        private void canvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //brush.StartPoint = new Vector2(brush.StartPoint.X - 20, brush.StartPoint.Y);
            
        }

        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {

        }
    }
}
