using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Simscop.Spindisk.Core.ViewModels;

/**
 * 蓝色，红色，绿色，紫色
 */

/// <summary>
/// 激光通道操作
/// </summary>
public partial class LaserViewModel:ObservableObject
{
    [ObservableProperty] 
    private double _channelAValue = 0;

    [ObservableProperty] 
    private bool _channelAEnable = false;

    [ObservableProperty]
    private double _channelBValue = 0;

    [ObservableProperty]
    private bool _channelBEnable = false;

    [ObservableProperty]
    private double _channelCValue = 0;

    [ObservableProperty]
    private bool _channelCEnable = false;

    [ObservableProperty]
    private double _channelDValue = 0;

    [ObservableProperty]
    private bool _channelDEnable = false;
}