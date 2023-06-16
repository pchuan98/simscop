using System;
using System.Collections.Generic;
using System.Text;

namespace Simscop.API.Native.Mshot;

public enum MshotAxis
{
    ONE = 0x01,
    TWO = 0x02,
    THREE = 0x04,
    ALL = ONE + TWO + THREE,
}

/// <summary>
/// Type速度类型（'V'普通定位速度，'J'Jog速度，'S'脉冲扫描速度，'F'开机找index速度）
/// </summary>
public enum MshotSpeed
{
    V = 'V',
    JOG = 'J',
    SPURT = 'S',
    FIND = 'F',
}

/// <summary>
/// 1查询控制器是否使能，2查询是否运动，3查询马达是否接入，4查询控制器是否有异常反馈
/// </summary>
public enum MshotAxisStatus
{
    ENABLE = 1,
    ACTION = 2,
    MOTOR = 3,
    CONTROL = 4,
}