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
        private CompositionLinearGradientBrush linearGradientBrush;
        private CompositionRadialGradientBrush circleGradientBrush;

        private CompositionColorGradientStop ColorStop1;
        private ColorKeyFrameAnimation color1Animation;
        private ColorKeyFrameAnimation color2Animation;

        private Compositor compositor;
        private ContainerVisual canvasVisual;
        private Vector3 pointerPosition;
        private ShapeVisual shapeVisualElipse;
        private ShapeVisual shapeVisualCircle;
        private ShapeVisual shapeVisualShadow;

        private Vector3KeyFrameAnimation _leftoffsetAnimation;
        private Vector3KeyFrameAnimation _rightoffsetAnimation;

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

        // Main circle
        private void CreateTopCircleButton()
        {
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

            shapeVisualCircle = compositor.CreateShapeVisual();
            shapeVisualCircle.Size = new Vector2(50, 50);
            shapeVisualCircle.Shapes.Add(compositionSpriteShape);
            shapeVisualCircle.Offset = new Vector3(sliderMargins.X, 0, 0);

            // Return animation
            _leftoffsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            _leftoffsetAnimation.InsertKeyFrame(1.0f, new Vector3(0 + sliderMargins.X, 0, 0));
            _leftoffsetAnimation.Duration = TimeSpan.FromSeconds(0.2f);
            _rightoffsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            _rightoffsetAnimation.InsertKeyFrame(1.0f, new Vector3(100 - sliderMargins.Y, 0, 0));
            _rightoffsetAnimation.Duration = TimeSpan.FromSeconds(0.2f);

        }

        private void CreateTopCircleButtonShadow()
        {
            CompositionRoundedRectangleGeometry circleShaowGeometry = compositor.CreateRoundedRectangleGeometry();
            circleShaowGeometry.Size = new Vector2(50, 50);
            circleShaowGeometry.CornerRadius = new Vector2(25, 25);
            CompositionSpriteShape compositionSpriteShapeShadow = compositor.CreateSpriteShape(circleShaowGeometry);
            CompositionRadialGradientBrush compositionLinearGradientShadowBrush = compositor.CreateRadialGradientBrush();
            compositionLinearGradientShadowBrush.ColorStops.Insert(0, compositor.CreateColorGradientStop(0.5f, Color.FromArgb(255, 86, 57, 14)));
            compositionLinearGradientShadowBrush.ColorStops.Insert(1, compositor.CreateColorGradientStop(1f, Color.FromArgb(255, 230, 160, 53)));
            compositionLinearGradientShadowBrush.Offset = new Vector2(0, 4);
            compositionSpriteShapeShadow.FillBrush = compositionLinearGradientShadowBrush;
            shapeVisualShadow = compositor.CreateShapeVisual();
            shapeVisualShadow.Size = new Vector2(50, 50);
            shapeVisualShadow.Shapes.Add(compositionSpriteShapeShadow);
        }

        private void CreateMainElipse()
        {
            CompositionRoundedRectangleGeometry roundedRectangle = compositor.CreateRoundedRectangleGeometry();
            roundedRectangle.Size = new Vector2(150, 50);
            roundedRectangle.CornerRadius = new Vector2(25, 25);
            CompositionSpriteShape compositionSpriteShape = compositor.CreateSpriteShape(roundedRectangle);

            linearGradientBrush = compositor.CreateLinearGradientBrush();
            linearGradientBrush.StartPoint = new Vector2(0, 1);
            linearGradientBrush.EndPoint = new Vector2(0f, 1);
            linearGradientBrush.ColorStops.Insert(0, compositor.CreateColorGradientStop(0f, backgroundColor));
            linearGradientBrush.ColorStops.Insert(1, compositor.CreateColorGradientStop(0.5f, waveColor));
            linearGradientBrush.ColorStops.Insert(2, compositor.CreateColorGradientStop(1f, backgroundColor));

            compositionSpriteShape.FillBrush = linearGradientBrush;
            shapeVisualElipse = compositor.CreateShapeVisual();
            shapeVisualElipse.Size = new Vector2(150, 50);
            shapeVisualElipse.Shapes.Add(compositionSpriteShape);
        }



        private void PageLoaded(object sender, RoutedEventArgs e)
        {

            // Text preparation
            switchText = "SWITCH";
            text = new TextBlock()
            {
                Text = switchText,
                Padding = new Thickness(0, 15, 25, 0),
                FontWeight = FontWeights.Bold,
                Height = 50,
                HorizontalTextAlignment = TextAlignment.Right
            };
            canvas.Content = text;
            Visual textVisual = ElementCompositionPreview.GetElementVisual(text);

            // Set new paddings
            theMileOfSwitch = (100.0f / 2f) - sliderMargins.Y + sliderMargins.X;

            // Take canvas compositor
            compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            canvasVisual = compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(canvas, canvasVisual);

            // ELIPSE
            CreateMainElipse();
            canvasVisual.Children.InsertAtTop(shapeVisualElipse);

            // Shadow
            CreateTopCircleButtonShadow();
            canvasVisual.Children.InsertAtTop(shapeVisualShadow);

            // TEXT
            canvasVisual.Children.InsertAtTop(textVisual);

            // CIRCLE
            CreateTopCircleButton();
            canvasVisual.Children.InsertAtTop(shapeVisualCircle);
        }

        private void Page_Unload(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }

        float i = 0; // DBG
        private void Canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            // Speed for Elipse gradient
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

        private bool isPressed = false;
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
                shapeVisualShadow.Offset = pointerPosition;
                shapeVisualCircle.Offset = pointerPosition;
            }
        }

        private void canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ColorStop1.StartAnimation(nameof(ColorStop1.Color), color2Animation);
            isPressed = false;
            if (pointerPosition.X < theMileOfSwitch)
            {
                shapeVisualCircle.StartAnimation(nameof(Visual.Offset), _leftoffsetAnimation);
                shapeVisualShadow.StartAnimation(nameof(Visual.Offset), _leftoffsetAnimation);
            }
            else
            {
                shapeVisualCircle.StartAnimation(nameof(Visual.Offset), _rightoffsetAnimation);
                shapeVisualShadow.StartAnimation(nameof(Visual.Offset), _rightoffsetAnimation);
            }
        }

        private void canvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (isPressed == true)
                canvas_PointerReleased(sender, e);
        }
    }
}
