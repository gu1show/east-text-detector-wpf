# east-text-detector-wpf
This is an application in WPF form that detects text in images. It was created as part of a term paper. If a folder is selected, each image in it will be processed and the result will be saved in a folder inside the selected folder.

## Requirements
.NET Framework 4.8

Use NuGet for installation:

OpenCvSharp4.Windows

You also need to download the neural network from the link, unzip it and paste the path to the resulting file in TextDetector.cs: https://www.dropbox.com/s/r2ingd0l3zt8hxs/frozen_east_text_detection.tar.gz?dl=1.
