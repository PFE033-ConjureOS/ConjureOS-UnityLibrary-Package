using ConjureOS.CustomWindow;
using ConjureOS.Logger;
using ConjureOS.Settings;
using UnityEditor;
using UnityEngine;

namespace ConjureOS.WebServer.Editor
{
    public class ConjureArcadeWebServerSettingsWindow : EditorWindow
    {
        private const string WindowName = "Arcade Web Server Settings";
        private const int minWidth = 400;
        private const int minHeight = 150;

        // Web server info
        private ConjureArcadeWebServerManager webServerManager;
        private ConjureSettings webServerSettings;
        private SerializedObject serializedObject;

        private static string username;
        private string password;
        private LoginInfo loginInfo;

        private void OnEnable()
        {
            titleContent = new GUIContent(WindowName);
            minSize = new Vector2(minWidth, minHeight);

            webServerManager = ConjureArcadeWebServerManager.Instance;
            webServerManager.OnLoginAttempt += SetLoginMessage;

            webServerSettings = ConjureSettingsLoader.Settings;
            serializedObject = new SerializedObject(webServerSettings);
        }

        private void OnGUI()
        {
            serializedObject.Update();

            // Add padding to the window
            int globalSpace = 2;
            int sectionSpace = 5;
            int uniformPadding = 5;
            RectOffset padding = new RectOffset(uniformPadding, uniformPadding, uniformPadding, uniformPadding);
            Rect area = new Rect(
                2 * padding.right,
                padding.top,
                position.width - (2 * padding.left + 2 * padding.right),
                position.height - (padding.top + padding.bottom));

            GUILayout.BeginArea(area);

            // Web Server Settings section
            GUILayout.Label("WEB SERVER SETTINGS", EditorStyles.boldLabel);
            GUILayout.Space(sectionSpace);

            // Disabled if logged in - START
            EditorGUI.BeginDisabledGroup(webServerManager.IsLogged());

            GUILayout.Space(sectionSpace);

            GUILayout.Label("User Settings", EditorStyles.boldLabel);

            username = EditorGUILayout.TextField("Username", username);
            GUILayout.Space(globalSpace);

            password = EditorGUILayout.PasswordField("Password", password);
            GUILayout.Space(globalSpace);

            // Validate properties and save data
            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(10);

            // Login / Logout buttons line
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Login", GUILayout.Width(75)))
            {
                // We discard the warning because there's no need to await the request in a UI context
                _ = webServerManager.Login(username, password);
            }

            EditorGUI.EndDisabledGroup();
            // Disabled on logged in - END

            // Enabled if logged in - START
            EditorGUI.BeginDisabledGroup(!webServerManager.IsLogged());
            if (GUILayout.Button("Logout", GUILayout.Width(75)))
            {
                webServerManager.Logout();
            }
            EditorGUI.EndDisabledGroup();
            // Enabled on logged in - END

            GUILayout.EndHorizontal();

            ShowLoginMessage();

            GUILayout.EndArea();
        }

        private void ShowLoginMessage()
        {
            GUIStyle style = EditorStyles.wordWrappedMiniLabel;
            style.normal.textColor = GUI.contentColor; // Reset the color
            string message = "";
            if (webServerManager.IsLogged())
            {
                message = $"Logged in as '{webServerManager.GetUsername()}'";
            }
            else
            {
                if (loginInfo.type == LogMessageType.Error)
                {
                    style.normal.textColor = ConjureArcadeGUI.Style.ColorError;
                }

                message = loginInfo.message;
            }

            GUILayout.Label(message, style);
        }

        private void SetLoginMessage(string message, LogMessageType type)
        {
            loginInfo.message = message;
            loginInfo.type = type;
        }

        private struct LoginInfo
        {
            public string message;
            public LogMessageType type;
        }
    }
}