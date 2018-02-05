using EasyDownloader.Utilities.Downloader.Cda;
using EasyDownloader.Utilities.Downloader.Trt;
using EasyDownloader.Utilities.Downloader.YouTube;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EasyDownloader.Utilities.Downloader
{
    public class DownloaderProvider
    {
        private readonly DownloaderDTO _downloaderDTO;
        private VideoDTO _currentVideo;
        private string _currentUrl;
        private EnumDownloaderSource _currentDownloaderSource;

        public DownloaderProvider(DownloaderDTO downloaderDTO)
        {
            _downloaderDTO = downloaderDTO;
        }

        public async Task<IEnumerable<VideoDTO>> GetVideos()
        {
            switch (_downloaderDTO.DownloaderSource)
            {
                case EnumDownloaderSource.YouTube:
                    return await GetVideosByType(new YouTubeDownloader(_downloaderDTO));
                case EnumDownloaderSource.Trt:
                    return await GetVideosByType(new TrtDownloader(_downloaderDTO));
                case EnumDownloaderSource.Cda:
                    return await GetVideosByType(new CdaDownloader(_downloaderDTO));
                default:
                    throw new Exception("Nieobsługiwany typ pobrania filmów");
            }
        }

        private async Task<IEnumerable<VideoDTO>> GetVideosByType(I_Downloader downloader)
            => await downloader.GetVideos(_downloaderDTO);

        public void DownloadVideos(IEnumerable<VideoDTO> videos)
        {
            if (!videos.Any())
            {
                throw new ArgumentException("Brak filmów do pobrania");
            }

            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
            Process p = new Process();
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            p = Process.Start(startInfo);
            p.OutputDataReceived += OutputDataReceived;
            p.BeginOutputReadLine();

            string strArgs;
            IEnumerable<VideoDTO> downloadVideos = _downloaderDTO.DownloaderType == EnumDownloaderType.ByPhrase ?
                videos.Where(x => x.IsSelected) : new List<VideoDTO>() { videos.First() };
            foreach (var video in downloadVideos)
            {
                _currentVideo = videos.Single(x => x.Url == video.Url);
                strArgs = GetCommandLineBySourceType(video);
                    //@".\youtube-dl.exe --download-archive Downloaded.txt  https://www.youtube.com/watch?v=" + str.Url;
                p.StandardInput.WriteLine(strArgs);
            }
        }

        private string GetCommandLineBySourceType(VideoDTO videoDTO)
        {
            _currentDownloaderSource = (EnumDownloaderSource)videoDTO.SourceType;

            switch ((EnumDownloaderSource)videoDTO.SourceType)
            {
                case EnumDownloaderSource.YouTube:
                    return @".\youtube-dl.exe --download-archive Downloaded.txt  https://www.youtube.com/watch?v=" + videoDTO.Url;
                case EnumDownloaderSource.Trt:
                    return @".\youtube-dl.exe --download-archive Downloaded.txt  https://www.trt.pl/film/" + videoDTO.Url + " / " + videoDTO.AdditionalUrl;
                case EnumDownloaderSource.Cda:
                    return @".\youtube-dl.exe --download-archive Downloaded.txt  https://www.cda.pl/video/" + videoDTO.AdditionalUrl;
            default:
                    throw new ArgumentException("Nie obsługowany typ źródła");
            }
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data.Length > 15 && e.Data.Contains("[youtube]") && _currentDownloaderSource == EnumDownloaderSource.YouTube)
            {
                _currentUrl = e.Data.Substring(10, 11);
            }

            if (e.Data.Length == 39 && e.Data.Contains("[generic]") && _currentDownloaderSource == EnumDownloaderSource.Trt)
            {
                string currentUrl = e.Data.Substring(10, 10);
                if (Common.Common.Videos.Any(x => x.Url == currentUrl))
                {
                    _currentUrl = currentUrl;
                }
            }

            if (e.Data.Contains("Destination:") && _currentDownloaderSource == EnumDownloaderSource.Cda)
            {
                foreach (var video in Common.Common.Videos)
                {
                    if (e.Data.Contains(video.Url))
                    {
                        _currentUrl = video.Url;
                    }
                }
            }

            if (e.Data.Length > 15 && e.Data.Contains("[download]") && e.Data.Contains("% of"))
            {
                VideoDTO youTubeVideo = Common.Common.Videos.Single(x => x.Url == _currentUrl);
                int indexOf = e.Data.IndexOf("of");
                youTubeVideo.DownloadedPercent = e.Data.Substring(10, indexOf - 10);
            }
        }
    }
}
