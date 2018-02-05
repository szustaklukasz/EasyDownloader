using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyDownloader.Utilities.Downloader.Trt
{
    public class TrtDownloader : I_Downloader
    {
        private DownloaderDTO _downloaderDTO;

        public TrtDownloader(DownloaderDTO _downloaderDTO)
        {
            this._downloaderDTO = _downloaderDTO;
        }

        public async Task<IEnumerable<VideoDTO>> GetVideos(DownloaderDTO downloaderDTO)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync($"https://www.trt.pl/szukaj-filmy/{downloaderDTO.SearchText}").Result)
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

                            IEnumerable<string> filteredVideos = trtVideosFromHtml.Where(x => x.Contains("<a href=\"/film/") && x.Contains("</a>") &&
                                         x.Contains("\">") && !x.Contains("class")).ToList();

                            filteredVideos = filteredVideos.Count() < 24 ? filteredVideos.Take(filteredVideos.Count() - 7) : filteredVideos.Take(24);

                            return ConvertVideoFromHtmlToDTOVideos(filteredVideos);
                        }
                        else
                        {
                            IEnumerable<string> filteredVideos = trtVideosFromHtml.Where(x => x.Contains("<a href=\"/film/") && x.Contains("</a>") &&
                                         x.Contains("\">") && !x.Contains("class") && x.Contains(downloaderDTO.SearchText)).ToList();

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
                    DownloadedPercent = textFileOfDownloaded.Contains(video.Substring(26, video.IndexOf("\">") - 26)) ? "100%" : "0%",
                    IsSelected = false,
                    Name = video.Substring(video.IndexOf("\">") + 2, video.IndexOf("</a>") - video.IndexOf("\">") - 2),
                    Url = video.Substring(15, 10),
                    AdditionalUrl = video.Substring(26, video.IndexOf("\">") - 26),
                    SourceType = (int)EnumDownloaderSource.Trt
                };
            }
        }
    }
}
