/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

namespace CwebizAPI.Services.Interfaces
{
    /// <summary>
    /// Interface Image Storage Service.
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public interface IImageStorageSvc
    {
        /// <summary>
        /// Lưu hình ảnh.
        /// </summary>
        /// <param name="imageBytes">Danh sách các byte.</param>
        /// <param name="imageName">Tên hình ảnh được lưu.</param>
        /// <returns>Đường dẫn hình ảnh.</returns>
        string Save(byte[] imageBytes, string imageName);

        /// <summary>
        /// Lưu hình ảnh.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="imageName">Tên ảnh.</param>
        /// <returns>Đường dẫn hình ảnh.</returns>
        string? SaveStudentImageByStream(Stream stream, string imageName);
    }
}
