using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HahaMarker
{
    /// <summary>
    /// Window for manual marking chuncks in sound file
    /// </summary>
    public partial class MainWindow : Window
    {

        static string testWavFile = @"..\..\..\..\Files\sound.wav";

        public MainWindow()
        {
            InitializeComponent();
        }

        AudioFileReader reader;
        WaveOut waveOut;
        bool playStart = false;
        bool playStopped = true;

        BackgroundWorker backgroundWorker1 = new BackgroundWorker();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(testWavFile)) { MessageBox.Show("Error", "No file " + testWavFile, MessageBoxButton.OK, MessageBoxImage.Error); return; }

            reader = new AudioFileReader(testWavFile);
            waveOut = new WaveOut(); // or WaveOutEvent()
            waveOut.Init(reader);


            var lenghtInSecs = reader.Length / reader.WaveFormat.AverageBytesPerSecond;
            PositionSlider.Minimum = 0;
            PositionSlider.Maximum = lenghtInSecs;
            PositionSlider.TickFrequency = 60;
            PositionSlider.Ticks = new DoubleCollection() { 60,120 };

            PositionSlider.Width = (lenghtInSecs / 60+1)*100;


            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;

            PrintState();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!playStart)
            {
                waveOut.Play();
                playStart = true;
                playStopped = false;
                backgroundWorker1.RunWorkerAsync();
            }
            else {
                waveOut.Resume();
                playStopped = false;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            waveOut.Pause();
            playStopped = true;
            backgroundWorker1.CancelAsync();
        }


        // This event handler is where the time-consuming work is done.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (true)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress(1);
                }
            }
        }


        // This event handler updates the progress.
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentTime.Text = TimePosition().ToString();
        }


        TimeSpan TimePosition()
        {
            double sec = waveOut.GetPosition() / waveOut.OutputWaveFormat.BitsPerSample / waveOut.OutputWaveFormat.Channels * 8 / waveOut.OutputWaveFormat.SampleRate;
            return reader.CurrentTime;// new TimeSpan(0, 0, (int)sec);
        }

        // This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                PrintState();
            }
            else if (e.Error != null)
            {
                CurrentTime.Text = "Error: " + e.Error.Message;
            }
            else
            {
                PrintState();
            }
        }

        void PrintState()
        {
            var s = playStopped ? " (stopped)" : "";
            CurrentTime.Text = (TimePosition().ToString()) + s;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
            {
                SetMusicPosition();
            }
        }

        private void SetMusicPosition()
        {
            var positionSecs = PositionSlider.Value;
            reader.CurrentTime = new TimeSpan(0, 0, (int)positionSecs);
            PrintState();
        }

        bool dragStarted = false;

        private void PositionSlider_DragStarted(object sender, RoutedEventArgs e)
        {
            dragStarted = true;
        }

        private void PositionSlider_DragCompleted(object sender, RoutedEventArgs e)
        {
            dragStarted = false;
            SetMusicPosition();
        }


        List<KeyValuePair<TimeSpan, TimeSpan>> marks = new List<KeyValuePair<TimeSpan, TimeSpan>>(); 

        TimeSpan markStart;

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var markEnd = TimePosition();
            if (markStart != markEnd)
            {
                marks.Add(new KeyValuePair<TimeSpan, TimeSpan>(markStart, markEnd));
            }
            SaveButton.Background = new SolidColorBrush(Colors.Green);
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            markStart = TimePosition();
            SaveButton.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < marks.Count; i++)
            {
                var k = marks[i];
                if (i > 0) sb.Append(Environment.NewLine);
                sb.Append(k.Key.ToString() + "-" + k.Value.ToString());
            }

            File.WriteAllText("laugth.txt",sb.ToString());
        }
    }
}
