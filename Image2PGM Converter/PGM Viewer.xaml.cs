using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Image2PGM_Converter
{
    /// <summary>
    /// Interaction logic for PGM_Viewer.xaml
    /// </summary>
    public partial class PGM_Viewer : Window
    {
        public PGM_Viewer()
        {
            InitializeComponent();
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void browseBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.CheckFileExists = true;
            fd.CheckPathExists = true;
            fd.DefaultExt = "pgm";
            fd.Filter = "PGM Images | *.pgm";
            fd.Multiselect = false;
            fd.Title = "Browse PGM files";
            if (fd.ShowDialog() == true)
            {
                string path = fd.FileName;
                ReadImage rm = new ReadImage(path);
                Bitmap bmp = new Bitmap(rm.getRow(), rm.getCol());
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0;  j < bmp.Height; j++)
                    {
                        bmp.SetPixel(i, j, Color.FromArgb(rm.getPixels()[i, j], rm.getPixels()[i, j], rm.getPixels()[i, j]));
                    }
                }
                bmp.RotateFlip(RotateFlipType.Rotate90FlipX);
                BitmapImage image = new BitmapImage();
                using (MemoryStream memory = new MemoryStream())
                {
                    bmp.Save(memory, ImageFormat.Bmp);
                    memory.Position = 0;
                    image.BeginInit();
                    image.StreamSource = memory;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                }
                imageView.Source = image;
            }
        }
    }

    class ReadImage
    {
        private int row, col, im_max;
        private int[,] pixels;
        private string im_label, im_comment, im_dim;
        public ReadImage(string file)
        {
            StreamReader sr = new StreamReader(file);
            im_label = sr.ReadLine();
            im_comment = sr.ReadLine();
            im_dim = sr.ReadLine();
            im_max = Convert.ToInt32(sr.ReadLine());
            row = Convert.ToInt32(im_dim.Split(' ')[1]);
            col = Convert.ToInt32(im_dim.Split(' ')[0]);
            pixels = new int[row, col];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    pixels[i, j] = Convert.ToInt32(sr.ReadLine());
                }
            }
        }
        public int[,] getPixels() { return pixels; }
        public int getRow() { return row; }
        public int getCol() { return col; }
        public string getLabel() { return im_label; }
        public string getComment() { return im_comment; }
        public int getMaxIntensity() { return im_max; }
    }
}
