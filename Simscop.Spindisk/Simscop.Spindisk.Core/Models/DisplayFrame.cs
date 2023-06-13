using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Simscop.Spindisk.Core.Models;

public class DisplayFrame
{
    public byte[] FrameObject { get; set; } = Array.Empty<byte>();

    public int Height { get; set; } = 0;

    public int Width { get; set; } = 0;

    public int Stride { get; set; } = 0;

    public void ToBitmapSource(out BitmapImage source)
    {
        GCHandle handle = GCHandle.Alloc(FrameObject, GCHandleType.Pinned);
        long scan = (long)handle.AddrOfPinnedObject();
        scan += (Height - 1) * Stride;

        Bitmap bitmap = new Bitmap(Width, Height, -Stride,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb, (IntPtr)scan);

        using (MemoryStream stream = new MemoryStream())
        {
            bitmap.Save(stream, ImageFormat.Bmp);

            stream.Position = 0;

            source = new();

            source.BeginInit();
            // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
            // Force the bitmap to load right now so we can dispose the stream.
            source.CacheOption = BitmapCacheOption.OnLoad;
            source.StreamSource = stream;
            source.EndInit();
            source.Freeze();
        }

        handle.Free();
    }
}