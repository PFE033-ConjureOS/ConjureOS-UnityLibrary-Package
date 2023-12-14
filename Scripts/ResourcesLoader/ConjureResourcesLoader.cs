using ConjureOS.Library;
using UnityEngine;
using ConjureOS.Logger;

namespace ConjureOS.ResourcesLoader
{
    public class ConjureResourceLoader
    {
        private static string ConjureResourcePath => $"{ConjureDataFolders.ConjureResourceFolderName}/ConjureResources";

        private static ConjureResourceLoader instance;

        public static ConjureResources Resource
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConjureResourceLoader();
                }

                return instance.resources;
            }
        }
        private readonly ConjureResources resources;

        private ConjureResourceLoader()
        {
            resources = Resources.Load<ConjureResources>(ConjureResourcePath);
            if (!resources)
            {
                ConjureArcadeLogger.LogError($"Could not load {resources} from the path {ConjureResourcePath}");
            }
        }
    }
}