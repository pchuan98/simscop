//http://shimat.github.io/opencvsharp/api/index.html

using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OpenCvSharp.Extensions;

namespace Simscop.Lib.cv;


public static class Converter
{
    // TODO 这里添加公共属性




    //public static void ToBitmapImage(out ,out BitmapImage image)
    //{

    //    // 将 Mat 对象转换为 Bitmap 对象
    //    Bitmap bitmap = mat.ToBitmap();



    //    // 将 Bitmap 对象转换为 BitmapImage 对象
    //    MemoryStream memoryStream = new MemoryStream();
    //    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
    //    memoryStream.Position = 0;
    //    image = new BitmapImage();
    //    image.BeginInit();
    //    image.CacheOption = BitmapCacheOption.OnLoad;
    //    image.StreamSource = memoryStream;
    //    image.EndInit();

    //}

    public static bool FromBytes(byte[] frame, out Mat mat)
    {
        try
        {
            mat = Cv2.ImDecode(frame, ImreadModes.Grayscale);
            return true;
        }
        catch (Exception e)
        {
            throw new Exception("From Bytes", e);
        }

    }

    public static bool FromBytes(byte[] frame,out BitmapImage? image)
    {

        image = null;
        if (!FromBytes(frame, out Mat mat)) return false;

        try
        {
            var bitmap = mat.ToBitmap();
            var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
            memoryStream.Position = 0;
            image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = memoryStream;
            image.EndInit();

            return true;
        }
        catch (Exception e)
        {
            throw new Exception("FromBytes", e);
        }
    }
}