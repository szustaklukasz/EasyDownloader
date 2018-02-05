using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EasyDownloader.Utilities
{
    public class PageDownloader
    {
        public async Task<string> DownloadPageAsync(string page)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(page))
                using (HttpContent content = response.Content)
                {
                    return await content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Nieprawidłowy link");
                return null;
            }
        }
    }
}
