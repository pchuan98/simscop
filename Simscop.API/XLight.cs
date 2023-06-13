using System;
using System.Collections.Generic;
using System.Text;

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
 * 2. todo
 */

public static class XLight
{
}