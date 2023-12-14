using ConjureOS.Metadata;
using ConjureOS.Metadata.Editor;
using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;
using ConjureOS.Logger;
using ConjureOS.WebServer;
using System.Threading.Tasks;

namespace ConjureOS.UploaderWindow.Editor
{
    public class ConjureArcadeUploadProcessor
    {
        private const string GameFolder = "game\\";
        private const string MediasFolder = "medias\\";
        private const string TempFolder = "temp\\";
        private const string MetadataFileName = "metadata.txt";

        private const string ThumbnailFileName = "thumbnail";
        private const string GameplayImageFileName = "gameplayImage";

        /// <summary>
        /// Build and upload the game to the web server.
        /// Must use valid Conjure Arcade metadata, or the process will fail.
        /// </summary>
        /// <param name="metadata">The Conjure Arcade metadata of the game</param>
        /// <param name="metadataValidator">The metadata validator</param>
        public async void BuildAndUpload(ConjureArcadeMetadata metadata, ConjureArcadeMetadataValidator metadataValidator)
        {
            // Step 1 to 5: .conj preparation and generation
            string conjFilePath = await GenerateConjFile(metadata, metadataValidator);
            string conjFileName = Path.GetFileName(conjFilePath);

            if (string.IsNullOrEmpty(conjFileName) )
            {
                return;
            }

            // Step 6: Upload game and metadata to the webserver
            ConjureArcadeWebServerManager webServerManager = ConjureArcadeWebServerManager.Instance;

            // Check if game exists on web server
            int gameAvailability = await webServerManager.CheckForGame(metadata.Id);

            bool wasUploaded = false;
            if (gameAvailability == 0)
            {
                // Game doesn't exists. Create a new entry 
                ConjureArcadeLogger.Log("Trying to create new entry for the game.");
                wasUploaded = await webServerManager.UploadGame(conjFilePath, conjFileName);
            }
            else if (gameAvailability == 1)
            {
                // Game exists. Update required
                ConjureArcadeLogger.Log("Trying to update current game.");
                wasUploaded = await webServerManager.UpdateGame(conjFilePath, conjFileName, metadata.NextVersionLevel);
            }

            if (!wasUploaded)
            {
                ShowErrorDialog("Couldn't upload game to the web server.");
                return;
            }

            metadata.UpdateCurrentVersion();

            // Step 7: Show success feedback to the user
            ShowSuccessDialog("Game was build and upload to the web server.");
        }

        /// <summary>
        /// Generate a .conj file
        /// </summary>
        /// <param name="metadata">The Conjure Arcade metadata of the game</param>
        /// <param name="netadataValidator">The validator used to validate metadata</param>
        public async void GenerateConjFileAt(ConjureArcadeMetadata metadata, ConjureArcadeMetadataValidator netadataValidator)
        {
            string conjFilePath = await GenerateConjFile(metadata, netadataValidator);

            if (string.IsNullOrEmpty(conjFilePath))
            {
                return;
            }

            string successMessage = $"'.conj' file was successfully generated at the location {conjFilePath}";
            ShowSuccessDialog(successMessage);
            ConjureArcadeLogger.Log(successMessage);
        }

        private async Task<string> GenerateConjFile(ConjureArcadeMetadata metadata, ConjureArcadeMetadataValidator metadataValidator)
        {
            // Step 1: Validate metadata
            if (!await metadataValidator.ValidateMetadata(metadata))
            {
                ShowErrorDialog(
                    "Some entries in the game metadata are incorrect. " +
                    "You must fix them in order to upload the game to the web server.");
                return "";
            }

            // Step 2: Build game
            string fileName;            // Name of the executable file
            string fileExtension;       // Extension of the executable file
            string buildDirectoryPath;  // Path to the build directory
            string tempDirectoryPath;   // Path to the temp directory, located inside the build directory

            try
            {
                // Get current build settings
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
                buildPlayerOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(buildPlayerOptions);

                // Get file and directory data
                fileName = Path.GetFileNameWithoutExtension(buildPlayerOptions.locationPathName);
                fileExtension = Path.GetExtension(buildPlayerOptions.locationPathName);
                buildDirectoryPath = Path.GetDirectoryName(buildPlayerOptions.locationPathName) + "\\";
                tempDirectoryPath = buildDirectoryPath + TempFolder;

                // Change Build Settings build directory to the temp directory
                // Generated location for the game is of the format "...BuildDirectory\temp\game\ProductName.extension"
                buildPlayerOptions.locationPathName =
                    tempDirectoryPath +
                    GameFolder +
                    fileName +
                    fileExtension;

                // Clear temp directory
                if (Directory.Exists(tempDirectoryPath))
                {
                    Directory.Delete(tempDirectoryPath, true);
                }

                // Build the game
                BuildPipeline.BuildPlayer(buildPlayerOptions);

                // "game" folder content is compressed
                CompressAndDelete(tempDirectoryPath + GameFolder, tempDirectoryPath + "game.zip");
            }
            catch (BuildPlayerWindow.BuildMethodException)
            {
                // Exception called when the user manually closes the "File Chooser" window
                ConjureArcadeLogger.Log("Build canceled.");
                return "";
            }
            catch (Exception e)
            {
                ShowErrorDialog("An error occured when building the game.");
                Debug.LogError(e);
                return "";
            }


            // Step 3: Copy images to the temp directory
            string mediasDirectoryPath = tempDirectoryPath + MediasFolder;
            string thumbnailPath = CopyAndRenameImage(metadata.Thumbnail, mediasDirectoryPath, ThumbnailFileName);
            string gameplayImagePath = CopyAndRenameImage(metadata.GameplayImage, mediasDirectoryPath, GameplayImageFileName);


            // Step 4: Generate metadata inside the temp directory
            metadata.UpdateDates();
            string metadataContent = ConjureArcadeMetadataConverter.GenerateMetadataText(
                metadata,
                MediasFolder + thumbnailPath,
                MediasFolder + gameplayImagePath,
                GameFolder + fileName + fileExtension);
            if (!WriteMetadataFile(metadataContent, tempDirectoryPath))
            {
                ShowErrorDialog("An error occured when generating the metadata file.");
                return "";
            }


            // Step 5: Convert all files int a ".conj"
            string conjFilePath = buildDirectoryPath + fileName + ".conj";
            try
            {
                if (File.Exists(conjFilePath))
                {
                    File.Delete(conjFilePath);
                }

                CompressAndDelete(tempDirectoryPath, conjFilePath);
            }
            catch
            {
                ShowErrorDialog("An error occured when generating .conj file.");
                return "";
            }

            // .conj was succefuly generated
            return conjFilePath;
        }

