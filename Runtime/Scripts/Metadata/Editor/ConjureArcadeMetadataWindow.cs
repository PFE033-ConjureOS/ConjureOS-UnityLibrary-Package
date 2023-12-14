using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using ConjureOS.CustomWindow;
using ConjureOS.WebServer;
using System.Collections.Generic;

namespace ConjureOS.Metadata.Editor
{
    public class ConjureArcadeMetadataWindow : EditorWindow
    {
        private const string WindowName = "Arcade Game Metadata Editor";
        private readonly string[] tabs = new string[] { "Metadata Editor", "Online Metadata" };

        // Game Metadata
        private ConjureArcadeMetadata metadata;
        private ConjureArcadeMetadataValidator metadataValidator;
        private ConjureArcadeWebServerManager webServerManager;
        private SerializedObject serializedObject;

        // Metadata Properties
        private SerializedProperty gameTitleProperty;
        private SerializedProperty nextVersionLevelProperty;
        private SerializedProperty descriptionProperty;
        private SerializedProperty minNumPlayerProperty;
        private SerializedProperty maxNumPlayerProperty;
        private SerializedProperty useLeaderboardProperty;
        private SerializedProperty thumbnailProperty;
        private SerializedProperty gameplayImageProperty;
        private SerializedProperty developersProperty;
        private SerializedProperty genresProperty;
        private SerializedProperty publicRepositoryLinkProperty;

        private ReorderableList developersList;
        private ReorderableList genresList;

        // UI Properties
        private Vector2 scrollPos = Vector2.zero;
        private int globalSpace = 2;
        private int uniformPadding = ConjureArcadeGUI.Style.UniformPadding;
        private int currentTab = 0;
        private string[] versionLevelTitles = new string[] { "Major", "Minor", "Patch" };
        private int versionSelectedLevel = 0;

        private void OnEnable()
        {
            metadataValidator = new ConjureArcadeMetadataValidator();
            titleContent = new GUIContent(WindowName);

            if (!metadata)
            {
                metadata = ConjureMetadataLoader.Metadata;
            }

            webServerManager = ConjureArcadeWebServerManager.Instance;

            serializedObject = new SerializedObject(metadata);
            SetUpProperties();
        }

        private void OnGUI()
        {
            serializedObject.Update();

            // Add padding to the window
            RectOffset padding = new RectOffset(uniformPadding, uniformPadding, 0, 0);
            Rect area = new Rect(
                padding.right,
                padding.top,
                position.width - (padding.left),
                position.height - (padding.top + padding.bottom));

            // Generate input fields
            GUILayout.BeginArea(area);

            // Scrollbar
            RectOffset vertScrollbarPadding = new RectOffset(uniformPadding, 0, 0, 0);
            GUIStyle vertScrollbarSkin = new GUIStyle(GUI.skin.verticalScrollbar);
            vertScrollbarSkin.margin = vertScrollbarPadding;

            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, new GUIStyle(GUI.skin.horizontalScrollbar), vertScrollbarSkin);

