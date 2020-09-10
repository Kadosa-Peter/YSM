namespace Ysm.Data
{
    internal static class Scopes
    {
        public static string YouTube { get; } = "https://www.googleapis.com/auth/youtube";
        public static string Profile { get; } = "https://www.googleapis.com/auth/userinfo.profile";
        public static string Email { get; } = "https://www.googleapis.com/auth/userinfo.email";
        public static string Comments { get; } = "https://www.googleapis.com/auth/youtube.force-ssl";

        // View and manage its own configuration data in your Google Drive
        //public static string Drive { get; } = "https://www.googleapis.com/auth/drive.appdata";

        // View and manage Google Drive files and folders that you have opened or created with this app
        //public static string Drive { get; } = "https://www.googleapis.com/auth/drive.file";

        // Full, permissive scope to access all of a user's files, excluding the Application Data folder. Request this scope only when it is strictly necessary.
        //public static string Drive { get; } = "https://www.googleapis.com/auth/drivec";
    }
}
