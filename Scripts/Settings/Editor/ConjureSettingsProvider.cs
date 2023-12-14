using System.Collections.Generic;
using ConjureOS.Library;
using UnityEditor;
using UnityEngine.UIElements;

namespace ConjureOS.Settings.Editor
{
    public class ConjureSettingsProvider : SettingsProvider
    {
        private SerializedObject serializedSettings;

        private static bool IsSettingsAvailable()
        {
            return ConjureSettingsLoader.Settings != null;
        }
        
        public ConjureSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : 
            base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            ConjureSettings settings = ConjureSettingsLoader.Settings;
            serializedSettings = settings.GetSerialized();
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.LabelField($"Version {ConjureLibraryVersion.Version.Full}");

            serializedSettings.Update();
            SerializedProperty property = serializedSettings.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name == "m_Script")
                    {
                        continue;
                    }
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty(property.name), true);
                } while (property.NextVisible(false));
            }
            serializedSettings.ApplyModifiedProperties();
        }
        
        [SettingsProvider]
        private static SettingsProvider CreateConjureSettingsProvider()
        {
            if (IsSettingsAvailable())
            {
                var provider = new ConjureSettingsProvider("Project/Conjure", SettingsScope.Project);
                return provider;
            }

            return null;
        }
    }
}