using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Simscop.Spindisk.Core.Models;
using Simscop.API;

using DhyanaObject = Simscop.API.Dhyana;
using Simscop.API.Native;

using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using System.Windows.Threading;

namespace Simscop.Spindisk.Core.ViewModels;

/**
 * NOTE
 * 
 * SDk Initialize Pass
 * Camera Connect Pass
 * Capture Start and Stop Pass
 * Exposure max/min/set/get
 *
 * DispatcherTimer DispatcherPriority
 *
 * BUG
 * 
 * 1. ROI设置有问题
 * 2. 曝光设置有问题
 *
 *
 * FIX
 * ROI 和 曝光问题已经解决，原因在于对API的set和get命令的时间反应不同
 * 
 * 
 */


/**
 * 常量参数 TODO
 * 
 * 曝光的最大，最小和步进 完成
 * 将Capture拆开
 * SetBrightness这个参数不太确作用
 *
 * 
 */

public partial class CameraViewModel : ObservableObject
{
    private const double DefaultFrameInterval = 200;

    static double GlobalTimerPeriod { get; set; } = DefaultFrameInterval + 5;

    public CameraViewModel()
    {
        GenerateTimer();

        // TODO 这里等会要删除了啊
        //DhyanaObject.InitializeSdk();
        //DhyanaObject.InitializeCamera(0);

        InitializeValue();
    }

    ~CameraViewModel()
    {
        DhyanaObject.UnInitializeCamera();
        DhyanaObject.UninitializeSdk();
    }




    /// <summary>
    /// 获取frame的定时器
    /// </summary>
    private readonly DispatcherTimer _frameTimer = new()
    {
        Interval = TimeSpan.FromMilliseconds(GlobalTimerPeriod),
    };

    void GenerateTimer()
    {
        _frameTimer.Tick += (s, m) =>
        {
            Task.Run(() =>
            {
                var display = new DisplayFrame();
                DhyanaObject.GetCurrentFrame((int)(GlobalTimerPeriod));
                if (Frame2Bytes(ref display, DhyanaObject.CurrentFrame))
                    WeakReferenceMessenger.Default.Send<DisplayFrame, string>(display, "Display");
            });
        };
    }





    /// <summary>
    /// 初始化赋值参数和获取参数值
    /// </summary>
    void InitializeValue()
    {

        InitalizeExposure();
    }


    public DhyanaInfoModel DhyanaInfo { get; set; } = new();

    /// <summary>
    /// 更新某些参数暂停Capture
    /// </summary>
    /// <param name="action"></param>
    private void RefreshCapture(Action action)
    {
        if (IsCapture == true)
        {
            StopCapture();

            action();

            StartCapture();
        }
        else action();
    }

    #region Camera

    //TODO 这里之后要记得写一个控件，在True和False之间切换会有图像切换

    [ObservableProperty]
    private bool _isCameraConnected = false;


    [RelayCommand]
    async Task ConnectCamera()
    {
        await Task.Run(() =>
        {
            if (!IsCameraConnected)
            {
                if (!DhyanaObject.InitializeSdk())
                {
                    MessageBox.Show("初始化失败");
                    return;
                }

                IsCameraConnected = DhyanaObject.InitializeCamera(0);

                if (!IsCameraConnected) MessageBox.Show("相机链接失败");

                IsHistc = true;
            }

            else
            {
                DhyanaObject.UnInitializeCamera();
                DhyanaObject.UninitializeSdk();
            }

        });
    }

    #endregion

    #region Capture

    [ObservableProperty]
    private bool _isCapture = false;


    [RelayCommand]
    void CaptureFrame()
    {
        if (!IsCapture) StartCapture();
        else StopCapture();
    }

    /// <summary>
    /// 开始捕获屏幕
    /// </summary>
    private void StartCapture()
    {
        if (IsCapture) return;

        IsCapture = true;
        InitalizeExposure();
        DhyanaObject.StartCapture();

        _frameTimer.Start();
    }