        /// <summary>
        /// Convert metadata into a metadata.txt file at a chosen location
        /// </summary>
        /// <param name="metadata">The metadata to convert</param>
        /// <param name="metadataValidator">The validator used to validate metadata</param>
        public async void GenerateMetadataFileAt(ConjureArcadeMetadata metadata, ConjureArcadeMetadataValidator metadataValidator)
        {
            // Validate metadata
            if (!await metadataValidator.ValidateMetadata(metadata))
            {
                ShowErrorDialog("Some entries in the game metadata are incorrect. 'metadata.txt' cannot be generated.");
                return;
            }

            // Build player is used in order to get the correct formated .exe file name 
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(buildPlayerOptions);
            string directoryPath = Path.GetDirectoryName(buildPlayerOptions.locationPathName);
            string metadataFilePath = directoryPath + MetadataFileName;
            string exeFileName = Path.GetFileName(buildPlayerOptions.locationPathName);
            
            // Generate metadata.txt content
            string thumbnailPath = GetImagePath(metadata.Thumbnail, ThumbnailFileName);
            string imagePath = GetImagePath(metadata.GameplayImage, GameplayImageFileName);
            string content = ConjureArcadeMetadataConverter.GenerateMetadataText(
                metadata, thumbnailPath, imagePath, GameFolder + exeFileName);

            if (File.Exists(metadataFilePath))
            {
                File.Delete(metadataFilePath);
            }

            if (!WriteMetadataFile(content, directoryPath))
            {
                ShowErrorDialog("An error occured when generating 'metadata.txt'");
                return;
            }

            string successMessage = $"File 'metadata.txt' was correctly generated at location {metadataFilePath}";
            ShowSuccessDialog(successMessage);
            ConjureArcadeLogger.Log(successMessage);
        }

        private void ShowErrorDialog(string description)
        {
            ConjureArcadeLogger.LogError(description);
            EditorUtility.DisplayDialog("Process Failed", description, "Ok");
        }

        private void ShowSuccessDialog(string description)
        {
            EditorUtility.DisplayDialog("Success", description, "Ok");
        }

        private string GetImagePath(Texture2D image, string fileName)
        {
            string imagePath = "";
            if (image != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(image);
                string fileExtension = Path.GetExtension(assetPath);
                imagePath = MediasFolder + fileName + fileExtension;
            }

            return imagePath;
        }

        private string CopyAndRenameImage(Texture2D image, string newDirectoryPath, string newFileName)
        {
            string relativePath = AssetDatabase.GetAssetPath(image);

            // We check and remove "Assets" at the start of the relative path because
            // Application.dataPath already gives us the path to the assets folder
            // Ex: Assets/Images/ --> Images/
            string oldAbsolutePath = Application.dataPath + relativePath.Substring(6);

            // We find the image, copy it to the new folder (newDirectoryPath), change its name and keep its extension
            FileInfo file = new FileInfo(oldAbsolutePath);
            string fullFileName = string.Format("{0}{1}", newFileName, file.Extension);
            if (file.Exists)
            {
                // Delete file in new directory if it already exists
                string newFilePath = string.Format("{0}{1}", newDirectoryPath, fullFileName);

                if (!Directory.Exists(newDirectoryPath))
                {
                    // Check if new directory exists, and create it
                    // If folder doesn't exist, the file cannot be copied
                    Directory.CreateDirectory(newDirectoryPath);
                }
                else if (File.Exists(newFilePath))
                {
                    // Check if file exists, and delete it
                    File.Delete(newFilePath);
                }

                file.CopyTo(newFilePath);
            }

            return fullFileName;
        }

        private bool WriteMetadataFile(string content, string directoryPath)
        {
            string fullPath = directoryPath + MetadataFileName;

            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                // Write content
                using (FileStream fs = File.Create(fullPath))
                {
                    byte[] encodedContent = new System.Text.UTF8Encoding(true).GetBytes(content);
                    fs.Write(encodedContent, 0, encodedContent.Length);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }

            return true;
        }

        private void CompressAndDelete(string sourceDirectoryPath, string destinationArchiveFileName)
        {
            ZipFile.CreateFromDirectory(sourceDirectoryPath, destinationArchiveFileName);
            Directory.Delete(sourceDirectoryPath, true);
        }
    }
}