using System.Collections.ObjectModel;
using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes;
using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes.Enums;
using CwebizAPI.DTOs.Responses;
using CwebizAPI.Services.Interfaces;
using CwebizAPI.Utils;

namespace CwebizAPI.Services;

/// <summary>
/// Subject Converter.
/// </summary>
/// <remarks>
/// Created Date: 21/07/2023
/// Author: Truong A Xin
/// </remarks>
public class SvcSubjectCvt : ISvcSubjectCvt
{
    private readonly ColorGenerator _colorGenerator;
    public SvcSubjectCvt(ColorGenerator colorGenerator)
    {
        _colorGenerator = colorGenerator;
    }

    public async Task<DtoRpSubject> ToDtoRpSubject(Subject subject)
    {
        string color = await _colorGenerator.GetColor(subject.CourseId);
        bool isSpecialSubject = subject.IsSpecialSubject();
        
        IEnumerable<Task<DtoRpClassGroup>> classGroupTasks = subject
            .ClassGroups
            .Select(item => ToDtoRpClassGroup(item, isSpecialSubject, color));

        DtoRpClassGroup[] classGroups = await Task.WhenAll(classGroupTasks);

        DtoRpSubject dtoRpSubject = new()
        {
            TeacherNames = subject.TempTeachers,
            SubjectName = subject.Name,
            SubjectCode = subject.SubjectCode,
            CourseId = subject.CourseId,
            StudyUnit = subject.StudyUnit,
            PrerequisiteSubjectAsString = GetMustStudySubjects(subject),
            ParallelSubjectAsString = GetParallelSubjects(subject),
            Color = color,
            IsSpecialSubject = isSpecialSubject,
            ClassGroups = classGroups,
            Description = subject.Description,
            Semester = subject.Semester,
            StudyType = subject.StudyType,
            StudyUnitType = subject.StudyUnitType
        };

        return dtoRpSubject;
    }

    public async Task<DtoRpClassGroup> ToDtoRpClassGroup(ClassGroup classGroup, bool isBelongSpecialSubject, string color)
    {
        DtoRpClassGroup dtoRpClassGroup = new()
        {
            Name = classGroup.Name,
            SubjectCode = classGroup.SubjectCode,
            // Kiểm tra xem nhóm lớp này có Lịch hay không. Đôi khi sau giai đoạn đăng ký tín
            // chỉ một số class group sẽ không còn lịch hiển thị hoặc chỉ hiển thị lịch bổ sung
            // mà không có lịch chính.
            HaveSchedule = classGroup.GetSchedule().ScheduleTime.Count > 0,
            Color = color,
            IsBelongSpecialSubject = isBelongSpecialSubject,
            Phase = classGroup.GetPhase(),
        };

        IEnumerable<Task<DtoRpSchoolClass>> schoolClassTasks = classGroup
            .SchoolClasses
            .Select(sc => ToDtoRpSchoolClass(sc, color));
        dtoRpClassGroup.NormalSchoolClassModels = await Task.WhenAll(schoolClassTasks);

        if (classGroup.SchoolClasses.Count <= 0) return dtoRpClassGroup;

        dtoRpClassGroup.Schedule = classGroup.GetSchedule();
        dtoRpClassGroup.CurrentSchoolClassModels = new List<DtoRpSchoolClass>();
        dtoRpClassGroup.Places = new ObservableCollection<Place>(classGroup.GetPlaces());
        dtoRpClassGroup.EmptySeat = classGroup.GetEmptySeat();
        dtoRpClassGroup.TempTeachers = classGroup.GetTempTeachers();
        dtoRpClassGroup.ImplementType = classGroup.GetImplementType();
        dtoRpClassGroup.RegistrationType = classGroup.GetRegistrationType();
        dtoRpClassGroup.RegisterCodes = classGroup.RegisterCodes;
        dtoRpClassGroup.ClassSuffix = GetClassSuffix(classGroup);
        dtoRpClassGroup.CompulsoryClass = await GetCompulsoryClass(classGroup, color);
        dtoRpClassGroup.IsSpecialClassGroup = EvaluateIsSpecialClassGroup(classGroup);

        switch (classGroup.SchoolClasses.Count)
        {
            // ClassGroup thường với duy nhất một SchoolClass
            case 1 when !ReferenceEquals(dtoRpClassGroup.CompulsoryClass, null):
                dtoRpClassGroup.CodeSchoolClass = dtoRpClassGroup.CompulsoryClass;
                dtoRpClassGroup.CurrentSchoolClassModels.Add(dtoRpClassGroup.CompulsoryClass);
                break;
            // ClassGroup thường với một SchoolClass bắt buộc và một SchoolClass chứa mã (hoặc không)
            case >= 2 when dtoRpClassGroup is { IsSpecialClassGroup: false, CompulsoryClass: not null }:
            {
                SchoolClass? schoolClass = classGroup
                    .SchoolClasses
                    .FirstOrDefault(sc => !sc.SchoolClassName.Equals(dtoRpClassGroup.CompulsoryClass.SchoolClassName));
                
                if (schoolClass is null) return dtoRpClassGroup;
                dtoRpClassGroup.CodeSchoolClass = await ToDtoRpSchoolClass(schoolClass, color);
                dtoRpClassGroup.CurrentSchoolClassModels.Add(dtoRpClassGroup.CompulsoryClass);
                dtoRpClassGroup.CurrentSchoolClassModels.Add(dtoRpClassGroup.CodeSchoolClass);
                break;
            }
        }

        return dtoRpClassGroup;
    }

