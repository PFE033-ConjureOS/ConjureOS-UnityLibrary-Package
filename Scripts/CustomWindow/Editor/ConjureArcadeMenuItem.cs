#if UNITY_EDITOR
using ConjureOS.Metadata.Editor;
using ConjureOS.UploaderWindow.Editor;
using ConjureOS.WebServer.Editor;
using UnityEditor;

namespace ConjureOS.CustomWindow
{
    public class ConjureArcadeMenuItem
    {
        private const string MenuName = "Conjure Arcade/";

        [MenuItem(MenuName + "Game Metadata Editor")]
        private static void OpenGameMetadataWindow()
        {
            EditorWindow.GetWindow(typeof(ConjureArcadeMetadataWindow));
        }

        [MenuItem(MenuName + "Upload Game")]
        private static void OpenUploadGameWindow()
        {
            EditorWindow.GetWindow(typeof(ConjureArcadeGameUploaderWindow));
        }

        [MenuItem(MenuName + "Web Server Settings")]
        private static void OpenWebServerSettingsWindow()
        {
            EditorWindow.GetWindow(typeof(ConjureArcadeWebServerSettingsWindow));
        }
    }
}
#endif