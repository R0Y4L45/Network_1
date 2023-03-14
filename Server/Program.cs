using Server;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;

Size size;
byte[] sendbuf;
byte[] buffer = new byte[1024];

Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
IPAddress ip = IPAddress.Parse("127.0.0.1");
int port = 45678;
IPEndPoint listenerEP = new IPEndPoint(ip, port);
listener.Bind(listenerEP);

EndPoint clientEP = new IPEndPoint(IPAddress.Parse("127.1.1.1"), 12);

while (true)
{
    SocketReceiveFromResult result = await listener.ReceiveFromAsync(buffer, SocketFlags.None, clientEP);

    if (result.ReceivedBytes > 0)
    {
        Console.Clear();
        size = ScreenResolution.GetScreenResolution();
        sendbuf = ScreenShot.ScreenShotMethod(size).ToArray();
        //OZUM BILEREKDEN CHUNK METHODU ISTIFADE ETMEDIM
        if (sendbuf.Length > 65506)
        {
            int whole_num = (int)(sendbuf.Length / 65506), resudial = sendbuf.Length % 65506, totalLenght = 0;
            whole_num++;

            await listener.SendToAsync(Encoding.Default.GetBytes(sendbuf.Length.ToString()), SocketFlags.None, clientEP);
            await listener.SendToAsync(Encoding.Default.GetBytes(whole_num.ToString()), SocketFlags.None, clientEP);

            Thread.Sleep(1000);
            for (int i = 0; i < whole_num; i++)
            {
                if (i + 1 != whole_num)
                    await listener.SendToAsync(new ArraySegment<byte>(sendbuf, totalLenght, 65506), SocketFlags.None, clientEP);
                else
                    await listener.SendToAsync(new ArraySegment<byte>(sendbuf, totalLenght, resudial), SocketFlags.None, clientEP);

                totalLenght += 65506;

                Console.WriteLine("Loading...");
                Thread.Sleep(10);
            }
            Console.Clear();
            Console.WriteLine("Operation completed....)");
        }
        else
        {
            await listener.SendToAsync(Encoding.Default.GetBytes(sendbuf.Length.ToString()), SocketFlags.None, clientEP);
            await listener.SendToAsync(Encoding.Default.GetBytes(1.ToString()), SocketFlags.None, clientEP);
            Thread.Sleep(1000);
            await listener.SendToAsync(sendbuf, SocketFlags.None, clientEP);
            Console.Clear();
            Console.WriteLine("Operation completed....)");
        }
    }
    else
    {
        Console.WriteLine("Doesn't receive any info from client...(");
        break;
    }
}
