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
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Composition.Interactions;
using Microsoft.Graphics.Canvas.UI.Composition;
using Windows.Graphics.DirectX;


// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace Slieder2017Cs
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>

    public sealed partial class MainPage : Page
    {
        private CompositionLinearGradientBrush linearGradientBrush;
        private CompositionRadialGradientBrush circleGradientBrush;

        private CompositionColorGradientStop ColorStop1;
        private ColorKeyFrameAnimation color1Animation;
        private ColorKeyFrameAnimation color2Animation;

        private Compositor compositor;
        private ContainerVisual canvasVisual;
        private Vector3 pointerPosition;

        public Vector2 sliderMargins = new Vector2(0, 0);
        public Color waveColor = Color.FromArgb(255, 244, 191, 106);
        public Color backgroundColor = Color.FromArgb(255, 240, 170, 55);
        public String switchText;
        private TextBlock text;
        private float theMileOfSwitch;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += PageLoaded;
            
        }

        private CompositionLinearGradientBrush createLinearGradientBrush(Compositor compositor)
        {
            CompositionLinearGradientBrush brush = compositor.CreateLinearGradientBrush();
            brush.StartPoint = new Vector2(0, 1);
            brush.EndPoint = new Vector2(0f, 1);
            brush.ColorStops.Insert(0, compositor.CreateColorGradientStop(0f, backgroundColor));
            brush.ColorStops.Insert(1, compositor.CreateColorGradientStop(0.5f, waveColor));
            brush.ColorStops.Insert(2, compositor.CreateColorGradientStop(1f, backgroundColor));
            return brush;
        }

        private ShapeVisual CreateTopCircleButton(Compositor compositor)
        {
            // Main circle
            CompositionRoundedRectangleGeometry circleGeometry = compositor.CreateRoundedRectangleGeometry();
            circleGeometry.Size = new Vector2(40, 40);
            circleGeometry.CornerRadius = new Vector2(20, 20);
            CompositionSpriteShape compositionSpriteShape = compositor.CreateSpriteShape(circleGeometry);
            circleGradientBrush = compositor.CreateRadialGradientBrush();

            // Color animation
            ColorStop1 = compositor.CreateColorGradientStop(1, Colors.White);
            circleGradientBrush.ColorStops.Add(ColorStop1);
            color1Animation = compositor.CreateColorKeyFrameAnimation();
            color1Animation.Duration = TimeSpan.FromSeconds(0.5);
            color1Animation.InsertKeyFrame(1.0f, Color.FromArgb(255, 247, 211, 156));
            color1Animation.InsertKeyFrame(0.0f, Colors.White);
            color2Animation = compositor.CreateColorKeyFrameAnimation();
            color2Animation.Duration = TimeSpan.FromSeconds(0.5);
            color2Animation.InsertKeyFrame(1.0f, Colors.White);
            color2Animation.InsertKeyFrame(0.0f, Color.FromArgb(255, 247, 211, 156));

            compositionSpriteShape.FillBrush = circleGradientBrush;
            compositionSpriteShape.Offset = new Vector2(5, 5);

            // Shadow
            CompositionRoundedRectangleGeometry circleShaowGeometry = compositor.CreateRoundedRectangleGeometry();
            circleShaowGeometry.Size = new Vector2(50, 50);
            circleShaowGeometry.CornerRadius = new Vector2(25, 25);
            CompositionSpriteShape compositionSpriteShapeShadow = compositor.CreateSpriteShape(circleShaowGeometry);
            CompositionRadialGradientBrush compositionLinearGradientShadowBrush = compositor.CreateRadialGradientBrush();
            compositionLinearGradientShadowBrush.ColorStops.Insert(0, compositor.CreateColorGradientStop(0.5f, Color.FromArgb(255, 86, 57, 14)));
            compositionLinearGradientShadowBrush.ColorStops.Insert(1, compositor.CreateColorGradientStop(1f, Color.FromArgb(255, 230, 160, 53)));
            compositionLinearGradientShadowBrush.Offset = new Vector2(0, 3);
            compositionSpriteShapeShadow.FillBrush = compositionLinearGradientShadowBrush;

            ShapeVisual shapeVisual = compositor.CreateShapeVisual();
            shapeVisual.Size = new Vector2(50, 50);
            shapeVisual.Shapes.Add(compositionSpriteShapeShadow);
            shapeVisual.Shapes.Add(compositionSpriteShape);
            shapeVisual.Offset = new Vector3(sliderMargins.X, 0, 0);

            _leftoffsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            _leftoffsetAnimation.InsertKeyFrame(1.0f, new Vector3(0 + sliderMargins.X, 0, 0));
            _leftoffsetAnimation.Duration = TimeSpan.FromSeconds(0.2f);
            _rightoffsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            _rightoffsetAnimation.InsertKeyFrame(1.0f, new Vector3(100 - sliderMargins.Y, 0, 0));
            _rightoffsetAnimation.Duration = TimeSpan.FromSeconds(0.2f);

            return shapeVisual;
        }

        private Vector3KeyFrameAnimation _leftoffsetAnimation;
        private Vector3KeyFrameAnimation _rightoffsetAnimation;
        private ShapeVisual shapeVisualCircle;

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            switchText = "SWITCH";
            text = new TextBlock()
            {
                Text = switchText,
                HorizontalTextAlignment = TextAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 25, 0),
                FontWeight = FontWeights.Bold

            };
            canvas.Content = text;
            Visual textVisual = ElementCompositionPreview.GetElementVisual(text);


            theMileOfSwitch = (100.0f / 2f) - sliderMargins.Y + sliderMargins.X;
            compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            ContainerVisual root = compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(canvas, root);

            // ELIPSE
            canvasVisual = root;
            CompositionRoundedRectangleGeometry roundedRectangle = compositor.CreateRoundedRectangleGeometry();
            roundedRectangle.Size = new Vector2(150, 50);
            roundedRectangle.CornerRadius = new Vector2(25, 25);
            CompositionSpriteShape compositionSpriteShape = compositor.CreateSpriteShape(roundedRectangle);
            linearGradientBrush = createLinearGradientBrush(compositor);
            compositionSpriteShape.FillBrush = linearGradientBrush;
            ShapeVisual shapeVisual = compositor.CreateShapeVisual();
            shapeVisual.Size = new Vector2(150, 50);
            shapeVisual.Shapes.Add(compositionSpriteShape);
            canvasVisual.Children.InsertAtTop(shapeVisual);
            // ----------------------------------------------

            // TEXT
            canvasVisual.Children.InsertAtTop(textVisual);

            // CIRCLE
            shapeVisualCircle = CreateTopCircleButton(compositor);

            canvasVisual.Children.InsertAtTop(shapeVisualCircle);
        }

        private void Page_Unload(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }

        private void Canvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {

        }

        float i = 0; // DBG
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
            //textBox.Text = i.ToString();
        }

        bool isPressed = false;
        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isPressed = true;
            ColorStop1.StartAnimation(nameof(ColorStop1.Color), color1Animation);
            canvas_PointerMoved(sender, e);
        }

        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isPressed == true)
            {
                pointerPosition = new Vector3(e.GetCurrentPoint(canvas).Position.ToVector2().X - 25, 0, 0);
                if (pointerPosition.X <= 0 + sliderMargins.X)
                    pointerPosition.X = 0 + sliderMargins.X;
                if (pointerPosition.X >= 100 - sliderMargins.Y)
                    pointerPosition.X = 100 - sliderMargins.Y;
                shapeVisualCircle.Offset = pointerPosition;
                textBox.Text = pointerPosition.X.ToString();
            }
        }

        private void canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ColorStop1.StartAnimation(nameof(ColorStop1.Color), color2Animation);
            isPressed = false;
            if (pointerPosition.X < theMileOfSwitch)
            {
                shapeVisualCircle.StartAnimation(nameof(Visual.Offset), _leftoffsetAnimation);
            }
            else
            {
                shapeVisualCircle.StartAnimation(nameof(Visual.Offset), _rightoffsetAnimation);
            }
        }

        private void canvas_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            canvas_PointerReleased(sender, e);
        }

        private void canvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //canvas_PointerReleased(sender, e);           
        }
    }
}
