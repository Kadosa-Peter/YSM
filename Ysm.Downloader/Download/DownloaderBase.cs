using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using YoutubeExplode.Models.MediaStreams;
using Ysm.Core;
using Ysm.Downloader.Models;

namespace Ysm.Downloader.Download
{    
    public abstract class DownloaderBase
    {
        public event EventHandler<DownloadEventArgs> DownloadStarted;
        public event EventHandler<DownloadEventArgs> DownloadFinished;
        public event EventHandler<DownloadEventArgs> DownloadProgessChanged;

        protected VideoStreamInfo VideoStream;
        protected AudioStreamInfo AudioStream;
        protected MuxedStreamInfo MuxedStream;

        protected CancellationTokenSource TokenSource;

        protected List<Process> Processes = new List<Process>();

        protected FFmpegInfo MuxedFFmpegInfo = new FFmpegInfo();
        protected FFmpegInfo MergeFFmpegInfo = new FFmpegInfo();
        protected FFmpegInfo ConvertFFmpegInfo = new FFmpegInfo();

        protected string Converter = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\ffmpeg.exe");

        protected string Format;


        protected List<string> TempOutputs = new List<string>();

        protected virtual void OnDownloadingStarted(DownloadEventArgs e)
        {
            DownloadStarted?.Invoke(this, e);
        }

        protected virtual void OnCanceled(DownloadEventArgs e)
        {
            DownloadFinished?.Invoke(this, e);
        }

        protected virtual void OnDownloadingFinished(DownloadEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                DownloadFinished?.Invoke(this, e);
            }));
        }

        protected virtual void OnDownloadingProgressChanged(DownloadEventArgs e)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                DownloadProgessChanged?.Invoke(this, e);
            }));
        }

        protected void Cleanup(params string[] files)
        {
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // ignore
                }
            }
        }

        protected string GetConverterArgument(string input, string output, string format)
        {
            string extension = Path.GetExtension(input);

            switch (format)
            {
                case "MKV":
                    {
                        if (input.EndsWith("mp4") == false)
                        {
                            return $"-i {input} {output}";
                        }
                        else
                        {
                            return $"-i {input} -c:v copy -c:a copy {output}";
                        }
                    }
                case "MP4":
                    {
                        if (extension == ".webm")
                        {
                            return $"-i {input} -qscale 0 {output}";
                        }
                        else
                        {
                            return $"-i {input} {output}";
                        }

                    }
                case "FLV":
                    {
                        if (extension == ".webm")
                        {
                            return $"-i {input} -qscale 8 -ar 44100 {output}";
                        }
                        else
                        {
                            return $"-i {input} -c:v libx264 {output}";
                        }
                    }
            }

            throw new ArgumentException();
        }

        protected string GetMergeArgument(string video, string audio, string output)
        {
            string extension = Path.GetExtension(video);

            if (extension == ".webm")
            {
                return $"-i {video} -i {audio} -c:v copy -c:a libvorbis -strict experimental {output}";
            }
            else
            {
                return $"-i {video} -i {audio} -c:v copy -c:a aac -strict experimental {output}";
            }
        }

        protected string GetTempOutput(string ext = null)
        {
            string fileName;

            if (ext != null)
            {
                fileName = $"{Identifier.Sort}.{ext}";
            }
            else
            {
                if (MuxedStream != null)
                {
                    fileName = $"{Identifier.Sort}.{MuxedStream.Container.GetFileExtension()}";
                }
                else
                {
                    fileName = $"{Identifier.Sort}.{VideoStream.Container.GetFileExtension()}";
                }

            }

            fileName = Files.CleanFileName(fileName);
            string output = Path.Combine(FileSystem.Temp, fileName);
            output = Files.ShortenFilePath(output);
            output = Files.VerifyFilePath(output);

            TempOutputs.Add(output);

            return output;
        }

        protected string GetOutput(string dir, string title, string ext = null)
        {
            string fileName;

            if (ext != null)
            {
                fileName = $"{title}.{ext}";
            }
            else
            {
                if (MuxedStream != null)
                {
                    fileName = $"{title}.{MuxedStream.Container.GetFileExtension()}";
                }
                else
                {
                    fileName = $"{title}.{VideoStream.Container.GetFileExtension()}";
                }
            }

            fileName = Files.CleanFileName(fileName);
            string output = Path.Combine(dir, fileName);
            output = Files.ShortenFilePath(output);
            output = Files.VerifyFilePath(output);

            return output;
        }

        public void Cancel()
        {
            TokenSource.Cancel();

            try
            {
                foreach (Process process in Processes)
                {
                    process?.Kill();
                }

                Task.Delay(1000).GetAwaiter().OnCompleted(() =>
                {
                    foreach (string output in TempOutputs)
                    {
                        File.Delete(output);
                    }
                });
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e.Message);
            }
        }

        protected void Move(string source, string target)
        {
            try
            {
                File.Move(source, target);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
