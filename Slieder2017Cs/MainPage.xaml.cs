﻿using System;
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


// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace Slieder2017Cs
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>

    public sealed partial class MainPage : Page
    {
        CompositionLinearGradientBrush linearGradientBrush;
        CompositionLinearGradientBrush circleGradientBrush;
        Compositor compositor;
        ContainerVisual canvasVisual;
        Vector2 sliderMargins = new Vector2(5, 5);
        CompositionPropertySet _pointerPositionProrSet;
        Vector3 pointerPosition;

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
            brush.ColorStops.Insert(0, compositor.CreateColorGradientStop(0f, Color.FromArgb(255, 240, 170, 55)));
            brush.ColorStops.Insert(1, compositor.CreateColorGradientStop(0.5f, Color.FromArgb(255, 244, 191, 106)));
            brush.ColorStops.Insert(2, compositor.CreateColorGradientStop(1f, Color.FromArgb(255, 240, 170, 55)));
            return brush;
        }

        private CompositionColorBrush createColorBrush(Compositor compositor, Color color)
        {
            CompositionColorBrush brush = compositor.CreateColorBrush();
            brush.Color = color;
            return brush;
        }

        private ShapeVisual CreateTopCircleButton(Compositor compositor)
        {
            CompositionSpriteShape compositionSpriteShape;
            CompositionRoundedRectangleGeometry circleGeometry = compositor.CreateRoundedRectangleGeometry();
            circleGeometry.Size = new Vector2(40, 40);
            circleGeometry.CornerRadius = new Vector2(20, 20);
            compositionSpriteShape = compositor.CreateSpriteShape(circleGeometry);
            circleGradientBrush = compositor.CreateLinearGradientBrush();
            circleGradientBrush.StartPoint = new Vector2(0, 1);
            circleGradientBrush.EndPoint = new Vector2(0f, 1);
            circleGradientBrush.ColorStops.Insert(0, compositor.CreateColorGradientStop(0f, Colors.White));
            circleGradientBrush.ColorStops.Insert(1, compositor.CreateColorGradientStop(0.5f, Color.FromArgb(255, 247, 211, 156)));
            circleGradientBrush.ColorStops.Insert(2, compositor.CreateColorGradientStop(1f, Colors.White));
            compositionSpriteShape.FillBrush = createColorBrush(compositor, Colors.Aqua);//
            ShapeVisual shapeVisual = compositor.CreateShapeVisual();
            shapeVisual.Size = new Vector2(40, 40);
            shapeVisual.Shapes.Add(compositionSpriteShape);
            shapeVisual.Offset = new Vector3(sliderMargins, 0);            
            return shapeVisual;
        }

        private Vector3KeyFrameAnimation _leftOffsetAnimation;
        private Vector3KeyFrameAnimation _rightOffsetAnimation;
        ShapeVisual shapeVisualCircle;

        private void PageLoaded(object sender, RoutedEventArgs e)
        {

            compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            ContainerVisual root = compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(canvas, root);

            _pointerPositionProrSet = compositor.CreatePropertySet();
            pointerPosition = new Vector3(sliderMargins, 0);
            _pointerPositionProrSet.InsertVector3("Vec", pointerPosition);

           
             
            canvasVisual = root;
            CompositionSpriteShape compositionSpriteShape;
            CompositionRoundedRectangleGeometry roundedRectangle = compositor.CreateRoundedRectangleGeometry();
            roundedRectangle.Size = new Vector2(150, 50);
            roundedRectangle.CornerRadius = new Vector2(25, 25);
            compositionSpriteShape = compositor.CreateSpriteShape(roundedRectangle);
            linearGradientBrush = createLinearGradientBrush(compositor);
            compositionSpriteShape.FillBrush = linearGradientBrush;
            ShapeVisual shapeVisual = compositor.CreateShapeVisual();
            shapeVisual.Size = new Vector2(150, 50);
            shapeVisual.Shapes.Add(compositionSpriteShape);
            canvasVisual.Children.InsertAtTop(shapeVisual);
            // ----------------------------------------------

            shapeVisualCircle = CreateTopCircleButton(compositor);

            var exp = compositor.CreateExpressionAnimation();
            exp.Expression = "_pointerPositionProrSet.Vec";
            exp.SetReferenceParameter("_pointerPositionProrSet", _pointerPositionProrSet);
            shapeVisualCircle.StartAnimation(nameof(shapeVisualCircle.Offset), exp);

            _leftOffsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            _leftOffsetAnimation.InsertKeyFrame(1, new Vector3(5, 5, 0));
            _leftOffsetAnimation.Duration = TimeSpan.FromSeconds(0.5f);
            _rightOffsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            _rightOffsetAnimation.InsertKeyFrame(1, new Vector3(5, 5, 0));
            _rightOffsetAnimation.Duration = TimeSpan.FromSeconds(0.5f);

            canvasVisual.Children.InsertAtTop(shapeVisualCircle);


            /*
            var offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.Target = nameof(Visual.Offset);
            offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(1500);
            var implicitAnimations = _compositor.CreateImplicitAnimationCollection();
            implicitAnimations[nameof(Visual.Offset)] = offsetAnimation;
            _visual.ImplicitAnimations = implicitAnimations;
            */
        }

        private void Page_Unload(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
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
            //textBox.Text = i.ToString();
        }

        bool isPressed = false;
        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isPressed = true;
            pointerPosition = new Vector3(e.GetCurrentPoint(canvas).Position.ToVector2().X - 20, 5, 0);
            if (pointerPosition.X <= 5) 
                pointerPosition.X = 5;
            if (pointerPosition.X >= 105)
                pointerPosition.X = 105;
            _pointerPositionProrSet.InsertVector3("Vec", pointerPosition);
            textBox.Text = pointerPosition.X.ToString();
        }

        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isPressed == true)
            {
                pointerPosition = new Vector3(e.GetCurrentPoint(canvas).Position.ToVector2().X - 20, 5, 0);
                if (pointerPosition.X <= 5)
                    pointerPosition.X = 5;
                if (pointerPosition.X >= 105)
                    pointerPosition.X = 105;
                _pointerPositionProrSet.InsertVector3("Vec", pointerPosition);
                textBox.Text = pointerPosition.X.ToString();
            }
        }

        private void canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isPressed = false;
            if (pointerPosition.X < 52.5f)
            {
                shapeVisualCircle.StartAnimation("Offset", _leftOffsetAnimation);
                //shapeVisualCircle.StopAnimation("Offset");
                pointerPosition.X = 5;
            }
            else
            {

                shapeVisualCircle.StartAnimation("Offset", _rightOffsetAnimation);
                //shapeVisualCircle.StopAnimation("Offset");
                pointerPosition.X = 105;
            }
            _pointerPositionProrSet.InsertVector3("Vec", pointerPosition);
        }

        private void canvas_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
        }

        private void canvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
           
        }
    }
}
