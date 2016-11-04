using IniParser;
using IniParser.Model;
using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using VideoDownloader.Web;

namespace VideoDownloader
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IniData data;
        public string settingsPath;

        public String _savePath;
        public String SavePath
        {
            get { return _savePath; }
            set
            {
                _savePath = value;
                OnPropertyChanged("SavePath");
            }
        }

        private BackgroundWorker backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            settingsPath = Environment.CurrentDirectory + "\\VideoDownloader.cfg";
            LoadSettings();

        }

        private void LoadSettings()
        {
            FileInfo sFI = new FileInfo(settingsPath);
            if (sFI.Exists)
            {
                var parser = new FileIniDataParser();
                data = parser.ReadFile(sFI.FullName);
                SavePath = data["Path"]["SavePath"];
            }
        }

        private void Download(string videoID)
        {
            VideoFile.Name = Downloader.SaveVideoToDisk(videoID, SavePath);
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

            if (!String.IsNullOrEmpty(tbVideoID.Text))
            {

                if (!SavePath.EndsWith("/") && !SavePath.EndsWith("\\"))
                {
                    SavePath += "\\";
                }

                backgroundWorker.RunWorkerAsync(videoID);
                if (backgroundWorker.IsBusy)
                {
                    VideoFile.Path = SavePath;
                    btDownload.IsEnabled = false;
                    btConvert.IsEnabled = false;
                }
            }
        }

        private void btConvert_Click(object sender, RoutedEventArgs e)
        {
            string PathAndName = VideoFile.Path + VideoFile.Name;
            MediaFile inputFile = new MediaFile(PathAndName+VideoFile.Extension);
            MediaFile outputFile = new MediaFile(PathAndName + ".mp3");

            using (var engine = new Engine())
            {
                engine.Convert(inputFile, outputFile);
            }
        }

        private void btSaveFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dr = System.Windows.Forms.DialogResult.OK;
            if (dr == fbd.ShowDialog())
            {
                SavePath = fbd.SelectedPath;
                lbSavePath.Content = SavePath;
                data["Path"]["SavePath"] = SavePath;
            }
        }

        private void btSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            var parser = new FileIniDataParser();
            FileInfo sfi = new FileInfo(settingsPath);
            if (sfi.Exists)
            {
                parser.WriteFile(settingsPath, data);
            }
        }


        private void btLoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = SavePath;
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string name = dlg.FileName;
                int startPos = 0;
                int endPos = name.LastIndexOf("\\") + 1;
                VideoFile.Path = name.Substring(startPos, endPos);

                startPos = VideoFile.Path.Length;
                endPos = name.LastIndexOf(".") - startPos;
                VideoFile.Name = name.Substring(startPos, endPos);
                VideoFile.Extension = name.Substring(name.LastIndexOf("."));
                btConvert.IsEnabled = true;
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