    /// <summary>
    /// 停止捕获屏幕
    /// </summary>
    private void StopCapture()
    {
        if (!IsCapture) return;
        IsCapture = false;

        _frameTimer.Stop();
        DhyanaObject.StopCapture();

    }

    /// <summary>
    /// 转换软件里面的Frame格式，这里之后可以用来自定义处理数据，然后存储图片和视频也要在这里做一个简单的修订
    /// </summary>
    /// <param name="display"></param>
    /// <param name="frame"></param>
    /// <returns></returns>
    private bool Frame2Bytes(ref DisplayFrame display, TUCAM_FRAME frame)
    {

        // HACK 这里修改Roi的时候会因为未知原因导致一个无法确定的CLR错误
        // 我的预估是ROI设置导致了某些尺寸没修改导致的
        try
        {
            int width = frame.usWidth;
            int height = frame.usHeight;
            int stride = (int)frame.uiWidthStep;

            var size = (int)(frame.uiImgSize + frame.usHeader);
            var raw = new byte[size];
            var actualRaw = new byte[frame.uiImgSize];

            // 要位移
            Marshal.Copy(frame.pBuffer, raw, 0, size);

            Buffer.BlockCopy(raw, frame.usHeader, actualRaw, 0, (int)frame.uiImgSize);


            display.Height = height;
            display.Width = width;
            display.Stride = stride;
            display.FrameObject = actualRaw;
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            return false;
        }
        return true;
    }

    #endregion

    #region Exposure

    [ObservableProperty]
    private double _exposure = 0;

    partial void OnExposureChanged(double value)
    {
        GlobalTimerPeriod = value + 100;
        _frameTimer.Interval = TimeSpan.FromMilliseconds(GlobalTimerPeriod);

        if (!IsAutoExposure)
        {
            DhyanaObject.SetExposure(value);
            ExposureModel.Exposure = value;
        }
    }

    /// <summary>
    /// 输入框里面的Exposure
    /// </summary>
    [ObservableProperty]
    private ExposureModel _exposureModel = new();

    [RelayCommand]
    void SetExposure() => Exposure = ExposureModel.Exposure;

    [ObservableProperty]
    private bool _isAutoExposure = false;

    partial void OnIsAutoExposureChanged(bool value)
    {
        DhyanaObject.SetAutoExposure(value);

        Task.Run(() =>
        {
            while (IsAutoExposure)
            {
                double val = -1;
                DhyanaObject.GetExposure(ref val);

                Exposure = val;
                ExposureModel.Exposure = val;

                Task.Delay(5000);
            }
        });
    }

    [RelayCommand]
    async Task SetOnceExposure()
    {
        await Task.Run(() =>
        {
            DhyanaObject.SetAutoExposure(true);

            double oldValue = -1;
            double newValue = -2;

            do
            {
                oldValue = newValue;
                DhyanaObject.GetExposure(ref newValue);

                Exposure = newValue;
                ExposureModel.Exposure = newValue;

                Task.Delay(3000);

            } while (Math.Abs(oldValue - newValue) > 0.01);

            DhyanaObject.SetAutoExposure(false);
        });
    }

    #endregion


    void InitalizeExposure()
    {
        // Expousure
        DhyanaObject.GetExposureAttr(out var attr);

        ExposureModel.MinExposure = attr.dbValMin;
        ExposureModel.MaxExposure = attr.dbValMax;
        ExposureModel.DefaultExposure = attr.dbValDft;
        ExposureModel.StepExposure = attr.dbValStep;
    }

    #region resolutions

    public List<string> Resolutions => Dhyana.Resolutions;

    [ObservableProperty]
    private int _resolutionIndex = 0;

    partial void OnResolutionIndexChanged(int value)
    {
        RefreshCapture(() => DhyanaObject.SetResolution(value));
        InitalizeExposure();
    }

    #endregion

    #region ROI

    public List<string> RoiMode => new List<string>()
    {
        "UnSet","1024 X 1024","512 X 512","256 X 256","128 X 128"
    };


