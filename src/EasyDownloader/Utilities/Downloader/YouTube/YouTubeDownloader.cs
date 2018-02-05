using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDownloader.Utilities.Downloader.YouTube
{
    public class YouTubeDownloader : I_Downloader
    {
        private DownloaderDTO _downloaderDTO;

        public YouTubeDownloader(DownloaderDTO _downloaderDTO)
        {
            this._downloaderDTO = _downloaderDTO;
        }

        public async Task<IEnumerable<VideoDTO>> GetVideos(DownloaderDTO downloaderDTO)
        {
            if (_downloaderDTO.DownloaderType == EnumDownloaderType.ByPhrase)
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyC6QBvO2DYcBUWIUHJFxhjDzGmB9n6Pilc",
                    ApplicationName = "EasyDownloader"
                });

                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = _downloaderDTO.SearchText;
                searchListRequest.MaxResults = 50;

                var searchListResponse = await searchListRequest.ExecuteAsync();

                List<VideoDTO> videos = new List<VideoDTO>();

                string textFileOfDownloaded = System.IO.File.ReadAllText(@".\Downloaded.txt");

                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            videos.Add(new VideoDTO()
                            {
                                Name = searchResult.Snippet.Title,
                                Url = searchResult.Id.VideoId,
                                IsSelected = false,
                                DownloadedPercent = textFileOfDownloaded.Contains(searchResult.Id.VideoId) ? "100%" : "0%",
                                SourceType = (int)EnumDownloaderSource.YouTube
                            });
                            break;
                    }
                }

                return videos;
            }
            else
            {
                if (_downloaderDTO.SearchText.Length != 11)
                {
                    throw new ArgumentException("Nie prawidłowa długość linka");
                }

                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyC6QBvO2DYcBUWIUHJFxhjDzGmB9n6Pilc",
                    ApplicationName = "EasyDownloader"
                });

                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = _downloaderDTO.SearchText;
                searchListRequest.MaxResults = 50;

                var searchListResponse = await searchListRequest.ExecuteAsync();

                string textFileOfDownloaded = System.IO.File.ReadAllText(@".\Downloaded.txt");

                foreach (var searchResult in searchListResponse.Items)
                {
                    if (searchResult.Id.VideoId == _downloaderDTO.SearchText)
                    {
                        return new List<VideoDTO>() { new VideoDTO()
                        {
                            Name = searchResult.Snippet.Title,
                            Url = searchResult.Id.VideoId,
                            IsSelected = false,
                            DownloadedPercent = textFileOfDownloaded.Contains(searchResult.Id.VideoId) ? "100%" : "0%"
                        }};
                    }
                }

                return new List<VideoDTO>();
            }
        }
    }
}
