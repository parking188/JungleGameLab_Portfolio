using UnityEngine;

public struct InputData
{
    public Vector3 direction;
    public Vector2 look;
    public Vector2 scrollWheel;
    public byte buttons;
    public byte buttonsDown;
    public byte buttonsUp;

    public const byte BUTTONESCAPE          = 0x01 << 0;
    public const byte BUTTONMOUSERIGHTCLICK = 0x01 << 1;
    public const byte BUTTONRESERVATION     = 0x01 << 2;
    public const byte BUTTONTOGGLETRAINICON = 0x01 << 3;
    public const byte BUTTONSPACEBAR        = 0x01 << 4;
    public const byte BUTTON1               = 0x01 << 5;
    public const byte BUTTON2               = 0x01 << 6;
    public const byte BUTTON3               = 0x01 << 7;

    public static bool IsButtonOn(byte inputByte, byte buttonByte)
    {
        if ((inputByte & buttonByte) == buttonByte)
        {
            return true;
        }

        return false;
    }
}
