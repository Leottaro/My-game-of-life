using System;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Game_Of_Life
{
    public partial class MainWindow : Window
    {
        GameOfLife Game;
        public int CellSize;
        public bool isOn;
        public int tick;
        public int gen;

        public MainWindow()
        {
            InitializeComponent();

            CellSize = 16;
            isOn = false;
            tick = 1000/30;
            gen = 0;

            float resize = 2 / 3f;
            MyCanvas.Width = CellSize * (int)((System.Windows.SystemParameters.PrimaryScreenWidth * resize) / CellSize);
            MyCanvas.Height = CellSize * (int)((System.Windows.SystemParameters.PrimaryScreenHeight * resize) / CellSize);

            MyWindow.Width = MyCanvas.Width + 16;
            MyWindow.Height = MyCanvas.Height + 39 + 30;

            ClearButton.Width = 50;
            ClearButton.Height = 30;
            ClearButton.Margin = new Thickness(MyCanvas.Width - ClearButton.Width, MyCanvas.Height, 0, 0);

            PauseButton.Width = 50;
            PauseButton.Height = 30;
            PauseButton.Margin = new Thickness(MyCanvas.Width - ClearButton.Width - PauseButton.Width, MyCanvas.Height, 0, 0);
            PauseButton.Content = "Play";

            LabelTick.Content = String.Format("{0}ms", tick);
            LabelTick.Width = 60;
            LabelTick.Height = 30;
            LabelTick.Margin = new Thickness(0, MyCanvas.Height, 0, 0);

            TickSlider.Value = tick;
            TickSlider.Width = MyCanvas.Width - (LabelTick.Width + ClearButton.Width + PauseButton.Width + 10);
            TickSlider.Height = 30;
            TickSlider.Margin = new Thickness(LabelTick.Width + 5, MyCanvas.Height + 5, 0, 0);

            Game = new GameOfLife((int)(MyCanvas.Width / CellSize), (int)(MyCanvas.Height / CellSize));
            for (int y = 0; y < Game.Height; y++)
            {
                for (int x = 0; x < Game.Width; x++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = CellSize,
                        Height = CellSize,
                        Margin = new Thickness(x * CellSize, y * CellSize, 0, 0),
                        StrokeThickness = 1,
                        Fill = Brushes.White,
                        Stroke = Brushes.Black
                    };
                    MyCanvas.Children.Add(rect);
                }
            }
        }

        public Rectangle GetRect(int y, int x) { return (Rectangle)(MyCanvas.Children[y * Game.Width + x + 5]); }
        
        public async void loop()
        {
            while (isOn)
            {
                List<(int, int)> changed = Game.nextGen();
                if (changed.Count == 0) isOn = false;
                foreach ((int y, int x) in changed)
                    GetRect(y, x).Fill = Game.getCell(y, x) ? Brushes.Black : Brushes.White;
                gen += 1;
                GenLabel.Content = String.Format("gen {0}", gen);
                await Task.Delay(tick);
            }
            pause();
        }

        public void pause()
        {
            gen = 0;
            GenLabel.Content = "gen 0";
            PauseButton.Content = "Play";
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                if (isOn)
                {
                    isOn = false;
                }
                else
                {
                    isOn = true;
                    PauseButton.Content = "Pause";
                    loop();
                }
            }
            else if (e.Key == Key.Escape)
                MyWindow.Close();
            if (e.Key == Key.C || e.Key == Key.R)
            {
                isOn = false;
                Game = new GameOfLife(Game.Width, Game.Height);
                for (int y = 0; y < Game.Height; y++)
                    for (int x = 0; x < Game.Width; x++)
                        GetRect(y, x).Fill = Brushes.White;
            }
        }

        private void MyCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isOn && e.OriginalSource is Rectangle)
            {
                Rectangle rect = (Rectangle)(e.OriginalSource);
                int x = (int)(rect.Margin.Left / CellSize);
                int y = (int)(rect.Margin.Top / CellSize);

                if (e.LeftButton == MouseButtonState.Pressed && rect.Fill == Brushes.White)
                {
                    Game.setCell(y, x, true);
                    rect.Fill = Brushes.Black;
                }
                if (e.RightButton == MouseButtonState.Pressed && rect.Fill == Brushes.Black)
                {
                    Game.setCell(y, x, false);
                    rect.Fill = Brushes.White;
                }
            }
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isOn && e.OriginalSource is Rectangle)
            {
                Rectangle rect = (Rectangle)(e.OriginalSource);
                int x = (int)(rect.Margin.Left / CellSize);
                int y = (int)(rect.Margin.Top / CellSize);

                if (e.LeftButton == MouseButtonState.Pressed && rect.Fill == Brushes.White)
                {
                    Game.setCell(y, x, true);
                    rect.Fill = Brushes.Black;
                }
                if (e.RightButton == MouseButtonState.Pressed && rect.Fill == Brushes.Black)
                {
                    Game.setCell(y, x, false);
                    rect.Fill = Brushes.White;
                }
            }
        }

        private void TickSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tick = (int)(e.NewValue);
            LabelTick.Content = String.Format("{0}ms", tick.ToString());
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isOn)
            {
                isOn = false;
            }
            else
            {
                isOn = true;
                PauseButton.Content = "Pause";
                loop();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            isOn = false;
            Game = new GameOfLife(Game.Width, Game.Height);
            for (int y = 0; y < Game.Height; y++)
                for (int x = 0; x < Game.Width; x++)
                    GetRect(y, x).Fill = Brushes.White;
        }
    }
} 