﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI;
using System.Numerics;
using Windows.UI.Text;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.IO;
using Windows.Storage.Streams;
using Microsoft.Graphics.Canvas.UI.Composition;
using Windows.Graphics.DirectX;
using Windows.Foundation;

namespace slider_class
{
    class MyCustomSlider
    {
        
        public Vector2 m_sliderMargins { get; set; }
        public Color m_waveColor { get; set; }
        public Color m_backgroundColor { get; set; }
        public String m_switchText { get; set; }
        public Vector4 m_margins { get; set; }

        private Compositor m_page_compositor;
        private ContainerVisual visuals;
        private CanvasAnimatedControl m_page;
        private CompositionLinearGradientBrush linearGradientBrush;
        private float theMileOfSwitch;
        private Vector2KeyFrameAnimation elipseColorStartPointAnimation;
        private Vector2KeyFrameAnimation elipseColorEndPointAnimation;
        private Visual textVisual;
        //-------------------------------------------


        private Vector3KeyFrameAnimation _leftoffsetAnimation;
        private Vector3KeyFrameAnimation _rightoffsetAnimation;

        private CompositionRadialGradientBrush circleGradientBrush;

        private CompositionColorGradientStop ColorStop1;
        private ColorKeyFrameAnimation color1Animation;
        private ColorKeyFrameAnimation color2Animation;

        private Vector3 pointerPosition;
        private ShapeVisual shapeVisualElipse;
        private ShapeVisual shapeVisualCircle;
        private ShapeVisual shapeVisualShadow;
        private bool isPressed = false;

