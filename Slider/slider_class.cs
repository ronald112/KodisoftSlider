using System;
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

namespace slider_class
{
    class MyCustomSlider
    {
        
        public Vector2 m_sliderMargins { get; set; }
        public Color m_waveColor { get; set; }
        public Color m_backgroundColor { get; set; }
        public String m_switchText { get; set; }
        public Vector4 m_margins { get; set; }

        private HorizontalAlignment m_ha;
        private VerticalAlignment m_va;
        private Compositor m_page_compositor;
        private ContainerVisual visuals;
        private UIElement m_page;

        public MyCustomSlider(HorizontalAlignment ha, VerticalAlignment va, UIElement page)
        {
            m_ha = ha;
            m_va = va;
            m_page = page;
            m_page_compositor = ElementCompositionPreview.GetElementVisual(m_page).Compositor; ;
            m_margins = new Vector4(0, 0, 0, 0);
            m_switchText = "Switch";
            m_backgroundColor = Color.FromArgb(255, 240, 170, 55);
            m_waveColor = Color.FromArgb(255, 244, 191, 106);
            m_sliderMargins = new Vector2(0, 0);
            createCanvas();
        }

        private void createCanvas()
        {
            /*
            m_canvas = new Canvas();
            m_canvas.HorizontalAlignment = m_ha;
            m_canvas.VerticalAlignment = m_va;
            m_canvas.Margin = new Thickness(0, 0, 0, 0);
            m_canvas.Width = 150;
            m_canvas.Height = 50;
            */
            visuals = m_page_compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(m_page, visuals);

            CompositionRoundedRectangleGeometry roundedRectangle = m_page_compositor.CreateRoundedRectangleGeometry();
            roundedRectangle.Size = new Vector2(150, 50);
            roundedRectangle.CornerRadius = new Vector2(25, 25);
            CompositionSpriteShape compositionSpriteShape = m_page_compositor.CreateSpriteShape(roundedRectangle);
            CompositionLinearGradientBrush linearGradientBrush = m_page_compositor.CreateLinearGradientBrush();
            linearGradientBrush.StartPoint = new Vector2(0, 1);
            linearGradientBrush.EndPoint = new Vector2(0f, 1);
            linearGradientBrush.ColorStops.Insert(0, m_page_compositor.CreateColorGradientStop(0f, m_backgroundColor));
            linearGradientBrush.ColorStops.Insert(1, m_page_compositor.CreateColorGradientStop(0.5f, m_waveColor));
            linearGradientBrush.ColorStops.Insert(2, m_page_compositor.CreateColorGradientStop(1f, m_backgroundColor));
            compositionSpriteShape.FillBrush = linearGradientBrush;
            ShapeVisual shapeVisualElipse = m_page_compositor.CreateShapeVisual();
            shapeVisualElipse.Size = new Vector2(150, 50);
            shapeVisualElipse.Shapes.Add(compositionSpriteShape);

            //m_canvas.Background = new SolidColorBrush(Colors.Aqua);

            m_page_compositor.CreateRoundedRectangleGeometry();

            
            
            visuals.Children.InsertAtTop(shapeVisualElipse);

        }

        ~MyCustomSlider()
        {
            
        }
    }
}
