using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using DPUruNet;
using System.Runtime.InteropServices;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<int[][]> images = new List<int[][]>();
        private bool reset;
        public Reader reader;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileOpen(object sender, RoutedEventArgs e)
        {
        
        OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select Image";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
        "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                Bitmap act = (Bitmap)System.Drawing.Image.FromFile(op.FileName);
                actual.Source = ToSource(act);
              //  images.Add();
            }
        }

        public BitmapImage ToSource(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private Bitmap ToBitmap(int[,,] act)
        {
            Bitmap newer = new Bitmap(act.GetUpperBound(0) + 1, act.GetUpperBound(1) + 1);
            for (int x = 0; x < act.GetUpperBound(0) + 1; x++)
            {
                for (int y = 0; y < act.GetUpperBound(1) + 1; y++)
                {
                    System.Drawing.Color meh = System.Drawing.Color.FromArgb(act[x, y, 0], act[x, y, 1], act[x, y, 2]);
                    newer.SetPixel(x, y, meh);
                }
            }
            return newer;
        }

        private int[,,] To3Map(Bitmap act)
        {
            int[,,] tab = new int[act.Width, act.Height, 3];
            for (int x = 0; x < act.Width; x++)
            {
                for (int y = 0; y < act.Height; y++)
                {
                    tab[x, y, 0] = act.GetPixel(x, y).R;
                    tab[x, y, 1] = act.GetPixel(x, y).G;
                    tab[x, y, 2] = act.GetPixel(x, y).B;
                }
            }
            return tab;
        }

        private void Driver(object sender, RoutedEventArgs e)
        {
            ReaderCollection rc = ReaderCollection.GetReaders();

            if (rc != null)
            {
                foreach (Reader r in rc)
                {
                    Console.WriteLine(r.Description.Name);
                }

                reader = rc[0];

                Console.WriteLine(reader.Description.Name);
            }

        }

        Constants.ResultCode captureResult;

        private void captureBtnClickEvent(object sender, RoutedEventArgs e)
        {
            if (reader == null) return;

            reader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
            reader.On_Captured += new Reader.CaptureCallback(OnCaptured);
            captureResult = reader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, reader.Capabilities.Resolutions[0]);

        }

        public void OnCaptured(CaptureResult captureResult)
        {
            Console.WriteLine(captureResult.ToString());
            
            Fid.Fiv f = captureResult.Data.Views[0];
            int height = f.Height;
            int width = f.Width;
            Console.WriteLine(height * width);
            byte[] bytes = f.RawImage;

            byte[] rgbBytes = new byte[bytes.Length * 3];

            for (int i = 0; i <= bytes.Length - 1; i++)
            {
                rgbBytes[(i * 3)] = bytes[i];
                rgbBytes[(i * 3) + 1] = bytes[i];
                rgbBytes[(i * 3) + 2] = bytes[i];
            }
            Console.WriteLine(bytes.Length);
            Console.WriteLine("Bytes = " + bytes.Length);

            Bitmap final = new Bitmap(width, height);

            for (int hi = 0; hi < height; hi++)
            {
                for (int wi = 0; wi < width; wi = wi + 3)
                {
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(bytes[hi * width + wi], bytes[hi * width + wi + 1], bytes[hi * width + wi + 2]);
                    Console.WriteLine(hi * width * 3 + wi);
                    Console.WriteLine(hi);
                    Console.WriteLine(wi);
                    Console.WriteLine();
                    //System.Drawing.Color color = SystemDrawing.Color.FromArgb(bytes[width * wi + hi], bytes[(width * wi + hi) + 1], bytes[(width * wi + hi) + 2]);
                    final.SetPixel(wi / 3, hi, color);
                }
            

        }


            actual.Source = ToSource(final);

        }

        private void Machine(object sender, RoutedEventArgs e)
        {
            int width = 5;
            int height = 17;
            byte[] bytes = new byte[255];
            Bitmap final = new Bitmap(width, height);
            for (int i = 255; i > 0; i--)
            {
                bytes[i-1] = Convert.ToByte(255-i);
            }

            
            for (int hi = 0; hi < height; hi++)
            {
                for (int wi = 0; wi < width*3; wi = wi + 3)
                {
                  System.Drawing.Color color = System.Drawing.Color.FromArgb(bytes[hi * width*3 + wi], bytes[hi * width * 3 + wi + 1], bytes[hi * width * 3 + wi + 2]);
                  Console.WriteLine(hi * width*3 + wi);
                    Console.WriteLine(hi);
                    Console.WriteLine(wi);
                    Console.WriteLine();
                    //System.Drawing.Color color = System.Drawing.Color.FromArgb(bytes[width * wi + hi], bytes[(width * wi + hi) + 1], bytes[(width * wi + hi) + 2]);
                   final.SetPixel(wi/3, hi, color);
                }
            }


            actual.Source = ToSource(final);

            byte[] meh = { 0, 63, 127, 255 };

            //obrazek.Source = LoadImage(meh);
        }

        private void SaveImage(object sender, RoutedEventArgs e)
        {

        }

        private void ExitProgram(object sender, RoutedEventArgs e)
        {

        }

        private void DilationFilter(object sender, RoutedEventArgs e)
        {

        }

        private void ErodeFilter(object sender, RoutedEventArgs e)
        {

        }

        private void OpeningFilter(object sender, RoutedEventArgs e)
        {

        }

        private void ClosingFilter(object sender, RoutedEventArgs e)
        {

        }
    }
}
