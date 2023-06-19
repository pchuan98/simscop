using Simscop.API.Native.Mshot;
using System;
using Motor = Simscop.API.Native.Mshot.Motor;

namespace Simscop.API;

public class MshotMotor
{
    private const double Factor = 1.0;

    private const uint XAddress = 1;

    private const uint YAddress = 2;

    private const uint ZAddress = 3;

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
        get => ((double)Motor.ReadPosition(XAddress) / Factor);
    }

    public double Y
    {
        get => (double)Motor.ReadPosition(YAddress) / Factor;
    }

    public double Z
    {
        get => (double)Motor.ReadPosition(ZAddress) / Factor;
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
        Motor.OpenQk(0);
    }
}