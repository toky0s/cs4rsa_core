/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using CwebizAPI.Converters;
using CwebizAPI.Crawlers.StudentCrawlerSvc.Crawlers.Interfaces;
using CwebizAPI.Crawlers.StudentCrawlerSvc.Models;
using CwebizAPI.Db.Enums;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.Services.Interfaces;
using CwebizAPI.Services.Tokens;
using CwebizAPI.Share.Database.Models;
using CwebizAPI.Share.DTOs.Requests;
using CwebizAPI.Share.DTOs.Responses;
using Microsoft.IdentityModel.Tokens;

namespace CwebizAPI.Businesses
{
    /// <summary>
    /// Lớp xử lý nghiệp vụ Đăng ký và Liên kết tài khoản.
    /// </summary>
    /// <remarks>
    /// Created Date: 16/06/2023
    /// Modified Date: 16/06/2023
    /// 15/07/2023: Chỉnh sửa tài liệu Register
    /// Author: Truong A Xin
    /// </remarks>
    public class BuRegister
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDtuStudentInfoCrawler _dtuStudentInfoCrawler;
        private readonly ISpecialStringCrawler _specialStringCrawler;
        private readonly IJwtTokenSvc _jwtTokenSvc;

        public BuRegister(
            IUnitOfWork unitOfWork,
            IDtuStudentInfoCrawler dtuStudentInfoCrawler,
            ISpecialStringCrawler specialStringCrawler, 
            IJwtTokenSvc jwtTokenSvc)
        {
            _unitOfWork = unitOfWork;
            _dtuStudentInfoCrawler = dtuStudentInfoCrawler;
            _specialStringCrawler = specialStringCrawler;
            _jwtTokenSvc = jwtTokenSvc;
        }
        
        /// <summary>
        /// Đăng ký một sinh viên vào hệ thống Cwebiz.
        /// </summary>
        /// <param name="registerInformation">Thông tin đăng ký.</param>
        /// <returns>DTO Response Register.</returns>
        /// <exception cref="BadHttpRequestException">
        /// ASP.NET Session ID không hợp lệ,
        /// Thông tin Email không trùng khớp
        /// </exception>
        public async Task<DtoRpRegister> Register(DtoRqRegister registerInformation)
        {
            string? specialString = await _specialStringCrawler.GetSpecialString(registerInformation.AspNetSessionId!);
            if (specialString is null) throw new BadHttpRequestException($"ASP.NET Session ID không hợp lệ");
            (CrawledCurriculum crawledCurriculum, DtuStudent dtuStudent) = await _dtuStudentInfoCrawler.Crawl(specialString);

            // Kiểm tra Email trùng khớp sau khi cào
            if (!dtuStudent.Emails.Contains(registerInformation.Email))
            {
                throw new BadHttpRequestException($"Thông tin không trùng khớp {registerInformation.Email}");
            }
            bool exists = await _unitOfWork.StudentRepository!.ExistsByStudentCode(dtuStudent.StudentId!);
                    
            // Kiểm tra sinh viên đã tồn tại hay chưa
            if (!exists)
            {
                Curriculum curriculum = crawledCurriculum.ToCurriculum();
                Student student = dtuStudent.ToStudent();
                student.Curriculum = curriculum;
                _unitOfWork.StudentRepository.Insert(curriculum);
                _unitOfWork.StudentRepository.Insert(student);
                await _unitOfWork.SaveChangeAsync();
            }
                    
            // Tạo token
            RegisterToken registerToken = new()
            {
                StudentId = dtuStudent.StudentId
            };
                
            JwtSecurityToken jwtSecurityToken = _jwtTokenSvc.GetRegisterJwtSecurityToken(registerToken);
                    
            return new DtoRpRegister()
            {
                JwtRegister = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };
        }

        /// <summary>
        /// Tạo và liên kết người dùng với một sinh viên.
        /// </summary>
        /// <param name="dtoRqLinkAccount">Thông tin người dùng</param>
        /// <returns>Thông tin người dùng đã được liên kết.</returns>
        /// <exception cref="BadHttpRequestException">
        /// - Token không hợp lệ: Khi token được yêu cầu không cung cấp đủ thông tin.
        /// - Tài khoản đã được liên kết: Tài khoản của sinh viên đã được liên kết trước đó.
        /// </exception>
        public async Task<DtoRpLinkStudent> CreateAndLinkAccount(DtoRqLinkAccount dtoRqLinkAccount)
        {
            // Validate token
            JwtSecurityToken jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(dtoRqLinkAccount.JwtRegister);

            if (!_jwtTokenSvc.ValidateToken(
                    dtoRqLinkAccount.JwtRegister!,
                    out SecurityToken? _,
                    out IPrincipal? _))
            {
                throw new BadHttpRequestException("Token không hợp lệ");
            }
            
            // Kiểm tra UserName tồn tại
            if (await _unitOfWork.UserRepository?.ExistsByUserName(dtoRqLinkAccount.Username!)!)
            {
                throw new BadHttpRequestException("Tên người dùng đã tồn tại");
            }
            
            string studentId = jwtSecurityToken.Claims.First(claim => claim.Type == "StudentId").Value;
            // Kiểm tra xem student đã được liên kết với bất kỳ tài khoản nào hay chưa.
            bool linked = await _unitOfWork.StudentRepository?.IsLinked(studentId)!;
            if (linked)
            {
                // Nếu đã được link, thông báo lỗi.
                throw new BadHttpRequestException("Tài khoản đã được liên kết");
            }

            // Nếu chưa được link, tạo tài khoản và link tài khoản với student này.
            Student student = await _unitOfWork.StudentRepository.GetByStudentId(studentId);
            CwebizUser cwebizUser = new()
            {
                Username = dtoRqLinkAccount.Username!,
                // Mã hoá Password.
                Password = BCrypt.Net.BCrypt.HashPassword(dtoRqLinkAccount.Password),
                UserType = UserRole.Student.DbValue,
                StudentId = student.StudentId
            };
                    
            _unitOfWork.UserRepository?.Insert(cwebizUser);
            await _unitOfWork.SaveChangeAsync();
            return new DtoRpLinkStudent()
            {
                StudentId = studentId,
                UserName = dtoRqLinkAccount.Username,
            };
        }
    }
}
