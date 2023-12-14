
using ConjureOS.Library;
using ConjureOS.Logger;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ConjureOS.Settings
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class ConjureSettingsLoader
    {
        private const string SettingsNamePrefix = "ConjureSettings";
        
        private static string SettingsName => $"{SettingsNamePrefix}.asset";
        private static string FullPath => $"Assets/{Path}";
        private static string Path => $"{ConjureDataFolders.ConjureResourceFolderPath}/{SettingsName}";
        private static string ResourcePath => $"{ConjureDataFolders.ConjureResourceFolderName}/{SettingsNamePrefix}";
        
#if UNITY_EDITOR
        static ConjureSettingsLoader()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                InitializeSettingsAsset();
            }
        }

        private static void InitializeSettingsAsset()
        {
            ConjureDataFolders.CreateFolderStructure();
            
            bool settingsAssetExists = !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(FullPath, AssetPathToGUIDOptions.OnlyExistingAssets)); 
            if (!settingsAssetExists) 
            {
                ConjureSettings createdSettings = ScriptableObject.CreateInstance<ConjureSettings>(); 
                if (createdSettings)
                {
                    AssetDatabase.CreateAsset(createdSettings, FullPath);
                    ConjureArcadeLogger.Log($"Settings file created ({Path})");
                }
            }
        }
#endif
        
        private static ConjureSettingsLoader instance;
        
        public static ConjureSettings Settings
        {
            get
            {
#if UNITY_EDITOR
                // In case the folder or the asset was deleted
                InitializeSettingsAsset();
#endif
                
                if (instance == null)
                {
                    instance = new ConjureSettingsLoader();
                }

                return instance.settings != null ? instance.settings : ScriptableObject.CreateInstance<ConjureSettings>();
            }
        }
        private readonly ConjureSettings settings;

        private ConjureSettingsLoader()
        {
            settings = Resources.Load<ConjureSettings>(ResourcePath);
            if (!settings) 
            {
                ConjureArcadeLogger.LogError($"Could not load {settings} from the path {ResourcePath}");
            }
        }
    }
}