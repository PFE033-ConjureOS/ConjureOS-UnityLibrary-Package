using UnityEngine;

namespace ConjureOS.CustomWindow 
{
    public class ConjureArcadeGUI
    {
        public struct Style
        {
            public const int UniformPadding = 10;
            public const int TextSizeError = 10;
            public readonly static Color32 ColorError = new Color(0.84f, 0.19f, 0.19f);
            public readonly static Color32 ColorSuccess = new Color(0.19f, 0.84f, 0.35f);

            public static GUIStyle ErrorStyle = GenerateGUIStyle(ColorError);
            public static GUIStyle SuccessStyle = GenerateGUIStyle(ColorSuccess);

            private static GUIStyle GenerateGUIStyle(Color32 color)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = color;
                style.fontSize = TextSizeError;
                style.wordWrap = true;

                return style;
            }
        }

        public struct Message
        {
            public const string MetadataFailedMessage = "{0} error{1} have been detected.";
            public const string MetadataConfirmedMessage = "No errors detected. The game is ready to be published.";
            public const string MetadataBasicValidationIndicator = 
                "Basic validation executed. No errors detected. " +
                "Server must still validate metadata before publishing the game.";
        }
    }
}