    [ObservableProperty]
    private uint _roiModeIndex = 0;

    [ObservableProperty]
    private RoiModel _roiModel = new();

    partial void OnRoiModeIndexChanged(uint value)
        => RefreshCapture(() =>
        {
            switch (value)
            {
                case 0:
                    DhyanaObject.UnSetRoi();
                    break;

                default:
                    var width = 1024 / (int)Math.Pow(2, value - 1);
                    var height = 1024 / (int)Math.Pow(2, value - 1);
                    DhyanaObject.SetRoi(width, height, 0, 0);
                    break;
            }

            TUCAM_ROI_ATTR attr = default;
            DhyanaObject.GetRoi(ref attr);

            RoiModel.HOffset = attr.nHOffset;
            RoiModel.VOffset = attr.nVOffset;
            RoiModel.Width = attr.nWidth;
            RoiModel.Height = attr.nHeight;

            RoiEnabled = attr.bEnable;
        });


    [ObservableProperty]
    private bool _roiEnabled = false;

    [RelayCommand]
    void SetRoi()
        => RefreshCapture(() =>
        {
            if (!RoiEnabled) DhyanaObject.UnSetRoi();
            else
            {
                RoiModeIndex = 0;
                DhyanaObject.SetRoi(RoiModel.Width, RoiModel.Height, RoiModel.HOffset, RoiModel.VOffset);
            }
        });

    #endregion

    #region Histogram

    private void OnAutoLevelChanged()
    {
        if (IsAutoLeftLevel && !IsAutoRightLevel) DhyanaObject.SetAutolevels(1);
        else if (!IsAutoLeftLevel && IsAutoRightLevel) DhyanaObject.SetAutolevels(2);
        else if (IsAutoLeftLevel && IsAutoRightLevel) DhyanaObject.SetAutolevels(3);
        else DhyanaObject.SetAutolevels(0);
    }

    /// <summary>
    /// 是否开启直方统计
    ///
    /// TODO 这个值没有写进capture里面 另外每一帧变动应该也要更新一下level的值，这个也没写
    /// 
    /// </summary>
    [ObservableProperty]
    private bool _isHistc = true;

    partial void OnIsHistcChanged(bool value) => DhyanaObject.SetHistc(value);

    /// <summary>
    ///
    /// </summary>
    [ObservableProperty]
    private bool _isAutoLeftLevel = false;

