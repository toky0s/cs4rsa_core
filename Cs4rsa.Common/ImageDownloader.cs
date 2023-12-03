using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cs4rsa.Common
{
    public abstract class ImageDownloader
    {
        /// <summary>
        /// Tải một hình ảnh từ một URL.
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="saveTo">Đường dẫn lưu ảnh.</param>
        /// <returns>Tải ảnh thành công trả về true, ngược lại trả về false.</returns>
        public static async Task<bool> DownloadImage(string url, string saveTo)
        {
            try
            {
                using var httpClient = new HttpClient();
                using var res = await httpClient.GetAsync(url);
                res.EnsureSuccessStatusCode();
                using var content = res.Content;
                var bytes = await content.ReadAsByteArrayAsync();
                File.WriteAllBytes(saveTo, bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
