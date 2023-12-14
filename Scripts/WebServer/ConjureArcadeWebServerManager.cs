using ConjureOS.Logger;
using System.Net.Http;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Web;
using ConjureOS.Metadata;
using ConjureOS.Settings;
using ConjureOS.Library;

namespace ConjureOS.WebServer
{
    public class ConjureArcadeWebServerManager
    {
        private readonly HttpClient client = new HttpClient();
        private ConjureSettings webServerSettings;
        private ConjureArcadeWebServerSession session;

        public delegate void LoginEvent(string message, LogMessageType type);
        public LoginEvent OnLoginAttempt;

        // Singleton
        private static ConjureArcadeWebServerManager instance = null;
        public static ConjureArcadeWebServerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConjureArcadeWebServerManager();
                }
                return instance;
            }
        }

        private ConjureArcadeWebServerManager()
        {
            webServerSettings = ConjureSettingsLoader.Settings;
            session = new ConjureArcadeWebServerSession();
        }

        private string GenericErrorMessage()
        {
            return 
                $"An error occured when contacting server '{webServerSettings.Address}'. " +
                $"Please, make sure that the address is valid and reachable.";
        }

        public string GetUsername()
        {
            return session.Username;
        }

        public bool IsLogged()
        {
            return session.IsLogged;
        }

        /// <summary>
        /// Login to the web server
        /// </summary>
        /// <param name="username">The account username to login</param>
        /// <param name="password">The password of the account.</param>
        /// <returns>True if login successful, otherwise false.</returns>
        public async Task<bool> Login(string username, string password)
        {
            const string path = "/login";

            var values = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };

            try
            {
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(webServerSettings.Address + path, content);
                var responseString = await response.Content.ReadAsStringAsync();
                int statusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = JsonUtility.FromJson<ConjureArcadeLoginResponse>(responseString);
                    session.OpenSession(username, responseJson.token);
                    OnLoginAttempt?.Invoke($"Logged in as '{username}'", LogMessageType.Log);
                    ConjureArcadeLogger.Log($"Token acquired. Your are now logged in as {username}.");
                    return true;
                }
                else
                {
                    OnLoginAttempt?.Invoke(
                        $"Wrong username or password. Server responded with code {statusCode}.",
                        LogMessageType.Error);
                    ConjureArcadeLogger.LogError($"Server responded with code {statusCode}. Response: {responseString}");
                    return false;
                }
            }
            catch
            {
                string message = GenericErrorMessage();
                OnLoginAttempt?.Invoke(message, LogMessageType.Error);
                ConjureArcadeLogger.LogError(message);
                return false;
            }
        }

        /// <summary>
        /// Logout from the web server.
        /// </summary>
        public void Logout()
        {
            session.CloseSession();
            OnLoginAttempt?.Invoke("Disconnected.", LogMessageType.Log);
            ConjureArcadeLogger.Log($"Web server session terminated.");
        }

        /// <summary>
        /// Checks if a game exists in the web server.
        /// </summary>
        /// <param name="id">The id of the game</param>
        /// <returns>Return 1 if it exists, 0 if it doesnt exist and -1 if an error occurred.</returns>
        public async Task<int> CheckForGame(string id)
        {
            string path = "/games/" + HttpUtility.UrlEncode(id);

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new System.Uri(webServerSettings.Address + path),
                    Method = HttpMethod.Get
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token.Trim('"'));

                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                int statusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    ConjureArcadeLogger.Log("Game found in the web server.");
                    return 1;
                }
                else if (statusCode == 500)
                {
                    ConjureArcadeLogger.Log("Game not found in the web server.");
                    return 0;
                }
                else
                {
                    ConjureArcadeLogger.LogError(
                        $"Can't check for game availability. " +
                        $"Server responded with {statusCode}. Response: {responseString}");
                    return -1;
                }
            }
            catch
            {
                ConjureArcadeLogger.LogError(GenericErrorMessage());
                return -1;
            }
        }

        /// <summary>
        /// Fetch metadata of the specified game on the web server.
        /// </summary>
        /// <param name="gameTitle">The title of the game</param>
        /// <returns>The metadata in JSON format.</returns>
        public async Task<string> FetchMetadata(string id)
        {
            string path = "/games/" + HttpUtility.UrlEncode(id);

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new System.Uri(webServerSettings.Address + path),
                    Method = HttpMethod.Get
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token.Trim('"'));

                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                int statusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    ConjureArcadeLogger.Log($"Game metadata were fetched from the web server.");
                    return responseString;
                }
                else
                {
                    ConjureArcadeLogger.LogError(
                        $"Couldn't fetch metadata from the web server. " +
                        $"Server responded with {statusCode}. Response: {responseString}");
                    return "";
                }
            }
            catch
            {
                ConjureArcadeLogger.LogError(GenericErrorMessage());
                return "";
            }
        }


