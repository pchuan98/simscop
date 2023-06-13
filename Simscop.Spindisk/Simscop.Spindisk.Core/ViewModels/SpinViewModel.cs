using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Simscop.API;

namespace Simscop.Spindisk.Core.ViewModels;

/// <summary>
/// 转盘操作
/// </summary>
public partial class SpinViewModel : ObservableObject
{
    public SpinViewModel()
    {
        ComList = Simscop.API.Helper.SerialHelper.GetAllCom();

        _timer = new DispatcherTimer();
        _timer.Tick += (s, e) =>
        {

        };
    }


    [ObservableProperty]
    private bool _spinChangeEnable = true;

    private DispatcherTimer _timer;

    /// <summary>
    /// 等待时间
    /// </summary>
    /// <param name="value"></param>
    private void Delay(int value = 1)
    {
        Task.Run(() =>
        {
            SpinChangeEnable = false;
            Task.Delay(value * 1000); 
            SpinChangeEnable = true;
        });
    }

    public List<string> ComList { get; set; }

    [ObservableProperty]
    private string _comName = "";

    [ObservableProperty]
    private bool _isConnected = false;

    [RelayCommand]
    void ConnectCom()
    {
        if (IsConnected) IsConnected = XLight.Connect(ComName);
        else XLight.Disconnect();

    }

    [RelayCommand]
    void Reset() => XLight.Reset();

    private bool _diskEnable = false;

    [RelayCommand]
    void SetDisk()
    {

        if (XLight.FlagD == 0) SpiningIndex = 1;

        if (_diskEnable) XLight.SetDichroic(1);
        else XLight.SetDisk(0);

        Delay(2);

        _diskEnable = !_diskEnable;
    }

    #region Spining

    public List<string> SpiningCollection => new List<string>()
    {
        "0","1"
    };

    [ObservableProperty]
    private uint _spiningIndex = 0;

    partial void OnSpiningIndexChanged(uint value)
    {
        XLight.SetSpining(value);
        Delay(8);
    }

    #endregion

    #region Dichroic

    public List<string> DichroicCollection => new List<string>()
    {
        "1","2","3","4","5"
    };

    [ObservableProperty]
    private uint _dichroicIndex = 0;

    partial void OnDichroicIndexChanged(uint value)
        => XLight.SetDichroic(value);

    #endregion

    #region Emission

    public List<string> EmissionCollection => new List<string>()
    {
        "1","2","3","4","5","6","7","8"
    };

    [ObservableProperty]
    private uint _emissionIndex = 0;

    partial void OnEmissionIndexChanged(uint value)
        => XLight.SetEmission(value);

    #endregion

    #region Excitation

    public List<string> ExcitationCollection => new List<string>()
    {
        "1","2","3","4","5","6","7","8"
    };

    [ObservableProperty]
    private uint _excitationIndex = 0;

    partial void OnExcitationIndexChanged(uint value)
        => XLight.SetExcitation(value);

    #endregion
}