using Microsoft.Win32;
using OpenCvSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace text_detector_wpf
{
    internal class ImageProcessing
    {
        private TextDetector detector;       
        private Mat image;
        private readonly String path;
        private readonly String directoryToSaveTextDetected;
        private readonly bool isFolder;

        public ImageProcessing(bool isFolder, String path)
        {
            this.isFolder = isFolder;
            this.path = path;
        }

        public ImageProcessing(bool isFolder, String path, String directoryToSaveTextDetected)
        {
            this.isFolder = isFolder;
            this.path = path;
            this.directoryToSaveTextDetected = directoryToSaveTextDetected;
        }

        public void DetectTextOnImage()
        {
            if (isFolder)
            {
                GetImageWithTextDetected(path);
                image.SaveImage(directoryToSaveTextDetected + "\\" +
                                Path.GetFileNameWithoutExtension(path) +
                                "_text_detected" + Path.GetExtension(path));
            }
            else GetImageWithTextDetected(path);
        }

        public void ShowAndSaveTextDetected()
        {
            String pathToTextDetected = Path.GetFileNameWithoutExtension(path) +
                                        "_text_detected" + Path.GetExtension(path);

            image.SaveImage(pathToTextDetected);
            Process.Start(pathToTextDetected);
            SaveImageWithDialog();

            File.Delete(pathToTextDetected);
        }


        private void GetImageWithTextDetected(String pathToImage)
        {
            detector = new TextDetector(new Mat(pathToImage));
            detector.DetectTextOnImage();
            image = detector.ReturnImage();
        }

        private void SaveImageWithDialog()
        {
            MessageBoxResult dialogResult =
                MessageBox.Show("Do you want to save image?", "Saving",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (dialogResult == MessageBoxResult.Yes)
            {
                SaveFileDialog saveImage = new SaveFileDialog();
                saveImage.Filter = "Image(*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";

                if (saveImage.ShowDialog() == true)
                    image.SaveImage(saveImage.FileName);
            }
        }
    }
}