#if UNITY_EDITOR
        /// <summary>
        /// Upload a game to the web server
        /// </summary>
        /// <param name="filePath">The path of the file to upload</param>
        /// <param name="fileName">The name of the file</param>
        /// <returns>True if upload was successful. Otherwise false.</returns>
        public async Task<bool> UploadGame(string filePath, string fileName)
        {
            const string path = "/games";
            byte[] fileContent = File.ReadAllBytes(filePath);
            
            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new System.Uri(webServerSettings.Address + path),
                    Method = HttpMethod.Post,
                    Content = new MultipartFormDataContent
                    {
                        { new StreamContent(new MemoryStream(fileContent)), "file", fileName },
                    }
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token.Trim('"'));

                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                int statusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    ConjureArcadeLogger.Log($"Game was uploaded to the web server");
                    return true;
                }
                else
                {
                    ConjureArcadeLogger.LogError(
                        $"Couldn't upload the game to the web server. " +
                        $"Server responded with code {statusCode}. Response: {responseString}");
                    return false;
                }
            }
            catch
            {
                ConjureArcadeLogger.LogError(GenericErrorMessage());
                return false;
            }
        }

        /// <summary>
        /// Update a game on the web server
        /// </summary>
        /// <param name="filePath">The path of the file to upload</param>
        /// <param name="fileName">The name of the file</param>
        /// <param name="nextVersionLevel">The new version type</param>
        /// <returns>True if upload/update was successful. Otherwise false.</returns>
        public async Task<bool> UpdateGame(string filePath, string fileName, ConjureVersionLevel nextVersionLevel)
        {
            string subRoute = "";
            switch (nextVersionLevel)
            {
                case ConjureVersionLevel.Major: subRoute = "major"; break;
                case ConjureVersionLevel.Minor: subRoute = "minor"; break;
                case ConjureVersionLevel.Patch: subRoute = "patch"; break;
            }
            
            string path = $"/games/{subRoute}";
            byte[] fileContent = File.ReadAllBytes(filePath);

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new System.Uri(webServerSettings.Address + path),
                    Method = HttpMethod.Put,
                    Content = new MultipartFormDataContent
                    {
                        { new StreamContent(new MemoryStream(fileContent)), "file", fileName },
                    }
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token.Trim('"'));

                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                int statusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    ConjureArcadeLogger.Log($"An update of the game was uploaded on the web server.");
                    return true;
                }
                else
                {
                    ConjureArcadeLogger.LogError(
                        $"Couldn't upload the new version of the game on the web server. " +
                        $"Server responded with code {statusCode}. Response: {responseString}");
                    return false;
                }
            }
            catch
            {
                ConjureArcadeLogger.LogError(GenericErrorMessage());
                return false;
            }
        }

        /// <summary>
        /// Validate metadata using the web server
        /// </summary>
        /// <param name="json">The metadata to validate in JSON format</param>
        /// <returns>Returns every errors that were found. 
        /// Returns null if an error occurs when communicating with the server.</returns>
        public async Task<ConjureArcadeMetadataErrors> ValidateMetadata(string metadataJson)
        {
            string path = "/validator/metadata";

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new System.Uri(webServerSettings.Address + path),
                    Method = HttpMethod.Post,
                    Content = new StringContent(metadataJson)
                };

                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                int statusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    ConjureArcadeLogger.Log($"Server responsed with code {statusCode}");
                    return new ConjureArcadeMetadataErrors();
                }
                else if (statusCode == 422)
                {
                    ConjureArcadeLogger.LogWarning(
                        $"Server responsed with code {statusCode}. Errors were detected within the metadata. " +
                        $"Errors: {responseString}");
                    return JsonUtility.FromJson<ConjureArcadeMetadataErrors>(responseString);
                }
                else
                {
                    ConjureArcadeLogger.LogError(
                        $"An error occured, and data couldn't be analyzed. " +
                        $"Server responded with code {statusCode}. Response: {responseString}");
                    return null;
                }
            }
            catch
            {
                ConjureArcadeLogger.LogError(GenericErrorMessage());
                return null;
            }
        }
#endif
    }
}