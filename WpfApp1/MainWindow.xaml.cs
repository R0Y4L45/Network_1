using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using System.Drawing;
using Image = System.Drawing.Image;
using System.Printing.IndexedProperties;
using System.Threading;
using System.Collections;
using System.IO;
using System.Security.Cryptography;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var ip = IPAddress.Parse("127.0.0.1");
            var port = 45678;

            var server = new IPEndPoint(ip, port);

            var clientEP = new IPEndPoint(IPAddress.Parse("127.1.1.1"), 12);

            var cilent = new UdpClient(clientEP);

            byte[] buffer1 = new byte[100000];

            var msg = string.Empty;
            var buffer = Array.Empty<byte>();

            buffer = Encoding.Default.GetBytes("hello");
            cilent.Send(buffer, buffer.Length, server);

            var size = cilent.Receive(ref server);
            int si = int.Parse(Encoding.Default.GetString(size));
            var c = cilent.Receive(ref server);
            int num = int.Parse(Encoding.Default.GetString(c));
            byte[] arr = new byte[si];

            List<byte[]> bytes = new List<byte[]>();
            int len = 0;

            for (int i = 0; i < num; i++)
            {
                var r = cilent.Receive(ref server);
                len += r.Length;
                bytes.Add(r);
            }

            int a = 0;
            foreach (var item in bytes)
            {
                for (int i = 0; i < item.Length; i++)
                {
                    arr[a++] = item[i];
                }
            }
            //8294400
            try
            {
                //using (MD5 md5 = MD5.Create())
                //{
                //    byte[] hash = md5.ComputeHash(arr);
                //    string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();
                //    MessageBox.Show("Image hash value: " + hashString);
                //}

                MessageBox.Show(
                    +'\n' + arr[65506].ToString()
                    + '\n' + arr[65507].ToString()
                    + '\n' + arr[8294399].ToString()
                    + '\n' + arr[8294398].ToString()
                    + '\n' + arr[8294397].ToString() + '\n' + "Len : " + len.ToString());

                BitmapImage bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(arr);
                bitmap.EndInit();

                MessageBox.Show(bitmap.Width.ToString() + ' ' + bitmap.Height.ToString());
                img.Source = bitmap;

            }
            catch (ArgumentException)
            {
                // The byte array does not contain valid image data
            }


        }
    }
}
