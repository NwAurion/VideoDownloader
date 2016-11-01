using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using VideoDownloader.Web;

namespace VideoDownloader
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //string videoID;
        string fullFileName;
        MediaFile inputFile;
        MediaFile outputFile;

        public string SavePath;
        //public static readonly DependencyProperty SavePathProperty = DependencyProperty.Register("SavePath", typeof(String), typeof(ContentControl));
        /*public String SavePath
        {
            get { return (String)GetValue(SavePathProperty); }
            set
            {
                SetValue(SavePathProperty, value);
            }
        }*/

        private BackgroundWorker backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            ReadSettings();
            // backgroundWorker.WorkerReportsProgress = true;
            // backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
        }

        private void ReadSettings()
        {
            SavePath = Properties.Settings.Default.SavePath;
        }

        private void Download(string videoID)
        {
            fullFileName = Downloader.SaveVideoToDisk(videoID, SavePath);
        }

        private string ExtractVideoID(string text)
        {
            string videoID;
            int startOfId = text.LastIndexOf("=");
            if (startOfId >= 0)
            {
                videoID = text.Substring(startOfId + 1, text.Length - startOfId - 1);
            }
            else if (startOfId < 0)
            {
                startOfId = text.LastIndexOf("/");
                videoID = text.Substring(startOfId + 1, text.Length - startOfId - 1);
            }
            else
            {
                videoID = text;
            }
            return videoID;
            
        }

        #region Backgroundworker
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Download(e.Argument as string);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btDownload.IsEnabled = true;
            btConvert.IsEnabled = true;
        }
        #endregion

        #region UI-interaction
        private void btDownload_Click(object sender, RoutedEventArgs e)
        {
            // extract the videoID from the entered text.
            string videoID = ExtractVideoID(tbVideoID.Text);
            string test = SavePath;
            if (!String.IsNullOrWhiteSpace(SavePath))
            {
                if (!SavePath.EndsWith("/") && !SavePath.EndsWith("\\"))
                {
                    SavePath += "\\";
                }
                backgroundWorker.RunWorkerAsync(videoID);
                if (backgroundWorker.IsBusy)
                {
                    btDownload.IsEnabled = false;
                    btConvert.IsEnabled = false;
                    lbSavePath.BorderThickness = new Thickness(1);
                    lbSavePath.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFABADB3");
                }
            }
            else
            {
                lbSavePath.BorderThickness = new Thickness(2);
                lbSavePath.BorderBrush = Brushes.Red;
            }
        }

        private void btConvert_Click(object sender, RoutedEventArgs e)
        {
            string file = SavePath + fullFileName;
            inputFile = new MediaFile(file + ".mp4");
            outputFile = new MediaFile(file + ".mp3");

            using (var engine = new Engine())
            {
                engine.Convert(inputFile, outputFile);
            }
        }

        private void btSaveFile_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dr = System.Windows.Forms.DialogResult.OK;
            if (dr == fbd.ShowDialog())
            {
                SavePath = fbd.SelectedPath;
                lbSavePath.Content = SavePath;
            }
        }

        private void btSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SavePath = SavePath;
        }
        #endregion

    }
}
