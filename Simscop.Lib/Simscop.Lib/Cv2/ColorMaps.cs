using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Simscop.Lib.Cv2;

public static class ColorMaps
{
    static ColorMaps()
    {
        for (int i = 0; i < 256; i++)
        {
            Green.Set(i, 0, new Vec3b((byte)0, (byte)i, (byte)0));
        }
    }

    public static Mat Green => new(256, 1, MatType.CV_8UC3);
    
}