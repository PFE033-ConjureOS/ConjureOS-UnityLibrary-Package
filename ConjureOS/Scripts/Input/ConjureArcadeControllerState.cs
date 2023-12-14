#if ENABLE_INPUT_SYSTEM
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// Reason: This is a configuration class with specific requirements for its interface.

// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo
// Reason: SHRT is not a typo in this case.

// Inspired by: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/HID.html
// This class describes the data received by the controller in a way that can be read by the new input system.
// If the physical controller ever changes, this class will need to be reworked.

namespace ConjureOS.Input
{
    [StructLayout(LayoutKind.Explicit, Size = ConjureArcadeControllerInfo.StateSizeInBytes)]
    public struct ConjureArcadeControllerState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('H', 'I', 'D');
        
        [FieldOffset(ConjureArcadeControllerInfo.ReportIdByte)] 
        public byte reportId;
        
        [InputControl(name = "stick", layout = "ConjureArcadeStick", format = "VC2B", displayName = "Stick", processors = "ConjureArcadeVector2(minX=0.0, maxX=1.0, minY=0.0, maxY=1.0, invertY)")] 
        [InputControl(name = "stick/x", offset = 0, format = "BYTE", parameters = "clamp=false, invert=false, normalize=false")]
        [InputControl(name = "stick/left", offset = 0, format = "BYTE")]
        [InputControl(name = "stick/right", offset = 0, format = "BYTE")]
        [InputControl(name = "stick/y", offset = 1, format = "BYTE", parameters = "clamp=false, invert=false, normalize=false")]
        [InputControl(name = "stick/up", offset = 1, format = "BYTE")]
        [InputControl(name = "stick/down", offset = 1, format = "BYTE")]
        [FieldOffset(ConjureArcadeControllerInfo.StickXByte)]
        public byte stickX;
        [FieldOffset(ConjureArcadeControllerInfo.StickYByte)] 
        public byte stickY;

        [InputControl(name = "home", layout = "Button", bit = (uint) ConjureArcadeControllerInfo.ButtonBit.Home, displayName = "Home")]
        [InputControl(name = "start", layout = "Button", bit = (uint) ConjureArcadeControllerInfo.ButtonBit.Start, displayName = "Start")]
        [InputControl(name = "button1", layout = "Button", bit = (uint) ConjureArcadeControllerInfo.ButtonBit.One, displayName = "Button 1", shortDisplayName = "1")]
        [InputControl(name = "button2", layout = "Button", bit = (uint) ConjureArcadeControllerInfo.ButtonBit.Two, displayName = "Button 2", shortDisplayName = "2")]
        [InputControl(name = "button3", layout = "Button", bit = (uint) ConjureArcadeControllerInfo.ButtonBit.Three, displayName = "Button 3", shortDisplayName = "3")]
        [InputControl(name = "buttonA", layout = "Button", bit = (uint) ConjureArcadeControllerInfo.ButtonBit.A, displayName = "Button A", shortDisplayName = "A")]
        [InputControl(name = "buttonB", layout = "Button", bit = (uint) ConjureArcadeControllerInfo.ButtonBit.B, displayName = "Button B", shortDisplayName = "B")]
        [InputControl(name = "buttonC", layout = "Button", bit = (uint) ConjureArcadeControllerInfo.ButtonBit.C, displayName = "Button C", shortDisplayName = "C")]
        [FieldOffset(ConjureArcadeControllerInfo.ButtonByte)]
        public byte buttons;
    }
}
#endif