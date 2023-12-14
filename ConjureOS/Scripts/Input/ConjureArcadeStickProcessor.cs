#if ENABLE_INPUT_SYSTEM
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

// ReSharper disable ConvertToConstant.Local
// Reason: Attributes cannot be made into constants here since they are changed by Unity's internal input system.

// Those classes exist to allow the conversion of the stick input into a normalized (-1.0, 1.0) range.
// They are registered when ConjureInputSystem is initialized and are used for stick inputs in the controller state and the stick control.

namespace ConjureOS.Input
{
    [UsedImplicitly]
    public class ConjureArcadeVector2Processor : InputProcessor<Vector2>
    {
        private readonly float minX = -1.0f;
        private readonly float maxX = 1.0f;
        private readonly bool invertX = false;
        
        private readonly float minY = -1.0f;
        private readonly float maxY = 1.0f;
        private readonly bool invertY = false;
        
        private readonly float deadZone = 0.1f;
        
        public override Vector2 Process(Vector2 value, InputControl control)
        {
            return new Vector2(
                ConjureArcadeStickProcessorHelper.ProcessValue(value.x, minX, maxX,  deadZone, invertX),
                ConjureArcadeStickProcessorHelper.ProcessValue(value.y, minY, maxY, deadZone, invertY));
        }
    }

    [UsedImplicitly]
    public class ConjureArcadeValueProcessor : InputProcessor<float>
    {
        private readonly float min = -1.0f;
        private readonly float max = 1.0f;
        private readonly bool invert = false;
        
        private readonly float deadZone = 0.1f;
        
        public override float Process(float value, InputControl control)
        {
            return ConjureArcadeStickProcessorHelper.ProcessValue(value, min, max, deadZone, invert);
        }
    }
    
    internal static class ConjureArcadeStickProcessorHelper
    {
        private const float MinStickValue = -1.0f;
        private const float MaxStickValue = 1.0f;
        private const float StickRange = MaxStickValue - MinStickValue;
        
        internal static float ProcessValue(float originalValue, float min, float max, float deadZone, bool invert)
        {
            float zero = (min + max) / 2;
            if (Mathf.Approximately(originalValue, zero))
            {
                return 0.0f;
            }

            float originalRange = max - min;
            if (Mathf.Approximately(originalRange, 0.0f))
            {
                return 0.0f;
            }

            float processedValue = (((originalValue - min) * StickRange) / originalRange) + MinStickValue;
            processedValue = Mathf.Clamp(processedValue, MinStickValue, MaxStickValue);
            processedValue = invert ? -processedValue : processedValue;
            processedValue = processedValue > -deadZone && processedValue < deadZone ? 0.0f : processedValue;
            
            return processedValue;
        }
    }
}
#endif