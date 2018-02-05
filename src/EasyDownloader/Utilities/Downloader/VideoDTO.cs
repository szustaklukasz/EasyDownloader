namespace EasyDownloader.Utilities.Downloader
{
    public class VideoDTO
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsSelected { get; set; }
        public string DownloadedPercent { get; set; }

        public string IsDownloadedName
        {
            get
            {
                if (DownloadedPercent.Contains("100%"))
                {
                    return "TAK";
                }

                return "NIE";
            }
        }

        public bool IsDownloaded
        {
            get
            {
                if (DownloadedPercent.Contains("100%"))
                {
                    return true;
                }

                return false;
            }
        }

        public string AdditionalUrl { get; set; }
        public int SourceType { get; set; }
    }
}
