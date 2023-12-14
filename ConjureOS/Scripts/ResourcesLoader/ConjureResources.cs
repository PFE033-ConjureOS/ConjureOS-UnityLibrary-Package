using UnityEngine;

namespace ConjureOS.ResourcesLoader
{
    public class ConjureResources : ScriptableObject
    {
        [SerializeField] 
        private GameObject conjureMenuPrefab;

        public GameObject ConjureMenuPrefab => conjureMenuPrefab;
    }
}