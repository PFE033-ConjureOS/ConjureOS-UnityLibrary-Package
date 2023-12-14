#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

// ReSharper disable InvalidXmlDocComment
// Reason: We don't want to comment the "this" parameter in the extension functions.

namespace ConjureOS.Input
{
    public static class InputActionAssetExtension
    {
        /// <summary>
        /// Assign a specific Conjure Arcade Controller to this Input Action Asset.
        /// All other Conjure Arcade Controllers will be disabled for this asset.
        /// If the controller cannot be found for the controller index, there will be no assigned Conjure Arcade Controller.
        /// </summary>
        /// <param name="keepOtherDevices">
        /// If this is false, all devices will be disabled for this Input Action Asset except for the Conjure Arcade Controller associated to the given controller index.
        /// If this is true, the devices that are not Conjure Arcade Controller (e.g. mouse/keyboard) will not be disabled.
        /// </param>
        public static void AssignConjureController(this InputActionAsset inputActionAsset, int controllerIndex, bool keepOtherDevices = true)
        {
            List<InputDevice> inputDevices = new List<InputDevice>();
            if (keepOtherDevices)
            {
                if (inputActionAsset.devices == null)
                {
                    inputDevices.AddRange(InputSystem.devices);
                }
                else
                {
                    inputDevices = new List<InputDevice>(inputActionAsset.devices);
                }
                
                inputDevices.RemoveAll(inputDevice => inputDevice is ConjureArcadeController);
            }

            ConjureArcadeController controller = ConjureArcadeController.GetForIndex(controllerIndex);
            if (controller != null)
            {
                inputDevices.Add(controller);
            }

            inputActionAsset.devices = inputDevices.ToArray();
        }

        /// <summary>
        /// Whether or not it is possible to assign a specific controller index to this Input Action Asset.
        /// </summary>
        public static bool CanAssignConjureController(this InputActionAsset inputActionAsset, int controllerIndex)
        {
            return ConjureArcadeController.ExistForIndex(controllerIndex);
        }

        /// <summary>
        /// Get the controller index of the Conjure Arcade Controller associated with this Input Action Asset.
        /// </summary>
        /// <returns>
        /// The controller index found.
        /// If there was no Conjure Arcade Controller associated with this Input Action Asset, return false.
        /// If there was multiple Conjure Arcade Controllers associated with this Input Action Asset, return the index of the first controller found.
        /// </returns>
        public static int GetConjureControllerIndex(this InputActionAsset inputActionAsset)
        {
            ConjureArcadeController[] controllers = GetConjureArcadeControllersFromDevices(inputActionAsset.devices);
            if (controllers.Length == 0)
            {
                return -1;
            }

            return controllers[0].ControllerIndex;
        }
        
        /// <summary>
        /// Get the controller index of all the Conjure Arcade Controllers associated with this Input Action Asset.
        /// </summary>
        /// <returns>
        /// The controller indexes found.
        /// If there was no Conjure Arcade Controller associated with this Input Action Asset, return an empty array.
        /// </returns>
        public static int[] GetConjureControllerIndexes(this InputActionAsset inputActionAsset)
        {
            ConjureArcadeController[] controllers = GetConjureArcadeControllersFromDevices(inputActionAsset.devices);
            return controllers.Select(controller => controller.ControllerIndex).ToArray();
        }

        private static ConjureArcadeController[] GetConjureArcadeControllersFromDevices(ReadOnlyArray<InputDevice>? devices)
        {
            if (devices == null)
            {
                return ConjureArcadeController.allControllers;
            }

            List<ConjureArcadeController> controllers = new List<ConjureArcadeController>();
            foreach (InputDevice inputDevice in devices)
            {
                if (inputDevice is ConjureArcadeController device)
                {
                    controllers.Add(device);
                }
            }

            return controllers.ToArray();
        }
    }
}
#endif