    public Task<DtoRpSchoolClass> ToDtoRpSchoolClass(SchoolClass schoolClass, string color)
    {
        DtoRpSchoolClass dtoRpSchoolClass = new()
        {
            SchoolClassName = schoolClass.SchoolClassName,
            RegisterCode = schoolClass.RegisterCode,
            Type = schoolClass.Type,
            EmptySeat = schoolClass.EmptySeat,
            RegistrationTermEnd = schoolClass.RegistrationTermEnd,
            RegistrationTermStart = schoolClass.RegistrationTermStart,
            StudyWeek = schoolClass.StudyWeek,
            Schedule = schoolClass.Schedule,
            SubjectCode = schoolClass.SubjectCode,
            Rooms = schoolClass.Rooms,
            Places = schoolClass.Places,
            TempTeachers = schoolClass.TempTeachers,
            RegistrationStatus = schoolClass.RegistrationStatus,
            ImplementationStatus = schoolClass.ImplementationStatus,
            DayPlaceMetaData = schoolClass.DayPlaceMetaData,
            SubjectName = schoolClass.SubjectName,
            Color = color,
            Phase = schoolClass.GetPhase()
        };

        return Task.FromResult(dtoRpSchoolClass);
    }
    
    private static string GetMustStudySubjects(Subject subject)
    {
        return subject.MustStudySubject.Any() 
            ? string.Join(", ", subject.MustStudySubject) 
            : "Không có môn tiên quyết";
    }

    private static string GetParallelSubjects(Subject subject)
    {
        return subject.ParallelSubject.Any() 
            ? string.Join(", ", subject.ParallelSubject) 
            : "Không có môn song hành";
    }
    
    private static string GetClassSuffix(ClassGroup classGroup)
    {
        return classGroup.Name[(classGroup.SubjectCode.Length + 1)..];
    }
    
    
    /**
         * Mô tả:
         *      Tìm kiếm base class, school class bắt buộc.
         *
         *
         * Trả về:
         *      Trả về Compulsory Class của một class group.
         *      1. Trường hợp có duy nhất một SchoolClass, nó sẽ là Compulsory Class.
         *      2. Trường hợp lớp có hai SchoolClass trở lên, 
         *         SchoolClass thằng không có mã đăng ký sẽ là Compulsory Class.
         */
    
    private async Task<DtoRpSchoolClass?> GetCompulsoryClass(ClassGroup classGroup, string color)
    {
        // Early exit and safe check
        if (classGroup.SchoolClasses.Count == 0)
        {
            return null;
        }

        if (classGroup.SchoolClasses[0].SchoolClassName.Equals(classGroup.Name)
            || classGroup.SchoolClasses.Count == 1)
        {
            return await ToDtoRpSchoolClass(classGroup.SchoolClasses[0], color);
        }

        IEnumerable<Task<DtoRpSchoolClass>> schoolClassTasks = classGroup.SchoolClasses
            .Where(schoolClass => string.IsNullOrWhiteSpace(schoolClass.RegisterCode))
            .Select(schoolClass => ToDtoRpSchoolClass(schoolClass, color));

        DtoRpSchoolClass[] schoolClasses = await Task.WhenAll(schoolClassTasks);

        // Trường hợp có nhiều hơn 1 school class,
        // chọn school class nào không có mã đăng ký.
        return schoolClasses.FirstOrDefault();
    }
    
    private static bool EvaluateIsSpecialClassGroup(ClassGroup classGroup)
    {
        return classGroup.SchoolClasses.Where(sc => !sc.SchoolClassName.Equals(classGroup.Name))
            .Distinct()
            .Count() >= 2;
    }
}