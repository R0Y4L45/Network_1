using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

static Size GetScreenResolution()
{
    var hdc = GetDC(IntPtr.Zero);
    var width = GetDeviceCaps(hdc, 8 /* HORZRES */);
    var height = GetDeviceCaps(hdc, 10 /* VERTRES */);
    ReleaseDC(IntPtr.Zero, hdc);
    return new Size(width, height);
}

[DllImport("user32.dll")]
static extern IntPtr GetDC(IntPtr hWnd);

[DllImport("user32.dll")]
static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

[DllImport("gdi32.dll")]
static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

var size = GetScreenResolution();

using Image bitmap = new Bitmap(size.Width, size.Height);
using (var g = Graphics.FromImage(bitmap))
{
    g.CopyFromScreen(0, 0, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
}
bitmap.Save("filename.png", ImageFormat.Png);


byte[] BitmapToByteArray(Bitmap bitmap)
{
    // Lock the bitmap's bits
    var bitmapData = bitmap.LockBits(
        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        ImageLockMode.ReadOnly,
        bitmap.PixelFormat);

    try
    {
        // Allocate a byte array to hold the bitmap data
        var byteArray = new byte[bitmapData.Stride * bitmapData.Height];

        // Copy the bitmap data to the byte array
        Marshal.Copy(bitmapData.Scan0, byteArray, 0, byteArray.Length);

        return byteArray;
    }
    finally
    {
        // Unlock the bitmap's bits
        bitmap.UnlockBits(bitmapData);
    }
}

byte[] sendbuf = BitmapToByteArray((Bitmap)bitmap);

EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);

var listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
var buffer = new byte[1024];
var ip = IPAddress.Parse("127.0.0.1");
var port = 45678;
var listenerEP = new IPEndPoint(ip, port);

listener.Bind(listenerEP);

EndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.1.1.1"), 12);

Console.WriteLine(
                    +'\n' + sendbuf[65506].ToString()
                    + '\n' + sendbuf[65507].ToString()
                    + '\n' + sendbuf[8294399].ToString()
                    + '\n' + sendbuf[8294398].ToString()
                    + '\n' + sendbuf[8294397].ToString());

var result = await listener.ReceiveFromAsync(buffer, SocketFlags.None, clientEP);
if (result.ReceivedBytes > 0)
{
    Console.WriteLine("Bitti");
    if (sendbuf.Length > 65506)
    {
        var s = (int)(sendbuf.Length / 65506);
        var mode = sendbuf.Length % 65506;
        listener.SendTo(Encoding.Default.GetBytes(sendbuf.Length.ToString()), SocketFlags.None, remoteEP);
        if (mode > 0)
        {
            s++;
            listener.SendTo(Encoding.Default.GetBytes(s.ToString()), SocketFlags.None, remoteEP);
            int a = 0;
            Thread.Sleep(2000);
            for (int i = 0; i < s; i++)
            {
                if (i != s - 1)
                {
                    listener.SendTo(new ArraySegment<byte>(sendbuf, a, 65506), SocketFlags.None, remoteEP);

                }
                else
                {
                    listener.SendTo(new ArraySegment<byte>(sendbuf, a, mode), SocketFlags.None, remoteEP);
                    a += mode;
                }

                a += 65506;

                Thread.Sleep(10);
            }
        }

        Console.WriteLine("End....");
        Console.ReadLine();
    }
}


