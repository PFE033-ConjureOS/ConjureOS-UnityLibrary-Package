namespace ConjureOS.WebServer
{
    public class ConjureArcadeWebServerSession
    {
        // Data
        public bool IsLogged { get; private set; }
        public string Token { get; private set; }
        public string Username { get; private set; }

        /// <summary>
        /// Open a session
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="token">Web server token. Proof that the user is connected.</param>
        public void OpenSession(string username, string token)
        {
            Username = username;
            Token = token;
            IsLogged = true;
        }

        /// <summary>
        /// Close a session.
        /// </summary>
        public void CloseSession()
        {
            Username = string.Empty;
            Token = string.Empty;
            IsLogged = false;
        }
    }
    
    public class ConjureArcadeLoginResponse
    {
        public string token = "";
    }
}