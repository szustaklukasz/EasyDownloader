using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyDownloader.Utilities.Downloader.Cda
{
    public class CdaDownloader : I_Downloader
    {
        private DownloaderDTO _downloaderDTO;

        public CdaDownloader(DownloaderDTO _downloaderDTO)
        {
            this._downloaderDTO = _downloaderDTO;
        }

        public async Task<IEnumerable<VideoDTO>> GetVideos(DownloaderDTO downloaderDTO)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync($"https://www.cda.pl/info/{downloaderDTO.SearchText}").Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string result = content.ReadAsStringAsync().Result;
                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(result);
                        List<string> trtVideosFromHtml = new List<string>();
                        var regex = new Regex("<a.*?href=\"(.*?)\".*/?>");
                        Match match;
                        for (match = regex.Match(document.ParsedText); match.Success; match = match.NextMatch())
                        {
                            foreach (Group group in match.Groups)
                            {
                                trtVideosFromHtml.Add(group.Value);
                            }
                        }
                        if (_downloaderDTO.DownloaderType == EnumDownloaderType.ByPhrase)
                        {

                            IEnumerable<string> filteredVideos = trtVideosFromHtml.Where(x => x.Contains("link-title-visit") && x.Contains("/video/")).ToList();
                            return ConvertVideoFromHtmlToDTOVideos(filteredVideos);
                        }
                        else
                        {
                            IEnumerable<string> filteredVideos = trtVideosFromHtml.Where(x => x.Contains("link-title-visit") && 
                                                                                              x.Contains("/video/") &&
                                                                                              x.Contains(downloaderDTO.SearchText)).ToList();

                            filteredVideos = filteredVideos.Any() ? filteredVideos.Take(1) : new List<string>();

                            return ConvertVideoFromHtmlToDTOVideos(filteredVideos);
                        }
                    }
                }
            }
        }

        private IEnumerable<VideoDTO> ConvertVideoFromHtmlToDTOVideos(IEnumerable<string> trtVideosFromHtml)
        {
            string textFileOfDownloaded = System.IO.File.ReadAllText(@".\Downloaded.txt");

            foreach (var video in trtVideosFromHtml)
            {
                yield return new VideoDTO()
                {
                    DownloadedPercent = textFileOfDownloaded.Contains(video.Substring(video.IndexOf("/video/") + 7, 9).Replace("/", string.Empty)) ? "100%" : "0%",
                    IsSelected = false,
                    Name = video.Substring(video.IndexOf("\">") + 2, video.IndexOf("</a>") - video.IndexOf("\">") - 2),
                    Url = video.Substring(video.IndexOf("/video/") + 7, 9).Replace("/", string.Empty),
                    SourceType = (int)EnumDownloaderSource.Cda,
                    AdditionalUrl = video.Substring(video.IndexOf("/video/") + 7, video.IndexOf("\">") - video.IndexOf("/video/") - 7)
                };
            }
        }
    }
}
