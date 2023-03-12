using System.Drawing;
using System.Runtime.InteropServices;

namespace Server;

internal class ScreenResolution
{
    [DllImport("user32.dll")]
    static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
    internal static Size GetScreenResolution()
    {
        var hdc = GetDC(IntPtr.Zero);
        var width = GetDeviceCaps(hdc, 8 /* HORZRES */);
        var height = GetDeviceCaps(hdc, 10 /* VERTRES */);
        ReleaseDC(IntPtr.Zero, hdc);

        return new Size(width, height);
    }
}
