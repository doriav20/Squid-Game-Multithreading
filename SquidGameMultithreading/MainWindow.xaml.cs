using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SquidGameMultithreading
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int N = 144;

        private DispatcherTimer timer;
        private int time_remaining; // In Seconds

        private Person[] people;
        private ImageWithText[] facesImages;

        private int toRemove;

        private Thread killer, remover;
        private Semaphore semaphore;

        public MainWindow()
        {
            InitializeComponent();

            people = ExternalMethods.InitializePeople(N);
            facesImages = new ImageWithText[N];

            CreateUI();
            toRemove = -1;

            time_remaining = 5 * N;
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
            semaphore = new Semaphore(0, 1);
            killer = new Thread(KillPlayer);
            killer.Start();
            remover = new Thread(RemoveFromUI);
            remover.Start();

        }

        private void CreateUI()
        {
            int row = 0;
            int col = 0;
            //16:9
            for (int i = 0; i < N; i++)
            {
                facesImages[i] = new ImageWithText();
                facesImages[i].Image = new Image();
                facesImages[i].TextBlock = new TextBlock();

                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.Length - 11) + $@"\imgs\{people[i].FaceImageID}.jpg";
                BitmapImage bmp_img = new BitmapImage(new Uri(path));
                facesImages[i].Image.Source = bmp_img;
                facesImages[i].Image.HorizontalAlignment = HorizontalAlignment.Left;
                facesImages[i].Image.VerticalAlignment = VerticalAlignment.Top;
                facesImages[i].Image.Margin = new Thickness(96 * row, 96 * col, 0, 0);
                facesImages[i].Image.Width = 96;
                facesImages[i].Image.Height = 96;

                facesImages[i].TextBlock.Text = people[i].ID.ToString();
                facesImages[i].TextBlock.FontFamily = new FontFamily("Calibri");
                facesImages[i].TextBlock.FontWeight = FontWeights.Bold;
                facesImages[i].TextBlock.FontSize = 16;
                facesImages[i].TextBlock.Foreground = new SolidColorBrush(Color.FromRgb(85, 255, 225));
                facesImages[i].TextBlock.Margin = new Thickness(96 * row + 96 / 2 - 10, 96 * col + 96 / 2 + 25, 0, 0);

                imagesGrid.Children.Add(facesImages[i].Image);
                imagesGrid.Children.Add(facesImages[i].TextBlock);

                col++;
                row = row + col / 9;
                col %= 9;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            time_remaining--;
            string hours = (time_remaining / (60 * 60)).ToString().PadLeft(2, '0');
            string minutes = (time_remaining / 60 % 60).ToString().PadLeft(2, '0');
            string seconds = (time_remaining % 60).ToString().PadLeft(2, '0');
            timer_label.Content = $"{hours}:{minutes}:{seconds}";
            if (time_remaining == 0)
            {
                timer.Stop();
                killer.Abort();
                remover.Abort();
                eliminationsTextBox.Foreground = Brushes.Red;
                if (people[toRemove] == null)
                {
                    for (int i = 0; i < N; i++)
                    {
                        if (people[i] != null)
                        {
                            toRemove = i;
                            break;
                        }
                    }
                }
                eliminationsTextBox.AppendText($"{people[toRemove].Name} Won!!!");
                eliminationsTextBox.AppendText(Environment.NewLine);
                eliminationsTextBox.AppendText($"ID: {people[toRemove].ID} Age: {people[toRemove].Age}");
                eliminationsTextBox.AppendText(Environment.NewLine);
                eliminationsTextBox.AppendText(Environment.NewLine);
                eliminationsTextBox.AppendText("The Game is Ended!");
            }
        }
        private void KillPlayer()
        {
            Random rand = new Random();
            int old_time_remaining = time_remaining;
            while (true)
            {
                while (old_time_remaining - time_remaining < 5) Thread.Sleep((int)(0.5 * 1000));
                old_time_remaining -= 5;
                while (true)
                {
                    toRemove = rand.Next(0, N);
                    if (people[toRemove] != null) break;
                }
                semaphore.Release();
                semaphore.WaitOne();
            }
        }
        private void RemoveFromUI()
        {
            while (true)
            {
                semaphore.WaitOne();
                Dispatcher.Invoke(() =>
                {
                    eliminationsTextBox.AppendText($"{people[toRemove].Name} Eliminated");
                    eliminationsTextBox.AppendText(Environment.NewLine);
                    eliminationsTextBox.AppendText($"ID: {people[toRemove].ID} Age: {people[toRemove].Age}");
                    eliminationsTextBox.AppendText(Environment.NewLine);
                    eliminationsTextBox.AppendText(Environment.NewLine);
                });

                Thread opacityThread = new Thread(ChangeOpacity);
                opacityThread.Start();

                people[toRemove] = null;
                semaphore.Release();
            }
        }

        private void ChangeOpacity()
        {
            double opacity = 1.0;
            while (opacity > 0)
            {
                opacity -= 0.05;
                Dispatcher.Invoke(() =>
                {
                    facesImages[toRemove].Image.Opacity = opacity;
                    facesImages[toRemove].TextBlock.Opacity = opacity;
                });
                Thread.Sleep(100);
            }
        }
    }
}
