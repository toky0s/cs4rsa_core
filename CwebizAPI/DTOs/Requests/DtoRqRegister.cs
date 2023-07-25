/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.ComponentModel.DataAnnotations;
using CwebizAPI.Validations;

namespace CwebizAPI.DTOs.Requests
{
    /// <summary>
    /// DTO Request Register
    /// Thông tin đăng ký sinh viên
    /// </summary>
    public sealed class DtoRqRegister
    {
        /// <summary>
        /// Email DTU
        /// </summary>
        [Required(ErrorMessage = "Trường email không được để trống")]
        [IsDtuEmail(ErrorMessage = "Phải là Email của Trường Đại học Duy Tân")]
        public string? Email { get; set; }

        /// <summary>
        /// ASP.NET_SessionID
        /// </summary>
        [Required(ErrorMessage = "Trường này không được để trống")]
        public string? AspNetSessionId { get; set; }
    }
}
