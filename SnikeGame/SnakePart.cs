using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SnikeGame
{
    public class SnakePart
    {
        public UIElement UiElement { get; set; } = new Rectangle
        {
            Width = 20,
            Height = 20,
            Fill = Brushes.Green,
            StrokeThickness = 0.5,
            Stroke = Brushes.Black
        };

        public int X { get; set; } = 200;
        public int Y { get; set; } = 200;

        public int SnakeX { get; set; }
        public int SnakeY { get; set; }

        public int DirectionX { get; set; } = 0;
        public int DirectionY { get; set; } = 0;

        public bool IsHead { get; set; } = false;
        public bool IsAss { get; set; } = false;
    }
}
