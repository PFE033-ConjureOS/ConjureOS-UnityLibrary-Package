using ConjureOS.Logger;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ConjureOS.Library
{
    public static class ConjureDataFolders
    {
        public const string DataFolderName = "ConjureOS_Data";
        public const string ResourceFolderName = "Resources";
        public const string ConjureResourceFolderName = "Conjure";
           
        
        public static string DataFolderFullPath => $"Assets/{DataFolderPath}";
        public static string ResourceFolderFullPath => $"Assets/{ResourceFolderPath}";
        public static string ConjureResourceFolderFullPath => $"Assets/{ConjureResourceFolderPath}";
        
        public static string DataFolderPath => $"{DataFolderName}";
        public static string ResourceFolderPath => $"{DataFolderName}/{ResourceFolderName}";
        public static string ConjureResourceFolderPath => $"{DataFolderName}/{ResourceFolderName}/{ConjureResourceFolderName}";
        
        public static void CreateFolderStructure()
        {
#if UNITY_EDITOR
            if (!AssetDatabase.IsValidFolder($"Assets/{DataFolderName}"))
            {
                AssetDatabase.CreateFolder("Assets", DataFolderName);
                ConjureArcadeLogger.Log($"Data folder created ({DataFolderName})");
            }
            
            if (!AssetDatabase.IsValidFolder($"Assets/{DataFolderName}/{ResourceFolderName}"))
            {
                AssetDatabase.CreateFolder($"Assets/{DataFolderName}", ResourceFolderName);
                ConjureArcadeLogger.Log($"Resource folder created ({DataFolderName}/{DataFolderName})");
            }
            
            if (!AssetDatabase.IsValidFolder($"Assets/{DataFolderName}/{ResourceFolderName}/{ConjureResourceFolderName}"))
            {
                AssetDatabase.CreateFolder($"Assets/{DataFolderName}/{ResourceFolderName}", ConjureResourceFolderName);
                ConjureArcadeLogger.Log($"Resource folder created ({DataFolderName}/{DataFolderName}/{ConjureResourceFolderName})");
            }
#endif
        }
    }
}