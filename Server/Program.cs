using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Text;

var listener = new Socket(AddressFamily.InterNetwork,
                          SocketType.Dgram,
                          ProtocolType.Udp);
var buffer = Array.Empty<byte>();
var ip = IPAddress.Parse("127.0.0.1");
var port = 45678;
var listenerEP = new IPEndPoint(ip, port);

listener.Bind(listenerEP);

EndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.1.1.1"), 12345);

MemoryStream stream = new MemoryStream();
Bitmap bitmap = GetDesktopImage(stream);//generate screenshot.

Bitmap GetDesktopImage(MemoryStream stream)
{
    Bitmap bitmap = new Bitmap(224, 450);

    Graphics graphics = Graphics.FromImage(bitmap as System.Drawing.Image);
    graphics.CopyFromScreen(25, 25, 25, 25, bitmap.Size);

    bitmap.Save(stream, ImageFormat.Jpeg);

    return bitmap;
}

byte[] sendbuf = stream.GetBuffer();

var count = 0;
var msg = string.Empty;

EndPoint clientEP = new IPEndPoint(IPAddress.Parse("127.1.1.1"), 12345);


while (true)
{
    var result = await listener.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, remoteEP);

    await listener.SendToAsync(new ArraySegment<byte>(sendbuf), SocketFlags.None, clientEP);
}

