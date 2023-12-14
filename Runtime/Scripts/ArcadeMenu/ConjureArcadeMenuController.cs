using ConjureOS.Input;
using UnityEngine;
using ConjureOS.ResourcesLoader;
using ConjureOS.Logger;

#if !ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
using System;
using ConjureOS.Settings;
#endif

namespace ConjureOS.ArcadeMenu
{
    public class ConjureArcadeMenuController : MonoBehaviour
    {
        public const string ArcadeMenuObjectName = "ConjureArcadeMenu";

#if !ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
        private enum ConjureHorizontalAxisPressedDirection { Middle, Left, Right }
        private bool isHorizontalAxisPressed;
#endif
        
        public bool IsOpened { get; private set; } = false;
        
        private void Start()
        {
#if !ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            ConjureSettings settings = ConjureSettingsLoader.Settings;
            if (!IsButtonAvailable(settings.HomeButton))
            {
                ConjureArcadeLogger.LogWarning(
                    $"{nameof(settings.HomeButton)} in {nameof(ConjureArcadeMenu)} must be a valid action name ('{settings.HomeButton}' is not set up). " +
                    "To modify actions, go to 'Edit > Project Settings > Input Manager'.");
            }
            
            if (!IsButtonAvailable(settings.SelectButton))
            {
                ConjureArcadeLogger.LogWarning(
                    $"{nameof(settings.SelectButton)} in {nameof(ConjureArcadeMenu)} must be a valid action name ('{settings.SelectButton}' is not set up). " +
                    "To modify actions, go to 'Edit > Project Settings > Input Manager'.");
            }
            
            if (!IsButtonAvailable(settings.BackButton))
            {
                ConjureArcadeLogger.LogWarning(
                    $"{nameof(settings.BackButton)} in {nameof(ConjureArcadeMenu)} must be a valid action name ('{settings.BackButton}' is not set up). " +
                    "To modify actions, go to 'Edit > Project Settings > Input Manager'.");
            }
            
            if (!IsAxisAvailable(settings.HorizontalAxis))
            {
                ConjureArcadeLogger.LogWarning(
                    $"{nameof(settings.HorizontalAxis)} in {nameof(ConjureArcadeMenu)} must be a valid action name ('{settings.HorizontalAxis}' is not set up). " +
                    "To modify actions, go to 'Edit > Project Settings > Input Manager'.");
            }
#endif
            CreateArcadeMenu();
        }

        private void CreateArcadeMenu()
        {
            if (ConjureArcadeMenu.HasInstance)
            {
                return;
            }

            ConjureResources resource = ConjureResourceLoader.Resource;
            if (!resource)
            {
                ConjureArcadeLogger.LogError($"Could not create the arcade menu because no {nameof(ConjureResources)} was found using the {nameof(ConjureResourceLoader)}.");
                return;
            }

            if (!resource.ConjureMenuPrefab)
            {
                ConjureArcadeLogger.LogError($"Could not create the arcade menu because no {nameof(resource.ConjureMenuPrefab)} in the {nameof(ConjureResources)}.");
                return;
            }

            GameObject arcadeMenuGameObject = Instantiate(resource.ConjureMenuPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            if (!arcadeMenuGameObject)
            {
                ConjureArcadeLogger.LogError($"There was a problem creating the {nameof(arcadeMenuGameObject)}");
                return;
            }

            arcadeMenuGameObject.name = ArcadeMenuObjectName;
            if (!arcadeMenuGameObject.GetComponent<ConjureArcadeMenu>())
            {
                ConjureArcadeMenu arcadeMenu = arcadeMenuGameObject.AddComponent<ConjureArcadeMenu>();
                if (!arcadeMenu)
                {
                    ConjureArcadeLogger.LogError($"There was a problem creating the {nameof(arcadeMenu)}");
                    return;
                }
            }

            DontDestroyOnLoad(arcadeMenuGameObject);
        }
        
#if !ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
    private bool IsButtonAvailable(string buttonName)
    {
        try
        {
            UnityEngine.Input.GetButton(buttonName);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
    
    private bool WasButtonPressedThisFrame(string buttonName)
    {
        try
        {
            return UnityEngine.Input.GetButtonDown(buttonName);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
    
    private bool IsAxisAvailable(string axisName)
    {
        try
        {
            UnityEngine.Input.GetAxis(axisName);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
    
    private bool WasAxisPressed(string axisName)
    {
        try
        {
            return UnityEngine.Input.GetAxisRaw(axisName) != 0;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
    
    private ConjureHorizontalAxisPressedDirection GetAxisValue(string axisName)
    {
        try
        {
            float axisValue = UnityEngine.Input.GetAxisRaw(axisName);
            if (axisValue > 0)
            {
                return ConjureHorizontalAxisPressedDirection.Right;
            }
            else if (axisValue < 0)
            {
                return ConjureHorizontalAxisPressedDirection.Left;
            }
            else
            {
                return ConjureHorizontalAxisPressedDirection.Middle;
            }
        }
        catch (ArgumentException)
        {
            return ConjureHorizontalAxisPressedDirection.Middle;
        }
    }
#endif

        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
            foreach (ConjureArcadeController arcadeController in ConjureArcadeController.allControllers)
            {
                if (arcadeController.home.wasPressedThisFrame)
                {
                    ToggleArcadeMenu();
                }
                else if (arcadeController.stick.left.wasPressedThisFrame)
                {
                    ConjureArcadeMenu.MovePrevious();
                }
                else if (arcadeController.stick.right.wasPressedThisFrame)
                {
                    ConjureArcadeMenu.MoveNext();
                }
                else if (arcadeController.button1.wasPressedThisFrame || 
                         arcadeController.buttonA.wasPressedThisFrame || 
                         arcadeController.start.wasPressedThisFrame)
                {
                    ConjureArcadeMenu.Select();
                }
                else if (arcadeController.button2.wasPressedThisFrame || 
                         arcadeController.buttonB.wasPressedThisFrame)
                {
                    ConjureArcadeMenu.Close();
                }
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            ConjureSettings settings = ConjureSettingsLoader.Settings;
            if (WasButtonPressedThisFrame(settings.HomeButton))
            {
                ToggleArcadeMenu();
            }
            else if (WasButtonPressedThisFrame(settings.SelectButton))
            {
                ConjureArcadeMenu.Select();
            }
            else if (WasButtonPressedThisFrame(settings.BackButton))
            {
                ConjureArcadeMenu.Close();
            }

            if (WasAxisPressed(settings.HorizontalAxis))
            {
                if (!isHorizontalAxisPressed)
                {
                    isHorizontalAxisPressed = true;

                    switch (GetAxisValue(settings.HorizontalAxis))
                    {
                        case ConjureHorizontalAxisPressedDirection.Left:
                            ConjureArcadeMenu.MovePrevious();
                            break;
                        case ConjureHorizontalAxisPressedDirection.Right:
                            ConjureArcadeMenu.MoveNext();
                            break;
                    }
                }
            }
            else
            {
                isHorizontalAxisPressed = false;
            }
#endif
        }

        private void ToggleArcadeMenu()
        {
            if (ConjureArcadeMenu.IsOpened)
            {
                ConjureArcadeMenu.Close();
            }
            else
            {
                ConjureArcadeMenu.Open();
            }
        }
    }
}