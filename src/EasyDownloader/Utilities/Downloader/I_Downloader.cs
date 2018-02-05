using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyDownloader.Utilities.Downloader
{
    public interface I_Downloader
    {
        Task<IEnumerable<VideoDTO>> GetVideos(DownloaderDTO downloaderDTO);
    }
}
