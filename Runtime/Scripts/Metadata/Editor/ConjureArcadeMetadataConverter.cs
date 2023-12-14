using ConjureOS.WebServer;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ConjureOS.Metadata.Editor
{
    public class ConjureArcadeMetadataConverter
    {
        /// <summary>
        /// Generate the content of a "metadata.txt" file, 
        /// which should be read by the Conjure Arcade software.
        /// </summary>
        /// <param name="metadata">The metadata to convert</param>
        /// <param name="thumbnailFilePath">The path to the thumbnail file</param>
        /// <param name="gameplayImageFilePath">The path to the image file</param>
        /// <param name="filePath">The path to the execute file</param>
        /// <returns>Returns the generated content.</returns>
        public static string GenerateMetadataText(
            ConjureArcadeMetadata metadata,
            string thumbnailFilePath,
            string gameplayImageFilePath,
            string filePath)
        {
            string newLine = Environment.NewLine;
            string content = "";
            content +=
                "id: " + metadata.Id + newLine +
                "game: " + metadata.GameTitle + newLine +
                "version: " + metadata.NextVersion + newLine +
                "unityLibraryVersion: " + ConjureLibraryVersion.Version.Full + newLine +
                "description: " + ReplaceLineBreak(metadata.Description) + newLine +
                "players: " + metadata.MinNumPlayer + "-" + metadata.MaxNumPlayer + newLine +
                "leaderboard: " + metadata.UseLeaderboard + newLine +
                "thumbnailPath: " + thumbnailFilePath + newLine +
                "imagePath: " + gameplayImageFilePath + newLine +
                "release: " + FormatDate(metadata.Release) + newLine +
                "modification: " + FormatDate(metadata.Modification) + newLine +
                "files: " + filePath + newLine +
                "publicRepositoryLink: " + metadata.PublicRepositoryLink + newLine;

            // Adding developers
            content += "developers: ";
            for (int i = 0; i < metadata.Developers.Length; i++)
            {
                if (i != 0)
                {
                    content += ", ";
                }
                content += metadata.Developers[i];
            }

            content += newLine;

            // Adding genres
            content += "genres: ";
            for (int i = 0; i < metadata.Genres.Length; i++)
            {
                string genre = ConjureArcadeMetadata.GenreOptions[metadata.Genres[i].selectedGenre];
                if (i != 0)
                {
                    content += ", ";
                }
                content += genre.ToLower();
            }

            return content;
        }

        /// <summary>
        /// Convert metadata into a JSON format
        /// </summary>
        /// <param name="metadata">The metadata to convert</param>
        /// <param name="thumbnailContent"></param>
        /// <param name="imageContent"></param>
        /// <returns></returns>
        public static string GenerateMetadataJson(ConjureArcadeMetadata metadata)
        {
            // Get image content
            byte[] thumbnailContent = FindImage(metadata.Thumbnail);
            byte[] imageContent = FindImage(metadata.GameplayImage);

            // Generate MetadataJson object that will get converted into JSON
            MetadataJson metadataJson = new MetadataJson();
            metadataJson.id = metadata.Id;
            metadataJson.game = metadata.GameTitle;
            metadataJson.description = ReplaceLineBreak(metadata.Description);
            metadataJson.players = $"{metadata.MinNumPlayer}-{metadata.MaxNumPlayer}";

            string[] genres = new string[metadata.Genres.Length];
            for (int i = 0; i < metadata.Genres.Length; i++)
            {
                genres[i] = ConjureArcadeMetadata.GenreOptions[metadata.Genres[i].selectedGenre].ToLower();
            }
            metadataJson.genres = genres;

            metadataJson.developers = metadata.Developers;
            metadataJson.thumbnail = (thumbnailContent.Length == 0) ? new byte[0] : thumbnailContent;
            metadataJson.leaderboard = metadata.UseLeaderboard;
            metadataJson.uploader = ConjureArcadeWebServerManager.Instance.GetUsername();
            metadataJson.version = metadata.NextVersion;
            metadataJson.unityLibraryVersion = ConjureLibraryVersion.Version.Full;
            metadataJson.release = FormatDate(metadata.Release);
            metadataJson.modification = FormatDate(metadata.Modification);
            metadataJson.image = (imageContent.Length == 0) ? new byte[0] : imageContent;
            metadataJson.files = "temp";
            metadataJson.publicRepositoryLink = metadata.PublicRepositoryLink;

            return JsonUtility.ToJson(metadataJson);
        }

        private static byte[] FindImage(Texture2D texture2D)
        {
            string relativePath = AssetDatabase.GetAssetPath(texture2D);
            if (!string.IsNullOrEmpty(relativePath))
            {
                byte[] content = File.ReadAllBytes(relativePath);
                return content;
            }

            return new byte[0];
        }

        private static string ReplaceLineBreak(string text)
        {
            return text.Replace("\n", " ");
        }

        private static string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        private class MetadataJson
        {
            public string id;
            public string game;
            public string description;
            public string players;
            public string[] genres;
            public string[] developers;
            public byte[] thumbnail;
            public bool leaderboard;
            public string uploader;
            public string version;
            public string unityLibraryVersion;
            public string release;
            public string modification;
            public byte[] image;
            public string files;
            public string publicRepositoryLink;
        }
    }
}