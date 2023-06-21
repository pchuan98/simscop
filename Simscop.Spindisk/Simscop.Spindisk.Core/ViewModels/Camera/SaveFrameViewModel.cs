using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Simscop.Spindisk.Core.Messages;
using Simscop.Spindisk.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

// ReSharper disable once CheckNamespace
namespace Simscop.Spindisk.Core.ViewModels;

/// <summary>
/// 存储frame相关服务
/// </summary>
public partial class SaveFrameViewModel : ObservableObject
{
    [ObservableProperty]
    private SaveFrameModel _saveModel = new();

    [RelayCommand]
    void LoadSaveDirectory()
    {
        var dialog = new FolderBrowserDialog();

        if (dialog.ShowDialog() == DialogResult.OK)
            SaveModel.Root = dialog.SelectedPath;
    }

    [RelayCommand]
    void SaveFile()
    {
        var model = new SaveFrameModel();
        model.CopyFrom(SaveModel);

        WeakReferenceMessenger.Default.Send<SaveFrameModel, string>(model, MessageManage.SaveCurrentCapture);
    }

    [RelayCommand]
    void QuickSaveFile()
    {
        var dialog = new SaveFileDialog();

        if (dialog.ShowDialog() == DialogResult.OK)
        {

        }
    }
}