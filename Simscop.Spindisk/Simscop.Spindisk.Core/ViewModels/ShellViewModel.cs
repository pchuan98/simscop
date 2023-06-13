using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Simscop.API.Native;
using Simscop.Spindisk.Core.Models;

namespace Simscop.Spindisk.Core.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private ImageSource _imageFirst;

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

        //WeakReferenceMessenger.Default.Register<DisplayFrame, string>(this, "ResetSize", (s, m) => {
        //    System.Windows.Application.Current.Dispatcher.Invoke(() => ResetBitmap(m));
        //});
    }

    //private void ResetBitmap(DisplayFrame frame)
    //{
    //    _wbBitmap = new WriteableBitmap(frame.Width, frame.Height, 96, 96, PixelFormats.Rgb24, null);
    //    ImageFirst = _wbBitmap;
    //}

    //// NOTE 这个方法暂时搁浅，因为涉及到了一些目前位置的数据格式转换，无法很好的完成Bitmap转换工作
    //private void ShowImage(DisplayFrame frame)
    //{
    //    unsafe
    //    {

    //    }

    //    _wbBitmap.Lock();
    //    Marshal.Copy(frame.FrameObject, 0, _wbBitmap.BackBuffer, frame.Height * frame.Width);
    //    _wbBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, frame.Width, frame.Height));
    //    _wbBitmap.Unlock();
    //}



    //private void ShowImageOld1(DisplayFrame frame)
    //{
    //    int width = frame.Width;
    //    int height = frame.Height;
    //    int stride = frame.Stride;

    //    GCHandle handle = GCHandle.Alloc(frame.FrameObject, GCHandleType.Pinned);
    //    long scan = (long)handle.AddrOfPinnedObject();
    //    scan += (height - 1) * stride;

    //    Bitmap bitmap = new Bitmap(width, height, -stride,
    //        System.Drawing.Imaging.PixelFormat.Format24bppRgb, (IntPtr)scan);

    //    ImageFirst = ToWpfBitmap(bitmap);

    //    handle.Free();

    //    //var show = bitmap.Clone(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
    //    //    bitmap.PixelFormat);


    //}


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