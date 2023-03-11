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
            var cilent = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            var ip = IPAddress.Parse("127.0.0.1");
            var port = 45678;

            EndPoint serverEP = new IPEndPoint(ip, port);

            var clientEndPoint = new IPEndPoint(IPAddress.Parse("127.1.1.1"), 12345);

            cilent.Bind(clientEndPoint);

            byte[] buffer1 = new byte[1024];

            var msg = string.Empty;
            var buffer = Array.Empty<byte>();

            while (true)
            {
                buffer = Encoding.Default.GetBytes(msg ?? "0");
                cilent.SendTo(buffer, clientEndPoint);

                var result = cilent.ReceiveFromAsync(buffer1, SocketFlags.None, serverEP);

                if(result.IsCompleted)
                {
                    Bitmap b = new Bitmap(224, 450);
                    ImageConverter ic = new ImageConverter();
                    Image img = (Image)ic.ConvertFrom(buffer1)!;
                    picture.DataContext = img;
                }
            }
        }
    }
}
