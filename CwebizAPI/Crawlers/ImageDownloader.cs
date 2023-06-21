/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Services.Interfaces;

namespace CwebizAPI.Crawlers
{
    /// <summary>
    /// Image Downloader
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public class ImageDownloader
    {
        private readonly HttpClient _httpClient;
        private readonly IImageStorageSvc _imageStorageSvc;
        private readonly ILogger<ImageDownloader> _logger;
        
        public ImageDownloader(
            HttpClient httpClient,
            IImageStorageSvc imageStorageSvc,
            ILogger<ImageDownloader> logger)
        {
            _httpClient = httpClient;
            _imageStorageSvc = imageStorageSvc;
            _logger = logger;
        }
        
        /// <summary>
        /// Tải một hình ảnh
        /// </summary>
        /// <param name="url">URL hình ảnh</param>
        /// <param name="imageName">Tên ảnh</param>
        /// <returns>Đường dẫn lưu ảnh</returns>
        public async Task<string> DownloadImage(string url, string imageName)
        {
            try
            {
                using HttpResponseMessage res = await _httpClient.GetAsync(url);
                res.EnsureSuccessStatusCode();
                using HttpContent content = res.Content;
                byte[] bytes = await content.ReadAsByteArrayAsync();
                return _imageStorageSvc.Save(bytes, imageName);
            }
            catch (Exception ex)
            {
                _logger.LogError("Have error occurs in image downloading progress: {ErrorMessage}", ex.Message);
                return string.Empty;
            }
        }
    }
}
