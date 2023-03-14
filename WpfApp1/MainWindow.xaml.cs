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
using System.Numerics;

namespace Client;

public partial class MainWindow : Window
{
    IPEndPoint? clientEP = null;
    IPEndPoint? server = null;
    UdpClient? client = null;
    public MainWindow()
    {
        InitializeComponent();

        clientEP = new IPEndPoint(IPAddress.Parse("127.1.1.1"), 12);
        server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 45678);
        client = new UdpClient(clientEP);
    }

    private async void btn_Click(object sender, RoutedEventArgs e)
    {
        var pulse = Encoding.Default.GetBytes("hello");

        if(client != null && server != null)
        {
            await client.SendAsync(pulse, pulse.Length, server);

            try
            {
                btn.IsEnabled = false;

                UdpReceiveResult rec_lenght = await client.ReceiveAsync();
                UdpReceiveResult rec_wholeNum = await client.ReceiveAsync();

                byte[] arr = await Task.Run<byte[]>(async () =>
                {
                    int lenght = int.Parse(Encoding.Default.GetString(rec_lenght.Buffer)),
                        wholeNum = int.Parse(Encoding.Default.GetString(rec_wholeNum.Buffer)),
                        len = 0, a = 0;

                    byte[] arr = new byte[lenght];
                    List<byte[]> bytes = new List<byte[]>();

                    for (int i = 0; i < wholeNum; i++)
                    {
                        UdpReceiveResult rec_Bytes = await client.ReceiveAsync();
                        len += rec_Bytes.Buffer.Length;
                        bytes.Add(rec_Bytes.Buffer);
                    }

                    foreach (var item in bytes)
                        for (int i = 0; i < item.Length; i++)
                            arr[a++] = item[i];
                    return arr;
                });
                try
                {
                    BitmapImage bitmap = new BitmapImage();

                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(arr);
                    bitmap.EndInit();

                    img.Source = bitmap;

                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btn.IsEnabled = true;
            }

        }
    }
}
