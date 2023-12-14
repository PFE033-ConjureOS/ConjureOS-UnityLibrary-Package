#if ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// Reason: It's normal that some stuff is not used here because this is meant to be used as a library.

// ReSharper disable InconsistentNaming
// Reason: According to our code guidelines, property should be UpperCamelCase.
//         However, to be consistent with other input devices in Unity (e.g. Joystick, Gamepad) we will use lowerCamelCase here.

namespace ConjureOS.Input
{
    [InputControlLayout(stateType = typeof(ConjureArcadeControllerState), displayName = "Conjure Arcade Controller")]
    public class ConjureArcadeController : InputDevice
    {
        public static event Action<ConjureArcadeController> OnControllerAdded;
        public static event Action<ConjureArcadeController> OnControllerRemoved;

        public ConjureArcadeStickControl stick { get; protected set; }
        
        public ButtonControl home { get; protected set; }
        public ButtonControl start { get; protected set; }
        
        public ButtonControl button1 { get; protected set; }
        public ButtonControl button2 { get; protected set; }
        public ButtonControl button3 { get; protected set; }

        public ButtonControl buttonA { get; protected set; }
        public ButtonControl buttonB { get; protected set; }
        public ButtonControl buttonC { get; protected set; }
        
        public static ConjureArcadeController current { get; private set; }
        public static ConjureArcadeController[] allControllers => allInstances.ToArray();
        public static int ControllerCount => count;

        /// <summary>
        /// The controller index of this specific Conjure Arcade Controller.
        /// The value will be between 0 and <see cref="ControllerCount"/> - 1.
        /// If the value is -1, it means this controller was not initialized correctly and is not valid.
        /// </summary>
        public int ControllerIndex
        {
            get
            {
                try
                {
                    return allInstances.IndexOf(this);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Whether or not a Conjure Arcade Controller exist for the following index.
        /// </summary>
        public static bool ExistForIndex(int controllerIndex)
        {
            return allInstances.Exists(instance => instance.ControllerIndex == controllerIndex);
        }

        /// <summary>
        /// Get the Conjure Arcade Controller associated with the specific controller index.
        /// </summary>
        /// <returns>The controller if it exist for the specific index or null if it does not exist.</returns>
        public static ConjureArcadeController GetForIndex(int controllerIndex)
        {
            return allInstances.Find(instance => instance.ControllerIndex == controllerIndex);
        }

        protected override void FinishSetup()
        {
            stick = GetChildControl<ConjureArcadeStickControl>("stick");

            home = GetChildControl<ButtonControl>("home");
            start = GetChildControl<ButtonControl>("start");
            
            button1 = GetChildControl<ButtonControl>("button1");
            button2 = GetChildControl<ButtonControl>("button2");
            button3 = GetChildControl<ButtonControl>("button3");
            
            buttonA = GetChildControl<ButtonControl>("buttonA");
            buttonB = GetChildControl<ButtonControl>("buttonB");
            buttonC = GetChildControl<ButtonControl>("buttonC");
            
            base.FinishSetup();
        }
        
        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }
        
        protected override void OnAdded()
        {
            base.OnAdded();

            if (!allInstances.Contains(this))
            {
                allInstances.Add(this);
                ++count;
                OnControllerAdded?.Invoke(this);
            }
        }

        protected override void OnRemoved()
        {
            if (current == this)
            {
                current = null;
            }

            if (allInstances.Remove(this))
            {
                --count;
                OnControllerRemoved?.Invoke(this);
            }
            
            base.OnRemoved();
        }

        private static int count;
        private static readonly List<ConjureArcadeController> allInstances = new();
    }
}
#endif