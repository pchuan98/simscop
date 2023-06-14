using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Simscop.API;

namespace Simscop.Spindisk.Core.ViewModels;

/**
 * Note 之后可以添加功能：
 *  自动识别串口并选择
 */

/// <summary>
/// 转盘操作
/// </summary>
public partial class SpinViewModel : ObservableObject
{
    public SpinViewModel()
    {
        ComList = Simscop.API.Helper.SerialHelper.GetAllCom();
    }

    [ObservableProperty]
    private bool _spinViewEnabled = true;

    private void DelaySpinViewEnabled(int value)
    {
        Task.Run(() =>
        {
            SpinViewEnabled = false;
            Task.Delay(value * 1000).Wait();
            SpinViewEnabled = true;
        });
    }

    [ObservableProperty]
    private bool _spinControlEnabled = true;

    public List<string> ComList { get; set; }

    [ObservableProperty]
    private string _comName = "COM6";

    [ObservableProperty]
    private bool _isConnected = false;

    [ObservableProperty]
    private bool _isConnectting = true;

    partial void OnIsConnectedChanged(bool value)
        => SpinControlEnabled = value;

    [RelayCommand]
    public void ConnectCom()
    {
        IsConnectting = false;
        try
        {
            if (IsConnected)
                IsConnected = XLight.Connect(ComName);
            else
            {
                XLight.Disconnect();
                IsConnected = false;
            }
        }
        catch (Exception e)
        {
            IsConnectting = true;
            IsConnected = false;
            MessageBox.Show("接口出现错误，连接失败");
        }
        finally
        {
            IsConnectting = true;
        }

    }

    [RelayCommand]
    void Reset()
    {
        XLight.Reset();
        DelaySpinViewEnabled(SpiningIndex == 1 ? 10 : 5);
    }

    [ObservableProperty]
    private bool _diskEnable = false;

    [RelayCommand]
    void SetDisk()
    {
        // TODO 这里之后要修改成为从串口获取实时数据来抉择是否完成后面的任务
        try
        {
            if (XLight.FlagD == 0) SpiningIndex = 1;
            XLight.SetDisk(DiskEnable ? (uint)1 : (uint)0);
            DelaySpinViewEnabled(2);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            DiskEnable=false;
        }
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
        DelaySpinViewEnabled(4);
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
    {
        XLight.SetDichroic(value);
        DelaySpinViewEnabled(3);
    }

    #endregion

    #region Emission

    public List<string> EmissionCollection => new List<string>()
    {
        "1","2","3","4","5","6","7","8"
    };

    [ObservableProperty]
    private uint _emissionIndex = 0;

    partial void OnEmissionIndexChanged(uint value)
    {
        XLight.SetEmission(value); 
        DelaySpinViewEnabled(3);

    }

    #endregion

    #region Excitation

    public List<string> ExcitationCollection => new List<string>()
    {
        "1","2","3","4","5","6","7","8"
    };

    [ObservableProperty]
    private uint _excitationIndex = 0;

    partial void OnExcitationIndexChanged(uint value)
    {
        XLight.SetExcitation(value);
        DelaySpinViewEnabled(3);
    }

    #endregion
}