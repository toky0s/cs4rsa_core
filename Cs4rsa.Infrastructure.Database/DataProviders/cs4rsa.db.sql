BEGIN TRANSACTION;
DROP TABLE IF EXISTS "Curriculums";
CREATE TABLE IF NOT EXISTS "Curriculums" (
	"CurriculumId"	INTEGER NOT NULL,
	"Name"	TEXT,
	CONSTRAINT "PK_Curriculums" PRIMARY KEY("CurriculumId" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "DbPreParSubjects";
CREATE TABLE IF NOT EXISTS "DbPreParSubjects" (
	"DbPreParSubjectId"	INTEGER NOT NULL,
	"SubjectCode"	TEXT,
	CONSTRAINT "PK_DbPreParSubjects" PRIMARY KEY("DbPreParSubjectId" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "Disciplines";
CREATE TABLE IF NOT EXISTS "Disciplines" (
	"DisciplineId"	INTEGER NOT NULL,
	"Name"	TEXT,
	CONSTRAINT "PK_Disciplines" PRIMARY KEY("DisciplineId" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "KeywordTeachers";
CREATE TABLE IF NOT EXISTS "KeywordTeachers" (
	"Id"	INTEGER NOT NULL,
	"CourseId"	INTEGER NOT NULL,
	"TeacherId"	INTEGER NOT NULL,
	CONSTRAINT "PK_KeywordTeachers" PRIMARY KEY("Id" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "Settings";
CREATE TABLE IF NOT EXISTS "Settings" (
	"Key"	TEXT NOT NULL,
	"Value"	TEXT,
	CONSTRAINT "PK_Settings" PRIMARY KEY("Key")
);
DROP TABLE IF EXISTS "Students";
CREATE TABLE IF NOT EXISTS "Students" (
	"StudentId"	TEXT NOT NULL,
	"SpecialString"	TEXT,
	"Name"	TEXT,
	"BirthDay"	TEXT NOT NULL,
	"Cmnd"	TEXT,
	"Email"	TEXT,
	"PhoneNumber"	TEXT,
	"Address"	TEXT,
	"AvatarImgPath"	TEXT,
	"CurriculumId"	INTEGER,
	CONSTRAINT "PK_Students" PRIMARY KEY("StudentId")
);
DROP TABLE IF EXISTS "Teachers";
CREATE TABLE IF NOT EXISTS "Teachers" (
	"TeacherId"	TEXT NOT NULL,
	"Name"	TEXT,
	"Sex"	TEXT,
	"Place"	TEXT,
	"Degree"	TEXT,
	"WorkUnit"	TEXT,
	"Position"	TEXT,
	"Subject"	TEXT,
	"Form"	TEXT,
	"TeachedSubjects"	TEXT,
	"Path"	TEXT,
	"Url"	TEXT,
	CONSTRAINT "PK_Teachers" PRIMARY KEY("TeacherId")
);
DROP TABLE IF EXISTS "UserSchedules";
CREATE TABLE IF NOT EXISTS "UserSchedules" (
	"UserScheduleId"	INTEGER NOT NULL,
	"Name"	TEXT,
	"SaveDate"	TEXT NOT NULL,
	"SemesterValue"	TEXT,
	"YearValue"	TEXT,
	"Semester"	TEXT,
	"Year"	TEXT,
	CONSTRAINT "PK_UserSchedules" PRIMARY KEY("UserScheduleId" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "DbProgramSubjects";
CREATE TABLE IF NOT EXISTS "DbProgramSubjects" (
	"DbProgramSubjectId"	INTEGER NOT NULL,
	"SubjectCode"	TEXT,
	"CourseId"	TEXT,
	"Name"	TEXT,
	"Credit"	INTEGER NOT NULL,
	"CurriculumId"	INTEGER NOT NULL,
	CONSTRAINT "PK_DbProgramSubjects" PRIMARY KEY("DbProgramSubjectId" AUTOINCREMENT),
	CONSTRAINT "FK_DbProgramSubjects_Curriculums_CurriculumId" FOREIGN KEY("CurriculumId") REFERENCES "Curriculums"("CurriculumId") ON DELETE CASCADE
);
DROP TABLE IF EXISTS "Keywords";
CREATE TABLE IF NOT EXISTS "Keywords" ( 
	"KeywordId"	INTEGER NOT NULL,
	"Keyword1"	TEXT,
	"CourseId"	TEXT,
	"SubjectName"	TEXT,
	"Color"	TEXT,
	"Cache"	TEXT,
	"DisciplineId"	INTEGER NOT NULL,
	CONSTRAINT "PK_Keywords" PRIMARY KEY("KeywordId" AUTOINCREMENT),
	CONSTRAINT "FK_Keywords_Disciplines_DisciplineId" FOREIGN KEY("DisciplineId") REFERENCES "Disciplines"("DisciplineId") ON DELETE CASCADE
);
DROP TABLE IF EXISTS "ScheduleDetails";
CREATE TABLE IF NOT EXISTS "ScheduleDetails" (
	"ScheduleDetailId"	INTEGER NOT NULL,
	"SubjectCode"	TEXT,
	"SubjectName"	TEXT,
	"ClassGroup"	TEXT,
	"RegisterCode"	TEXT,
	"SelectedSchoolClass"	TEXT,
	"UserScheduleId"	INTEGER NOT NULL,
	CONSTRAINT "PK_ScheduleDetails" PRIMARY KEY("ScheduleDetailId" AUTOINCREMENT),
	CONSTRAINT "FK_ScheduleDetails_UserSchedules_UserScheduleId" FOREIGN KEY("UserScheduleId") REFERENCES "UserSchedules"("UserScheduleId") ON DELETE CASCADE
);
DROP TABLE IF EXISTS "ParProDetails";
CREATE TABLE IF NOT EXISTS "ParProDetails" (
	"ProgramSubjectId"	INTEGER NOT NULL,
	"PreParSubjectId"	INTEGER NOT NULL,
	CONSTRAINT "PK_ParProDetails" PRIMARY KEY("ProgramSubjectId","PreParSubjectId"),
	CONSTRAINT "FK_ParProDetails_DbPreParSubjects_PreParSubjectId" FOREIGN KEY("PreParSubjectId") REFERENCES "DbPreParSubjects"("DbPreParSubjectId") ON DELETE CASCADE,
	CONSTRAINT "FK_ParProDetails_DbProgramSubjects_ProgramSubjectId" FOREIGN KEY("ProgramSubjectId") REFERENCES "DbProgramSubjects"("DbProgramSubjectId") ON DELETE CASCADE
);
DROP TABLE IF EXISTS "PreProDetails";
CREATE TABLE IF NOT EXISTS "PreProDetails" (
	"ProgramSubjectId"	INTEGER NOT NULL,
	"PreParSubjectId"	INTEGER NOT NULL,
	CONSTRAINT "PK_PreProDetails" PRIMARY KEY("ProgramSubjectId","PreParSubjectId"),
	CONSTRAINT "FK_PreProDetails_DbPreParSubjects_PreParSubjectId" FOREIGN KEY("PreParSubjectId") REFERENCES "DbPreParSubjects"("DbPreParSubjectId") ON DELETE CASCADE,
	CONSTRAINT "FK_PreProDetails_DbProgramSubjects_ProgramSubjectId" FOREIGN KEY("ProgramSubjectId") REFERENCES "DbProgramSubjects"("DbProgramSubjectId") ON DELETE CASCADE
);
DROP INDEX IF EXISTS "IX_DbProgramSubjects_CurriculumId";
CREATE INDEX IF NOT EXISTS "IX_DbProgramSubjects_CurriculumId" ON "DbProgramSubjects" (
	"CurriculumId"
);
DROP INDEX IF EXISTS "IX_Keywords_DisciplineId";
CREATE INDEX IF NOT EXISTS "IX_Keywords_DisciplineId" ON "Keywords" (
	"DisciplineId"
);
DROP INDEX IF EXISTS "IX_ParProDetails_PreParSubjectId";
CREATE INDEX IF NOT EXISTS "IX_ParProDetails_PreParSubjectId" ON "ParProDetails" (
	"PreParSubjectId"
);
DROP INDEX IF EXISTS "IX_PreProDetails_PreParSubjectId";
CREATE INDEX IF NOT EXISTS "IX_PreProDetails_PreParSubjectId" ON "PreProDetails" (
	"PreParSubjectId"
);
DROP INDEX IF EXISTS "IX_ScheduleDetails_UserScheduleId";
CREATE INDEX IF NOT EXISTS "IX_ScheduleDetails_UserScheduleId" ON "ScheduleDetails" (
	"UserScheduleId"
);
COMMIT;
