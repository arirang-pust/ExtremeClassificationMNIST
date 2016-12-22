using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mine.Data.Readers;
using Mine.Engines.Classifiers;
using System.ComponentModel;
using System.IO;

namespace Mine.Apps.OCR.ExtremeClassificationMNISTDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GLMClassifierSelectAlgorithm alg = new GLMClassifierSelectAlgorithm();

        BackgroundWorker worker = new BackgroundWorker();
        BackgroundWorker worker2 = new BackgroundWorker();
        DateTime begin;

        public MainWindow()
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            worker2.WorkerReportsProgress = true;
            worker2.WorkerSupportsCancellation = true;
            worker2.DoWork += new DoWorkEventHandler(worker2_DoWork);
            worker2.ProgressChanged += new ProgressChangedEventHandler(worker2_ProgressChanged);
            worker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker2_RunWorkerCompleted);
        }

        void worker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                m_labelState.Content = "Cancelled";
            }
            else
            {
                m_labelState.Content = "Training Complete";
                m_buttonWrite.IsEnabled = true;
            }

            m_buttonTrain.Content = "Train";
            m_buttonInit.IsEnabled = true;
            m_buttonTrain.IsEnabled = false;
            m_textBoxIter.IsEnabled = true;
            m_textBoxDp.IsEnabled = true;
            m_progressBar.Visibility = Visibility.Collapsed;
        }

        void worker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan ts = now.Subtract(begin);
            double milli = ts.TotalMilliseconds;
            int t = (int)e.UserState;
            double dt = milli / (t - 1);
            double remainMilli = dt * (m_progressBar.Maximum - t);
            double remainSec = remainMilli / 1000;

            m_progressBar.Value = t;

            if (remainSec > 60)
            {
                m_labelState.Content = Math.Ceiling(remainSec / 60).ToString() + " Min left";
            }
            else
            {
                m_labelState.Content = Math.Ceiling(remainSec).ToString() + " Sec left";
            }
        }

        void worker2_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] paras = e.Argument as object[];
            GLMClassifierSelectAlgorithm alg = paras[0] as GLMClassifierSelectAlgorithm;
            int iter = (int)paras[1];
            BackgroundWorker back = sender as BackgroundWorker;

            for (int i = 0; i < iter; i++)
            {
                if (back.CancellationPending == true)
                {
                    e.Cancel = true;

                    break;
                }

                alg.ExecuteOneIteration();

                back.ReportProgress((i + 1) * 100 / iter, i + 1);
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            KeyValuePair<byte[], byte> temp;
            byte[] data;
            WriteableBitmap wBmp;
            Grid grid;
            Image image;
            Label digit;

            // ui
            m_labelState.Content = "Initialized";
            m_buttonInit.IsEnabled = true;
            m_buttonTrain.IsEnabled = true;
            m_textBoxDp.IsEnabled = true;

            // train data display
            for (int i = 0; i < alg.trainData.Count; i++)
            {
                temp = alg.trainData[i];
                data = temp.Key;
                wBmp = new WriteableBitmap(28, 28, 96, 96, PixelFormats.Gray8, null);
                wBmp.Lock();
                wBmp.WritePixels(new Int32Rect(0, 0, 28, 28), data, 28, 0);
                wBmp.Unlock();

                grid = new Grid();
                grid.Height = 28;
                grid.Width = 160;
                grid.HorizontalAlignment = HorizontalAlignment.Left;
                grid.VerticalAlignment = VerticalAlignment.Top;

                image = new Image();
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.VerticalAlignment = VerticalAlignment.Top;
                image.Margin = new Thickness(10, 0, 0, 0);
                image.Source = wBmp;

                digit = new Label();
                digit.HorizontalAlignment = HorizontalAlignment.Right;
                digit.VerticalAlignment = VerticalAlignment.Stretch;
                digit.HorizontalContentAlignment = HorizontalAlignment.Left;
                digit.VerticalContentAlignment = VerticalAlignment.Center;
                digit.Width = 50;
                digit.Content = temp.Value;

                grid.Children.Add(image);
                grid.Children.Add(digit);

                m_listBoxTrain.Items.Add(grid);
            }

            // Validation data display
            for (int i = 0; i < alg.valData.Count; i++)
            {
                temp = alg.valData[i];
                data = temp.Key;
                wBmp = new WriteableBitmap(28, 28, 96, 96, PixelFormats.Gray8, null);
                wBmp.Lock();
                wBmp.WritePixels(new Int32Rect(0, 0, 28, 28), data, 28, 0);
                wBmp.Unlock();

                grid = new Grid();
                grid.Height = 28;
                grid.Width = 160;
                grid.HorizontalAlignment = HorizontalAlignment.Left;
                grid.VerticalAlignment = VerticalAlignment.Top;

                image = new Image();
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.VerticalAlignment = VerticalAlignment.Top;
                image.Margin = new Thickness(10, 0, 0, 0);
                image.Source = wBmp;

                digit = new Label();
                digit.HorizontalAlignment = HorizontalAlignment.Right;
                digit.VerticalAlignment = VerticalAlignment.Stretch;
                digit.HorizontalContentAlignment = HorizontalAlignment.Left;
                digit.VerticalContentAlignment = VerticalAlignment.Center;
                digit.Width = 50;
                digit.Content = temp.Value;

                grid.Children.Add(image);
                grid.Children.Add(digit);

                m_listBoxVal.Items.Add(grid);
            }

            // Classifiers display
            for (int i = 0; i < alg.classifiers.Count; i++)
            {
                m_listBoxclf.Items.Add(alg.classifiers[i].ToString());
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] paras = e.Argument as object[];
            GLMClassifierSelectAlgorithm alg = paras[0] as GLMClassifierSelectAlgorithm;
            int dp = (int)paras[1];

            alg.Initialize(dp, dp);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ui
            m_buttonTrain.IsEnabled = false;
            m_progressBar.Visibility = Visibility.Collapsed;
            m_buttonWrite.IsEnabled = false;
        }

        private void m_buttonInit_Click(object sender, RoutedEventArgs e)
        {
            int dp;

            try
            {
                dp = int.Parse(m_textBoxDp.Text);
            }
            catch (System.Exception)
            {
                MessageBox.Show("Error in input formats.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            // ui
            m_labelState.Content = "Initializing...";
            m_buttonInit.IsEnabled = false;
            m_buttonWrite.IsEnabled = false;
            m_textBoxDp.IsEnabled = false;

            //
            worker.RunWorkerAsync(new object[]{alg, dp});

            // new
            m_listBoxclf.Items.Clear();
            m_listBoxTrain.Items.Clear();
            m_listBoxVal.Items.Clear();
        }

        private void m_buttonTrain_Click(object sender, RoutedEventArgs e)
        {
            int iter = 0;

            if ((string)m_buttonTrain.Content == "Train")
            {
                try
                {
                    iter = int.Parse(m_textBoxIter.Text);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error in input formats.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                    return;
                }

                m_buttonTrain.Content = "Stop";
                m_buttonInit.IsEnabled = false;
                m_progressBar.Visibility = Visibility.Visible;
                m_progressBar.Minimum = 0;
                m_progressBar.Maximum = iter;
                m_progressBar.Value = 0;
                m_textBoxIter.IsEnabled = false;
                m_textBoxDp.IsEnabled = false;

                begin = DateTime.Now;

                worker2.RunWorkerAsync(new object[] { alg, iter });
            }
            else
            {
                worker2.CancelAsync();
            }
        }

        private void m_buttonWrite_Click(object sender, RoutedEventArgs e)
        {
            alg.WriteAllData();
            m_labelState.Content = "Write Complete";
            m_buttonWrite.IsEnabled = false;
        }

        private void m_listBoxclf_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((string)m_labelState.Content != "Training Complete" && (string)m_labelState.Content != "Write Complete") || m_listBoxclf.SelectedIndex < 0)
            {
                return;
            }

            GLMExtremeClassifier<byte> cl = alg.classifiers[m_listBoxclf.SelectedIndex];
            TestWindow testWin = new TestWindow(cl);

            testWin.ShowDialog();
        }
    }
}
