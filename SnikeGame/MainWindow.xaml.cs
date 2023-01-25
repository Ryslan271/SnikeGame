using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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

        private List<SnakePart> SnakesTemporary = new List<SnakePart>();
        private List<EatElement> EatElementTemporary = new List<EatElement>();

        private Random rnd = new Random();

        private SnakePart Snake = new SnakePart() { IsHead = true};

        public MainWindow()
        {
            Snakes.Add(Snake);

            InitializeComponent();
        }

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
            int countEssences = rnd.Next(10, 15);

            while (countEssences >= 0)
            {
                EatElement essence = new EatElement()
                {
                    Y = rnd.Next(1, 20) * 20,
                    X = rnd.Next(1, 20) * 20
                };

                EatElements.Add(essence);

                GameArea.Children.Add(essence.UiElement);
                Canvas.SetTop(essence.UiElement, essence.Y);
                Canvas.SetLeft(essence.UiElement, essence.X);
                countEssences--;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < Snakes.Count; i++)
            {
                if (i == 0)
                {
                    Snakes[i].SnakeX = Snakes[i].X;
                    Snakes[i].SnakeY = Snakes[i].Y;

                    switch (e.Key)
                    {
                        case Key.D:
                            Snakes[i].X += SnakeSquareSize;
                            break;

                        case Key.A:
                            Snakes[i].X -= SnakeSquareSize;
                            break;

                        case Key.W:
                            Snakes[i].Y -= SnakeSquareSize;
                            break;

                        case Key.S:
                            Snakes[i].Y += SnakeSquareSize;
                            break;
                    }
                }
                else
                {
                    Snakes[i].SnakeX = Snakes[i].X;
                    Snakes[i].SnakeY = Snakes[i].Y;

                    Snakes[i].X = Snakes[i - 1].SnakeX;
                    Snakes[i].Y = Snakes[i - 1].SnakeY;
                }
            }

            SnakesTemporary = new List<SnakePart>();
            EatElementTemporary = new List<EatElement>();

            ValidateEatElementSnakes();

            Snakes.AddRange(SnakesTemporary);

            foreach (var item in EatElementTemporary)
                EatElements.Remove(item);

            AddGameArenaSnakes();
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
