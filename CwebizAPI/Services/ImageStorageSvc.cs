/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Services.Interfaces;

namespace CwebizAPI.Services;

/// <summary>
/// Image Storage Service
/// </summary>
/// <remarks>
/// Created Date: 21/06/2023
/// Modified Date: 21/06/2023
/// Author: Truong A Xin
/// </remarks>
public class ImageStorageSvc : IImageStorageSvc
{
    private readonly ILogger<ImageStorageSvc> _logger;
    public ImageStorageSvc(ILogger<ImageStorageSvc> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Lưu hình ảnh.
    /// </summary>
    /// <param name="imageBytes">Mảng byte.</param>
    /// <param name="imageName">Tên ảnh.</param>
    /// <returns>Đường dẫn tới ảnh đã lưu.</returns>
    public string Save(byte[] imageBytes, string imageName)
    {
        _logger.LogInformation("Save image by byte array");
        return "https://images.unsplash.com/photo-1672406053556-ad4d836a70b0?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80";
    }

    /// <summary>
    /// Lưu hình ảnh.
    /// </summary>
    /// <param name="stream">Stream.</param>
    /// <param name="imageName">Tên ảnh.</param>
    /// <returns>Đường dẫn tới ảnh đã lưu.</returns>
    public string? SaveStudentImageByStream(Stream stream, string imageName)
    {
        _logger.LogInformation("Save image by stream");
        return "https://images.unsplash.com/photo-1672406053556-ad4d836a70b0?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80";
    }
}