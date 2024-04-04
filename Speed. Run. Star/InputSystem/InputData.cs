using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputData
{
    public float directionX;
    public float directionY;
    public byte buttons;
    public byte buttonsDown;
    public byte buttonsUp;

    public const byte JUMPBUTTON = 0x01 << 1;
    public const byte DASHBUTTON = 0x01 << 2;
    public const byte LANDBUTTON = 0x01 << 3;
}
