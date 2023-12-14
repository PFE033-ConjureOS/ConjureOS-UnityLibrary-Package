using ConjureOS.Logger;
using ConjureOS.WebServer;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ConjureOS.Metadata.Editor
{
    public class ConjureArcadeMetadataValidator
    {
        // String errors
        private const string EmptyStringError = "Text field cannot be empty.";
        private const string VersionFormatError = "The version doesn't match the required format 'x.y.z'.";

        // Min/max player errors
        private const string IncorrectMinPlayerValueError = "The number of min players can't be 0 or lower.";
        private const string IncorrectMaxPlayerValueError = "The number of max players can't be 0 or lower.";
        private const string MinPlayerHigherThanMaxError = "Minimum number of players can't be higher than the maximum.";

        // Image errors
        private const string MissingImageError = "No image was selected.";
        private const string InvalidImagePathError = "Invalid path. Chosen picture should be placed inside the 'Assets/' folder.";

        // Other errors
        private const string SameGenreSelectedMultipleTimeError = "Same genre is selected twice or more.";

        // Data
        private ConjureArcadeMetadataErrors errors = new ConjureArcadeMetadataErrors();
        public ConjureArcadeMetadataErrors Errors => errors;

        private bool wasVerified = false; // Indicate if a validation has already been executed
        private bool didBasicValidation = false; // Indicate if last validation was basic

        /// <summary>
        /// Validate the specified game metadata
        /// </summary>
        /// <param name="metadata">The metadata to be validated</param>
        /// <returns>Return true if metadata are valid. Otherwise, returns false.</returns>
        public async Task<bool> ValidateMetadata(ConjureArcadeMetadata metadata)
        {
            // Verify metadata via the server
            string json = ConjureArcadeMetadataConverter.GenerateMetadataJson(metadata);
            errors = await ConjureArcadeWebServerManager.Instance.ValidateMetadata(json);

            if (errors == null)
            {
                // Starting basic validation because server is unreachable
                ConjureArcadeLogger.Log("Starting basic metadata validation because server is unreachable.");
                errors = new ConjureArcadeMetadataErrors();
                DoBasicValidation(metadata);
                didBasicValidation = true;
            }
            else
            {
                didBasicValidation = false;
            }

            // Execute Unity related validation
            ValidateImageForUnity(metadata.Thumbnail, Errors.errors.thumbnail);
            ValidateImageForUnity(metadata.GameplayImage, Errors.errors.image);

            wasVerified = true;
            return errors.GetErrorCount() == 0;
        }

        private void DoBasicValidation(ConjureArcadeMetadata metadata)
        {
            ValidateTextField(metadata.GameTitle, Errors.errors.game);
            ValidateTextField(metadata.Description, Errors.errors.description);
            ValidateVersion(metadata.NextVersion);
            ValidateNumPlayerFields(metadata.MinNumPlayer, metadata.MaxNumPlayer);
            ValidateGenresList(metadata.Genres);
        }

        private void ValidateTextField(string text, List<string> errorMessages)
        {
            // Check for empty string
            if (string.IsNullOrWhiteSpace(text))
            {
                errorMessages.Add(EmptyStringError);
            }
        }

        private void ValidateVersion(string version)
        {
            // Required format is "x.y.z"
            Regex rx = new Regex(@"^[a-zA-Z0-9]+\.[a-zA-Z0-9]+\.[a-zA-Z-0-9]+$");
            var matches = rx.Match(version);
            if (!matches.Success)
            {
                List<string> errorMessages = Errors.errors.version;
                errorMessages.Add(VersionFormatError);
            }
        }

        private void ValidateNumPlayerFields(int minNumPlayer, int maxNumPlayer)
        {
            List<string> errorMessages = Errors.errors.players;

            if (minNumPlayer <= 0)
            {
                errorMessages.Add(IncorrectMinPlayerValueError);
            }

            if (maxNumPlayer <= 0)
            {
                errorMessages.Add(IncorrectMaxPlayerValueError);
            }

            if (minNumPlayer > maxNumPlayer)
            {
                errorMessages.Add(MinPlayerHigherThanMaxError);
            }
        }

        private void ValidateImageForUnity(Texture2D image, List<string> errorMessages)
        {
            if (image == null)
            {
                if (didBasicValidation)
                {
                    // If a basic validation was done, error for missing picture is added to the list
                    errorMessages.Add(MissingImageError);
                }
                return;
            }

            // Verify if image location is under "Assets/" folder
            string imageRelativePath = AssetDatabase.GetAssetPath(image);
            if (!imageRelativePath.StartsWith("Assets/"))
            {
                // Images should be located inside the Assets folder
                errorMessages.Add(InvalidImagePathError);
            }
        }

        private void ValidateGenresList(GameGenre[] genres)
        {
            List<string> errorMessages = Errors.errors.genres;

            // Check if a genre has been selected twice or more
            for (int i = 0; i < genres.Length - 1; i++)
            {
                for (int j = 0; j < genres.Length; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (genres[i].selectedGenre == genres[j].selectedGenre)
                    {
                        errorMessages.Add(SameGenreSelectedMultipleTimeError);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns the validation state based on the current amount of errors detected</returns>
        public MetadataValidationStateType GetValidationStateType()
        {
            if (!wasVerified)
            {
                // Data haven't been verified yet
                return MetadataValidationStateType.NotVerified;
            }
            else if (Errors.GetErrorCount() == 0)
            {
                // No error, means it's a success
                if (didBasicValidation)
                {
                    return MetadataValidationStateType.BasicValidated;
                }
                else
                {
                    return MetadataValidationStateType.Validated;
                }
            }
            else
            {
                // Errors were detected
                return MetadataValidationStateType.Failed;
            }
        }
    }
    
    public enum MetadataValidationStateType
    {
        NotVerified,
        Validated,
        BasicValidated,
        Failed
    }
}