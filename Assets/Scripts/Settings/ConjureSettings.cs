using System;
using UnityEditor;
using UnityEngine;

namespace ConjureOS.Settings
{
    [Serializable]
    public class ConjureSettings : ScriptableObject
    {
#if !ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
        [Header("Home menu buttons")]
        [SerializeField]
        private string homeButton = "Home";
        
        [SerializeField]
        private string selectButton = "Select";
        
        [SerializeField]
        private string backButton = "Back";
        
        [SerializeField]
        private string horizontalAxis = "Horizontal";
        
        public string HomeButton => homeButton;
        public string SelectButton => selectButton;
        public string BackButton => backButton;
        public string HorizontalAxis => horizontalAxis;
#endif

        [SerializeField] 
        private string leaderboardStrategy = "Local";
        public string LeaderboardStrategy => leaderboardStrategy;

        [Header("Web settings")]
        [SerializeField]
        [Tooltip("Address of the web server.")]
        private string address;
        public string Address => address;

#if UNITY_EDITOR
        public SerializedObject GetSerialized()
        {
            return new SerializedObject(this);
        }
#endif
    }
}