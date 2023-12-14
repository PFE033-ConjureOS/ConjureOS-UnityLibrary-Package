#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace ConjureOS.Metadata
{
    /// <summary>
    /// This class should contains the error messages for each datafield.
    /// For now, it only contains the "errors" field as its structure must be the same
    /// as what the server returns in order to deserialize the JSON content.
    /// </summary>
    [Serializable]
    public class ConjureArcadeMetadataErrors
    {
        public ConjureArcadeMetadataErrorMessages errors = new ConjureArcadeMetadataErrorMessages();

        public int GetErrorCount()
        {
            return
                errors.id.Count +
                errors.game.Count +
                errors.description.Count +
                errors.players.Count +
                errors.genres.Count +
                errors.developers.Count +
                errors.leaderboard.Count +
                errors.version.Count +
                errors.release.Count +
                errors.modification.Count +
                errors.thumbnail.Count +
                errors.image.Count +
                errors.publicRepositoryLink.Count;
        }
    }

    /// <summary>
    /// This class should contains the error messages for each metadata field.
    /// Its structure must be the same as what the server should return if an error is detected
    /// in order to deserialize the JSON content.
    /// </summary>
    [Serializable]
    public class ConjureArcadeMetadataErrorMessages
    {
        public List<string> id = new List<string>();
        public List<string> game = new List<string>();
        public List<string> description = new List<string>();
        public List<string> players = new List<string>();
        public List<string> genres = new List<string>();
        public List<string> developers = new List<string>();
        public List<string> leaderboard = new List<string>();
        public List<string> version = new List<string>();
        public List<string> release = new List<string>();
        public List<string> modification = new List<string>();
        public List<string> thumbnail = new List<string>();
        public List<string> image = new List<string>();
        public List<string> publicRepositoryLink = new List<string>();
    }
}
#endif