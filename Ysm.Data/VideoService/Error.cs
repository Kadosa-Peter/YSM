namespace Ysm.Data
{
    public static class VideoError
    {
        public static int ApiError { get; set; }

        public static int FeedError { get; set; }

        public static int ExplodeError { get; set; }

        public static void Reset()
        {
            ApiError = 0;
            FeedError = 0;
            ExplodeError = 0;
        }
    }
}
