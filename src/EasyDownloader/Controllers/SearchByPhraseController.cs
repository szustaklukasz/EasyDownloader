using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyDownloader.Utilities.Downloader;
using EasyDownloader.Utilities.Common;
using EasyDownloader.Utilities;
using System.Linq;

namespace EasyDownloader.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SearchByPhraseController : Controller
    {
        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<VideoDTO>> Search(string searchText, string downloadSource)
        {
            Common.Videos = await new DownloaderProvider(new DownloaderDTO()
            {
                SearchText = searchText,
                DownloaderSource = (EnumDownloaderSource)Convert.ToInt32(downloadSource),
                DownloaderType = EnumDownloaderType.ByPhrase
            }).GetVideos();

            return Common.Videos;
        }

        [HttpGet]
        public IEnumerable<VideoDTO> AskForProgress()
        {
            return Common.Videos;
        }

        // POST api/values
        [HttpPost]
        public bool Download([FromBody]IEnumerable<VideoDTO> videos)
        {
            try
            {
                Common.Videos = videos;
                new DownloaderProvider(new DownloaderDTO()
                {
                    DownloaderType = EnumDownloaderType.ByPhrase
                }).DownloadVideos(videos);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // POST api/values
        [HttpPost]
        public void UpdateSelectedOfVideos([FromBody]IEnumerable<VideoDTO> videos)
        {
            try
            {
                foreach (var videoFrontEnd in videos)
                {
                    VideoDTO videoDTO = Common.Videos.SingleOrDefault(x => x.Url == videoFrontEnd.Url);
                    if (videoDTO != null)
                    {
                        videoDTO.IsSelected = videoFrontEnd.IsSelected;
                    }
                }
            }
            catch (Exception ex) { }
        }
    }
}


