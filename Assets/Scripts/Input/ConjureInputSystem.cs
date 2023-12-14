using UnityEditor;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem;
#endif

namespace ConjureOS.Input
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class ConjureInputSystem
    {
#if UNITY_EDITOR
        static ConjureInputSystem()
        {
            Initialize();
        }
#endif
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
#if ENABLE_INPUT_SYSTEM
            InputSystem.RegisterProcessor(typeof(ConjureArcadeVector2Processor), "ConjureArcadeVector2");
            InputSystem.RegisterProcessor(typeof(ConjureArcadeValueProcessor), "ConjureArcadeValue");
            
            InputSystem.RegisterLayout<ConjureArcadeStickControl>("ConjureArcadeStick");
            
            InputSystem.RegisterLayout<ConjureArcadeController>(
                matches: new InputDeviceMatcher()
                    .WithInterface(ConjureArcadeControllerInfo.Interface)
                    .WithProduct(ConjureArcadeControllerInfo.Product));
#endif
        }
    }
}
