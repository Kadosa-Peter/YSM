using System;
using System.Text.RegularExpressions;
using Ysm.Downloader.Models;

namespace Ysm.Downloader.Assets
{
    public static class OutputPharser
    {
        private static string _titleExpression = @"(\s+|)title(\s+|):(\s+|)(?<vtitle>.+)"; // media title
        private static string _commentExpression = @"(\s+|)comment(\s+|):(\s+|)Footage(\s+|):(\s+|)(?<comment_footage>.+?\|)(\s+|)Producer(\s+|):(\s+|)(?<comment_producer>.+?\|)(\s+|)Music(\s+|):(\s+|)(?<comment_music>.+)"; // complete comment
        private static string _minfoPattern = @"Duration: (?<hours>\d{1,3}):(?<minutes>\d{2}):(?<seconds>\d{2})(.(?<fractions>\d{1,3}))?, start: (?<start>\d+(\.\d+)?), bitrate: (?<bitrate>\d+(\.\d+)?(\s+)kb/s)?";  //Duration: 00:20:04.08, start: 30.000000, bitrate: 513 kb/s
        private static string _strmPattern = @"Stream(\s|)#(?<number>\d+?\.\d+?)(\((?<language>\w+)\))?(\s|):(\s|)(?<type>\w+)(\s|):(\s|)(?<data>.+)"; // Stream #0.0(eng): Audio: wmav2, 44100 Hz, 2 channels, s16, 192 kb/s OR Stream #0.1(eng): Video: vc1 (Advanced), yuv420p, 1280x720, 5942 kb/s, 29.97 tbr, 1k tbn, 1k tbc
        private static string _frameInfoPattern = @"frame=(\s+|)(?<frame>\d+)?(\s+|)fps=(\s+|)(?<fps>\d+)?(\s+|)q=(?<q>\d+.\d+)?(\s+|)size=(\s+|)(?<fsize>\d+)?kB(\s+|)time=(?<time_hr>\d+)?:(?<time_min>\d+)?:(?<time_sec>\d+)?.(?<time_frac>\d+)?\sbitrate=(\s+|)(?<bitrate>\d+.\d+)kbits/s"; //frame= 34 fps= 0 q=31.0 size= 0kB time=00:00:00.00 bitrate= 0.0kbits/s
        private static string _frameInfoPattern_q_minus = @"frame=(\s+|)(?<frame>\d+)?(\s+|)fps=(\s+|)(?<fps>\d+)?(\s+|)q=-(?<q>\d+.\d+)?(\s+|)size=(\s+|)(?<fsize>\d+)?kB(\s+|)time=(?<time_hr>\d+)?:(?<time_min>\d+)?:(?<time_sec>\d+)?.(?<time_frac>\d+)?\sbitrate=(\s+|)(?<bitrate>\d+.\d+)kbits/s"; //frame= 34 fps= 0 q=-31.0 size= 0kB time=00:00:00.00 bitrate= 0.0kbits/s
        private static string _frameInfoPattern_mp3 = @"size=(\s+|)(?<fsize>\d+)?kB(\s+|)time=(?<time_hr>\d+)?:(?<time_min>\d+)?:(?<time_sec>\d+)?.(?<time_frac>\d+)?\sbitrate=(\s+|)(?<bitrate>\d+.\d+)kbits/s"; //size= 0kB time=00:00:00.00 bitrate= 0.0kbits/s

        // Amikor egyesítettem az auido és a video streamet akkor az output-ban a "q" érték negatív előjelű.
        // private static string _frameInfoPattern = @"frame=(\s+|)(?<frame>\d+)?(\s+|)fps=(\s+|)(?<fps>\d+)?(\s+|)q=-(?<q>\d+.\d+)?(\s+|)size=(\s+|)(?<fsize>\d+)?kB(\s+|)time=(?<time_hr>\d+)?:(?<time_min>\d+)?:(?<time_sec>\d+)?.(?<time_frac>\d+)?\sbitrate=(\s+|)(?<bitrate>\d+.\d+)kbits/s"; //frame= 34 fps= 0 q=31.0 size= 0kB time=00:00:00.00 bitrate= 0.0kbits/s

