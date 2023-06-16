﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Window = System.Windows.Window;

namespace Simscop.Lib.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }


        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            //var img = OpenCvSharp.Cv2.ImRead(@"F:\Code\Company\test\2.tif");
            var img = OpenCvSharp.Cv2.ImRead(@"F:\Code\Company\test\0000.png");

            // 将输入图像转换为灰度图像
            Mat gray = new Mat();
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);

            Mat enhance = new Mat();

            Mat customColorMap = new Mat(256, 1, MatType.CV_8UC3);
            for (int i = 0; i < 256; i++)
            {
                customColorMap.Set(i, 0, new Vec3b((byte)0, (byte)i, (byte)0));
            }

            Cv2.EqualizeHist(gray, enhance);

            Mat single = new Mat(1, 256, MatType.CV_8UC1, new Scalar(0,255,0));


            var dst = new Mat();

            Cv2.ApplyColorMap(enhance, dst, customColorMap);
            //Cv2.LUT(gray,single,dst);


            ImageShower1.Source = gray.ToBitmapSource();
            ImageShower2.Source = enhance.ToBitmapSource();
        }
    }
}
