using Simscop.API.Native.Mshot;
using System;
using Motor = Simscop.API.Native.Mshot.Motor;

namespace Simscop.API;

public class MshotMotor
{
    /// <summary>
    /// 转成um单位
    /// </summary>
    private const double Factor = 20.0;

    private const uint XAddress = 1;

    private const uint YAddress = 2;

    private const uint ZAddress = 3;


    public bool InitializeMotor()
    {
        if (!Motor.OpenQk(true)) return false;

        Motor.SetControlAxis(MshotAxis.ALL);

        return false;
    }

    #region Position

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

    #endregion

    #region Enable

    public bool XEnabled
    {
        get => Motor.GetAxisStatus(XAddress, MshotAxisStatus.ENABLE);
    }

    public bool YEnabled
    {
        get => Motor.GetAxisStatus(YAddress, MshotAxisStatus.ENABLE);
    }

    public bool ZEnabled
    {
        get => Motor.GetAxisStatus(ZAddress, MshotAxisStatus.ENABLE);
    }

    #endregion

    #region Action

    public bool XAction
    {
        get => Motor.GetAxisStatus(XAddress, MshotAxisStatus.ACTION);
    }

    public bool YAction
    {
        get => Motor.GetAxisStatus(YAddress, MshotAxisStatus.ACTION);
    }

    public bool ZAction
    {
        get => Motor.GetAxisStatus(ZAddress, MshotAxisStatus.ACTION);
    }

    #endregion

    #region Exception

    public bool XException
    {
        get => Motor.GetAxisStatus(XAddress, MshotAxisStatus.CONTROL);
    }

    public bool YException
    {
        get => Motor.GetAxisStatus(YAddress, MshotAxisStatus.CONTROL);
    }

    public bool ZException
    {
        get => Motor.GetAxisStatus(ZAddress, MshotAxisStatus.CONTROL);
    }

    #endregion

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