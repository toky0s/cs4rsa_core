SELECT 
	dps.SubjectCode, 
	dps.[Name], 
	dps.Credit, 
	kw.CourseId, 
	kw.Color,
	kw.Cache
FROM DbProgramSubjects AS dps
LEFT JOIN Keywords AS kw
	ON kw.CourseId = dps.CourseId
	AND dps.CurriculumId = 866
