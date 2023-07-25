-- Cwebiz v0.0.1
-- Install packages:
-- Microsoft.EntityFrameworkCore.Design
-- Microsoft.EntityFrameworkCore.SqlServer

-- Run Migrate.cmd

USE master;
ALTER DATABASE [Cwebiz] set single_user with rollback immediate
DROP DATABASE [Cwebiz]

IF
     
NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'Cwebiz')
BEGIN
    CREATE DATABASE Cwebiz;
END;

GO
USE Cwebiz;
GO
    
-- CwebizUser Table
    -- Thông tin người dùng Mapping 1-1 với thông tin sinh viên
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CwebizUser' AND xtype='U')
BEGIN
CREATE TABLE CwebizUser
(
    [Id]          INT           PRIMARY KEY IDENTITY (1, 1),
    [Username]    VARCHAR(30)   UNIQUE NOT NULL,
    [Password]    VARCHAR(MAX)  NOT NULL,
    [UserType]    INT           NOT NULL,
    [StudentId]   VARCHAR(20)
);
END;

-- Curriculum Table
    -- Chứa thông tin ngành của sinh viên
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Curriculum' AND xtype='U')
BEGIN
CREATE TABLE Curriculum
(
    [Id]    INT             PRIMARY KEY,
    [Name]  NVARCHAR(100)   UNIQUE,
);
END ;

-- Student Table
    -- Mã sinh viên cố định 11 số, hoặc cao hơn một xí :v
    -- 12 số trên thẻ Căn cước công dân (CCCD) là số định danh cá nhân. 
    -- Email max length 320
    -- PhoneNumber VN max length 10, but 15 cho chắc
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Student' AND xtype='U')
BEGIN
CREATE TABLE Student
(
    [StudentId]       VARCHAR(20)     PRIMARY KEY,
    [SpecialString]   VARCHAR(50)     UNIQUE NOT NULL,
    [Name]            NVARCHAR(MAX)   NOT NULL,
    [BirthDay]        DATE,
    [Cmnd]            VARCHAR(12)     UNIQUE,
    [Email]           VARCHAR(320),
    [PhoneNumber]     VARCHAR(15),
    [Address]         NVARCHAR(200),
    [AvatarImgPath]   VARCHAR(MAX),
    [CurriculumId]    INT,
    CONSTRAINT CurriculumId_Curriculum_Id
        FOREIGN KEY (CurriculumId)
            REFERENCES Curriculum(Id),
);
END ;

-- Course
    -- Chứa thông tin năm học và học kỳ
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Course' AND xtype='U')
BEGIN
CREATE TABLE Course
(
    [Id]            INT             PRIMARY KEY IDENTITY (1, 1),
    [YearInfor]     NVARCHAR(100)   UNIQUE,
    [YearValue]     VARCHAR(10)     UNIQUE,
    [SemesterInfor] NVARCHAR(100)   UNIQUE,
    [SemesterValue] VARCHAR(10)     UNIQUE,
    [CreatedDate]   DATETIME        DEFAULT CURRENT_TIMESTAMP
);
END ;
    
-- Discipline Table
    -- Chứa thông tin mã môn
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Discipline' AND xtype='U')
BEGIN
CREATE TABLE Discipline
(
    [Id]        INT             NOT NULL PRIMARY KEY,
    [Name]      VARCHAR(10)   UNIQUE,
    [CourseId]  INT,
    CONSTRAINT CourseId_Course_Id
        FOREIGN KEY (CourseId)
            REFERENCES Course(Id)
);
END ;

-- Keyword Table
    -- Chứa thông tin môn
    -- Color: #RRGGBB (7)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Keyword' AND xtype='U')
BEGIN
CREATE TABLE Keyword
(
    [Id]            INT             NOT NULL PRIMARY KEY,
    [Keyword1]      VARCHAR(10)      NOT NULL,
    [CourseId]      VARCHAR(4)      NOT NULL UNIQUE,
    [SubjectName]   NVARCHAR(100)   NOT NULL,
    [Color]         VARCHAR(7)      NOT NULL UNIQUE,
    [DisciplineId]  INT,
    CONSTRAINT DisciplineId_Discipline_Id
        FOREIGN KEY (DisciplineId)
            REFERENCES Discipline(Id),
);
END;
