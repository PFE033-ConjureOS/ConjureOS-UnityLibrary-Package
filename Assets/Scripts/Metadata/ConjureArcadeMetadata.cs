using ConjureOS.Library;
using ConjureOS.Logger;
using System;
using UnityEditor;
using UnityEngine;

namespace ConjureOS.Metadata
{
    [Serializable]
    public class ConjureArcadeMetadata : ScriptableObject
    {
        // Properties Validation
        public const int MaxSelectedGenres = 3;
        public static readonly string[] GenreOptions =
        {
            "Platformer", "Shooter", "Fighting", "Beatemup", "Stealth", "Survival",
            "Rhythm", "Horror", "Metroidvania", "Puzzle", "RPG", "Roguelike",
            "Simulation", "Strategy", "Action", "Adventure", "Arcade", "RTS", "MOBA",
            "BattleRoyale", "Racing", "Sports", "Board", "Idle", "Trivia",
            "Art", "Educational", "Sandbox", "Open World", "Other"
        };

        // Editable settings
        [SerializeField, HideInInspector]
        [Tooltip("The title of the game.")]
        private string gameTitle = "";
        public string GameTitle => gameTitle;

        [SerializeField, HideInInspector]
        private ConjureVersionLevel nextVersionLevel = ConjureVersionLevel.Patch;
        public ConjureVersionLevel NextVersionLevel => nextVersionLevel;
        public string NextVersion => currentVersion.PreviewNextVersion(nextVersionLevel);

        [SerializeField, HideInInspector]
        [TextArea]
        [Tooltip("Small description of the game.")]
        private string description = "";
        public string Description => description;

        [SerializeField, HideInInspector]
        [Tooltip("The minimum required number of players to play the game.")]
        private int minNumPlayer = 1;
        public int MinNumPlayer => minNumPlayer;

        [SerializeField, HideInInspector]
        [Tooltip("The maximum required number of players to play the game.")]
        private int maxNumPlayer = 1;
        public int MaxNumPlayer => maxNumPlayer;

        [SerializeField, HideInInspector]
        [Tooltip("Indicates if the game uses a leaderboard.")]
        private bool useLeaderboard;
        public bool UseLeaderboard => useLeaderboard;

        [SerializeField, HideInInspector]
        [Tooltip("Thumbnail image of the game. Displayed in the game selection screen.")]
        private Texture2D thumbnail;
        public Texture2D Thumbnail => thumbnail;

        [SerializeField, HideInInspector]
        [Tooltip("An image that shows what the game looks like.")]
        private Texture2D gameplayImage;
        public Texture2D GameplayImage => gameplayImage;

        [SerializeField, HideInInspector]
        [Tooltip("The names of the game developers.")]
        private string[] developers = new string[0];
        public string[] Developers => developers;

        [SerializeField, HideInInspector]
        [Tooltip("Genres associated with the game.")]
        private GameGenre[] genres = new GameGenre[0];
        public GameGenre[] Genres => genres;

        [SerializeField, HideInInspector]
        [Tooltip("Link to the public repository of the game.")]
        private string publicRepositoryLink = "";
        public string PublicRepositoryLink => publicRepositoryLink;

        // Uneditable settings
        [SerializeField, HideInInspector]
        private string id;
        public string Id => id;

        [SerializeField, HideInInspector]
        private DateTime release;
        public DateTime Release => release;

        [SerializeField, HideInInspector]
        private DateTime modification;
        public DateTime Modification => modification;

        [SerializeField, HideInInspector]
        private ConjureGameVersion currentVersion = new ConjureGameVersion();
        public string CurrentVersion => currentVersion.Full;

        public ConjureArcadeMetadata()
        {
            // Generate GUID
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }
        }

#if UNITY_EDITOR
        // Other data
        private ConjureArcadeMetadataJson onlineMetadata;
        public ConjureArcadeMetadataJson OnlineMetadata => onlineMetadata;

        /// <summary>
        /// Updates modification date and release date (if not yet set)
        /// </summary>
        public void UpdateDates()
        {
            // Generate dates
            if (release == null || release == DateTime.MinValue)
            {
                release = DateTime.Now;
            }

            modification = DateTime.Now;

            SaveChanges();
        }

        /// <summary>
        /// Update current version
        /// </summary>
        public void UpdateCurrentVersion()
        {
            currentVersion.UpdateVersion(nextVersionLevel);
            SaveChanges();
        }

        /// <summary>
        /// Deserialize Json metadata sent by the web server
        /// </summary>
        public void UpdateOnlineMetadata(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    onlineMetadata = JsonUtility.FromJson<ConjureArcadeMetadataJson>(json);
                    onlineMetadata.AreMetadataValid = true;
                }
                catch
                {
                    onlineMetadata = null;
                    ConjureArcadeLogger.LogError("Couldn't read fetched metadata.");
                    ShowFetchErrorMessage();
                }
            }
            else
            {
                onlineMetadata = null;
                ShowFetchErrorMessage();
            }
        }

        private void SaveChanges()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

        private void ShowFetchErrorMessage()
        {
            EditorUtility.DisplayDialog("Error", "Couldn't fetch or read metadata.", "Ok");
        }
#endif
    }

#if UNITY_EDITOR
    // Important: The name of the variables should be the same as the ones generated in the
    // metadata.txt file and sent by the server when fetching game metadata.
    // This class is used to convert online metadata (Json) into a readable format.
    [Serializable]
    public class ConjureArcadeMetadataJson
    {
        public string id = "";
        public string game = "";
        public string version = "";
        public string unityLibraryVersion = "";
        public string description = "";
        public string players = "";
        public string release = "";
        public string modification = "";
        public string files = "";
        public string thumbnailPath = "";
        public string imagePath = "";
        public string publicRepositoryLink = "";

        public bool leaderboard = false;
        public string[] developers = new string[0];
        public string[] genres = new string[0];

        public bool AreMetadataValid = false;
    }
#endif
}
