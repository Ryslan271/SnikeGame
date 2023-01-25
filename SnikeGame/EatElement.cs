using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SnikeGame
{
    public class EatElement
    {
        public UIElement UiElement { get; set; } = new Ellipse
        {
            Width = 20,
            Height = 20,
            Fill = Brushes.Red,
            StrokeThickness = 0.5,
            Stroke = Brushes.Black
        };

        public int X { get; set; } = 200;
        public int Y { get; set; } = 200;

    }
}
