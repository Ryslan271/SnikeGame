using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnikeGame
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int SnakeSquareSize = 20;

        private List<EatElement> EatElements = new List<EatElement>();
        private List<SnakePart> Snakes = new List<SnakePart>();

        private TrafficSide DirectionMovement = TrafficSide.Right;

        private List<SnakePart> SnakesTemporary = new List<SnakePart>();
        private List<EatElement> EatElementTemporary = new List<EatElement>();

        private Random rnd = new Random();

        private SnakePart Snake = new SnakePart() { IsHead = true };

        public MainWindow()
        {
            Snakes.Add(Snake);

            InitializeComponent();

            TimerStart();
        }

        #region Таймер движения
        private void TimerStart()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Snakes.Count; i++)
            {
                if (i != 0)
                {
                    Snakes[i].SnakeX = Snakes[i].X;
                    Snakes[i].SnakeY = Snakes[i].Y;

                    Snakes[i].X = Snakes[i - 1].SnakeX;
                    Snakes[i].Y = Snakes[i - 1].SnakeY;

                    continue;
                }

                Snakes[i].SnakeX = Snakes[i].X;
                Snakes[i].SnakeY = Snakes[i].Y;

                switch (DirectionMovement)
                {
                    case TrafficSide.Right:
                        Snakes[i].X += SnakeSquareSize;
                        break;

                    case TrafficSide.Left:
                        Snakes[i].X -= SnakeSquareSize;
                        break;

                    case TrafficSide.Top:
                        Snakes[i].Y -= SnakeSquareSize;
                        break;

                    case TrafficSide.Bottom:
                        Snakes[i].Y += SnakeSquareSize;
                        break;
                }
            }

            RenderingHelperMethodForDrawing();
        }
        #endregion

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            DrawGameArea();

            DrawEssences();

            GameArea.Children.Add(Snake.UiElement);
            Canvas.SetTop(Snake.UiElement, Snake.Y);
            Canvas.SetLeft(Snake.UiElement, Snake.X);
        }

        private void DrawGameArea()
        {
            bool doneDrawingBackground = false;
            int nextX = 0, nextY = 0;
            int rowCounter = 0;
            bool nextIsOdd = false;

            while (doneDrawingBackground == false)
            {
                Rectangle rect = new Rectangle
                {
                    Width = SnakeSquareSize,
                    Height = SnakeSquareSize,
                    Fill = Brushes.White,
                    StrokeThickness = 0.5,
                    Stroke = Brushes.Black
                };

                GameArea.Children.Add(rect);
                Canvas.SetTop(rect, nextY);
                Canvas.SetLeft(rect, nextX);

                nextIsOdd = !nextIsOdd;
                nextX += SnakeSquareSize;
                if (nextX >= GameArea.ActualWidth)
                {
                    nextX = 0;
                    nextY += SnakeSquareSize;
                    rowCounter++;
                    nextIsOdd = (rowCounter % 2 != 0);
                }

                if (nextY >= GameArea.ActualHeight)
                    doneDrawingBackground = true;
            }
        }

        private void DrawEssences()
        {
            if (QuantityCheckEatInDameArena())
                return;

            int essencesX = rnd.Next(1, 20);
            int essencesY = rnd.Next(1, 20);

            while (true)
            {
                if (ValidateSnakeAndEatPosition(essencesX, essencesY))
                    continue;

                EatElement essence = new EatElement()
                {
                    Y = essencesX * 20,
                    X = essencesY * 20
                };

                EatElements.Add(essence);

                GameArea.Children.Add(essence.UiElement);
                Canvas.SetTop(essence.UiElement, essence.Y);
                Canvas.SetLeft(essence.UiElement, essence.X);

                break;
            }
        }

        private bool ValidateSnakeAndEatPosition(int eatX, int eatY) =>
            Snakes.FirstOrDefault(snakeElement => snakeElement.SnakeX == eatX * 20 && snakeElement.SnakeY == eatY * 20) != null;

        private bool QuantityCheckEatInDameArena() =>
            EatElements.Count() >= 1;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.D:
                    DirectionMovement = TrafficSide.Right;
                    break;

                case Key.A:
                    DirectionMovement = TrafficSide.Left;
                    break;

                case Key.W:
                    DirectionMovement = TrafficSide.Top;
                    break;

                case Key.S:
                    DirectionMovement = TrafficSide.Bottom;
                    break;
            }
        }

        private void AddGameArenaSnakes()
        {
            foreach (var snakeItem in Snakes)
            {
                GameArea.Children.Remove(snakeItem.UiElement);
                GameArea.Children.Add(snakeItem.UiElement);
                Canvas.SetTop(snakeItem.UiElement, snakeItem.Y);
                Canvas.SetLeft(snakeItem.UiElement, snakeItem.X);
            }
        }

        private void RenderingHelperMethodForDrawing()
        {
            SnakesTemporary = new List<SnakePart>();
            EatElementTemporary = new List<EatElement>();

            ValidateEatElementSnakes();

            Snakes.AddRange(SnakesTemporary);

            foreach (var item in EatElementTemporary)
                EatElements.Remove(item);

            AddGameArenaSnakes();
            DrawEssences();
        }

        private void ValidateEatElementSnakes()
        {
            foreach (EatElement item in EatElements)
            {
                foreach (SnakePart snakeItem in Snakes)
                {
                    if (item.Y == snakeItem.Y &&
                        item.X == snakeItem.X)
                    {
                        SnakesTemporary.Add(new SnakePart
                        {
                            X = item.X,
                            Y = item.Y
                        });

                        EatElementTemporary.Add(item);
                        GameArea.Children.Remove(item.UiElement);
                    }
                }
            }
        }
    }
}
