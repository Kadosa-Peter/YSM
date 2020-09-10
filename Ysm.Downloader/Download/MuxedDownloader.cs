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
    //https://kwizzu.com/construct.html
    public class MuxedDownloader : DownloaderBase, IDownloader
    {
        public MuxedDownloader(MuxedStreamInfo muxedStream)
        {
            MuxedStream = muxedStream;
            TokenSource = new CancellationTokenSource();
        }

        public void Download(string title, string dir)
        {
            CancellationToken token = TokenSource.Token;

            OnDownloadingStarted(null);

            try
            {
                Task.Run(async () =>
                {
                    string output = GetOutput(dir, title);
                    string tempOutput = GetTempOutput();

                    await Download(tempOutput, token);

                    Move(tempOutput, output);

                    Cleanup(tempOutput);

                    OnDownloadingFinished(new DownloadEventArgs { IsComplete = true, Output = output });

                }, token);
            }
            catch (TaskCanceledException)
            {
                OnDownloadingFinished(new DownloadEventArgs { IsComplete = false });
            }
            catch (AggregateException ae)
            {
                foreach (Exception exception in ae.InnerExceptions)
                {
                    Logger.Log(MethodBase.GetCurrentMethod(), exception.Message);
                }
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e.Message);
            }
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

            try
            {
                Task.Run(async () =>
                {
                    string ext = format.ToLower();
                    string downloadOutput = GetTempOutput();
                    string convertOutput = GetTempOutput(ext);
                    string output = GetOutput(dir, title, ext);
                    string argument = GetConverterArgument(downloadOutput, convertOutput, format);

                    await Download(downloadOutput, token);

                    OnDownloadingProgressChanged(new DownloadEventArgs { Message = $"{Properties.Resources.Title_ConvertingTo} {format}", IsConverting = true, Percent = 0});

                    Convert(argument);

                    Move(convertOutput, output);

                    OnDownloadingFinished(new DownloadEventArgs { IsComplete = true, Output = output });

                    Cleanup(downloadOutput, convertOutput);

                }, token);
            }
            catch (TaskCanceledException)
            {
                OnDownloadingFinished(new DownloadEventArgs { IsComplete = false });
            }
            catch (AggregateException ae)
            {
                foreach (Exception exception in ae.InnerExceptions)
                {
                    Logger.Log(MethodBase.GetCurrentMethod(), exception.Message);
                }
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e.Message);
            }
        }

        private async Task Download(string path, CancellationToken token)
        {
            YoutubeClient client = new YoutubeClient();

            Progress<double> progress = new Progress<double>(percent =>
            {
                OnDownloadingProgressChanged(new DownloadEventArgs { Message = Properties.Resources.Title_Downloading, Percent = Math.Round(percent * 100, 0) });
            });

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await client.DownloadMediaStreamAsync(MuxedStream, stream, progress, token);
            }
        }

        private void Convert(string argument)
        {
            using (Process process = new Process())
            {
                Processes.Add(process);
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
            OutputPharser.Phars(e.Data, ConvertFFmpegInfo);

            OnDownloadingProgressChanged(new DownloadEventArgs { Message = $"{Properties.Resources.Title_ConvertingTo} {Format}", Percent = ConvertFFmpegInfo.ProcessingCompleted });

            Debug.WriteLine(e.Data);
        }

    }
}
