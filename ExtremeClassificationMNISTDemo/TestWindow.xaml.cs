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
using System.Windows.Shapes;
using Mine.Engines.Classifiers;
using Mine.Data.Readers;
using System.ComponentModel;

namespace Mine.Apps.OCR.ExtremeClassificationMNISTDemo
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        GLMExtremeClassifier<byte> classifier;
        MNISTReader reader = new MNISTReader();
        List<KeyValuePair<byte[], byte>> testData;
        int cnt;
        BackgroundWorker worker = new BackgroundWorker();

        public TestWindow(GLMExtremeClassifier<byte> cl)
        {
            InitializeComponent();

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.WorkerReportsProgress = true;

            classifier = cl;

            m_labelType.Content = classifier.Type;
            m_labelTau.Content = classifier.Tau;
            m_labelLambda.Content = classifier.Lambda;
            m_labelMinError.Content = classifier.ValErrs.Min() + "%";

            if (classifier.NormType == 0)
            {
                m_labelNorm.Content = "Trace";
            }
            else if (classifier.NormType == 1)
            {
                m_labelNorm.Content = "L1";
            }
            else if (classifier.NormType == 2)
            {
                m_labelNorm.Content = "L2";
            }            
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            double per = cnt * 100.0 / testData.Count;

            m_labelTestError.Content = per.ToString("F2") + "%";
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object[] paras = e.UserState as object[];
            byte res = (byte)paras[1];
            int i = (int)paras[0];
            Grid grid;
            Label result;

            if (testData[i].Value != res)
            {
                cnt++;
            }

            grid = m_listBoxTest.Items[i] as Grid;

            result = new Label();
            result.HorizontalAlignment = HorizontalAlignment.Right;
            result.VerticalAlignment = VerticalAlignment.Stretch;
            result.HorizontalContentAlignment = HorizontalAlignment.Left;
            result.VerticalContentAlignment = VerticalAlignment.Center;
            result.Margin = new Thickness(0, 0, 0, 0);
            result.Width = 40;
            result.Content = res;

            grid.Children.Add(result);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            KeyValuePair<byte[], byte> temp;
            byte res;
            BackgroundWorker back = sender as BackgroundWorker;

            for (int i = 0; i < testData.Count; i++)
            {
                temp = testData[i];

                res = classifier.TestVal(temp.Key);

                back.ReportProgress(0, new object[] { i, res });
            }
        }

        private void m_buttonTest_Click(object sender, RoutedEventArgs e)
        {
            cnt = 0;

            worker.RunWorkerAsync();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            testData = reader.GetAllTestSamples();

            KeyValuePair<byte[], byte> temp;
            byte[] data;
            Grid grid;
            Image image;
            Label digit;
            Label no;

            for (int i = 0; i < testData.Count; i++)
            {
                no = new Label();
                no.HorizontalAlignment = HorizontalAlignment.Left;
                no.VerticalAlignment = VerticalAlignment.Stretch;
                no.HorizontalContentAlignment = HorizontalAlignment.Right;
                no.VerticalContentAlignment = VerticalAlignment.Center;
                no.Margin = new Thickness(0, 0, 0, 0);
                no.Width = 42;
                no.Content = (i + 1);

                temp = testData[i];
                data = temp.Key;
                WriteableBitmap wBmp = new WriteableBitmap(28, 28, 96, 96, PixelFormats.Gray8, null);
                wBmp.Lock();
                wBmp.WritePixels(new Int32Rect(0, 0, 28, 28), data, 28, 0);
                wBmp.Unlock();

                grid = new Grid();
                grid.Height = 28;
                grid.Width = 290;
                grid.HorizontalAlignment = HorizontalAlignment.Left;
                grid.VerticalAlignment = VerticalAlignment.Top;

                image = new Image();
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.VerticalAlignment = VerticalAlignment.Top;
                image.Margin = new Thickness(56, 0, 0, 0);
                image.Source = wBmp;

                digit = new Label();
                digit.HorizontalAlignment = HorizontalAlignment.Right;
                digit.VerticalAlignment = VerticalAlignment.Stretch;
                digit.HorizontalContentAlignment = HorizontalAlignment.Left;
                digit.VerticalContentAlignment = VerticalAlignment.Center;
                digit.Margin = new Thickness(0, 0, 90, 0);
                digit.Width = 60;
                digit.Content = temp.Value;

                grid.Children.Add(no);
                grid.Children.Add(image);
                grid.Children.Add(digit);

                m_listBoxTest.Items.Add(grid);
            }
        }
    }
}
