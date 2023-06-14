using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Text;
using System.IO.Ports;
using System.Linq.Expressions;
using System.Windows;

namespace Simscop.API;

/**
 * D - [0,1] - Confrocal Disk Slider
 * C - [1,5] - Dichroic Wheel
 * B - [1,8] - Autom Emission Wheel
 * A - [1,8] -
 * todo - [] -
 * N - [0,1] - Confocal Disk Motor
 * R - [0,1] - Response on/off
 * H - NULL - Rehome all devices
 * q - NULL - Query state of all devices
 *      =>  the order is : B C D N
 * r - [ID] - Read current position of individual devices
 * v - NULL - Read version of all devices
 *
 *
 * NOTE:
 * 1. 使用N1前需要D1
 *
 * 初始化阶段，我们需要设置的东西有：
 *  -> 设置R为1
 *  -> 设置H 为了复位
 *  -> 使用r来读取当前位置
 *
 */

// TODO 之后抽象COM操作到Lib中去

// NOTE Timeout功能暂时搁置，不必要这么复杂，做好串接就好，或者未来写个事件池，自己在后台判断命令是不是都读完了

// NOTE 上面的结论只适合初期，后面依旧要要求返回值的，因为必须需要返回值来标定每一次数据更新带来的事件是否完成成功

public static class XLight
{
    /// <summary>
    /// 串口对象
    /// </summary>
    private static SerialPort? _serial;

    /// <summary>
    /// 判断最终字符
    /// </summary>
    private const char EndChar = '\r';

    /// <summary>
    /// 最大接受字符时间,超出判断接受失误
    /// </summary>
    private const uint Timeout = 5;

    /// <summary>
    /// Confocal Disk Motor
    /// 0 - 1 (Disk)
    /// </summary>
    public static uint FlagN { get; set; } = 0;

    /// <summary>
    /// Confrocal Disk Slider
    /// 0 - 1 (Spining)
    /// </summary>
    public static uint FlagD { get; set; } = 0;

    /// <summary>
    /// Dichroic Wheel
    /// 1 - 5 (Dichroic)
    /// </summary>
    public static uint FlagC { get; set; } = 0;

    /// <summary>
    /// Autom Emission Wheel
    /// 1 - 8 (Emission)
    /// </summary>
    public static uint FlagB { get; set; } = 0;

    /// <summary>
    /// Excitation
    /// 1 - 8 (Excitation)
    /// </summary>
    public static uint FlagA { get; set; } = 0;

    /// <summary>
    /// 是否已经连接了
    /// </summary>
    public static bool IsConnected => _serial?.IsOpen ?? false;

    /// <summary>
    /// 连接COM端口
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static bool Connect(string port)
    {
        try
        {
            _serial = new SerialPort()
            {
                PortName = port,
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None
            };
            if (!_serial.IsOpen)
                _serial.Open();

            Debug.WriteLine("[XXX] Serial Connected Success");

            return _serial.IsOpen;
        }
        catch (System.Exception e)
        {
            throw new Exception("Open COM error.", e);
        }
    }

    /// <summary>
    /// 解开COM端口
    /// </summary>
    public static void Disconnect() => _serial?.Close();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool SetSpining(uint value)
    {
        try
        {
            FlagD = value <= 1 ? value : 0;
            _serial?.Write($"D{FlagN}\r");

            Debug.WriteLine("[XXX] Spining Write Success");

        }
        catch (Exception e)
        {
            throw new Exception("Recall Error.", e);
        }

        return true;
    }

    public static bool SetDisk(uint value)
    {
        try
        {
            FlagN = value <= 1 ? value : 0;
            _serial?.Write($"N{FlagN}\r");
        }
        catch (Exception e)
        {
            throw new Exception("Recall Error.", e);
        }

        return true;
    }

    public static bool SetDichroic(uint value)
    {
        try
        {
            FlagC= value is >= 1 and <= 5 ? value : 1;
            _serial?.Write($"C{FlagC}\r");
        }
        catch (Exception e)
        {
            throw new Exception("Recall Error.", e);
        }

        return true;
    }

    public static bool SetEmission(uint value)
    {
        try
        {
            FlagB = value is >= 1 and <= 8 ? value : 1;
            _serial?.Write($"B{FlagB}\r");
        }
        catch (Exception e)
        {
            throw new Exception("Recall Error.", e);
        }

        return true;
    }

    public static bool SetExcitation(uint value)
    {
        try
        {
            FlagA = value is >= 1 and <= 8 ? value : 1;
            _serial?.Write($"A{FlagA}\r");
        }
        catch (Exception e)
        {
            throw new Exception("Recall Error.", e);
        }

        return true;
    }

    public static bool Reset()
    {
        try
        {
            _serial?.Write("H\r");
        }
        catch (Exception e)
        {
            throw new Exception("Recall Error.", e);
        }

        return true;
    }

    public static bool Read(char value)
    {
        throw new NotImplementedException();
    }

    public static string Version
    {
        get
        {
            if (_serial is null) return "";
            try
            {
                _serial.Write("V\r");
                var content = "";

                _serial.DataReceived += (s, e) =>
                {
                    do content += _serial.ReadExisting();
                    while (content.Contains(EndChar));
                };

                return content;
            }
            catch (Exception e)
            {
                throw new Exception("Recall Error.", e);
            }
        }
    }

    public static bool SetRecall(bool rec = true)
    {
        try
        {
            _serial?.Write($"R{(rec ? 1 : 0)}\r");
        }
        catch (Exception e)
        {
            throw new Exception("Recall Error.", e);
        }

        return true;
    }
}