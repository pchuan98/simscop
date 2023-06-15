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
                m.Test(out var bitmap);
                ImageFirst = bitmap;
            });
        });
    }
}