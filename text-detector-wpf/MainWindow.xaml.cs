using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace text_detector_wpf
{
    public partial class MainWindow : Window
    {
        bool isFolder = false;
        ImageProcessing imageProcessing;
        BackgroundWorker worker;
        String pathFolder;
        int numberProcessed = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareChoosingPath();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image(*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PathTextBox.Text = openFileDialog.FileName;
                isFolder = false;
                Progress.Maximum = 1;
            }
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareChoosingPath();
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PathTextBox.Text = folderDialog.SelectedPath;
                isFolder = true;
                Progress.Maximum = Directory.GetFiles(PathTextBox.Text).Length;
            }
        }

        private void PrepareChoosingPath()
        {
            PathTextBox.Text = "";
            LogTextBox.Text = "";
            Progress.Value = 0;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (PathTextBox.Text != "")
            {
                CreateBackgroundWorker();

                if ((isFolder) && (StartButton.Content.ToString() == "Start"))
                {
                    pathFolder = CreateNewDirectory();

                    numberProcessed = 0;
                    Progress.Value = 0;
                    PrepareInterfaceForProcessing();

                    StartButton.Content = "Stop";
                    worker.RunWorkerAsync(PathTextBox.Text);
                }
                else if (StartButton.Content.ToString() == "Stop")
                {
                    worker.CancelAsync();
                    OpenFolderButton.IsEnabled = true;
                    OpenFileButton.IsEnabled = true;

                    StartButton.Content = "Start";
                    LogTextBox.Text += $"\nProcessed images at all: {Progress.Value + 1}" +
                                       $"\nProcess is stopped";
                }
                else
                {
                    PrepareInterfaceForProcessing();
                    numberProcessed = 1;
                    Progress.Value = 1;
                    ShowProcessing();

                    imageProcessing = new ImageProcessing(isFolder, PathTextBox.Text);
                    imageProcessing.DetectTextOnImage();
                    imageProcessing.ShowAndSaveTextDetected();
                }
            }
        }        

        private String CreateNewDirectory()
        {
            String newDirectory;

            if (Directory.Exists(PathTextBox.Text + @"\text_detected"))
            {
                int numberSameFolders = 0;
                while (Directory.Exists(PathTextBox.Text + $@"\text_detected({numberSameFolders})"))
                    numberSameFolders++;
                Directory.CreateDirectory(PathTextBox.Text + $@"\text_detected({numberSameFolders})");
                newDirectory = PathTextBox.Text + $@"\text_detected({numberSameFolders})";
            }
            else
            {
                Directory.CreateDirectory(PathTextBox.Text + $@"\text_detected");
                newDirectory = PathTextBox.Text + $@"\text_detected";
            }

            return newDirectory;
        }

        private void PrepareInterfaceForProcessing()
        {
            OpenFolderButton.IsEnabled = false;
            OpenFileButton.IsEnabled = false;
            LogTextBox.Text = "Started processing";
        }

        private void ShowProcessing()
        {
            if ((Progress.Value == 1) && (Progress.Value == Progress.Maximum))
                LogTextBox.Text += $"\nProcessed images: 1";
            else if (Progress.Value % 10 == 0)
                LogTextBox.Text += $"\nProcessed images: {Progress.Value}";

            if (Progress.Value == Progress.Maximum)
            {
                LogTextBox.Text += $"\nProcessed images at all: {Progress.Value}" +
                                   $"\nProcessing completed";
                OpenFolderButton.IsEnabled = true;
                OpenFileButton.IsEnabled = true;
                StartButton.Content = "Start";
            }
        }

        private void CreateBackgroundWorker()
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_RunDetection;
            worker.ProgressChanged += Worker_ProgressShow;
        }

        private void Worker_ProgressShow(object sender, ProgressChangedEventArgs e)
        {
            Progress.Value = Convert.ToInt32(numberProcessed);
            ShowProcessing();
        }

        private void Worker_RunDetection(object sender, DoWorkEventArgs e)
        {
            if (worker.CancellationPending) e.Cancel = true;
            else
                foreach (String imageFileName in Directory.GetFiles(e.Argument.ToString()))
                    if (worker.CancellationPending) e.Cancel = true;
                    else
                    {
                        imageProcessing = new ImageProcessing(isFolder, imageFileName, pathFolder);
                        imageProcessing.DetectTextOnImage();
                        numberProcessed++;
                        (sender as BackgroundWorker).ReportProgress(numberProcessed);
                    }
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/gu1show/east-text-detector-wpf/blob/main/README.md");
        }
    }
}
