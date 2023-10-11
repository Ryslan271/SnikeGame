using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private EatElement EatElements = null;

        private TrafficSide DirectionMovement = TrafficSide.Right;

        private readonly DispatcherTimer timer = new DispatcherTimer();

        private readonly Random rnd = new Random();

        private readonly List<SnakePart> Snakes = new List<SnakePart>();
        private readonly SnakePart Snake = new SnakePart() { IsHead = true };

        public MainWindow()
        {
            Snakes.Add(Snake);

            InitializeComponent();

            timer.Tick += new EventHandler(timer_Tick);
            TimerStart(300);
        }

        #region Таймер движения
        private void TimerStart(int milliseconds = default)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Snakes.Count; i++)
            {
                Snakes[i].SnakeX = Snakes[i].X;
                Snakes[i].SnakeY = Snakes[i].Y;
                
                if (i != 0)
                {

                    Snakes[i].X = Snakes[i - 1].SnakeX;
                    Snakes[i].Y = Snakes[i - 1].SnakeY;

                    continue;
                }

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

            AddGameArenaSnakes();

            MoveSnakes();
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
                    nextIsOdd = rowCounter % 2 != 0;
                }

                if (nextY >= GameArea.ActualHeight)
                    doneDrawingBackground = true;
            }
        }

        private void DrawEssences()
        {
            if (EatElements != null)
            {
                return;
            }

            int essencesX = rnd.Next(1, 20);
            int essencesY = rnd.Next(1, 20);

            while (true)
            {
                if (ValidateSnakeAndEatPosition(essencesX, essencesY))
                {
                    continue;
                }

                EatElement essence = new EatElement()
                {
                    Y = essencesX * 20,
                    X = essencesY * 20
                };

                EatElements = essence;

                GameArea.Children.Add(essence.UiElement);
                Canvas.SetTop(essence.UiElement, essence.Y);
                Canvas.SetLeft(essence.UiElement, essence.X);

                break;
            }
        }

        private bool ValidateSnakeAndEatPosition(int eatX, int eatY)
            => Snakes.FirstOrDefault(snakeElement => snakeElement.SnakeX == eatX && snakeElement.SnakeY == eatY) != null;

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
            // Проверка на количество элементов змейки на поле и последующее их добавление 
            if (Snakes.Count > GameArea.Children.Count - 401)
                for (int i = 0; i < Snakes.Count - (GameArea.Children.Count - 401); i++)
                    GameArea.Children.Add(Snakes[Snakes.IndexOf(Snakes.Last()) - i].UiElement);
        }

        private void MoveSnakes()
        {
            // движение клеток
            foreach (SnakePart snakeItem in Snakes)
            {
                Canvas.SetTop(snakeItem.UiElement, snakeItem.Y);
                Canvas.SetLeft(snakeItem.UiElement, snakeItem.X);
            }
        }

        private void RenderingHelperMethodForDrawing()
        {
            ValidateEatElementSnakes(); // проверка сьела ли змейка еду и добавление змейке новый элемент

            AddGameArenaSnakes(); // добавление нового элемента змейки на поле

            MoveSnakes(); //  движение всех элементов змеки

            DrawEssences(); // добавление новой еды на поле
        }

        private void ValidateEatElementSnakes()
        {
            if (EatElements.Y == Snakes[0].Y &&
                EatElements.X == Snakes[0].X)
            {
                GameArea.Children.Remove(EatElements.UiElement);

                for (int i = 0; i < CountingQuantity(EatElements); i++)
                {
                    SnakePart newSnake = new SnakePart
                    {
                        X = EatElements.X,
                        Y = EatElements.Y
                    };

                    Snakes.Add(newSnake);
                }

                EatElements = null;
            }
        }

        // Определяет количество элементов добавляющихся в змейку от цвета еды
        private int CountingQuantity(EatElement eatItem)
        {
            if (eatItem.Fill == Brushes.Red)
                return 1;
            else
                return eatItem.Fill == Brushes.Blue ? 2 : 3;
        }

        #region изменение скорости змейки
        private void TurtleSpeedSnake(object sender, RoutedEventArgs e)
            => TimerStart(250);

        private void DogSpeedSnake(object sender, RoutedEventArgs e)
            => TimerStart(150);

        private void BunnySpeedSnake(object sender, RoutedEventArgs e)
            => TimerStart(50);
        #endregion
    }
}
