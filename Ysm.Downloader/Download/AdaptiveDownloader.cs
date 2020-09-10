using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using Ysm.Core;
using Ysm.Downloader.Assets;

namespace Ysm.Downloader.Download
{
    public class AdaptiveDownloader : DownloaderBase, IDownloader
    {
        public AdaptiveDownloader(VideoStreamInfo videoStream, AudioStreamInfo audioStream)
        {
            AudioStream = audioStream;
            VideoStream = videoStream;

            TokenSource = new CancellationTokenSource();
        }

        public void Download(string title, string dir)
        {
            CancellationToken token = TokenSource.Token;

            OnDownloadingStarted(null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    string output = GetOutput(dir, title);
                    string mergeOutput = GetTempOutput();

                    Task<string> audioTask = Task.Run(() => DownloadAudio(token), token);
                    Task<string> videoTask = Task.Run(() => DownloadVideo(token), token);

                    Task.WaitAll(videoTask, audioTask);

                    string videoOutput = videoTask.Result;
                    string audioOutput = audioTask.Result;

                    Merge(videoOutput, audioOutput, mergeOutput);

                    Move(mergeOutput, output);

                    Cleanup(videoOutput, audioOutput, mergeOutput);

                    OnDownloadingFinished(new DownloadEventArgs { IsComplete = true, Output = output });
                }
                catch (Exception ex)
                {
                    Logger.Log(MethodBase.GetCurrentMethod(), ex.Message);
                }

            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        }

        public void DownloadAudio(string title, string dir)
        {
            // Ignore
        }

        public void DownloadAndConvert(string title, string dir, string format)
        {
            Format = format;

            CancellationToken token = TokenSource.Token;

            OnDownloadingStarted(null);

            Task.Factory.StartNew(() =>
            {
                string output = GetOutput(dir, title);
                string mergeOutput = GetTempOutput();
                string convertOutput = GetTempOutput(format);

                Task<string> audioTask = Task.Run(() => DownloadAudio(token), token);
                Task<string> videoTask = Task.Run(() => DownloadVideo(token), token);

                audioTask.Start();
                videoTask.Start();

                Task.WaitAll(videoTask, audioTask);

                string videoTemp = videoTask.Result;
                string audioTemp = audioTask.Result;

                Merge(videoTemp, audioTemp, mergeOutput);

                if (string.Equals(format, VideoStream.Container.GetFileExtension(), StringComparison.OrdinalIgnoreCase) == false)
                {
                    Convert(mergeOutput, convertOutput, format);
                    Move(convertOutput, output);
                }
                else
                {
                    Move(mergeOutput, output);
                }

                Cleanup(videoTemp, audioTemp, mergeOutput, convertOutput);

                OnDownloadingFinished(new DownloadEventArgs { IsComplete = true, Output = output });

            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        // ----------- //

        private async Task<string> DownloadAudio(CancellationToken token)
        {
            string temp = GetTempOutput(AudioStream.Container.GetFileExtension());

            YoutubeClient client = new YoutubeClient();

            using (FileStream stream = new FileStream(temp, FileMode.Create))
            {
                await client.DownloadMediaStreamAsync(AudioStream, stream, null, token);
            }

            return temp;
        }

        private async Task<string> DownloadVideo(CancellationToken token)
        {
            Progress<double> progress = new Progress<double>(percent =>
            {
                OnDownloadingProgressChanged(new DownloadEventArgs { Message = Properties.Resources.Title_Downloading, Percent = Math.Round(percent * 100, 0) });
            });

            string temp = GetTempOutput(VideoStream.Container.GetFileExtension());

            YoutubeClient client = new YoutubeClient();

            using (FileStream stream = new FileStream(temp, FileMode.Create))
            {
                await client.DownloadMediaStreamAsync(VideoStream, stream, progress, token);
            }

            return temp;
        }

        private void Convert(string input, string output, string format)
        {
            using (Process process = new Process())
            {
                string argument = GetConverterArgument(input, output, format);

                Processes.Add(process);

                OnDownloadingProgressChanged(new DownloadEventArgs { Message = $"{Properties.Resources.Title_ConvertingTo} {format}", IsConverting = true, Percent = 0});

                process.StartInfo.FileName = Path.GetFullPath(Converter);
                process.StartInfo.Arguments = argument;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.ErrorDataReceived += Convert_ConsoleOutputRecived;
                process.Start();
                process.BeginErrorReadLine();
                process.WaitForExit();

                Processes.Remove(process);

            }
        }

        private void Convert_ConsoleOutputRecived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                OutputPharser.Phars(e.Data, ConvertFFmpegInfo);

                OnDownloadingProgressChanged(new DownloadEventArgs { Message = $"{Properties.Resources.Title_ConvertingTo} {Format}", Percent = ConvertFFmpegInfo.ProcessingCompleted });

                Debug.WriteLine(e.Data);
            }
        }

        private void Merge(string video, string audio, string output)
        {
            using (Process process = new Process())
            {
                Processes.Add(process);

                OnDownloadingProgressChanged(new DownloadEventArgs { Message = Properties.Resources.Title_Merging, IsConverting = true, Percent = 0});

                string argument = GetMergeArgument(video, audio, output);

                process.StartInfo.FileName = Path.GetFullPath(Converter);
                process.StartInfo.Arguments = argument;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.ErrorDataReceived += Merge_ConsoleOutputRecived;
                process.Start();
                process.BeginErrorReadLine();
                process.WaitForExit();

                Processes.Remove(process);
            }
        }

        private void Merge_ConsoleOutputRecived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                OutputPharser.Phars(e.Data, MergeFFmpegInfo);

                OnDownloadingProgressChanged(new DownloadEventArgs { Message = Properties.Resources.Title_Merging, Percent = MergeFFmpegInfo.ProcessingCompleted });

                Debug.WriteLine(e.Data);
            }
        }
    }
}
