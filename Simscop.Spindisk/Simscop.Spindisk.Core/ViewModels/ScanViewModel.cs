using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Simscop.Spindisk.Core.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OpenCvSharp.Stitcher;

namespace Simscop.Spindisk.Core.ViewModels;

public partial class ScanViewModel : ObservableObject
{
    [ObservableProperty]
    private string _root = "";

    [RelayCommand]
    void SelectRoot()
    {
        var dialog = new FolderBrowserDialog();

        if (dialog.ShowDialog() == DialogResult.OK)
            Root = dialog.SelectedPath;
    }

    [ObservableProperty]
    private double _xStart = 0;

    [ObservableProperty]
    private double _xEnd = 0;

    [ObservableProperty]
    private double _xStep = 0;

    [ObservableProperty]
    private bool _xEnable = true;

    [ObservableProperty]
    private double _yStart = 0;

    [ObservableProperty]
    private double _yEnd = 0;

    [ObservableProperty]
    private double _yStep = 0;

    [ObservableProperty]
    private bool _yEnable = true;

    [ObservableProperty]
    private double _zStart = 0;

    [ObservableProperty]
    private double _zEnd = 0;

    [ObservableProperty]
    private double _zStep = 0;

    [ObservableProperty]
    private bool _zEnable = true;

    [ObservableProperty]
    private double _percent = 0;

    partial void OnPercentChanged(double value)
    {
        if (Percent == 0) Title= "自动扫描";

        Title = $"自动扫描 {Percent:.2f} %";
    }

    [ObservableProperty] 
    private string _title = "自动扫描";

    void StartScan(uint mode)
    {
        var flags = new string[]
        {
            "X","Y","Z"
        };

        var flag = flags[mode];

        void EnableAction(bool value) => GetType().GetProperty($"{flag}Enable")!.SetValue(this, value);

        var startValue = (double)GetType().GetProperty($"{flag}Start")!.GetValue(this)!;
        var endValue = (double)GetType().GetProperty($"{flag}End")!.GetValue(this)!;
        var stepValue = (double)GetType().GetProperty($"{flag}Step")!.GetValue(this)!;

        var message = SteerMessage.GetValue($"Move{flag}")!;

        Task.Run(() =>
        {
            EnableAction(false);
            Percent = 0;

            if (endValue <= startValue | stepValue <= 0)
            {
                EnableAction(true);
                return;
            }

            var pos = startValue;

            var count = (int)Math.Ceiling((endValue - startValue) / stepValue);
            var step = 0;

            WeakReferenceMessenger.Default.Send<string, string>(pos.ToString(CultureInfo.InvariantCulture), message);
            Thread.Sleep(5000);
            do
            {
                Debug.WriteLine($"[INFO] {flag} -> {pos}");
                WeakReferenceMessenger.Default.Send<string, string>(pos.ToString(CultureInfo.InvariantCulture), message);

                Thread.Sleep(1500);

                var path = System.IO.Path.Join(Root, $"{flag}_{pos}.TIF");
                WeakReferenceMessenger.Default.Send<string, string>(path, MessageManage.SaveACapture);

                pos += stepValue;
                pos = Math.Min(endValue, pos);

                Percent = (double)++step / count * 100;
            } while (step <= count);


            EnableAction(true);

        });
    }

    [RelayCommand]
    public void StartScanX()
        => StartScan(0);

    [RelayCommand]
    public void StartScanY()
        => StartScan(1);

    [RelayCommand]
    public void StartScanZ()
        => StartScan(2);

    [ObservableProperty] 
    public int _testInterval = 1000;

    [ObservableProperty] public double _testStart = 0;

    [ObservableProperty] public double _testEnd = 0;

    [ObservableProperty] public double _testStep = 0;

    [RelayCommand]
    public void Test()
    {
        Task.Run(() =>
        {
            var pos = TestStart;

            WeakReferenceMessenger.Default.Send<string, string>(pos.ToString(CultureInfo.InvariantCulture), SteerMessage.MoveZ);
            Thread.Sleep(2000);
            do
            {
                Debug.WriteLine($"[INFO] Z -> {pos}");

                WeakReferenceMessenger.Default.Send<string, string>(pos.ToString(CultureInfo.InvariantCulture), SteerMessage.MoveZ);
                Thread.Sleep(TestInterval);

                pos += TestStep;
                pos = Math.Min(TestEnd, pos);

            } while (pos <= TestEnd);
        });
    }
}