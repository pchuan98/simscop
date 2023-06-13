using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Simscop.Spindisk.Core.Models;

public partial class SaveModel:ObservableObject
{
    [ObservableProperty]
    private bool _isRaw = false;

    [ObservableProperty]
    private bool _isTif = false;

    [ObservableProperty]
    private bool _isPng = false;

    [ObservableProperty]
    private bool _isJpg = false;

    [ObservableProperty]
    private bool _isBmp = false;

    [ObservableProperty] 
    private string _root = "";

    [ObservableProperty] 
    private string _name = "";

    [ObservableProperty] 
    private bool _isTimeSuffix = false;

}