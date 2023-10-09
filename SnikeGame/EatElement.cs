using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SnikeGame
{
    public class EatElement
    {
        public UIElement UiElement { get; set; }
        public int X { get; set; } = 200;
        public int Y { get; set; } = 200;
        public Brush Fill { get; set; }

        public EatElement()
        {
            Fill = ChangeEatElementColor();

            UiElement  = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Fill,
                StrokeThickness = 0.5,
                Stroke = Brushes.Black
            };
        }

        /// <summary>
        /// Отпределение цвета еды
        /// </summary>
        /// <returns>Цвет еды</returns>
        private static Brush ChangeEatElementColor()
        {
            int percent = new Random().Next(0, 100);

            if (percent < 10 && percent >= 0)
                return Brushes.Green;
            else if (percent < 20 && percent > 10)
                return Brushes.Blue;
            else
                return Brushes.Red;
        }
    }
}
