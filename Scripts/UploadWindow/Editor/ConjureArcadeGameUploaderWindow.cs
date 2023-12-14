using ConjureOS.Metadata;
using ConjureOS.Metadata.Editor;
using ConjureOS.CustomWindow;
using UnityEditor;
using UnityEngine;
using ConjureOS.WebServer;
using System;

namespace ConjureOS.UploaderWindow.Editor
{
    public class ConjureArcadeGameUploaderWindow : EditorWindow
    {
        private const string WindowName = "Arcade Game Uploader";
        private const int minWidth = 500;
        private const int minHeight = 320;

        // Game Metadata
        private ConjureArcadeMetadata metadata;
        private ConjureArcadeMetadataValidator metadataValidator;
        private ConjureArcadeUploadProcessor uploadProcessor;

        // Web server
        private ConjureArcadeWebServerManager webServerManager;

        private void OnEnable()
        {
            titleContent = new GUIContent(WindowName);
            minSize = new Vector2(minWidth, minHeight);
            
            metadataValidator = new ConjureArcadeMetadataValidator();
            uploadProcessor = new ConjureArcadeUploadProcessor();

            webServerManager = ConjureArcadeWebServerManager.Instance;

            if (!metadata)
            {
                metadata = ConjureMetadataLoader.Metadata;
            }
        }

        private void OnGUI()
        {
            // Add padding to the window
            int sectionSpace = 20;
            int uniformPadding = ConjureArcadeGUI.Style.UniformPadding;
            RectOffset padding = new RectOffset(uniformPadding, uniformPadding, 0, 0);
            Rect area = new Rect(
                padding.right,
                padding.top,
                position.width - (padding.left),
                position.height - (padding.top + padding.bottom));

            GUILayout.BeginArea(area);


            // Metadata section
            GUILayout.Label("METADATA", EditorStyles.boldLabel);
            GUILayout.Label("Be sure to have valid metadata before uploading the game to the web server.", EditorStyles.wordWrappedLabel);
            
            ShowValidationResultMessage();
            if (GUILayout.Button("Validate Metadata", GUILayout.Width(150)))
            {
                _ = metadataValidator.ValidateMetadata(metadata);
            }

            GUILayout.Space(sectionSpace);


            // Build and Upload section
            bool isLogged = webServerManager.IsLogged();

            EditorGUI.BeginDisabledGroup(!webServerManager.IsLogged());

            GUILayout.Label("BUILD AND UPLOAD", EditorStyles.boldLabel);
            GUILayout.Label("When ready, you can build and upload the game to the web server.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Build & Upload", GUILayout.Width(150)))
            {
                uploadProcessor.BuildAndUpload(metadata, metadataValidator);
                GUIUtility.ExitGUI();
            }

            EditorGUI.EndDisabledGroup();

            if (!isLogged)
            {
                GUILayout.Label(
                    $"IMPORTANT: You must be logged in in order to build and upload the game.{Environment.NewLine}" +
                    $"You can access the web server settings via the top menu Conjure Arcade > Web Server Settings",
                    ConjureArcadeGUI.Style.ErrorStyle);
            }

            GUILayout.Space(sectionSpace);

            // Utilities sections
            GUILayout.Label("UTILITIES", EditorStyles.boldLabel);
            GUILayout.Label("This will generate a sample of 'metadata.txt' using current metadata.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Generate 'metadata.txt'", GUILayout.Width(150)))
            {
                uploadProcessor.GenerateMetadataFileAt(metadata, metadataValidator);
            }

            GUILayout.Space(10);

            GUILayout.Label("This will generate a '.conj' file at the selected location.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Generate '.conj' file", GUILayout.Width(150)))
            {
                uploadProcessor.GenerateConjFileAt(metadata, metadataValidator);
            }

            GUILayout.EndArea();
        }

        private void ShowValidationResultMessage()
        {
            MetadataValidationStateType validationState = metadataValidator.GetValidationStateType();

            switch (validationState)
            {
                case MetadataValidationStateType.NotVerified:
                    // No message is displayed
                    return;

                case MetadataValidationStateType.Validated:
                    // No error, means it's a success
                    GUILayout.Label(ConjureArcadeGUI.Message.MetadataConfirmedMessage, ConjureArcadeGUI.Style.SuccessStyle);
                    GUILayout.Space(3);
                    break;

                case MetadataValidationStateType.BasicValidated:
                    // No error during basic validation
                    GUILayout.Label(ConjureArcadeGUI.Message.MetadataBasicValidationIndicator, ConjureArcadeGUI.Style.SuccessStyle);
                    GUILayout.Space(3);
                    break;

                case MetadataValidationStateType.Failed:
                    // Errors were detected
                    int errorCount = metadataValidator.Errors.GetErrorCount();
                    string plural = (errorCount > 1) ? "s" : "";
                    string formatedMessage =
                        string.Format(ConjureArcadeGUI.Message.MetadataFailedMessage, errorCount, plural) +
                        string.Format(" Please, fix the error{0} in the Game Metadata window ('Arcade > Game Metadata Editor').", plural);
                    GUILayout.Label(formatedMessage, ConjureArcadeGUI.Style.ErrorStyle);
                    GUILayout.Space(3);
                    break;
            }
        }
    }
}