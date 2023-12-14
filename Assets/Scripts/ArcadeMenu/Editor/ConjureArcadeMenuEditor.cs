using UnityEditor;
using UnityEngine;

namespace ConjureOS.ArcadeMenu.Editor
{
    [CustomEditor(typeof(ConjureArcadeMenu))]
    public class ConjureArcadeMenuEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            GUI.enabled = !ConjureArcadeMenu.IsOpened;
            if (GUILayout.Button("Open"))
            {
                ConjureArcadeMenu.Open();
            }
            
            GUI.enabled = ConjureArcadeMenu.IsOpened;
            if (GUILayout.Button("Close"))
            {
                ConjureArcadeMenu.Close();
            }
            if (GUILayout.Button("Select"))
            {
                ConjureArcadeMenu.Select();
            }
            if (GUILayout.Button("Move Next"))
            {
                ConjureArcadeMenu.MoveNext();
            }
            if (GUILayout.Button("Move Previous"))
            {
                ConjureArcadeMenu.MovePrevious();
            }
            GUI.enabled = true;

            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
        }
    }
}