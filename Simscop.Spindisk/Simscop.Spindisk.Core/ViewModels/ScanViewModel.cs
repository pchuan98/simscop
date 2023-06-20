using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Simscop.Spindisk.Core.Messages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
    private double _zStart = 0;

    [ObservableProperty]
    private double _zEnd = 0;

    [ObservableProperty]
    private double _zStep = 0;

    [ObservableProperty]
    private bool _zEnable = true;

    [RelayCommand]
    public void StartScanZ()
    {
        ZEnable = false;

        if (ZEnd <= ZStart) return;

        var pos = ZStart;
        WeakReferenceMessenger.Default.Send<string, string>(SteerMessage.MoveZ, pos.ToString(CultureInfo.InvariantCulture));
        Thread.Sleep(2000);
        do
        {
            WeakReferenceMessenger.Default.Send<string, string>(SteerMessage.MoveZ, pos.ToString(CultureInfo.InvariantCulture));

            Thread.Sleep(100);
            
            var path = System.IO.Path.Join(Root, $"Z_{pos}.TIF");
            WeakReferenceMessenger.Default.Send<string, string>(MessageManage.SaveACapture, path);

            pos += ZStep;
        } while (pos + ZStep < ZEnd);

        ZEnable = true;
    }
}