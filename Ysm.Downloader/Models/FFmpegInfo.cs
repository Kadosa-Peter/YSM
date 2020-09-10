namespace Ysm.Downloader.Models
{
    public class FFmpegInfo
    {
        // OUTPUT PROPERTIES

        // ERROR CODE

        // General values

        // Audio values

        // Video Values

        // New Properties

        public string ProcessID { set; get; } = "";

        public long ProcessedSize { set; get; }

        public int ProcessedTime { set; get; }

        public long TotalSize { set; get; }

        public double ProcessingLeft { set; get; } = 100;

        public double ProcessingCompleted { set; get; }

        public string FileName { get; set; } = "";

        public string Duration { get; set; } = "";

        public int Duration_Sec { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }

        public int Seconds { get; set; }

        public string Start { get; set; } = "";

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; } = "";

        public string FFMPEGOutput { get; set; } = "";

        // Output Properties

        public string Acodec { get; set; } = "";

        public string Vcodec { get; set; } = "";

        public string SamplingRate { get; set; } = "";

        public string Channel { get; set; } = "";

        public string Audio_Bitrate { get; set; } = "";

        public string Video_Bitrate { get; set; } = "";

        public int Width { get; set; }

        public int Height { get; set; }

        public string FrameRate { get; set; } = "";

        public string Type { get; set; } = "";

        // Input Properties

        public string Input_Acodec { get; set; } = "";

        public string Input_Vcodec { get; set; } = "";

        public string Input_SamplingRate { get; set; } = "";

        public string Input_Channel { get; set; } = "";

        public string Input_Audio_Bitrate { get; set; } = "";

        public string Input_Video_Bitrate { get; set; } = "";

        public int Input_Width { get; set; }

        public int Input_Height { get; set; }

        public string Input_FrameRate { get; set; } = "";

        public string Input_Type { get; set; } = "";

        public string Music { get; set; } = "";

        public string Footage { get; set; } = "";

        public string Producer { get; set; } = "";

        public string Title { get; set; } = "";
    }
}
