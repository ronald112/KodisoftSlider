using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;

namespace CustomSlider
{
    class Slider
    {
        public Vector2 m_sliderMargins { get; set; }
        public Color m_waveColor { get; set; }
        public Color m_backgroundColor { get; set; }
        public string m_switchText { get; set; }
        public Vector4 m_margins { get; set; }
        public bool m_status { get; set; }


        private Compositor m_page_compositor;
        private ContainerVisual visuals;
        private CanvasAnimatedControl m_page;
        private CompositionLinearGradientBrush linearGradientBrush;
        private float theMileOfSwitch;
        private Vector2KeyFrameAnimation elipseColorStartPointAnimation;
        private Vector2KeyFrameAnimation elipseColorEndPointAnimation;
        private Visual textVisual;

        private Vector3 pointerPosition;
        private ShapeVisual shapeVisualElipse;
        private ShapeVisual shapeVisualCircle;
        private ShapeVisual shapeVisualCircleShadow;
        private bool isPressed = false;

        private CompositionColorGradientStop CircleBottonColorStop;
        private ColorKeyFrameAnimation CircleBottonColorAnimSecond;
        private ColorKeyFrameAnimation CircleBottonColorAnimFirst;

        private Vector3KeyFrameAnimation _leftoffsetAnimation;
        private Vector3KeyFrameAnimation _rightoffsetAnimation;
        private CompositionRadialGradientBrush circleGradientBrush;

        public Slider(CanvasAnimatedControl page)
        {
            m_page = page;
            m_page_compositor = ElementCompositionPreview.GetElementVisual(m_page).Compositor; ;
            m_margins = new Vector4(0, 0, 0, 0);
            m_switchText = "Switch";
            m_backgroundColor = Color.FromArgb(255, 240, 170, 55);
            m_waveColor = Color.FromArgb(255, 244, 191, 106);
            m_sliderMargins = new Vector2(0, 0);
            m_status = false;
            createCanvas();
        }

        private void CreateMainElipse()
        {
            CompositionRoundedRectangleGeometry roundedRectangle = m_page_compositor.CreateRoundedRectangleGeometry();
            roundedRectangle.Size = new Vector2(150, 50);
            roundedRectangle.CornerRadius = new Vector2(25, 25);
            CompositionSpriteShape compositionSpriteShape = m_page_compositor.CreateSpriteShape(roundedRectangle);
            linearGradientBrush = m_page_compositor.CreateLinearGradientBrush();
            linearGradientBrush.StartPoint = new Vector2(-0.2f, 1);
            linearGradientBrush.EndPoint = new Vector2(0f, 1);
            linearGradientBrush.ColorStops.Insert(0, m_page_compositor.CreateColorGradientStop(0f, m_backgroundColor));
            linearGradientBrush.ColorStops.Insert(1, m_page_compositor.CreateColorGradientStop(0.5f, m_waveColor));
            linearGradientBrush.ColorStops.Insert(2, m_page_compositor.CreateColorGradientStop(1f, m_backgroundColor));

            compositionSpriteShape.FillBrush = linearGradientBrush;
            shapeVisualElipse = m_page_compositor.CreateShapeVisual();
            shapeVisualElipse.Size = new Vector2(150, 50);
            shapeVisualElipse.Shapes.Add(compositionSpriteShape);
        }

        private void setupColorAnimationForMainElipse()
        {
            // Gradient animation
            elipseColorStartPointAnimation = m_page_compositor.CreateVector2KeyFrameAnimation();
            elipseColorStartPointAnimation.Duration = TimeSpan.FromSeconds(1.5);
            elipseColorStartPointAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            elipseColorStartPointAnimation.DelayTime = TimeSpan.FromSeconds(5);
            elipseColorStartPointAnimation.InsertKeyFrame(0f, new Vector2(-0.2f, 1));
            elipseColorStartPointAnimation.InsertKeyFrame(1f, new Vector2(1.3f, 1));

            elipseColorEndPointAnimation = m_page_compositor.CreateVector2KeyFrameAnimation();
            elipseColorEndPointAnimation.Duration = TimeSpan.FromSeconds(1.5);
            elipseColorEndPointAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            elipseColorEndPointAnimation.DelayTime = TimeSpan.FromSeconds(5);
            elipseColorEndPointAnimation.InsertKeyFrame(0f, new Vector2(0f, 1));
            elipseColorEndPointAnimation.InsertKeyFrame(1f, new Vector2(1.5f, 1));
        }

