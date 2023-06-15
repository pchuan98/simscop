// https://blog.walterlv.com/post/wpf-high-performance-bitmap-rendering.html


using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Simscop.Spindisk.Core.Models;


namespace Simscop.Spindisk.Core.ViewModels;

//public partial class ShellViewModel : ObservableObject
//{
//    [ObservableProperty]
//    private ImageSource? _imageFirst;

//    //private WriteableBitmap _wbBitmap;

//    public ShellViewModel()
//    {
//        WeakReferenceMessenger.Default.Register<DisplayFrame, string>(this, "Display", (s, m) =>
//        {
//            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
//            {
//                m.Test(out var bitmap);
//                ImageFirst = bitmap;
//            });
//        });
//    }
//}

// TODO 测试使用位图渲染的方式
public partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private ImageSource? _imageFirst;

    WriteableBitmap _writeable
        = new WriteableBitmap(0, 0, 96, 96, PixelFormats.Bgr24, null);

    private int _width = 0;
    private int _height = 0;

    private int length = 0;

    private int stride = 0;
    private int offset = 0;

    public ShellViewModel()
    {
        WeakReferenceMessenger.Default.Register<DisplayFrame, string>(this, "Display", (s, m) =>
        {
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {

                if (Math.Abs(_height - m.Height) > 0.01 || Math.Abs(_width - m.Width) > 0.01)
                {
                    _writeable = new WriteableBitmap(_width, _height, 96, 96, PixelFormats.Bgr24, null);
                    _imageFirst = _writeable;

                    _width = m.Width;
                    _height = m.Height;

                    length = _width * _height;

                    stride = _width * _height * 3;
                    offset = _width * 3;
                }

                try
                {
                    _writeable.Lock();

                    Marshal.Copy(m.FrameObject, 0, _writeable.BackBuffer, length);
                    _writeable.AddDirtyRect(new Int32Rect(0,0,_width,_height));

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
                finally
                {
                    _writeable.Unlock();

                }


            });
        });
    }
}