using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

static public class InputSystem
{
    static public List<KeyCode> bufferedKeys = new();

    public enum BUTTONS : uint
    {
        PAD_SELECT      = 0x100,
        PAD_L3          = 0x200,
        PAD_R3          = 0x400,
        PAD_START       = 0x800,
        PAD_UP          = 0x1000,
        PAD_RIGHT       = 0x2000,
        PAD_DOWN        = 0x4000,
        PAD_LEFT        = 0x8000,
        PAD_L2          = 0x1,
        PAD_R2          = 0x2,
        PAD_L1          = 0x4,
        PAD_R1          = 0x8,
        PAD_TRIANGLE    = 0x10,
        PAD_CIRCLE      = 0x20,
        PAD_CROSS       = 0x40,
        PAD_SQUARE      = 0x80
    }    

    public class Pad
    {   
        public int state;

        public uint tapped, held, held_prev, held_prev2, tapped_prev;
    }

    static public class Keybinds
    {
        static public readonly Dictionary<KeyCode, BUTTONS> P1_binds = new Dictionary<KeyCode, BUTTONS>
        {
            { KeyCode.Backspace,    BUTTONS.PAD_SELECT },
            { KeyCode.Keypad3,      BUTTONS.PAD_R3 },
            { KeyCode.Keypad1,      BUTTONS.PAD_L3 },
            { KeyCode.Return,       BUTTONS.PAD_START },
            { KeyCode.W,            BUTTONS.PAD_UP },
            { KeyCode.D,            BUTTONS.PAD_RIGHT },
            { KeyCode.S,            BUTTONS.PAD_DOWN },
            { KeyCode.A,            BUTTONS.PAD_LEFT },
            { KeyCode.Keypad7,      BUTTONS.PAD_L2 },
            { KeyCode.Keypad9,      BUTTONS.PAD_R2 },
            { KeyCode.Keypad4,      BUTTONS.PAD_L1 },
            { KeyCode.Keypad6,      BUTTONS.PAD_R1 },
            { KeyCode.I,            BUTTONS.PAD_TRIANGLE },
            { KeyCode.L,            BUTTONS.PAD_CIRCLE },
            { KeyCode.K,            BUTTONS.PAD_CROSS },
            { KeyCode.J,            BUTTONS.PAD_SQUARE }
        };
    }

    static public void PlayerUpdate(Pad[] _pads)
    {
        Pad pad;
        uint held;
        int i;

        for(i = 0; i < _pads.Length; i++)
        {  
            pad = _pads[i];
            pad.held_prev2 = pad.held_prev;
            pad.tapped_prev = pad.tapped;
            pad.held_prev = pad.held;

            held = PadRead(i);

            if((pad.held & (uint)BUTTONS.PAD_UP) > 0)
                pad.held &= 0xBFFF; //If Up is held, then ignore Down
            if((pad.held & (uint)BUTTONS.PAD_LEFT) > 0)
                pad.held &= 0xDFFF; //If Left is held, then ignore Right

            pad.tapped = (~pad.held_prev & pad.held) & 0xF9FF; //Select bits for buttons not held in previous frame (except L3 & R3)
        }
    }

    static public void PBakUpdate(Pad[] _pads)
    {
        throw new NotImplementedException();
    }

    static public uint PadRead(int padNumber)
    {
        uint held;

        held = 0;
        if(padNumber != 0) {return held;} //Skip non-primary pads

        foreach(KeyCode bind in bufferedKeys)
        {
            if (Keybinds.P1_binds.TryGetValue(bind, out BUTTONS btn))
            {
                held |= (uint)btn;
            }
        }

        return held;
    }
}