            currentTab = GUILayout.Toolbar(currentTab, tabs);
            switch(currentTab)
            {
                case 0: ShowMetadataEditorTab(); break;
                case 1: ShowOnlineMetadataTab(); break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void ShowMetadataEditorTab()
        {
            ConjureArcadeMetadataErrorMessages errorMessages = metadataValidator.Errors.errors;

            GUILayout.Space(uniformPadding);
            GUILayout.Label("IMPORTANT FIELDS", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(gameTitleProperty, new GUIContent("Game Title"));
            ShowErrorMessages(errorMessages.game);
            GUILayout.Space(globalSpace);

            // Version - start
            GUILayout.BeginHorizontal();
            GUILayout.Label("Version", GUILayout.Width(145));
            versionSelectedLevel = GUILayout.SelectionGrid(versionSelectedLevel, versionLevelTitles, 3, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            // Update index
            nextVersionLevelProperty.intValue = versionSelectedLevel;

            // Preview new version
            GUILayout.Label(
                $"Current version is {metadata.CurrentVersion}. Next version will be set to: {metadata.NextVersion}",
                EditorStyles.wordWrappedMiniLabel);

            ShowErrorMessages(errorMessages.version);
            GUILayout.Space(globalSpace);
            // Version - end

            EditorGUILayout.PropertyField(descriptionProperty, new GUIContent("Description"), GUILayout.Height(150));
            ShowErrorMessages(errorMessages.description);
            GUILayout.Space(globalSpace);

            EditorGUILayout.PropertyField(minNumPlayerProperty, new GUIContent("Min Number of Players"));
            GUILayout.Space(globalSpace);

            EditorGUILayout.PropertyField(maxNumPlayerProperty, new GUIContent("Max Number of Players"));
            ShowErrorMessages(errorMessages.players);
            GUILayout.Space(globalSpace);

            EditorGUILayout.PropertyField(useLeaderboardProperty, new GUIContent("Use Leaderboard"));
            ShowErrorMessages(errorMessages.leaderboard);
            GUILayout.Space(globalSpace);

            EditorGUILayout.PropertyField(thumbnailProperty, new GUIContent("Thumbnail"));
            ShowErrorMessages(errorMessages.thumbnail);
            GUILayout.Space(globalSpace);

            EditorGUILayout.PropertyField(gameplayImageProperty, new GUIContent("Gameplay Image"));
            ShowErrorMessages(errorMessages.image);
            GUILayout.Space(globalSpace);

            GUILayout.Space(10);
            developersList.DoLayoutList();
            ShowErrorMessages(errorMessages.developers);

            GUILayout.Space(10);
            UpdateReorderableListInteractions(genresList, ConjureArcadeMetadata.MaxSelectedGenres);
            genresList.DoLayoutList();
            ShowErrorMessages(errorMessages.genres);

            GUILayout.Space(10);

            // Optional fields
            GUILayout.Space(uniformPadding);
            GUILayout.Label("OPTIONAL FIELDS", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(publicRepositoryLinkProperty, new GUIContent("Public Repository Link"));
            ShowErrorMessages(errorMessages.publicRepositoryLink);
            GUILayout.Space(globalSpace);

            GUILayout.Space(10);

            // Other data
            GUILayout.Space(uniformPadding);
            GUILayout.Label("OTHER DATA", EditorStyles.boldLabel);

            // Show uneditable settings
            GUILayout.Label("Id: " + metadata.Id);
            GUILayout.Label("Unity Library Version: " + ConjureLibraryVersion.Version.Full);
            if (metadata.Release != DateTime.MinValue)
            {
                GUILayout.Label("Release Date: " + metadata.Release.ToString());
                ShowErrorMessages(errorMessages.release);
            }
            if (metadata.Modification != DateTime.MinValue)
            {
                GUILayout.Label("Last Change: " + metadata.Modification.ToString());
                ShowErrorMessages(errorMessages.modification);
            }

            // Validate properties and save data
            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(30);
            if (GUILayout.Button("Execute Validation", GUILayout.Width(150)))
            {
                _ = metadataValidator.ValidateMetadata(metadata);
            }

            GUILayout.Space(globalSpace);

            ShowValidationResultMessage();

            GUILayout.Space(uniformPadding);
        }

        private void ShowOnlineMetadataTab()
        {
            GUILayout.Space(uniformPadding);
            GUILayout.Label("ONLINE METADATA", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(!webServerManager.IsLogged());
            if (GUILayout.Button("Fetch Metadata", GUILayout.Width(150)))
            {
                FetchOnlineMetadata();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(15);

            ConjureArcadeMetadataJson onlineMetadata = metadata.OnlineMetadata;
            if (metadata.OnlineMetadata != null && metadata.OnlineMetadata.AreMetadataValid)
            {
                // Show metadata if any online metadata has been fetched
                // Important metadata
                GUILayout.Space(uniformPadding);
                GUILayout.Label("IMPORTANT FIELDS", EditorStyles.boldLabel);
                ShowLabelWithTitle(nameof(onlineMetadata.game), onlineMetadata.game);
                ShowLabelWithTitle(nameof(onlineMetadata.version), onlineMetadata.version);
                ShowLabelWithTitle(nameof(onlineMetadata.description), onlineMetadata.description);
                ShowLabelWithTitle(nameof(onlineMetadata.players), onlineMetadata.players);
                ShowLabelWithTitle(nameof(onlineMetadata.leaderboard), onlineMetadata.leaderboard.ToString());
                ShowLabelWithTitle(nameof(onlineMetadata.thumbnailPath), onlineMetadata.thumbnailPath);
                ShowLabelWithTitle(nameof(onlineMetadata.imagePath), onlineMetadata.imagePath);
                
                ShowLabelWithTitle(nameof(onlineMetadata.files), onlineMetadata.files);

                string developers = string.Join(", ", onlineMetadata.developers);
                ShowLabelWithTitle(nameof(onlineMetadata.developers), developers);

                string genres = string.Join(", ", onlineMetadata.genres);
                ShowLabelWithTitle(nameof(onlineMetadata.genres), genres);

                // Optional metadata
                GUILayout.Space(uniformPadding);
                GUILayout.Label("OPTIONAL FIELDS", EditorStyles.boldLabel);
                ShowLabelWithTitle(nameof(onlineMetadata.publicRepositoryLink), onlineMetadata.publicRepositoryLink);

                // Other data
                GUILayout.Space(uniformPadding);
                GUILayout.Label("OTHER DATA", EditorStyles.boldLabel);
                ShowLabelWithTitle(nameof(onlineMetadata.id), onlineMetadata.id);
                ShowLabelWithTitle(nameof(onlineMetadata.unityLibraryVersion), onlineMetadata.unityLibraryVersion);
                ShowLabelWithTitle(nameof(onlineMetadata.release), onlineMetadata.release);
                ShowLabelWithTitle(nameof(onlineMetadata.modification), onlineMetadata.modification);
            }
            else
            {
                GUILayout.Label("No metadata to show.");
            }
        }

        private void ShowLabelWithTitle(string title, string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{title}: ", EditorStyles.boldLabel, GUILayout.Width(130));
            GUILayout.Label(text, EditorStyles.label);
            GUILayout.EndHorizontal();
        }

        private void ShowErrorMessage(string errorMessage)
        {
            if (errorMessage != string.Empty)
            {
                GUILayout.Label(errorMessage, ConjureArcadeGUI.Style.ErrorStyle);
                GUILayout.Space(3);
            }
        }

        private void ShowErrorMessages(List<string> errorMessages)
        {
            // Convert a list of error messages into a single line label
            if (errorMessages.Count > 0)
            {
                string text = string.Join(" ", errorMessages);
                GUILayout.Label(text, ConjureArcadeGUI.Style.ErrorStyle);
                GUILayout.Space(3);
            }
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
                    string formatedMessage = string.Format(
                        ConjureArcadeGUI.Message.MetadataFailedMessage, 
                        errorCount, 
                        plural);
                    ShowErrorMessage(formatedMessage);
                    break;
            }
        }

        private async void FetchOnlineMetadata()
        {
            string onlineMetadata = await webServerManager.FetchMetadata(metadata.Id);
            metadata.UpdateOnlineMetadata(onlineMetadata);
        }

        private void SetUpProperties()
        {
            // Get data
            gameTitleProperty = serializedObject.FindProperty("gameTitle");
            nextVersionLevelProperty = serializedObject.FindProperty("nextVersionLevel");
            descriptionProperty = serializedObject.FindProperty("description");
            minNumPlayerProperty = serializedObject.FindProperty("minNumPlayer");
            maxNumPlayerProperty = serializedObject.FindProperty("maxNumPlayer");
            useLeaderboardProperty = serializedObject.FindProperty("useLeaderboard");
            thumbnailProperty = serializedObject.FindProperty("thumbnail");
            gameplayImageProperty = serializedObject.FindProperty("gameplayImage");
            publicRepositoryLinkProperty = serializedObject.FindProperty("publicRepositoryLink");

            // Get developers data and prepare list
            developersProperty = serializedObject.FindProperty("developers");
            developersList = new ReorderableList(serializedObject, developersProperty, true, true, true, true);
            developersList.drawElementCallback = DrawDevelopersListItems;
            developersList.drawHeaderCallback = DrawDevelopersListHeader;
            developersList.onReorderCallback = OnDevelopersReorder;


            // Get genres data and prepare list
            genresProperty = serializedObject.FindProperty("genres");
            genresList = new ReorderableList(serializedObject, genresProperty, true, true, true, true);
            genresList.drawElementCallback = DrawGenresListItems;
            genresList.drawHeaderCallback = DrawGenresListHeader;
        }

        private void UpdateReorderableListInteractions(ReorderableList list, int countLimit)
        {
            // Hide or show the "Add" button only if the count limit has not been reached
            if (genresList.count >= countLimit)
            {
                list.displayAdd = false;
            }
            else
            {
                list.displayAdd = true;
            }
        }

        // ReorderableList Callbacks =====
        private void DrawGenresListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            // Get the selected genre of the current item
            var element = genresProperty.GetArrayElementAtIndex(index);
            var selectedGenre = element.FindPropertyRelative(nameof(GameGenre.selectedGenre));

            var popupHeight = EditorGUI.GetPropertyHeight(selectedGenre);

            // Create popup
            selectedGenre.intValue = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, popupHeight), selectedGenre.intValue, ConjureArcadeMetadata.GenreOptions);
        }

        private void DrawDevelopersListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            // Get the selected genre of the current item
            var element = developersProperty.GetArrayElementAtIndex(index);

            // Create popup
            GUIStyle fieldErrorIndicatorStyle = new GUIStyle(EditorStyles.textField);
            fieldErrorIndicatorStyle.normal.textColor = ConjureArcadeGUI.Style.ColorError;

            element.stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, rect.height), element.stringValue);
        }

        private void DrawGenresListHeader(Rect rect)
        {
            DrawListHeader(rect, "Genres");
        }

        private void DrawDevelopersListHeader(Rect rect)
        {
            DrawListHeader(rect, "Developers");
        }

        private void DrawListHeader(Rect rect, string name)
        {
            EditorGUI.LabelField(rect, name);
        }

        private void OnDevelopersReorder(ReorderableList reList)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}