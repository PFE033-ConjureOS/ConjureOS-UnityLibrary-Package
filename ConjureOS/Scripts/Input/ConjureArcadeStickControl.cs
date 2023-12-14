#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// Reason: It's normal that some stuff is not used here because this is meant to be used as a library.

// ReSharper disable InconsistentNaming
// Reason: According to our code guidelines, property should be UpperCamelCase.
//         However, to be consistent with other input controls in Unity (e.g. StickControl) we will use lowerCamelCase here.

// This class exists to allow the Conjure arcade stick input to work with the input system.
// It uses the ConjureArcadeValue processor in order to define the down/left/right/up buttons.
// It is registered in when ConjureInputSystem is initialized and is used for the stick input in the controller state.

namespace ConjureOS.Input
{
    public class ConjureArcadeStickControl : Vector2Control
    {
        [InputControl(useStateFrom = "y", processors = "ConjureArcadeValue(min=0.0, max=1.0, invert)", synthetic = true, displayName = "Up")]
        [InputControl(name = "x", minValue = -1f, maxValue = 1f, layout = "Axis", processors = "ConjureArcadeValue(min=0.0, max=1.0)", format = "BYTE", sizeInBits = 8)]
        [InputControl(name = "y", minValue = -1f, maxValue = 1f, layout = "Axis", processors = "ConjureArcadeValue(min=0.0, max=1.0, invert)", format ="BYTE", sizeInBits = 8, offset = 1)]
        public ButtonControl up { get; set; }
        
        [InputControl(useStateFrom = "y" , processors = "ConjureArcadeValue(min=0.0, max=1.0)", synthetic = true, displayName = "Down")]
        public ButtonControl down { get; set; }
        
        [InputControl(useStateFrom = "x", processors = "ConjureArcadeValue(min=0.0, max=1.0, invert)", synthetic = true, displayName = "Left")]
        public ButtonControl left { get; set; }
        
        [InputControl(useStateFrom = "x", processors = "ConjureArcadeValue(min=0.0, max=1.0)", synthetic = true, displayName = "Right")]
        public ButtonControl right { get; set; }
    
        protected override void FinishSetup()
        {
            base.FinishSetup();
            up = GetChildControl<ButtonControl>("up");
            down = GetChildControl<ButtonControl>("down");
            left = GetChildControl<ButtonControl>("left");
            right = GetChildControl<ButtonControl>("right");
        }
    }
}
#endif