using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cs4rsa.Utils
{
    public class ImageDownloader
    {
        private readonly HttpClient _httpClient;
        public ImageDownloader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Tải một hình ảnh từ một URL.
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="saveTo">Đường dẫn lưu ảnh.</param>
        /// <returns>Tải ảnh thành công trả về true, ngược lại trả về false.</returns>
        public async Task<bool> DownloadImage(string url, string saveTo)
        {
            Debug.Assert(url != null);
            Debug.Assert(saveTo != null);

            try
            {
                using HttpResponseMessage res = await _httpClient.GetAsync(url);
                res.EnsureSuccessStatusCode();
                byte[] bytes = await res.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(saveTo, bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
