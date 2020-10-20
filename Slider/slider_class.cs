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

        private Canvas m_canvas;
        private HorizontalAlignment m_ha;
        private VerticalAlignment m_va;

        public MyCustomSlider(HorizontalAlignment ha, VerticalAlignment va)
        {
            m_ha = ha;
            m_va = va;
            m_margins = new Vector4(0, 0, 0, 0);
            m_switchText = "Switch";
            m_backgroundColor = Color.FromArgb(255, 240, 170, 55);
            m_waveColor = Color.FromArgb(255, 244, 191, 106);
            m_sliderMargins = new Vector2(0, 0);
            createCanvas();
        }

        private void createCanvas()
        {
            m_canvas = new Canvas();
            m_canvas.HorizontalAlignment = m_ha;
            m_canvas.VerticalAlignment = m_va;
            m_canvas.Margin = new Thickness(0, 0, 0, 0);
            m_canvas.Width = 150;
            m_canvas.Height = 50;

            m_canvas.Background = new SolidColorBrush(Colors.Aqua);
            
        }

        ~MyCustomSlider()
        {
            m_canvas = null;
        }
    }
}
