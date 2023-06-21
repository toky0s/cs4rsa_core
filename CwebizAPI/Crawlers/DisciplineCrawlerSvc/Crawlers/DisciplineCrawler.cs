/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Crawlers.CourseSearchSvc.Crawlers;
using CwebizAPI.Db.Comparers;
using CwebizAPI.Utils;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.Share.Database.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace CwebizAPI.Crawlers.DisciplineCrawlerSvc.Crawlers
{
    /// <summary>
    /// Discipline Crawler
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public class DisciplineCrawler : BaseCrawler
    {
        private readonly CourseCrawler _courseCrawler;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HtmlWeb _htmlWeb;
        private readonly ILogger<DisciplineCrawler> _logger;

        public DisciplineCrawler(
            CourseCrawler courseCrawler,
            IUnitOfWork unitOfWork,
            HtmlWeb htmlWeb,
            ILogger<DisciplineCrawler> logger
        )
        {
            _courseCrawler = courseCrawler;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _htmlWeb = htmlWeb;
        }

        /// <summary>
        /// Cào data từ Course DTU và lưu vào database.
        /// 1. Kiểm tra thông tin học kỳ hiện tại
        ///     2. Nếu trong DB trống (thực hiện insert trong lần init server) hoặc trùng với DB.
        ///         3. Thực hiện check và thêm vào các môn mới.
        ///     4. Nếu là học kỳ mới
        ///         5. Xoá toàn bộ và thêm mới lại.
        /// </summary>
        public async Task GetDisciplineAndKeyword()
        {
            await using IDbContextTransaction trans = await _unitOfWork.DbContext.Database.BeginTransactionAsync();
            const int update = 1;
            const int create = 2;
            int mode;

            #region Kiểm tra thông tin học kỳ hiện tại

            Course? course = null;
            await _courseCrawler.InitInformation();
            if (_courseCrawler is { CurrentYearValue: not null, CurrentSemesterValue: not null }
                && !await _unitOfWork.DisciplineRepository?.Exists(_courseCrawler.CurrentYearValue,
                    _courseCrawler.CurrentSemesterValue)!
               )
            {
                course = new Course
                {
                    YearInfor = _courseCrawler.CurrentYearInfo,
                    YearValue = _courseCrawler.CurrentYearValue,
                    SemesterInfor = _courseCrawler.CurrentSemesterInfo,
                    SemesterValue = _courseCrawler.CurrentSemesterValue,
                    CreatedDate = DateTime.Today
                };
                _unitOfWork.DisciplineRepository.Insert(course);
                mode = create;
            }
            else
            {
                if (_courseCrawler is { CurrentYearValue: not null, CurrentSemesterValue: not null })
                {
                    course = await _unitOfWork.DisciplineRepository?.GetCourse(_courseCrawler.CurrentYearValue,
                        _courseCrawler.CurrentSemesterValue
                    )!;
                }

                mode = update;
            }

            #endregion

            #region Lấy thông Disciplines

            List<Discipline?> disciplines = new();
            List<Keyword> keywords = new();

            if (mode == create)
            {
                _logger.LogInformation("Start clear discipline and keyword");
                _unitOfWork.DisciplineRepository?.DeleteAllDisciplineAndKeyword();
            }

            _logger.LogInformation("Start get information");
            string url =
                $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={course?.SemesterValue}&t={GetTimeFromEpoch()}";

            HtmlDocument document = await _htmlWeb.LoadFromWebAsync(url);
            IEnumerable<HtmlNode> trTags = document.DocumentNode
                .Descendants("tr")
                .Where(node => node.HasClass("lop"));

            string? currentDisciplineName = null;
            int disciplineId = 0;
            int keywordId = 1;
            foreach (HtmlNode trTag in trTags)
            {
                HtmlNode[] tdTags = trTag.Descendants("td").ToArray();
                HtmlNode disciplineAnchorTag = tdTags[0].Element("a");
                string courseId = GetCourseIdFromHref(disciplineAnchorTag.Attributes["href"].Value);
                string disciplineAndKeyword = disciplineAnchorTag.InnerText.Trim();
                string[] disciplineAndKeywordSplit = StringHelper.SplitAndRemoveAllSpace(disciplineAndKeyword);

                string disciplineName = disciplineAndKeywordSplit[0];

                if (currentDisciplineName == null || !currentDisciplineName.Equals(disciplineName))
                {
                    currentDisciplineName = disciplineName;

                    /* 
                     * Tự động tăng ID trong lần đầu tiên khởi tạo,
                     * hoặc có thay đổi trong thông tin học kỳ.
                     */
                    disciplineId++;
                    Discipline discipline = new()
                    {
                        Id = disciplineId,
                        Name = disciplineName,
                        Course = course,
                        Keywords = new List<Keyword>()
                    };
                    disciplines.Add(discipline);
                }

                if (disciplineName != currentDisciplineName) continue;
                string keyword1 = disciplineAndKeywordSplit[1];
                string color = ColorGenerator.GenerateColor();
                HtmlNode subjectNameAnchorTag = tdTags[1].Element("a");
                string subjectName = subjectNameAnchorTag.InnerText.Trim();
                // UnitOfWork Keyword Insert
                Keyword keyword = new()
                {
                    Id = keywordId,
                    Keyword1 = keyword1,
                    CourseId = int.Parse(courseId),
                    SubjectName = subjectName,
                    Color = color,
                    DisciplineId = disciplineId,
                    Discipline = disciplines.First(d => d != null && d.Id == disciplineId)
                };
                //_unitOfWork.DisciplineRepository.Insert(kw);
                disciplines.First(d => d != null && d.Id == disciplineId)?.Keywords.Add(keyword);
                keywords.Add(keyword);
                keywordId++;
            }

            _logger.LogInformation("Finished get information");

            #endregion

            if (mode == create)
            {
                _logger.LogInformation("Go into Create mode");
                _logger.LogInformation(
                    message: "New Disciplines\n {ExceptDisciplines}",
                    string.Join('\n', disciplines.Select(d => d?.Name)));
                _unitOfWork.DisciplineRepository?.InsertAll(disciplines);
            }
            else
            {
                _logger.LogInformation("Go into Update mode");

                #region Kiểm tra và thêm mới Discipline nếu có.

                List<Discipline> dbDisciplines = await _unitOfWork.DisciplineRepository?.GetAllDiscipline()!;
                List<Discipline?> exceptDisciplines = disciplines
                    .Except(dbDisciplines, new DisciplineEqualityComparer()!)
                    .ToList();


                if (exceptDisciplines.Count > 0)
                {
                    _logger.LogWarning("Except Disciplines: {Amount}", exceptDisciplines.Count);
                    _logger.LogInformation(
                        message: "Except Disciplines\n{ExceptDisciplines}",
                        string.Join('\n', exceptDisciplines.Select(d => d?.Name)));
                }
                else
                {
                    _logger.LogInformation("Have no changes in Disciplines");
                }

                foreach (Discipline? discipline in exceptDisciplines.Where(discipline => discipline != null))
                {
                    if (discipline != null)
                    {
                        discipline.Id = default;
                    }
                }

                _unitOfWork.DisciplineRepository.InsertAll(exceptDisciplines);
                await _unitOfWork.SaveChangeAsync();

                #endregion

                #region Kiểm tra và thêm mới Keyword của từng Discipline nếu có

                List<Keyword> contextKeywords = await _unitOfWork.DisciplineRepository.GetAllKeyword();

                List<Keyword> exceptKeywords = keywords
                    .Except(contextKeywords, new KeywordEqualityComparer())
                    .ToList();

                if (exceptKeywords.Count > 0)
                {
                    _logger.LogInformation("Except Keywords amount {Amount}", exceptKeywords.Count);
                    _logger.LogInformation(
                        message: "Except Keywords\n{ExceptKeywords}",
                        string.Join('\n', exceptKeywords.Select(kw => kw.Discipline?.Name + " " + kw.SubjectName)));
                    /*
                     * Trong trường hợp các Discipline được cập nhật thêm các Keyword
                     * thì phải sửa lại các Keyword.DisciplineId trong từng Keyword 
                     * tương ứng với các Discipline sở hữu chúng trong DB - đây là
                     * kết quả của việc cào dự liệu và set các DisciplineId một cách tuần tự.
                     * Nhằm tránh FK_Discipline của Keyword khi Insert mới vào DB không tìm thấy.
                     */
                    foreach (Keyword keyword in exceptKeywords.Where(keyword => keyword.Discipline != null))
                    {
                        Discipline actualDiscipline =
                            await _unitOfWork.DisciplineRepository.GetDisciplineByName(keyword.Discipline?.Name);
                        keyword.Discipline = actualDiscipline;
                        keyword.DisciplineId = actualDiscipline.Id;
                    }

                    _unitOfWork.DisciplineRepository.InsertAll(exceptKeywords);
                }
                else
                {
                    _logger.LogInformation("Have no changes in Keywords");
                }

                #endregion
            }

            _logger.LogInformation("Save information");
            await _unitOfWork.SaveChangeAsync();
            await trans.CommitAsync();
        }

        private static string GetCourseIdFromHref(string hrefValue)
        {
            string[] hrefValueSlides = hrefValue.Split('&');
            return hrefValueSlides[1].Split('=')[1];
        }

        /// <summary>
        /// Lấy ra số lượng môn học hiện có trong học kỳ hiện tại.
        /// </summary>
        /// <returns>Số lượng môn học hiện có.</returns>
        public int GetNumberOfSubjects()
        {
            string url =
                $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={_courseCrawler.CurrentSemesterValue}&t={GetTimeFromEpoch()}";
            HtmlDocument document = _htmlWeb.Load(url);

            HtmlNode node = document.DocumentNode;
            HtmlNode result = node.SelectSingleNode("/div/table/thead/tr/th");
            string innerText = result.InnerText;
            Regex numberMatchingRegex = new("([0-9][0-9][0-9])");
            Match match = numberMatchingRegex.Match(innerText);
            return int.Parse(match.Value);
        }
    }
}