        private void CreateTopCircleButton()
        {
            CompositionRoundedRectangleGeometry circleGeometry = m_page_compositor.CreateRoundedRectangleGeometry();

            circleGeometry.Size = new Vector2(40, 40);
            circleGeometry.CornerRadius = new Vector2(20, 20);

            CompositionRectangleGeometry rectangleGeometry = m_page_compositor.CreateRectangleGeometry();

            rectangleGeometry.Size = new Vector2(40, 40);

            CompositionSpriteShape compositionSpriteShape = m_page_compositor.CreateSpriteShape(circleGeometry);
            circleGradientBrush = m_page_compositor.CreateRadialGradientBrush();

            CircleBottonColorStop = m_page_compositor.CreateColorGradientStop(1, Colors.White);
            circleGradientBrush.ColorStops.Add(CircleBottonColorStop);

            compositionSpriteShape.FillBrush = circleGradientBrush;
            compositionSpriteShape.Offset = new Vector2(5, 5);

            shapeVisualCircle = m_page_compositor.CreateShapeVisual();
            shapeVisualCircle.Size = new Vector2(50, 50);
            shapeVisualCircle.Shapes.Add(compositionSpriteShape);
            shapeVisualCircle.Offset = new Vector3(m_sliderMargins.X, 0, 0);
        }

        private void setupAnimation()
        {
            // Color animation main circle
            CircleBottonColorAnimSecond = m_page_compositor.CreateColorKeyFrameAnimation();
            CircleBottonColorAnimSecond.Duration = TimeSpan.FromSeconds(0.5);
            CircleBottonColorAnimSecond.InsertKeyFrame(1.0f, Color.FromArgb(255, 247, 211, 156));
            CircleBottonColorAnimSecond.InsertKeyFrame(0.0f, Colors.White);
            CircleBottonColorAnimFirst = m_page_compositor.CreateColorKeyFrameAnimation();
            CircleBottonColorAnimFirst.Duration = TimeSpan.FromSeconds(0.5);
            CircleBottonColorAnimFirst.InsertKeyFrame(1.0f, Colors.White);
            CircleBottonColorAnimFirst.InsertKeyFrame(0.0f, Color.FromArgb(255, 247, 211, 156));

            // Return animation main circle
            _leftoffsetAnimation = m_page_compositor.CreateVector3KeyFrameAnimation();
            _leftoffsetAnimation.InsertKeyFrame(1.0f, new Vector3(0 + m_sliderMargins.X, 0, 0));
            _leftoffsetAnimation.Duration = TimeSpan.FromSeconds(0.2f);
            _rightoffsetAnimation = m_page_compositor.CreateVector3KeyFrameAnimation();
            _rightoffsetAnimation.InsertKeyFrame(1.0f, new Vector3(100 - m_sliderMargins.Y, 0, 0));
            _rightoffsetAnimation.Duration = TimeSpan.FromSeconds(0.2f);


            var exp = m_page_compositor.CreateExpressionAnimation();
            exp.Expression = "Math.Abs(button.Offset.X - stretch.Offset.X)";
            exp.SetReferenceParameter("button", shapeVisualCircle);
            exp.SetReferenceParameter("stretch", stretchGeometryCircle);

            stretchGeometryCircle.StartAnimation(nameof(stretchGeometryCircle.Offset.X), exp);

            /*
            CubicBezierEasingFunction easing = m_page_compositor.CreateCubicBezierEasingFunction(new Vector2(0.42f, 0.0f), new Vector2(1.0f, 1.0f));

            Vector2KeyFrameAnimation sizeAnimation = m_page_compositor.CreateVector2KeyFrameAnimation();
            //sizeAnimation.InsertExpressionKeyFrame(0.0f, "this.StartingValue");
            sizeAnimation.Target = nameof(stretchGeometryCircle.Size);
            sizeAnimation.InsertExpressionKeyFrame(1.0f, "FinalValue");
            //sizeAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            //sizeAnimation.SetReferenceParameter("prop", stretchGeometryCircle.Properties);
            //stretchGeometryCircle.StartAnimation(nameof(stretchGeometryCircle.Size), sizeAnimation);
            Vector2KeyFrameAnimation offsetAnimation = m_page_compositor.CreateVector2KeyFrameAnimation();
            offsetAnimation.Target = nameof(stretchGeometryCircle.Offset);
            offsetAnimation.InsertExpressionKeyFrame(0.0f, "StartingValue");
            offsetAnimation.InsertExpressionKeyFrame(0.8f, "FinalValue / 2");
            offsetAnimation.InsertExpressionKeyFrame(1.0f, "FinalValue");


            var sizeAnimation2 = m_page_compositor.CreateImplicitAnimationCollection();
            sizeAnimation2[nameof(stretchGeometryCircle.Size)] = sizeAnimation;
            sizeAnimation2[nameof(stretchGeometryCircle.Offset)] = offsetAnimation;
            */

        }

