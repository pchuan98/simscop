using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simscop.Spindisk.Core.ViewModels;

/// <summary>
/// 舵机操作
/// </summary>
public partial class SteerViewModel:ObservableObject
{
    [ObservableProperty]
    private double _x = 0;

    [ObservableProperty]
    private double _y = 0;

    [ObservableProperty]
    private double _z = 0;
}