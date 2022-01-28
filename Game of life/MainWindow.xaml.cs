using System;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Game_Of_LIfe
{
    public partial class MainWindow : Window
    {
        const float resize = 2 / 3f;
        public int[,] Grid = new int[0, 0];
        public int GridWidth = 10;
        public int GridHeight = 10;
        public int CellSize = 10;
        public bool isOn = false;
        public DispatcherTimer timer = new DispatcherTimer();
        public int tick = 100;
        public int gen = 0;

        public MainWindow()
        {
            InitializeComponent();

            tick = 30;
            CellSize = 10;
            GridWidth = 10;
            GridHeight = 10;

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

            LabelTick.Content = tick.ToString() + "ms";
            LabelTick.Width = 60;
            LabelTick.Height = 30;
            LabelTick.Margin = new Thickness(0, MyCanvas.Height, 0, 0);

            TickSlider.Value = tick;
            TickSlider.Width = MyCanvas.Width - (LabelTick.Width + ClearButton.Width + PauseButton.Width + 10);
            TickSlider.Height = 30;
            TickSlider.Margin = new Thickness(LabelTick.Width + 5, MyCanvas.Height + 5, 0, 0);

            GridWidth = (int)(MyCanvas.Width / CellSize);
            GridHeight = (int)(MyCanvas.Height / CellSize);

            Grid = new int[GridHeight, GridWidth];
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
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
                    Grid[y, x] = 0;
                    MyCanvas.Children.Add(rect);
                }
            }

            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, tick);
        }

        public Rectangle GetRect(int x, int y)
        {
            return (Rectangle)(MyCanvas.Children[y * GridWidth + x + 5]);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var nextgen = new (int, int)[GridWidth * GridHeight];
            int i = 0;

            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    int score = 0;

                    if (x > 0)
                    {
                        if (y > 0)
                        {
                            if (Grid[y - 1, x - 1] == 1) { score++; }
                        }
                        if (y < GridHeight - 1)
                        {
                            if (Grid[y + 1, x - 1] == 1) { score++; }
                        }
                        if (Grid[y, x - 1] == 1) { score++; }
                    }
                    if (x < GridWidth - 1)
                    {
                        if (y > 0)
                        {
                            if (Grid[y - 1, x + 1] == 1) { score++; }
                        }
                        if (y < GridHeight - 1)
                        {
                            if (Grid[y + 1, x + 1] == 1) { score++; }
                        }
                        if (Grid[y, x + 1] == 1) { score++; }
                    }
                    if (y > 0)
                    {
                        if (Grid[y - 1, x] == 1) { score++; }
                    }
                    if (y < GridHeight - 1)
                    {
                        if (Grid[y + 1, x] == 1) { score++; }
                    }

                    if (Grid[y, x] == 1)
                    {
                        if (score < 2 || score > 3)
                        {
                            nextgen[i] = (x, y);
                            i++;
                        }
                    }
                    else
                    {
                        if (score == 3)
                        {
                            nextgen[i] = (x, y);
                            i++;
                        }
                    }
                }
            }

            if (i == 0) { isOn = false; }

            for (int j = 0; j < i; j++)
            {
                (int x, int y) = nextgen[j];

                if (Grid[y, x] == 1)
                {
                    Grid[y, x] = 0;
                    GetRect(x, y).Fill = Brushes.White;
                }
                else
                {
                    Grid[y, x] = 1;
                    GetRect(x, y).Fill = Brushes.Black;
                }
            }

            GenLabel.Content = gen.ToString();
            if (isOn)
            {
                gen += 1;
                timer.Start();
            }
            else
            {
                timer.Stop();
                PauseButton.Content = "Play";
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                if (isOn)
                {
                    isOn = false;
                    PauseButton.Content = "Play";
                }
                else
                {
                    isOn = true;
                    timer.Start();
                    PauseButton.Content = "Pause";
                }
            }
            else if (e.Key == Key.Escape)
            {
                MyWindow.Close();
            }
            if (e.Key == Key.C || e.Key == Key.R)
            {
                isOn = false;
                PauseButton.Content = "Play";
                timer.Stop();
                GenLabel.Content = "0";
                gen = 0;

                Grid = new int[GridHeight, GridWidth];
                for (int y = 0; y < GridHeight; y++)
                {
                    for (int x = 0; x < GridWidth; x++)
                    {
                        Grid[y, x] = 0;
                        GetRect(x, y).Fill = Brushes.White;
                    }
                }
            }
        }

        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            if (!isOn && e.OriginalSource is Rectangle)
            {
                Rectangle rect = (Rectangle)(e.OriginalSource);
                int x = (int)(rect.Margin.Left / CellSize);
                int y = (int)(rect.Margin.Top / CellSize);

                if (e.LeftButton == MouseButtonState.Pressed && rect.Fill == Brushes.White)
                {
                    Grid[y, x] = 1;
                    rect.Fill = Brushes.Black;
                }
                if (e.RightButton == MouseButtonState.Pressed && rect.Fill == Brushes.Black)
                {
                    Grid[y, x] = 0;
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
                    Grid[y, x] = 1;
                    rect.Fill = Brushes.Black;
                }
                if (e.RightButton == MouseButtonState.Pressed && rect.Fill == Brushes.Black)
                {
                    Grid[y, x] = 0;
                    rect.Fill = Brushes.White;
                }
            }
        }

        private void TickSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tick = (int)(e.NewValue);
            LabelTick.Content = tick.ToString() + "ms";
            timer.Interval = new TimeSpan(0, 0, 0, 0, tick);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isOn)
            {
                isOn = false;
                PauseButton.Content = "Play";
            }
            else
            {
                isOn = true;
                PauseButton.Content = "Pause";
                timer.Start();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            isOn = false;
            timer.Stop();
            GenLabel.Content = "0";
            gen = 0;

            Grid = new int[GridHeight, GridWidth];
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    Grid[y, x] = 0;
                    GetRect(x, y).Fill = Brushes.White;
                }
            }
        }
    }
}