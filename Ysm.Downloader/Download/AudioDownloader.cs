using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using Ysm.Downloader.Assets;

namespace Ysm.Downloader.Download
{
    internal class AudioDownloader : DownloaderBase, IDownloader
    {
        public AudioDownloader(AudioStreamInfo audioStream)
        {
            AudioStream = audioStream;
            TokenSource = new CancellationTokenSource();
        }

        public void Download(string title, string dir)
        {
            // Ignore
        }

        public void DownloadAudio(string title, string dir)
        {
            CancellationToken token = TokenSource.Token;

            OnDownloadingStarted(null);

            Task.Run(async () =>
            {
                string output = GetOutput(dir, title, "mp3");
                string convertOutput = GetTempOutput("mp3");
                string downloadOutput = GetTempOutput(AudioStream.Container.GetFileExtension());

                await Download(downloadOutput, token);

                Convert(downloadOutput, convertOutput);

                OnDownloadingProgressChanged(new DownloadEventArgs { Message = $"{Properties.Resources.Title_ConvertingTo} MP3", IsConverting = true });

                Move(convertOutput, output);

                Cleanup(downloadOutput, convertOutput);

                OnDownloadingFinished(new DownloadEventArgs { IsComplete = true, Output = output });

            }, token);
        }

        public void DownloadAndConvert(string title, string dir, string format)
        {
            // Ignore
        }

        private async Task Download(string path, CancellationToken token)
        {
            Progress<double> progress = new Progress<double>(percent =>
            {
                OnDownloadingProgressChanged(new DownloadEventArgs { Message = Properties.Resources.Title_Downloading, Percent = Math.Round(percent * 100, 0) });
            });

            YoutubeClient client = new YoutubeClient();

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await client.DownloadMediaStreamAsync(AudioStream, stream, progress, token);
            }
        }

        private void Convert(string audio, string output)
        {
            using (Process process = new Process())
            {
                Processes.Add(process);

                process.StartInfo.FileName = Path.GetFullPath(Converter);
                process.StartInfo.Arguments = $"-i {audio} -f mp3 -b:a 192000 -vn {output}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.ErrorDataReceived += Converter_ConsoleOutputRecived;
                process.Start();
                process.BeginErrorReadLine();
                process.WaitForExit();

                Processes.Remove(process);
            }
        }


        private void Converter_ConsoleOutputRecived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                OutputPharser.Phars(e.Data, ConvertFFmpegInfo);

                OnDownloadingProgressChanged(new DownloadEventArgs { Message = $"{Properties.Resources.Title_ConvertingTo} MP3", Percent = ConvertFFmpegInfo.ProcessingCompleted, IsConverting = true});

                Debug.WriteLine($"AudioConverter: {e.Data}");
            }
        }
    }
}
