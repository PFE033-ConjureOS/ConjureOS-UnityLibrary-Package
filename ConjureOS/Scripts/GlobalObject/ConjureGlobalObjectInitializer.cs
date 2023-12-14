using ConjureOS.ArcadeMenu;
using ConjureOS.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ConjureOS.GlobalObject
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class ConjureGlobalObjectInitializer
    {
        public const string GlobalObjectName = "ConjureGlobalObject";
        
        public static readonly System.Type[] GlobalObjectComponentTypes =
        {
            typeof(ConjureArcadeMenuController)
        };
        
#if UNITY_EDITOR
        static ConjureGlobalObjectInitializer()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                InitializeGlobalObject();
            }
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            // Unsubscribe before subscribing to the event to be sure the method is not called twice.
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            InitializeGlobalObject();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            InitializeGlobalObject();
        }
        
        private static void InitializeGlobalObject()
        {
            GameObject globalObject = GameObject.Find(GlobalObjectName);
            if (!globalObject)
            {
                globalObject = new GameObject(GlobalObjectName);
            }
            InitializeComponentsOnGlobalObject(globalObject);
        }

        private static void InitializeComponentsOnGlobalObject(GameObject globalObject)
        {
            foreach (var globalObjectComponentType in GlobalObjectComponentTypes)
            {
                if (globalObject.GetComponent(globalObjectComponentType))
                {
                    continue;
                }
                
                if (!globalObjectComponentType.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    ConjureArcadeLogger.LogError($"Cannot add component '{nameof(globalObjectComponentType.FullName)}' to global object because it is not a component.");
                    continue;
                }
            
                globalObject.AddComponent(globalObjectComponentType);
            }
        }
    }
}