        private void createText()
        {
            // Text preparation
            TextBlock text = new TextBlock()
            {
                Text = m_switchText,
                Padding = new Thickness(0, 15, 25, 0),
                FontWeight = FontWeights.Bold,
                Height = 50,
                HorizontalTextAlignment = TextAlignment.Right
            };
            m_page.Content = text;
            textVisual = ElementCompositionPreview.GetElementVisual(text);
        }


        private ShapeVisual stretchVisualCircle;
        private CompositionRoundedRectangleGeometry stretchGeometryCircle;

        private void setupstretchVisualForCircleButton()
        {
            stretchGeometryCircle = m_page_compositor.CreateRoundedRectangleGeometry();

            stretchGeometryCircle.Size = new Vector2(40, 40);
            stretchGeometryCircle.CornerRadius = new Vector2(20, 20);
            stretchGeometryCircle.Properties.InsertVector2("newOffset", new Vector2(40, 40));

            CompositionSpriteShape compositionSpriteShape = m_page_compositor.CreateSpriteShape(stretchGeometryCircle);
            CompositionRadialGradientBrush GradientBrush = m_page_compositor.CreateRadialGradientBrush();

            GradientBrush.ColorStops.Add(m_page_compositor.CreateColorGradientStop(1, Color.FromArgb(255, 247, 211, 156)));

            compositionSpriteShape.FillBrush = GradientBrush;
            compositionSpriteShape.Offset = new Vector2(5, 5);

            stretchVisualCircle = m_page_compositor.CreateShapeVisual();
            stretchVisualCircle.Size = new Vector2(150, 50);
            stretchVisualCircle.Shapes.Add(compositionSpriteShape);
            stretchVisualCircle.Offset = new Vector3(m_sliderMargins.X, 0, 0);
        }

        private void createCanvas()
        {

            // Set new paddings
            theMileOfSwitch = (100.0f / 2f) - m_sliderMargins.Y + m_sliderMargins.X;

            // Create visual container and setup child uielement of visual
            visuals = m_page_compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(m_page, visuals);

            // INIT
            // ELIPSE
            CreateMainElipse();
            // SHADOW
            //CreateTopCircleButtonShadow();
            // TEXT
            createText();
            // CIRCLE
            CreateTopCircleButton();
            // Animation for main elipse
            setupColorAnimationForMainElipse();
            // Animation for shadow
            //setupMoveAnimationForBottonShadow();
            //Stretch
            setupstretchVisualForCircleButton();

            // PLACE ON THE FORM
            visuals.Children.InsertAtTop(shapeVisualElipse);
            //visuals.Children.InsertAtTop(shapeVisualCircleShadow);
            visuals.Children.InsertAtTop(textVisual);
            visuals.Children.InsertAtTop(stretchVisualCircle);
            visuals.Children.InsertAtTop(shapeVisualCircle);


            setupAnimation();

            // Handle pointer events
            m_page.PointerMoved += M_page_PointerMoved;
            m_page.PointerPressed += M_page_PointerPressed;
            m_page.PointerReleased += M_page_PointerReleased;
            m_page.PointerExited += M_page_PointerExited;

            // Gradient animation
            linearGradientBrush.StartAnimation(nameof(linearGradientBrush.StartPoint), elipseColorStartPointAnimation);
            linearGradientBrush.StartAnimation(nameof(linearGradientBrush.EndPoint), elipseColorEndPointAnimation);

        }

        private void M_page_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (isPressed == true)
                M_page_PointerReleased(sender, e);
        }

        private void M_page_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            CircleBottonColorStop.StartAnimation(nameof(CircleBottonColorStop.Color), CircleBottonColorAnimFirst);
            isPressed = false;
            if (pointerPosition.X < theMileOfSwitch)
            {
                shapeVisualCircle.StartAnimation(nameof(Visual.Offset), _leftoffsetAnimation);
                m_status = false;
            }
            else
            {
                shapeVisualCircle.StartAnimation(nameof(Visual.Offset), _rightoffsetAnimation);
                m_status = true;
            }
        }

        private void M_page_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isPressed = true;
            CircleBottonColorStop.StartAnimation(nameof(CircleBottonColorStop.Color), CircleBottonColorAnimSecond);
            M_page_PointerMoved(sender, e);
        }

        private void M_page_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isPressed == true)
            {
                pointerPosition = new Vector3(e.GetCurrentPoint((UIElement)sender).Position.ToVector2().X - 25, 0, 0);
                if (pointerPosition.X <= 0 + m_sliderMargins.X)
                    pointerPosition.X = 0 + m_sliderMargins.X;
                if (pointerPosition.X >= 100 - m_sliderMargins.Y)
                    pointerPosition.X = 100 - m_sliderMargins.Y;
                shapeVisualCircle.Offset = pointerPosition;
            }
        }

        ~Slider()
        {

        }
    }
}
