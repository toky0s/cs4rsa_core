﻿using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.TeacherCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.TeacherCrawlerSvc.Models;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Services.TeacherCrawlerSvc.Crawlers
{
    public class TeacherCrawler : ITeacherCrawler
    {
        #region Properties
        private readonly string _strSavingTeacherImageFolderPath;
        #endregion

        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFolderManager _folderManager;
        private readonly HtmlWeb _htmlWeb;
        private readonly ImageDownloader _imageDownloader;
        #endregion

        public TeacherCrawler(
            IUnitOfWork unitOfWork,
            IFolderManager folderManager,
            HtmlWeb htmlWeb,
            ImageDownloader imageDownloader)
        {
            _unitOfWork = unitOfWork;
            _folderManager = folderManager;
            _htmlWeb = htmlWeb;
            _imageDownloader = imageDownloader;

            string path = Path.Combine(AppContext.BaseDirectory, IFolderManager.FdTeacherImgs);
            _strSavingTeacherImageFolderPath = _folderManager.CreateFolderIfNotExists(path);
        }

        private static string GetIntructorId(string url)
        {
            string[] slideChars = { "&" };
            string[] separatingStrings = { "=" };
            string intructorIdParam = url.Split(slideChars, StringSplitOptions.RemoveEmptyEntries)[2];
            return intructorIdParam.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        public async Task<TeacherModel> Crawl(string url, int courseId, bool isUpdate)
        {
            if (url == null)
            {
                return null;
            }

            int teacherId = int.Parse(GetIntructorId(url));
            TeacherModel teacherModel;
            if (_unitOfWork.Teachers.ExistByID(teacherId) && !isUpdate)
            {
                Teacher teacher = _unitOfWork.Teachers.GetTeacherById(teacherId);
                teacher.Url = url;
                _unitOfWork.Teachers.Update(teacher);
                teacherModel = new TeacherModel(teacher);
            }
            else
            {
                HtmlDocument _htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
                if (_htmlDocument == null)
                {
                    return null;
                }
                HtmlNodeCollection infoNodes = _htmlDocument
                    .DocumentNode
                    .SelectNodes("//span[contains(@class, 'info_gv')]");
                string id = StringHelper.SuperCleanString(infoNodes[0].InnerText);
                string name = StringHelper.SuperCleanString(infoNodes[1].InnerText);
                string sex = StringHelper.SuperCleanString(infoNodes[2].InnerText);
                string place = StringHelper.SuperCleanString(infoNodes[3].InnerText);
                string degree = StringHelper.SuperCleanString(infoNodes[4].InnerText);
                string workUnit = StringHelper.SuperCleanString(infoNodes[5].InnerText);
                string position = StringHelper.SuperCleanString(infoNodes[6].InnerText);
                string subject = StringHelper.SuperCleanString(infoNodes[7].InnerText);
                string form = StringHelper.SuperCleanString(infoNodes[8].InnerText);
                string xpathLiNode = "//ul[contains(@class, 'thugio')]/li";

                /// Tham khảo CS0026 https://github.com/toky0s/cs4rsa_core/issues/37
                IEnumerable<string> teachedSubjects = _htmlDocument.DocumentNode
                    .SelectNodes(xpathLiNode)
                    .Select(item => StringHelper.SuperCleanString(item.InnerText));

                string strPath = await OnDownloadImage(id);

                if (isUpdate && _unitOfWork.Teachers.ExistByID(teacherId))
                {
                    Teacher teacher = _unitOfWork.Teachers.GetTeacherById(teacherId);
                    teacher.Name = name;
                    teacher.Sex = sex;
                    teacher.Place = place;
                    teacher.Degree = degree;
                    teacher.WorkUnit = workUnit;
                    teacher.Position = position;
                    teacher.Subject = subject;
                    teacher.Form = form;
                    teacher.Path = strPath;
                    teacher.TeachedSubjects = string.Join("$", teachedSubjects);
                    _unitOfWork.Teachers.Update(teacher);
                }
                else
                {
                    Teacher teacher = new()
                    {
                        TeacherId = int.Parse(id),
                        Name = name,
                        Sex = sex,
                        Place = place,
                        Degree = degree,
                        WorkUnit = workUnit,
                        Position = position,
                        Subject = subject,
                        Form = form,
                        Path = strPath,
                        TeachedSubjects = string.Join("$", teachedSubjects),
                        Url = url
                    };
                    _unitOfWork.Teachers.Add(teacher);
                    CreateKeywordSubjectIfNotExist(teacherId, courseId);
                }
                teacherModel = new TeacherModel(
                    int.Parse(id),
                    name,
                    sex,
                    place,
                    degree,
                    workUnit,
                    position,
                    subject,
                    form,
                    teachedSubjects,
                    strPath,
                    url
                );
            }

            return teacherModel;
        }

        private void CreateKeywordSubjectIfNotExist(int teacherId, int courseId)
        {
            if (!_unitOfWork.KeywordTeachers.Exists(teacherId, courseId))
            {
                KeywordTeacher kt = new()
                {
                    CourseId = courseId,
                    TeacherId = teacherId
                };
                _unitOfWork.KeywordTeachers.Add(kt);
            }
        }

        private static string GetTeacherImagePath(string teacherId)
        {
            return $"http://hfs1.duytan.edu.vn/Upload/dichvu/gv_{teacherId}_01.jpg";
        }

        private static string GetTeacherImageName(string teacherId)
        {
            return $"gv_{teacherId}_01.jpg";
        }

        private async Task<string> OnDownloadImage(string teacherId)
        {
            string imageUrl = GetTeacherImagePath(teacherId);
            string imageName = GetTeacherImageName(teacherId);
            string strImageFullPath = Path.Combine(_strSavingTeacherImageFolderPath, imageName);
            File.Delete(strImageFullPath);
            bool result = await _imageDownloader.DownloadImage(imageUrl, strImageFullPath);
            return result ? strImageFullPath : string.Empty;
        }
    }
}
