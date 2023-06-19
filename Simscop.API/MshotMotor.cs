using Simscop.API.Native.Mshot;
using System;
using System.Collections.Generic;
using System.Text;
using Motor = Simscop.API.Native.Mshot.Motor;
using System.Security.Cryptography;
using OpenCvSharp;

namespace Simscop.API;

public class MshotMotor
{
    private const double Factor = 20.0;

    public bool XEnable { get; set; } = false;

    public bool YEnable { get; set; } = false;

    public bool ZEnable { get; set; } = false;

    public bool InitializeMotor()
    {
        if (!Motor.OpenQk(true)) return false;

        Motor.SetControlAxis(MshotAxis.ALL);

        return Motor.AxisEnable(1, true)
            && Motor.AxisEnable(2, true)
            && Motor.AxisEnable(3, true);
    }

    public double X
    {
        get => ((double)Motor.ReadPosition(1) / Factor);
    }

    public double Y
    {
        get => (double)Motor.ReadPosition(2) / Factor;
    }

    public double Z
    {
        get => (double)Motor.ReadPosition(3) / Factor;
    }

    public bool SetOffset(uint axis, double value)
    {
        var offset = (int)(value * Factor);
        if (Motor.PositionRelativeMove(axis, offset) == 1) return true;

        var code = Motor.GetError();
        Motor.ErrorMessage = Enum.IsDefined(typeof(MshotErrorCode), code) ? (MshotErrorCode)code : MshotErrorCode.NO_DEFINE;

        return false;
    }

    public bool SetPosition(uint axis,double value)
    {
        var position = (int)(value * Factor);
        if (Motor.PositionAbsoluteMove(axis, position) == 1) return true;

        var code = Motor.GetError();
        Motor.ErrorMessage = Enum.IsDefined(typeof(MshotErrorCode), code) ? (MshotErrorCode)code : MshotErrorCode.NO_DEFINE;

        return false;
    }

    ~MshotMotor()
    {
        Motor.SetZeroPosition(1);
        Motor.SetZeroPosition(2);
        Motor.SetZeroPosition(3);

        Motor.OpenQk(0);
    }
}