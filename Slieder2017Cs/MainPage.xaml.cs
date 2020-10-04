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
        CompositionDrawingSurface drawingSurface;
        CompositionLinearGradientBrush linearGradientBrush;
        Compositor compositor;
        ContainerVisual containerVisual;

        private CompositionLinearGradientBrush createLinearGradientBrush(Compositor compositor)
        {
            CompositionLinearGradientBrush brush = compositor.CreateLinearGradientBrush();
            brush.StartPoint = new Vector2(0, 1);
            brush.EndPoint = new Vector2(0f, 1);
            brush.ColorStops.Insert(0, compositor.CreateColorGradientStop(0f, Color.FromArgb(255, 240, 170, 55)));
            brush.ColorStops.Insert(1, compositor.CreateColorGradientStop(0.5f, Color.FromArgb(255, 244, 191, 106)));
            brush.ColorStops.Insert(2, compositor.CreateColorGradientStop(1f, Color.FromArgb(255, 240, 170, 55)));
            return brush;
        }

        public MainPage()
        {
            this.InitializeComponent(); 

            containerVisual = GetVisual(canvas);
            compositor = containerVisual.Compositor;

            CompositionSpriteShape compositionSpriteShape;
            CompositionRoundedRectangleGeometry roundedRectangle = compositor.CreateRoundedRectangleGeometry();
            roundedRectangle.Size = new Vector2(150, 50);
            roundedRectangle.CornerRadius = new Vector2(25, 25);
            compositionSpriteShape = compositor.CreateSpriteShape(roundedRectangle);
            linearGradientBrush = createLinearGradientBrush(compositor);
            compositionSpriteShape.FillBrush = linearGradientBrush;
            compositionSpriteShape.CenterPoint = new Vector2(canvas.CenterPoint.X, canvas.CenterPoint.Y);
            ShapeVisual shapeVisual = compositor.CreateShapeVisual();
            shapeVisual.Size = new Vector2(150, 50);
            shapeVisual.Shapes.Add(compositionSpriteShape);
            containerVisual.Children.InsertAtTop(shapeVisual);
        }

        private void Page_Unload(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }

        private ContainerVisual GetVisual(UIElement element)
        {
            Visual hostVisual = ElementCompositionPreview.GetElementVisual(element);
            ContainerVisual root = hostVisual.Compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(element, root);
            return root;
        }

        private void Canvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {

        }

        float i = 0;
        private void CanvasControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            
        }

        private void Canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            if (i < 0.5f)
                i += 0.03f;
            else if (i > 0.5f && i < 2.0f)
                i += 0.05f;
            else if (i > 1.0f && i < 2.0f)
                i += 0.1f;
            else
                i += 0.1f;
            linearGradientBrush.StartPoint = new Vector2(i - 0.1f, 1);
            linearGradientBrush.EndPoint = new Vector2(i + 0.2f, 1);
            if (i >= 14.0f)
                i = 0f;
            
        }

        private void canvas_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            textBox.Text = i.ToString();
        }

        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            textBox.Text = i.ToString();
        }
    }
}
