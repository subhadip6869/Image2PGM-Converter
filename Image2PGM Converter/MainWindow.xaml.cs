using System.Windows;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System;
using System.Threading;

namespace Image2PGM_Converter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int width, height;
        Bitmap bmp, newBmp;
        string path;
        public MainWindow()
        {
            InitializeComponent();
            imageView.Source = new BitmapImage(new Uri(@"Resources/no_preview.jpg", UriKind.Relative));
        }

        private void browseBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Filter = "Image Files (.jpg, .jpeg, .png, .bmp) | *.jpg; *.png; *.jpeg; *.bmp";
            ofd.Title = "Browse Images";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == true)
            {
                fileNameField.Text = ofd.FileName;
                bmp = new Bitmap(ofd.FileName);
                setImageView(bmp);
                slider.IsEnabled = true;
                saveBtn.IsEnabled = true;
                resizeImage();
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            resizeImage();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Portable Gray Map | *.pgm";
            saveFileDialog.Title = "Save to";
            saveFileDialog.DefaultExt = "pgm";
            if (saveFileDialog.ShowDialog() == true)
            {
                path = saveFileDialog.FileName;
                Thread t = new Thread(() =>
                {
                    save_image(path);
                    Action action = () => status.Text = "Saved to: " + path;
                    Application.Current.Dispatcher.BeginInvoke(action);
                    
                });
                t.Start();
                status.Text = "Saving file...";
            }
        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            bmp.Dispose();
            //newBmp.Dispose();
            fileNameField.Clear();
            scaleText.Text = "Image Quality: " + slider.Value.ToString("F" + "2") + "% (" + 0 + "x" + 0 + ")";
            saveBtn.IsEnabled = false;
            slider.IsEnabled = false;
            imageView.Source = new BitmapImage(new Uri(@"Resources/no_preview.jpg", UriKind.Relative));
            path = "";
            status.Text = "";
        }

        private void resizeImage()
        {
            try
            {
                width = (int)(bmp.Width * (slider.Value / 100));
                height = (int)(bmp.Height * (slider.Value / 100));
                newBmp = new Bitmap(bmp, new System.Drawing.Size(width, height));
                setImageView(newBmp);
            }
            catch (System.NullReferenceException)
            {
                scaleText.Text = "Image Quality: " + slider.Value.ToString("F" + "2") + "% (" + 0 + "x" + 0 + ")";
            }
        }

        private void setImageView(Bitmap bit)
        {
            BitmapImage image = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                bit.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                image.BeginInit();
                image.StreamSource = memory;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
            }
            imageView.Source = image;
            scaleText.Text = "Image Quality: " + slider.Value.ToString("F" + "2") + "% (" + bit.Width + "x" + bit.Height + ")";
        }

        void save_image(object path)
        {
            StreamWriter streamWriter = new StreamWriter(path.ToString());
            streamWriter.WriteLine("P2");
            streamWriter.WriteLine("# Created by Image2PGM Converter; © Subhadip Dutta");
            streamWriter.WriteLine(newBmp.Width + " " + newBmp.Height);
            streamWriter.WriteLine("255");
            for (int y = 0; y < newBmp.Height; y++)
            {
                for (int x = 0; x < newBmp.Width; x++)
                {
                    Color pixel = newBmp.GetPixel(x, y);
                    int gray = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
                    streamWriter.WriteLine(gray.ToString());
                }
            }
            streamWriter.Close();
        }
    }
}
