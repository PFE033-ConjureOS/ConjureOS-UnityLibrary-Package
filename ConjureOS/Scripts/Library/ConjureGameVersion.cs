using ConjureOS.Library;
using System;

public class ConjureGameVersion : ConjureVersion, ICloneable
{
    public ConjureGameVersion()
    {
    }

    public ConjureGameVersion(int major, int minor, int patch) : base(major, minor, patch)
    {
    }

    /// <summary>
    /// Update version depending on the version level 
    /// </summary>
    /// <param name="versionLevel">The version level to increment</param>
    public void UpdateVersion(ConjureVersionLevel versionLevel)
    {
        if (versionLevel == ConjureVersionLevel.Major)
        {
            major++;
            minor = 0;
            patch = 0;
        }
        else if (versionLevel == ConjureVersionLevel.Minor)
        {
            minor++;
            patch = 0;
        }
        else
        {
            patch++;
        }
    }

    /// <summary>
    /// Preview what the next version will look like depending on the specified version level
    /// </summary>
    /// <param name="versionLevel">The version level to increment</param>
    /// <returns>Returns the preview of the next version</returns>
    public string PreviewNextVersion(ConjureVersionLevel versionLevel)
    {
        ConjureGameVersion nextVersion = (ConjureGameVersion)Clone();
        nextVersion.UpdateVersion(versionLevel);
        return nextVersion.Full;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}