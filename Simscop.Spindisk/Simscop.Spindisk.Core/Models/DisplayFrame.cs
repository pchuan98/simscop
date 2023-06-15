﻿using OpenCvSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;


namespace Simscop.Spindisk.Core.Models;

public class DisplayFrame
{
    public byte[] FrameObject { get; set; } = Array.Empty<byte>();

    public int Height { get; set; } = 0;

    public int Width { get; set; } = 0;

    public int Stride { get; set; } = 0;

    public byte Depth { get; set; } = 0;

    public byte Channels { get; set; } = 0;

    /// <summary>
    /// TODO 这里之后必须得优化
    /// </summary>
    /// <param name="source"></param>
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

    // 将 Frame 对象转换为 BitmapImage 对象


    public void Test(out BitmapSource source)
    {
        int type = MatType.MakeType(Depth, Channels);

        Mat mat = new Mat(Height, Width, type, FrameObject);

        Mat hsv = new Mat();
        Cv2.CvtColor(mat, hsv, ColorConversionCodes.BGR2HSV);

        // 选择绿色范围内的像素
        Mat mask = new Mat();
        Cv2.InRange(hsv, new Scalar(36, 25, 25), new Scalar(86, 255, 255), mask);

        Mat result = new Mat();
        Cv2.BitwiseAnd(mat, mat, result, mask);

        source = mat.ToBitmapSource();
    }


}
