using CommunityToolkit.Mvvm.Messaging;
using OpenCvSharp;
using Simscop.Spindisk.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Simscop.Spindisk.Core.Messages;

public static class Aggregate
{
    public static void AddSaveFrameMessage(object obj, ref Mat? mat, ref SaveFrameModel model)
    {
        WeakReferenceMessenger.Default
            .Register<SaveFrameModel, string>(obj, MessageManage.SaveCurrentCapture, (s, m) =>
        {
            Task.Run(() =>
            {
                // copy mat
                var matCopy = mat.Clone();
            });
        });
    }


}