        public static void Phars(string data, FFmpegInfo info)
        {
            if(data == null)
                return;

            if (data.Contains("Lsize"))
                data = data.Replace("Lsize", "size");

            Match match;
            if (data.Contains("Press [q] to stop,"))
            {
                // processing started properly
            }
            else if (data.Contains("title"))
            {
                match = Regex.Match(data.Trim(), _titleExpression);
                if (match.Success)
                {
                    // fetch media title information
                    info.Title = match.Groups["vtitle"].Value;
                }
            }
            else if (data.Contains("comment"))
            {
                match = Regex.Match(data.Trim(), _commentExpression);
                if (match.Success)
                {
                    // fetch media comment information
                    info.Footage = match.Groups["comment_footage"].Value;
                    info.Producer = match.Groups["comment_producer"].Value;
                    info.Music = match.Groups["comment_music"].Value;
                }
            }
            else if (data.Contains("Duration"))
            {
                match = Regex.Match(data, _minfoPattern);
                if (match.Success)
                {
                    info.Hours = Convert.ToInt32(match.Groups["hours"].Value);
                    info.Minutes = Convert.ToInt32(match.Groups["minutes"].Value);
                    info.Seconds = Convert.ToInt32(match.Groups["seconds"].Value);
                    int fractions = Convert.ToInt32(match.Groups["fractions"].Value);
                    if (fractions > 5)
                        info.Seconds = info.Seconds + 1;

                    info.Start = match.Groups["start"].Value;
                    info.Video_Bitrate = match.Groups["bitrate"].Value;

                    info.Duration = info.Hours + ":" + info.Minutes + ":" + info.Seconds;
                    info.Duration_Sec = (info.Hours * 3600) + (info.Minutes * 60) + info.Seconds;
                }
            }
            else if (data.StartsWith("size") && data.Contains("bitrate"))
            {
                match = Regex.Match(data, _frameInfoPattern_mp3);
                if (match.Success)
                {
                    info.ProcessedSize = Convert.ToInt64(match.Groups["fsize"].Value);

                    int phours = Convert.ToInt32(match.Groups["time_hr"].Value);
                    int pminutes = Convert.ToInt32(match.Groups["time_min"].Value);
                    int pseconds = Convert.ToInt32(match.Groups["time_sec"].Value);
                    int fractions = Convert.ToInt32(match.Groups["time_frac"].Value);
                    if (fractions > 5)
                        pseconds = pseconds + 1;

                    int hr_sec = phours * 3600;
                    int min_sec = pminutes * 60;
                    info.ProcessedTime = hr_sec + min_sec + pseconds;

                    // based on time rather than content size
                    int totalprocessingleft = info.Duration_Sec - info.ProcessedTime;

                    info.ProcessingLeft = Math.Round((double)(totalprocessingleft * 100) / info.Duration_Sec, 0);
                    if (info.ProcessingLeft < 0.5)
                        info.ProcessingLeft = 0;
                    info.ProcessingCompleted = Math.Round(100 - info.ProcessingLeft, 0);
                    //long totalprocessingleft = TotalSize - ProcessedSize;
                }
                // frame information
            }
            else if (data.Contains("frame") && data.Contains("size"))
            {
                string pattern;

                if (data.Contains("q=-"))
                {
                    pattern = _frameInfoPattern_q_minus;
                }
                else
                {
                    pattern = _frameInfoPattern;
                }

                match = Regex.Match(data, pattern);
                if (match.Success)
                {
                    int frame = Convert.ToInt32(match.Groups["frame"].Value);
                    info.ProcessedSize = Convert.ToInt64(match.Groups["fsize"].Value);

                    int phours = Convert.ToInt32(match.Groups["time_hr"].Value);
                    int pminutes = Convert.ToInt32(match.Groups["time_min"].Value);
                    int pseconds = Convert.ToInt32(match.Groups["time_sec"].Value);
                    int fractions = Convert.ToInt32(match.Groups["time_frac"].Value);
                    if (fractions > 5)
                        pseconds = pseconds + 1;

                    int hr_sec = phours * 3600;
                    int min_sec = pminutes * 60;
                    info.ProcessedTime = hr_sec + min_sec + pseconds;

                    // based on time rather than content size
                    int totalprocessingleft = info.Duration_Sec - info.ProcessedTime;

                    info.ProcessingLeft = Math.Round((double)(totalprocessingleft * 100) / info.Duration_Sec, 0);
                    if (info.ProcessingLeft < 0.5)
                        info.ProcessingLeft = 0;
                    info.ProcessingCompleted = Math.Round(100 - info.ProcessingLeft, 0);
                    //long totalprocessingleft = TotalSize - ProcessedSize;
                }
                // frame information
            }
            else if (data.Contains("Stream"))
            {
                string video_data = "";
                string audio_data = "";
                match = Regex.Match(data, _strmPattern);
                if (match.Success)
                {
                    info.Input_Type = match.Groups["type"].Value;
                    if (info.Input_Type == "Video")
                        video_data = match.Groups["data"].Value;
                    else
                        audio_data = match.Groups["data"].Value;
                }
                else
                {
                    // if above logic failed -> replace (.) with (:) under Replace Stream #0.0 with Stream #0:0
                    _strmPattern = @"Stream #(?<number>\d+:\d+?)(\((?<language>\w+)\))?(\[(?<abc>\w+)\])?:\s(?<type>\w+):\s(?<data>.+)";
                    match = Regex.Match(data, _strmPattern);
                    if (match.Success)
                    {
                        info.Input_Type = match.Groups["type"].Value;
                        if (info.Input_Type == "Video")
                            video_data = match.Groups["data"].Value;
                        else
                            audio_data = match.Groups["data"].Value;
                    }
                }
                string bt_pattern = @"(?<bitrate>\d+(\.\d+)?(\s+)kb/s)"; // bitrate pattern 200 kb/s
                string codec_pattern = @"(?<codec>\w+)?,"; // codec pattern
                string fr_pattern = @"(?<framerate>\d+(\.\d+)?(\s|)(tbr|fps))"; // framerate pattern // 29.97 tbr, 29.97 fps
                string smp_pattern = @"(?<samplingrate>\d+(\.\d+)?(\s+)Hz),(\s|)(?<channel>\w+)"; // sampling rate, channel pattern 22050 Hz, sterio
                if (video_data != "")
                {
                    // get video width and height
                    string size_pattern = @"(?<width>\d+)x(?<height>\d+)"; // width / height pattern
                    match = Regex.Match(video_data, size_pattern);
                    if (match.Success)
                    {
                        info.Width = Convert.ToInt32(match.Groups["width"].Value);
                        info.Height = Convert.ToInt32(match.Groups["height"].Value);
                        if (info.Width == 0 || info.Width > 4000 || info.Height == 0 || info.Height > 4000)
                        {
                            if (info.Width == 0 && info.Height == 0)
                            { }
                            else
                            {
                                info.Width = 0;
                                info.Height = 0;
                                match = match.NextMatch();
                                info.Width = Convert.ToInt32(match.Groups["width"].Value);
                                info.Height = Convert.ToInt32(match.Groups["height"].Value);
                            }
                        }
                    }

                    match = Regex.Match(video_data, bt_pattern);
                    if (match.Success)
                    {
                        info.Video_Bitrate = match.Groups["bitrate"].Value;
                    }
                    // Codec Parsing
                    match = Regex.Match(video_data, codec_pattern);
                    if (match.Success)
                    {
                        info.Vcodec = match.Groups["codec"].Value;
                    }
                    // Frame Rate
                    match = Regex.Match(video_data, fr_pattern);
                    if (match.Success)
                    {
                        info.FrameRate = match.Groups["framerate"].Value;
                    }
                }
                if (audio_data != "")
                {
                    // Audio Data Parsing
                    // sampling rate
                    match = Regex.Match(audio_data, smp_pattern);
                    if (match.Success)
                    {
                        info.SamplingRate = match.Groups["samplingrate"].Value;
                        info.Channel = match.Groups["channel"].Value;
                    }
                    // Channel -> Under Construction
                    // Audio Bitrate
                    match = Regex.Match(audio_data, bt_pattern);
                    if (match.Success)
                    {
                        info.Audio_Bitrate = match.Groups["bitrate"].Value;
                    }
                    // Codec Parsing
                    match = Regex.Match(audio_data, codec_pattern);
                    if (match.Success)
                    {
                        info.Acodec = match.Groups["codec"].Value;
                    }
                }
            }
        }
    }
}