    partial void OnIsAutoLeftLevelChanged(bool value) => OnAutoLevelChanged();

    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty]

    private bool _isAutoRightLevel = false;

    partial void OnIsAutoRightLevelChanged(bool value) => OnAutoLevelChanged();


    /// <summary>
    /// 8bit  - 254
    /// 16bit - 65534
    /// </summary>
    [ObservableProperty]
    private int _leftLevel = 0;

    /// <summary>
    /// 8bit  - 254
    /// 16bit - 65534
    /// </summary>
    [ObservableProperty]
    private int _rightLevel = 0;
    #endregion

    #region Orientation

    [ObservableProperty]
    private bool _horizontal = false;

    partial void OnHorizontalChanged(bool value)
        => DhyanaObject.SetHorizontal(value);


    [ObservableProperty]
    private bool _vertical = false;

    partial void OnVerticalChanged(bool value)
        => DhyanaObject.SetHorizontal(value);

    #endregion

    #region fans

    /**
     * Note 关于风扇和温度，这里有一些概念问题亟待了解，暂时不写
     */

    #endregion

    #region Noise

    public List<string> NoiseModes => new List<string>()
    {
        "0","1","2","3"
    };

    [ObservableProperty]
    private uint _noiseModeIndex = 0;

    partial void OnNoiseModeIndexChanged(uint value)
        => DhyanaObject.SetNoiseLevel(value);

    #endregion

    #region ImageProperty


    public List<string> ImageModes => Dhyana.ImageModeRename;

    [ObservableProperty]
    private uint _imageModeIndex = 0;

    partial void OnImageModeIndexChanged(uint value)
        => RefreshCapture(() =>
        {
            // 这部分完全去按照操作手册里面给的来写
            if (ImageModeIndex == 0)
            {
                GlobalGain = 0;
                ImageMode = 1;
            }
            else if (ImageModeIndex == 1)
            {
                GlobalGain = 0;
                ImageMode = 2;
            }
            else if (ImageModeIndex == 2)
            {
                GlobalGain = 1;
                ImageMode = 3;
            }
            else if (ImageModeIndex == 3)
            {
                GlobalGain = 2;
                ImageMode = 4;
            }
            else if (ImageModeIndex == 4)
            {
                // NOTE 这里不确定如果等于2是个什么情况
                GlobalGain = 1;
                ImageMode = 5;
            }
            else
            {

            }
        });

    [ObservableProperty]
    private int _globalGain = 0;

    partial void OnGlobalGainChanged(int value)
        => RefreshCapture(() => DhyanaObject.SetGlobalGain(GlobalGain));

    [ObservableProperty]
    private int _imageMode = 0;

    partial void OnImageModeChanged(int value)
        => RefreshCapture(() => DhyanaObject.SetImageMode(ImageMode));

    [ObservableProperty]
    private int _gamma = 0;

    partial void OnGammaChanged(int value)
        => RefreshCapture(() => DhyanaObject.SetGamma(Gamma));

    [ObservableProperty]
    private int _contrast = 0;

    partial void OnContrastChanged(int value)
        => RefreshCapture(() => DhyanaObject.SetContrast(Contrast));
    #endregion

    #region ImageOutput

    [ObservableProperty]
    private SaveModel _saveModel = new();

    [ObservableProperty]
    private int _savePictureCount = 1;

    [ObservableProperty]
    private int _savePictureInterval = 5;

    private int _saveCountFlag = 0;

    /// <summary>
    /// 当前依旧是调用的API，后期可能会对图片进行增删改
    /// </summary>
    [RelayCommand]
    async Task SavePictures()
    {
        _saveCountFlag = 0;

        DispatcherTimer timer = new(priority: DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromSeconds((double)SavePictureInterval),
        };

        timer.Tick += (sender, args) =>
        {
            if (_saveCountFlag < SavePictureCount)
            {
                SaveOneFrame();
                _saveCountFlag++;
            }
            else
            {
                timer.Stop();
            }
        };

        await Task.Run(() =>
        {
            timer.Start();

            var span = SavePictureInterval == 0 ? 1000 : SavePictureInterval * 1000;

            while (timer.IsEnabled)
            {
                Task.Delay(span);
            }
        });
    }

    void SaveOneFrame()
    {
        var suffix = SaveModel.IsTimeSuffix ? $"_{System.DateTime.Now:yyyyMMdd_HH_mm_ss}" : "";
        string path = $"{SaveModel.Root}\\{SaveModel.Name}{suffix}";

        if (SaveModel.IsRaw)
            DhyanaObject.SaveCurrentFrame(path, 0);
        if (SaveModel.IsTif)
            DhyanaObject.SaveCurrentFrame(path, 1);
        if (SaveModel.IsPng)
            DhyanaObject.SaveCurrentFrame(path, 2);
        if (SaveModel.IsJpg)
            DhyanaObject.SaveCurrentFrame(path, 3);
        if (SaveModel.IsBmp)
            DhyanaObject.SaveCurrentFrame(path, 4);
    }

    [ObservableProperty]
    private float _videoFps = 25;

    [RelayCommand]
    private void SaveVideo()
    {
        var suffix = SaveModel.IsTimeSuffix ? $"_{System.DateTime.Now:yyyyMMdd_HH_mm_ss}" : "";
        string path = $"{SaveModel.Root}\\{SaveModel.Name}{suffix}.avi";

        var interval = Exposure + 100;
        DhyanaObject.SaveVideo((int)interval, VideoFps, path);
    }

    #endregion
}