        public MyCustomSlider(CanvasAnimatedControl page)
        {
            m_page = page;
            m_page_compositor = ElementCompositionPreview.GetElementVisual(m_page).Compositor; ;
            m_margins = new Vector4(0, 0, 0, 0);
            m_switchText = "Switch";
            m_backgroundColor = Color.FromArgb(255, 240, 170, 55);
            m_waveColor = Color.FromArgb(255, 244, 191, 106);
            m_sliderMargins = new Vector2(0, 0);
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

        private void CreateTopCircleButtonShadow()
        {
            CompositionRoundedRectangleGeometry circleShaowGeometry = m_page_compositor.CreateRoundedRectangleGeometry();
            circleShaowGeometry.Size = new Vector2(50, 50);
            circleShaowGeometry.CornerRadius = new Vector2(25, 25);
            CompositionSpriteShape compositionSpriteShapeShadow = m_page_compositor.CreateSpriteShape(circleShaowGeometry);
            CompositionRadialGradientBrush compositionLinearGradientShadowBrush = m_page_compositor.CreateRadialGradientBrush();
            compositionLinearGradientShadowBrush.ColorStops.Insert(0, m_page_compositor.CreateColorGradientStop(0.5f, Color.FromArgb(255, 86, 57, 14)));
            compositionLinearGradientShadowBrush.ColorStops.Insert(1, m_page_compositor.CreateColorGradientStop(1f, Color.FromArgb(255, 230, 160, 53)));
            compositionLinearGradientShadowBrush.Offset = new Vector2(0, 4);
            compositionSpriteShapeShadow.FillBrush = compositionLinearGradientShadowBrush;
            shapeVisualShadow = m_page_compositor.CreateShapeVisual();
            shapeVisualShadow.Size = new Vector2(50, 50);
            shapeVisualShadow.Shapes.Add(compositionSpriteShapeShadow);
        }

        private void setupMoveAnimationForBottonShadow()
        {
            var exp = m_page_compositor.CreateExpressionAnimation();
            exp.Expression = "Visual.Offset";
            exp.SetReferenceParameter("Visual", shapeVisualCircle);

            shapeVisualShadow.StartAnimation(nameof(shapeVisualShadow.Offset), exp);
        }

        private void CreateTopCircleButton()
        {
            CompositionRoundedRectangleGeometry circleGeometry = m_page_compositor.CreateRoundedRectangleGeometry();
            
            circleGeometry.Size = new Vector2(40, 40);
            circleGeometry.CornerRadius = new Vector2(20, 20);
            
            CompositionSpriteShape compositionSpriteShape = m_page_compositor.CreateSpriteShape(circleGeometry);
            circleGradientBrush = m_page_compositor.CreateRadialGradientBrush();

            ColorStop1 = m_page_compositor.CreateColorGradientStop(1, Colors.White);
            circleGradientBrush.ColorStops.Add(ColorStop1);

            compositionSpriteShape.FillBrush = circleGradientBrush;
            compositionSpriteShape.Offset = new Vector2(5, 5);

            shapeVisualCircle = m_page_compositor.CreateShapeVisual();
            shapeVisualCircle.Size = new Vector2(150, 50);
            shapeVisualCircle.Shapes.Add(compositionSpriteShape);
            shapeVisualCircle.Offset = new Vector3(m_sliderMargins.X, 0, 0);
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

        private void setupColorAnimationForCircleButton()
        {
            // Color animation
            color1Animation = m_page_compositor.CreateColorKeyFrameAnimation();
            color1Animation.Duration = TimeSpan.FromSeconds(0.5);
            color1Animation.InsertKeyFrame(1.0f, Color.FromArgb(255, 247, 211, 156));
            color1Animation.InsertKeyFrame(0.0f, Colors.White);
            color2Animation = m_page_compositor.CreateColorKeyFrameAnimation();
            color2Animation.Duration = TimeSpan.FromSeconds(0.5);
            color2Animation.InsertKeyFrame(1.0f, Colors.White);
            color2Animation.InsertKeyFrame(0.0f, Color.FromArgb(255, 247, 211, 156));
        }

        // Return animation
        private void setupReturnAnimationForCircleButton()
        {           
            _leftoffsetAnimation = m_page_compositor.CreateVector3KeyFrameAnimation();
            _leftoffsetAnimation.InsertKeyFrame(1.0f, new Vector3(0 + m_sliderMargins.X, 0, 0));
            _leftoffsetAnimation.Duration = TimeSpan.FromSeconds(0.2f);
            _rightoffsetAnimation = m_page_compositor.CreateVector3KeyFrameAnimation();
            _rightoffsetAnimation.InsertKeyFrame(1.0f, new Vector3(100 - m_sliderMargins.Y, 0, 0));
            _rightoffsetAnimation.Duration = TimeSpan.FromSeconds(0.2f);
        }


        private ShapeVisual stretchVisual;

        Vector3KeyFrameAnimation offsetAnimationstretchVisual;
        private void setupOffsetAnimationForCircleButton()
        {
            CompositionRoundedRectangleGeometry stretchGeometry = m_page_compositor.CreateRoundedRectangleGeometry();

            stretchGeometry.Size = new Vector2(40, 40);
            stretchGeometry.CornerRadius = new Vector2(20, 20);

            CompositionSpriteShape compositionSpriteShape = m_page_compositor.CreateSpriteShape(stretchGeometry);
            CompositionRadialGradientBrush GradientBrush = m_page_compositor.CreateRadialGradientBrush();

            GradientBrush.ColorStops.Add(m_page_compositor.CreateColorGradientStop(1, Colors.Aqua));

            compositionSpriteShape.FillBrush = GradientBrush;
            compositionSpriteShape.Offset = new Vector2(5, 5);

            stretchVisual = m_page_compositor.CreateShapeVisual();
            stretchVisual.Size = new Vector2(150, 50);
            stretchVisual.Shapes.Add(compositionSpriteShape);
            stretchVisual.Offset = new Vector3(m_sliderMargins.X, 0, 0);



            offsetAnimationstretchVisual = m_page_compositor.CreateVector3KeyFrameAnimation();
            offsetAnimationstretchVisual.Target = nameof(Visual.Offset);

            offsetAnimationstretchVisual.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            offsetAnimationstretchVisual.StopBehavior = AnimationStopBehavior.LeaveCurrentValue;
            //offsetAnimation.DelayTime = TimeSpan.FromSeconds(0.1);
            offsetAnimationstretchVisual.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
            //offsetAnimationstretchVisual.Duration = TimeSpan.FromMilliseconds(800);

            var implicitAnimation = m_page_compositor.CreateImplicitAnimationCollection();
            implicitAnimation[nameof(shapeVisualCircle.Offset)] = offsetAnimationstretchVisual;

            stretchVisual.ImplicitAnimations = implicitAnimation;
        }

        private void createWidthObject()
        {
            
        }

        private void createCanvas()
        {

            // Set new paddings
            theMileOfSwitch = (100.0f / 2f) - m_sliderMargins.Y + m_sliderMargins.X;

            // Create visual container and setup child uielement of visual
            visuals = m_page_compositor.CreateContainerVisual();            
            ElementCompositionPreview.SetElementChildVisual(m_page, visuals);

            // ELIPSE
            CreateMainElipse();
            setupColorAnimationForMainElipse();
            visuals.Children.InsertAtTop(shapeVisualElipse);

            // SHADOW
            CreateTopCircleButtonShadow();
            visuals.Children.InsertAtTop(shapeVisualShadow);

            // TEXT
            createText();
            visuals.Children.InsertAtTop(textVisual);

            // CIRCLE
            CreateTopCircleButton();
            setupColorAnimationForCircleButton();
            setupReturnAnimationForCircleButton();
            visuals.Children.InsertAtTop(shapeVisualCircle);

            // Animation for shadow
            setupMoveAnimationForBottonShadow();

            //
            setupOffsetAnimationForCircleButton();
            visuals.Children.InsertAtTop(stretchVisual);

            // Handle pointer events
            m_page.PointerMoved += M_page_PointerMoved;
            m_page.PointerPressed += M_page_PointerPressed;
            m_page.PointerReleased += M_page_PointerReleased;
            m_page.PointerExited += M_page_PointerExited;
            m_page.Update += M_page_Update;

            // Gradient animation
            linearGradientBrush.StartAnimation(nameof(linearGradientBrush.StartPoint), elipseColorStartPointAnimation);
            linearGradientBrush.StartAnimation(nameof(linearGradientBrush.EndPoint), elipseColorEndPointAnimation);

        }

        private void M_page_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            stretchVisual.Offset = shapeVisualCircle.Offset;
            
        }

        private void M_page_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (isPressed == true)
                M_page_PointerReleased(sender, e);
        }

        private void M_page_PointerReleased(object sender, PointerRoutedEventArgs e)
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

        private void M_page_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isPressed = true;
            ColorStop1.StartAnimation(nameof(ColorStop1.Color), color1Animation);
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
                float time = Math.Abs(shapeVisualCircle.CenterPoint.X - pointerPosition.X);
                offsetAnimationstretchVisual.Duration = TimeSpan.FromMilliseconds(time > 0 ? time : 1);
                shapeVisualCircle.Offset = pointerPosition;
            }
        }

        ~MyCustomSlider()
        {
            
        }
    }
}
