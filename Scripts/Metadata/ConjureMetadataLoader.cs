using ConjureOS.Library;
using ConjureOS.Logger;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ConjureOS.Metadata
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class ConjureMetadataLoader
    {
        private const string MetadataNamePrefix = "ConjureMetadata";
        
        private static string MetadataName => $"{MetadataNamePrefix}.asset";
        private static string FullPath => $"Assets/{Path}";
        private static string Path => $"{ConjureDataFolders.ConjureResourceFolderPath}/{MetadataName}";
        private static string ResourcePath => $"{ConjureDataFolders.ConjureResourceFolderName}/{MetadataNamePrefix}";
        
#if UNITY_EDITOR
        static ConjureMetadataLoader()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                InitializeMetadataAsset();
            }
        }

        private static void InitializeMetadataAsset()
        {
            ConjureDataFolders.CreateFolderStructure();
            
            bool settingsAssetExists = !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(FullPath, AssetPathToGUIDOptions.OnlyExistingAssets)); 
            if (!settingsAssetExists) 
            {
                ConjureArcadeMetadata createdSettings = ScriptableObject.CreateInstance<ConjureArcadeMetadata>(); 
                if (createdSettings)
                {
                    AssetDatabase.CreateAsset(createdSettings, FullPath);
                    ConjureArcadeLogger.Log($"Metadata file created ({Path})");
                }
            }
        }
#endif
        
        private static ConjureMetadataLoader instance;
        
        public static ConjureArcadeMetadata Metadata
        {
            get
            {
#if UNITY_EDITOR
                // In case the folder or the asset was deleted
                InitializeMetadataAsset();
#endif
                
                if (instance == null)
                {
                    instance = new ConjureMetadataLoader();
                }

                return instance.metadata != null ? instance.metadata : ScriptableObject.CreateInstance<ConjureArcadeMetadata>();
            }
        }
        private readonly ConjureArcadeMetadata metadata;

        private ConjureMetadataLoader()
        {
            metadata = Resources.Load<ConjureArcadeMetadata>(ResourcePath);
            if (!metadata) 
            {
                ConjureArcadeLogger.LogError($"Could not load {metadata} from the path {ResourcePath}");
            }
        }
    }
}