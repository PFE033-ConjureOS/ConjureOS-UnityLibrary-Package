using System;

namespace ConjureOS.Library
{
    public class ConjureVersion
    {
        protected int major = 1;
        public int Major => major;

        protected int minor = 0;
        public int Minor => minor;

        protected int patch = 0;
        public int Patch => patch;

        public string Full => $"{Major}.{Minor}.{Patch}";

        public ConjureVersion()
        {
        }

        public ConjureVersion(int major, int minor, int patch)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
        }
    }

    public enum ConjureVersionLevel
    {
        Major = 0,  // x.0.0
        Minor = 1,  // 0.x.0
        Patch = 2   // 0.0.x
    }
}