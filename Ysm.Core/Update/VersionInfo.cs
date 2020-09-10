using System.Diagnostics;

#pragma warning disable 660,661

namespace Ysm.Core.Update
{
    public class VersionInfo
    {
        public int Major { get; set; }

        public int Minor { get; set; }

        public int Build { get; set; }

        public VersionInfo(FileVersionInfo fileVersionInfo)
        {
            if (fileVersionInfo != null)
            {
                try
                {
                    Major = fileVersionInfo.FileMajorPart;
                    Minor = fileVersionInfo.FileMinorPart;
                    Build = fileVersionInfo.FileBuildPart;
                }
                catch
                {
                    DefaultVersion();
                }
            }
            else
            {
                DefaultVersion();
            }

        }

        public VersionInfo(string versionString)
        {
            if (versionString.IsNull())
            {
                DefaultVersion();
            }
            else
            {
                try
                {
                    Major = int.Parse(versionString[0].ToString());
                    Minor = int.Parse(versionString[2].ToString());
                    Build = int.Parse(versionString[4].ToString());
                }
                catch
                {
                    DefaultVersion();
                }
            }
        }

        private void DefaultVersion()
        {
            Major = 0;
            Minor = 0;
            Build = 0;
        }

        public static bool operator <(VersionInfo v1, VersionInfo v2)
        {
            return Comparison(v1, v2) < 0;
        }

        public static bool operator >(VersionInfo v1, VersionInfo v2)
        {
            return Comparison(v1, v2) > 0;
        }

        public static bool operator ==(VersionInfo v1, VersionInfo v2)
        {
            return Comparison(v1, v2) == 0;
        }

        public static bool operator !=(VersionInfo v1, VersionInfo v2)
        {
            return Comparison(v1, v2) != 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is VersionInfo)) return false;

            return this == (VersionInfo)obj;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}.";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static int Comparison(VersionInfo v1, VersionInfo v2)
        {

            if (v1.Major > v2.Major)
            {
                return 1;
            }

            if (v1.Major == v2.Major)
            {
                if (v1.Minor > v2.Minor)
                {
                    return 1;
                }

                if (v1.Minor == v2.Minor)
                {
                    if (v1.Build > v2.Build)
                    {
                        return 1;
                    }

                    if (v1.Build == v2.Build)
                    {
                        return 0;
                    }

                    return -1;
                }

                return -1;
            }

            return -1;
        }
    }

  
}
