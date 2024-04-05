using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputData
{
    public Vector3 direction;
    public Vector2 look;
    public byte buttons;
    public byte buttonsDown;
    public byte buttonsUp;

    public const byte JUMPBUTTON            = 0x01 << 1;
    public const byte DASHBUTTON            = 0x01 << 2;

    public static bool IsButtonOn(byte inputByte, byte buttonByte)
    {
        if ((inputByte & buttonByte) == buttonByte)
        {
            return true;
        }

        return false;
    }
}
