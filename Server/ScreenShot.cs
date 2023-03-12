using System.Drawing.Imaging;
using System.Drawing;

namespace Server;

internal class ScreenShot
{
    internal static MemoryStream ScreenShotMethod(Size size)
    {
        MemoryStream stream = new MemoryStream();

        using Image bitmap = new Bitmap(size.Width, size.Height);
        
        using (var g = Graphics.FromImage(bitmap))
        {
            g.CopyFromScreen(0, 0, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
        }

        bitmap.Save(stream, ImageFormat.Png);

        return stream;
    }
}
