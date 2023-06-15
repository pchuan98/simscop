using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using OpenCvSharp;
using Simscop.Spindisk.Core.Models;


namespace Simscop.Spindisk.Core.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private ImageSource? _imageFirst;

    //private WriteableBitmap _wbBitmap;

    public ShellViewModel()
    {
        WeakReferenceMessenger.Default.Register<DisplayFrame, string>(this, "Display", (s, m) =>
        {
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                m.ToBitmapSource(out var bitmap);
                ImageFirst = bitmap;
            });
        });


        // TODO 这里之后单独抽出来，之后所有的关于图片的转换，处理，或者其他功能都应该由某个单独的对象综合处理，因为不同的viewmodel都会对
        // TODO 实际的Image对象进行操作


        //WeakReferenceMessenger.Default.Register<DisplayFrame, string>(this, "ResetSize", (s, m) => {
        //    System.Windows.Application.Current.Dispatcher.Invoke(() => ResetBitmap(m));
        //});
    }

    // NOTE 这里Stride这个东西，需要之后认真调研一下，上面的代码终究还是太繁琐了

    private void ShowImageOld2(DisplayFrame frame)
    {
        //int width = frame.Width;
        //int height = frame.Height;
        //int stride = frame.Stride;

        //// https://blog.csdn.net/weixin_45824717/article/details/115579107
        //// https://zhuanlan.zhihu.com/p/387944669

        //Bitmap bitmap = new Bitmap(width, height, -stride,
        //    stem.Drawing.Imaging.PixelFormat.Format24bppRgb, (IntPtr)scan);


    }
}