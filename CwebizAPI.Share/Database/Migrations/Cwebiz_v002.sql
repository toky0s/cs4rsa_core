-- Seed data
-- Account
-- "Username": "vominhhieu3",
-- "Password": "vominhhieu3"
--         
-- "Username": "nguyentthanhtam33",
-- "Password": "nguyentthanhtam33"
--    
-- "Username": "nguyennminhtrang1",
-- "Password": "nguyennminhtrang1"

USE [Cwebiz]
GO
INSERT [dbo].[Curriculum] ([Id], [Name]) VALUES (629, N'K-24 - Dược Sĩ (Đại Học)')
GO
INSERT [dbo].[Curriculum] ([Id], [Name]) VALUES (697, N'K-25 - Công Nghệ Phần Mềm (Đại Học)')
GO
INSERT [dbo].[Curriculum] ([Id], [Name]) VALUES (866, N'K-26 - Thiết kế Đồ họa (Đại Học - VJ)')
GO
INSERT [dbo].[Student] ([StudentId], [SpecialString], [Name], [BirthDay], [Cmnd], [Email], [PhoneNumber], [Address], [AvatarImgPath], [CurriculumId]) VALUES (N'24205205493', N'7lOQDrVSAcaBZ3e6nJXYRQ==', N'Nguyễn Trần Thanh Tâm', CAST(N'2000-08-26' AS Date), N'215518087', N'nguyentthanhtam33@dtu.edu.vn', N'7515243', N'tấn thạnh I, P. Hoài Hảo, Q. 129, 8, Việt Nam', N'https://images.unsplash.com/photo-1672406053556-ad4d836a70b0?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80', 629)
GO
INSERT [dbo].[Student] ([StudentId], [SpecialString], [Name], [BirthDay], [Cmnd], [Email], [PhoneNumber], [Address], [AvatarImgPath], [CurriculumId]) VALUES (N'25211210412', N'zdUfnT1JfRo0srXxjYTIuQ==', N'Võ Minh Hiếu', CAST(N'2001-05-04' AS Date), N'233300249', N'vominhhieu3@dtu.edu.vn', N'0986538788', N'thôn 7, P. 19662, Q. 402, 34, Việt Nam', N'https://images.unsplash.com/photo-1672406053556-ad4d836a70b0?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80', 697)
GO
INSERT [dbo].[Student] ([StudentId], [SpecialString], [Name], [BirthDay], [Cmnd], [Email], [PhoneNumber], [Address], [AvatarImgPath], [CurriculumId]) VALUES (N'26204335764', N'miH4p+yECKvNwzWcywSlQQ==', N'Nguyễn Ngọc Minh Trang', CAST(N'2002-05-17' AS Date), N'206288817', N'nguyennminhtrang1@dtu.edu.vn', N'0905351642', N'tổ 5, P. 18553, Q. 192, 15, Việt Nam', N'https://images.unsplash.com/photo-1672406053556-ad4d836a70b0?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80', 866)
GO
SET IDENTITY_INSERT [dbo].[Course] ON
GO
INSERT [dbo].[Course] ([Id], [YearInfor], [YearValue], [SemesterInfor], [SemesterValue], [CreatedDate]) VALUES (1, N'Năm Học 2022-2023', N'78', N'Học Kỳ Hè', N'81', CAST(N'2023-06-24T00:00:00.000' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Course] OFF
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (1, N'ACC', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (2, N'AES', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (3, N'AET', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (4, N'AHI', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (5, N'ANA', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (6, N'ARC', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (7, N'ART', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (8, N'AUD', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (9, N'BCH', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (10, N'BIO', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (11, N'BNK', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (12, N'CHE', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (13, N'CHI', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (14, N'CIE', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (15, N'CMU-CS', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (16, N'CMU-ENG', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (17, N'CMU-IS', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (18, N'CMU-SE', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (19, N'COM', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (20, N'CR', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (21, N'CS', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (22, N'CSU-CIE', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (23, N'CSU-ENG', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (24, N'CUL', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (25, N'DEN', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (26, N'DMS', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (27, N'DTE', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (28, N'ECO', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (29, N'EE', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (30, N'ENG', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (31, N'EVR', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (32, N'EVT', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (33, N'FIN', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (34, N'FST', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (35, N'HIS', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (36, N'HOS', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (37, N'HRM', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (38, N'IB', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (39, N'ID', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (40, N'INR', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (41, N'IS', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (42, N'IS-ENG', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (43, N'JAP', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (44, N'KOR', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (45, N'LAW', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (46, N'LIN', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (47, N'LIT', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (48, N'MEC', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (49, N'MED', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (50, N'MGO', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (51, N'MGT', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (52, N'MKT', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (53, N'MT', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (54, N'MTH', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (55, N'NTR', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (56, N'NUR', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (57, N'OB', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (58, N'PGY', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (59, N'PHC', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (60, N'PHI', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (61, N'PHM', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (62, N'PHY', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (63, N'PMY', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (64, N'PNU-EE', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (65, N'POS', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (66, N'PSU-ACC', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (67, N'PSU-COM', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (68, N'PSU-ECO', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (69, N'PSU-ENG', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (70, N'PSU-FIN', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (71, N'PSU-HOS', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (72, N'PSU-MGT', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (73, N'PSU-OB', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (74, N'PSU-TOU', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (75, N'PSY', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (76, N'PTH', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (77, N'REM', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (78, N'SCM', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (79, N'SE', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (80, N'SOC', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (81, N'SPM', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (82, N'STA', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (83, N'THR', 1)
GO
INSERT [dbo].[Discipline] ([Id], [Name], [CourseId]) VALUES (84, N'TOU', 1)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (1, N'201', 301, N'Nguyên Lý Kế Toán 1', N'#B3FE83', 1)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (2, N'202', 320, N'Nguyên Lý Kế Toán 2', N'#7FB38A', 1)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (3, N'301', 321, N'Kế Toán Quản Trị 1', N'#CDC793', 1)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (4, N'303', 333, N'Kế Toán Quản Trị 2', N'#BAF0EA', 1)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (5, N'382', 1449, N'Kế Toán Thuế', N'#C5A88E', 1)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (6, N'411', 374, N'Phân Tích Hoạt Động Kinh Doanh', N'#DEAE8B', 1)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (7, N'421', 376, N'Phân Tích Báo Cáo Tài Chính', N'#F3E0A3', 1)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (8, N'423', 379, N'Kế Toán Tài Chính Thương Mại Dịch Vụ', N'#A0B9AE', 1)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (9, N'426', 369, N'Kế Toán Ngân Hàng', N'#B18A9A', 1)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (10, N'452', 377, N'Kế Toán Tài Chính Nâng Cao', N'#D2DEFC', 1)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (11, N'251', 176, N'Đại Cương Mỹ Học', N'#8AF7CC', 2)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (12, N'297', 2117, N'Đồ án CDIO', N'#88C3E4', 3)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (13, N'347', 2118, N'Đồ án CDIO', N'#949296', 3)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (14, N'391', 174, N'Lịch Sử Kiến Trúc Phương Đông & Việt Nam', N'#C4D191', 4)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (15, N'392', 173, N'Lịch Sử Kiến Trúc Phương Tây', N'#86EAD8', 4)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (16, N'301', 1291, N'Mô Phôi Cho Y Khoa', N'#9AECC0', 5)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (17, N'306', 1863, N'Mô Phôi Răng Miệng', N'#B0BB83', 5)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (18, N'216', 1118, N'Hình Họa Mỹ Thuật 3', N'#9DA4B4', 6)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (19, N'341', 561, N'Brand Design Studio', N'#E7ECA5', 7)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (20, N'353', 383, N'Kiểm Toán Nội Bộ', N'#88E4FA', 8)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (21, N'402', 384, N'Kiểm Toán Tài Chính 1', N'#ADF6FD', 8)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (22, N'251', 1300, N'Hóa Sinh Y Học', N'#B0CEE9', 9)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (23, N'101', 36, N'Sinh Học Đại Cương', N'#CF8EA5', 10)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (24, N'213', 535, N'Sinh Lý Học', N'#E086AE', 10)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (25, N'354', 896, N'Thanh Toán Quốc Tế', N'#B5B2A1', 11)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (26, N'401', 370, N'Ngân Hàng Trung Ương', N'#9EF6CF', 11)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (27, N'101', 35, N'Hóa Học Đại Cương', N'#F99285', 12)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (28, N'203', 451, N'Hóa Hữu Cơ', N'#8ED681', 12)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (29, N'273', 924, N'Hóa Hữu Cơ cho Dược', N'#E1E2F1', 12)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (30, N'102', 9, N'Trung Ngữ Sơ Cấp 2', N'#F493E7', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (31, N'116', 1740, N'Nói (tiếng Trung) 1', N'#C4F292', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (32, N'151', 1743, N'Tiếng Trung Quốc Tổng Hợp 1', N'#A8CC92', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (33, N'166', 1809, N'Nói (tiếng Trung) 2', N'#A18EF6', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (34, N'168', 1806, N'Nghe (tiếng Trung) 2', N'#EFF3E5', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (35, N'169', 1813, N'Đọc (tiếng Trung) 2', N'#88E2A1', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (36, N'216', 1810, N'Nói (tiếng Trung) 3', N'#A8C7C9', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (37, N'217', 1817, N'Viết (tiếng Trung) 3', N'#F4EFCD', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (38, N'251', 1803, N'Tiếng Trung Quốc Tổng Hợp 3', N'#B5B2E5', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (39, N'262', 1791, N'Từ Vựng Tiếng Trung', N'#83CED4', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (40, N'302', 21, N'Trung Ngữ Cao Cấp 2', N'#E0FA8D', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (41, N'350', 1819, N'Dẫn Luận Ngôn Ngữ Học (Tiếng Trung)', N'#EB9FC1', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (42, N'355', 1827, N'Ngôn Ngữ Học Đối Chiếu (Tiếng Trung)', N'#E291C9', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (43, N'371', 1829, N'Phiên Dịch (Tiếng Trung)', N'#C2ADFF', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (44, N'376', 1828, N'Biên Dịch (Tiếng Trung)', N'#B8A6B0', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (45, N'414', 1794, N'Ngữ Pháp cho Khảo Sát HSK', N'#DAA090', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (46, N'424', 1834, N'Phiên Dịch Tiếng Trung trong Du Lịch', N'#C8CAAD', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (47, N'433', 1832, N'Dịch Thuật trong Khoa Học – Kỹ Thuật', N'#DCA3AD', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (48, N'438', 1831, N'Dịch Thuật trong Tin Tức – Thời Sự', N'#B195EF', 13)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (49, N'111', 212, N'Vẽ Kỹ Thuật & CAD', N'#AEA3EE', 14)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (50, N'439', 273, N'Đồ Án Thi Công Cầu', N'#E0F7C3', 14)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (51, N'441', 240, N'Quản Lý Dự Án Xây Dựng', N'#D282E6', 14)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (52, N'458', 904, N'Kỹ Thuật & Tổ Chức Thi Công Cầu', N'#E4C484', 14)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (53, N'467', 278, N'Đường Phố & Giao Thông Đô Thị', N'#C4AD9C', 14)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (54, N'477', 236, N'Kết Cấu Bê Tông Cốt Thép Ứng Lực Trước', N'#8EF289', 14)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (55, N'480', 1938, N'Thí Nghiệm và Kiểm Định Công Trình', N'#B6C4B1', 14)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (56, N'483', 258, N'Kỹ Thuật Thi Công Đặc Biệt', N'#E9BBC7', 14)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (57, N'489', 276, N'Khai Thác, Kiểm Định & Gia Cố Cầu', N'#EBAADE', 14)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (58, N'252', 609, N'Introduction to Network & Telecommunications Technology', N'#C1EBB3', 15)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (59, N'303', 610, N'Fundamentals of Computing 1', N'#D5D6C2', 15)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (60, N'311', 608, N'Object-Oriented Programming C++ (Advanced Concepts in Computing)', N'#D4918E', 15)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (61, N'428', 638, N'Hacking Exposed', N'#8EDAAB', 15)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (62, N'130', 1091, N'Anh Văn Chuyên Ngành cho Sinh Viên CMU 1', N'#C7C5DF', 16)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (63, N'401', 630, N'Information System Applications', N'#BCD6DA', 17)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (64, N'432', 618, N'Software Project Management', N'#85A6C4', 17)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (65, N'303', 620, N'Software Testing (Verification & Validation)', N'#C287D0', 18)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (66, N'433', 619, N'Software Process & Quality Management', N'#BCCDB3', 18)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (67, N'141', 2318, N'Nói & Trình Bày (tiếng Việt)', N'#C0F1CD', 19)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (68, N'142', 2319, N'Viết (tiếng Việt)', N'#F684B3', 19)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (69, N'435', 350, N'Quan Hệ Công Chúng', N'#9EBEC3', 19)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (70, N'210', 61, N'Lắp Ráp & Bảo Trì Hệ Thống', N'#87D8C3', 20)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (71, N'250', 62, N'Nền Tảng Hệ Thống Máy Tính', N'#C2FBB2', 20)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (72, N'332', 898, N'Nhập Môn Lập Trình Vi Điều Khiển', N'#DABFFE', 20)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (73, N'424', 81, N'Lập Trình Ứng Dụng cho các Thiết Bị Di Động', N'#8E9C80', 20)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (74, N'100', 48, N'Giới Thiệu về Khoa Học Máy Tính', N'#F595FE', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (75, N'201', 23, N'Tin Học Ứng Dụng', N'#D88EB6', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (76, N'211', 56, N'Lập Trình Cơ Sở', N'#C28EA6', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (77, N'226', 63, N'Hệ Điều Hành Unix / Linux', N'#A091B6', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (78, N'252', 65, N'Mạng Máy Tính', N'#88B1C3', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (79, N'297', 965, N'Đồ Án CDIO', N'#C6B3A7', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (80, N'303', 66, N'Phân Tích & Thiết Kế Hệ Thống', N'#F2D2CE', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (81, N'311', 57, N'Lập Trình Hướng Đối Tượng', N'#89ADE2', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (82, N'312', 2324, N'Thiết Kế Web', N'#8BDEB1', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (83, N'316', 55, N'Giới Thiệu Cấu Trúc Dữ Liệu & Giải Thuật', N'#D6CFA9', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (84, N'353', 99, N'Phân Tích & Thiết Kế Hướng Đối Tượng', N'#FDCDB7', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (85, N'372', 73, N'Quản Trị Mạng', N'#98A597', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (86, N'403', 100, N'Công Nghệ Phần Mềm', N'#93F182', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (87, N'417', 94, N'Trí Tuệ Nhân Tạo (Biểu Diễn & Giải Thuật)', N'#A893BF', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (88, N'420', 72, N'Hệ Phân Tán (J2EE, .NET)', N'#D7B9B4', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (89, N'421', 74, N'Thiết Kế Mạng', N'#8DF6F9', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (90, N'428', 78, N'Tấn Công Mạng', N'#FFC9C1', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (91, N'434', 101, N'Công Cụ & Phương Pháp Thiết Kế - Quản Lý (Phần Mềm)', N'#92E4EE', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (92, N'462', 102, N'Kiểm Thử & Đảm Bảo Chất Lượng Phần Mềm', N'#F9D1F0', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (93, N'464', 2317, N'Lập Trình Ứng Dụng .NET', N'#F6D0E2', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (94, N'466', 910, N'Perl & Python', N'#DDC9D3', 21)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (95, N'378', 760, N'Kết Cấu Thép', N'#94EBAD', 22)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (96, N'441', 765, N'Quản Lý Dự Án Xây Dựng', N'#AC8FCC', 22)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (97, N'430', 1100, N'Anh Văn Chuyên Ngành cho Sinh Viên CSU 4', N'#C4B5DF', 23)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (98, N'251', 147, N'Cơ Sở Văn Hóa Việt Nam', N'#F891DB', 24)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (99, N'379', 1801, N'Văn Hóa Trung Hoa', N'#8097E4', 24)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (100, N'446', 1998, N'Thực Tập Cộng Đồng Cho Răng - Hàm - Mặt 1', N'#D78E88', 25)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (101, N'634', 1990, N'Nha Khoa Cấy Ghép', N'#B3B480', 25)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (102, N'643', 1987, N'Phục Hình 3', N'#DD9AAF', 25)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (103, N'663', 1996, N'Nhân Học Răng & Pháp Nha', N'#EA8EE1', 25)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (104, N'221', 537, N'CorelDraw & Adobe Illustrator', N'#90F6CD', 26)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (105, N'231', 538, N'Adobe Photoshop', N'#C2FBCD', 26)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (106, N'341', 560, N'3D Modeling & Animations', N'#F9DC99', 26)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (107, N'201', 40, N'Đạo Đức trong Công Việc', N'#D0ADA9', 27)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (108, N'151', 311, N'Căn Bản Kinh Tế Vi Mô', N'#A2BFFF', 28)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (109, N'152', 312, N'Căn Bản Kinh Tế Vĩ Mô', N'#9ABE84', 28)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (110, N'302', 314, N'Kinh Tế Trong Quản Trị', N'#DFEAC8', 28)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (111, N'303', 469, N'Kinh Tế Trong Quản Trị Dịch Vụ', N'#A48BE4', 28)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (112, N'391', 524, N'Kinh Tế Môi Trường', N'#EBD3FE', 28)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (113, N'200', 67, N'Mạch và Linh Kiện Điện Tử', N'#99E8C5', 29)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (114, N'251', 822, N'Kỹ Thuật Điện', N'#AF87FE', 29)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (115, N'404', 831, N'Mô Hình Hóa & Mô Phỏng Hệ Thống Điều Khiển', N'#82FEDB', 29)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (116, N'416', 827, N'Ngắt Mạch & Bảo Vệ Rơ-le trong Hệ Thống Điện', N'#F3F780', 29)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (117, N'436', 110, N'Kỹ Thuật Điện Thoại & Tổng Đài', N'#D6E4BA', 29)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (118, N'439', 1417, N'Truyền Thông Không Dây', N'#9FF9C3', 29)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (119, N'454', 1726, N'DSP Cho Hệ Thống Nhúng Thời Gian Thực', N'#FDB480', 29)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (120, N'491', 835, N'Điều Khiển Số', N'#A9E5AA', 29)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (121, N'495', 833, N'Robot Công Nghiệp', N'#89AADF', 29)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (122, N'104', 138, N'Ngữ Pháp Anh Văn Căn Bản', N'#9D90D6', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (123, N'107', 126, N'Viết 1', N'#E5F8D0', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (124, N'108', 130, N'Nghe 1', N'#E1839B', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (125, N'109', 134, N'Nói 1', N'#AD83EC', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (126, N'116', 1067, N'Reading - Level 1', N'#F5F1E3', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (127, N'117', 1068, N'Writing - Level 1', N'#D19DA6', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (128, N'118', 1069, N'Listening - Level 1', N'#88BE9A', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (129, N'119', 1070, N'Speaking - Level 1', N'#B6C4E9', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (130, N'126', 1232, N'Reading - Level 1 (International School)', N'#A1E78E', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (131, N'127', 1233, N'Writing - Level 1 (International School)', N'#AEF2B8', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (132, N'128', 1234, N'Listening - Level 1 (International School)', N'#DEBCB9', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (133, N'129', 1235, N'Speaking - Level 1 (International School)', N'#EFA38F', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (134, N'135', 1292, N'Anh Văn Cho Y và Sinh', N'#95F9FE', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (135, N'166', 1071, N'Reading - Level 2', N'#99D8C6', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (136, N'167', 1072, N'Writing - Level 2', N'#B8B7F7', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (137, N'168', 1073, N'Listening - Level 2', N'#BCCCC6', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (138, N'169', 1074, N'Speaking - Level 2', N'#B0ACA9', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (139, N'204', 139, N'Ngữ Pháp Anh Văn Nâng Cao', N'#D9B9E6', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (140, N'206', 123, N'Đọc 2', N'#9ED082', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (141, N'207', 127, N'Viết 2', N'#90DCE2', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (142, N'208', 131, N'Nghe 2', N'#AD8396', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (143, N'209', 135, N'Nói 2', N'#C0EB89', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (144, N'216', 1075, N'Reading - Level 3', N'#8FAF8B', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (145, N'217', 1076, N'Writing - Level 3', N'#ACBFC3', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (146, N'218', 1077, N'Listening - Level 3', N'#88AB89', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (147, N'219', 1078, N'Speaking - Level 3', N'#86EF9F', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (148, N'226', 1236, N'Reading - Level 2 (International School)', N'#B982C9', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (149, N'227', 1237, N'Writing - Level 2 (International School)', N'#A790D4', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (150, N'228', 1238, N'Listening - Level 2 (International School)', N'#ABBF9F', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (151, N'229', 1239, N'Speaking - Level 2 (International School)', N'#C0E1AE', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (152, N'266', 1079, N'Reading - Level 4', N'#B4B6FC', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (153, N'267', 1080, N'Writing - Level 4', N'#DE9489', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (154, N'268', 1081, N'Listening - Level 4', N'#C5809B', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (155, N'269', 1082, N'Speaking - Level 4', N'#DD87E1', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (156, N'296', 1015, N'Tranh Tài Giải Pháp PBL', N'#83C681', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (157, N'306', 124, N'Đọc 3', N'#CCB1E9', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (158, N'307', 128, N'Viết 3', N'#B3BABA', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (159, N'309', 136, N'Nói 3', N'#FF9CD3', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (160, N'319', 140, N'Ngữ Âm - Âm Vị Học', N'#ADFCB1', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (161, N'335', 1280, N'Anh Văn Chuyên Ngành Kiến Trúc', N'#ACB984', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (162, N'357', 129, N'Viết 4', N'#EBF6F8', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (163, N'359', 137, N'Nói 4', N'#B9B9A4', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (164, N'369', 1086, N'Speaking - Level 5', N'#A9A9BC', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (165, N'383', 296, N'Anh Văn Lễ Tân', N'#F6E7DF', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (166, N'422', 731, N'Dịch Thuật Văn Chương', N'#82B0B6', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (167, N'427', 293, N'Thời Sự Trong Nước - Việt-Anh', N'#A5BC7F', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (168, N'430', 295, N'Dịch Hội Nghị', N'#C59CFD', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (169, N'432', 297, N'Anh Văn Thư Tín Thương Mại', N'#CBF7BD', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (170, N'434', 298, N'Anh Văn Đàm Phán', N'#FCE689', 30)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (171, N'205', 43, N'Sức Khỏe Môi Trường', N'#B6ECCF', 31)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (172, N'404', 1187, N'Quản Lý Môi Trường Nông Nghiệp & Nông Thôn', N'#B3B9DA', 31)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (173, N'406', 1104, N'Quản Lý Tài Nguyên Đất', N'#93BEDF', 31)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (174, N'354', 2217, N'Quản Trị Dịch Vụ Giải Trí & Thư Giãn', N'#C981DF', 32)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (175, N'362', 2224, N'Quản Trị Sự Kiện Thể Thao', N'#A3947F', 32)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (176, N'301', 327, N'Quản Trị Tài Chính 1', N'#FBD6B6', 33)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (177, N'302', 331, N'Quản Trị Tài Chính 2', N'#98A983', 33)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (178, N'383', 1419, N'Thị Trường Chứng Khoán', N'#82EFD7', 33)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (179, N'401', 366, N'Các Tổ Chức Tài Chính', N'#FEE1FA', 33)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (180, N'403', 339, N'Tài Chính Chứng Khoán', N'#CAE9C7', 33)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (181, N'413', 481, N'Quản Trị Tài Chính Khách Sạn', N'#C587E7', 33)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (182, N'424', 1420, N'Tài Chính Vi Mô', N'#FCE6FD', 33)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (183, N'442', 211, N'Lập Dự Án Đầu Tư Xây Dựng', N'#C495BF', 33)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (184, N'445', 2207, N'Đầu Tư & Tài Trợ Bất Động Sản', N'#A4D085', 33)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (185, N'414', 889, N'Tổ Chức Công Tác Kế Toán', N'#A4A8E1', 34)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (186, N'221', 41, N'Lịch Sử Văn Minh Thế Giới 1', N'#F4C0DE', 35)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (187, N'222', 42, N'Lịch Sử Văn Minh Thế Giới 2', N'#E3F6C4', 35)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (188, N'362', 1860, N'Lịch Sử Đảng Cộng Sản Việt Nam', N'#E4A299', 35)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (189, N'151', 300, N'Tổng Quan Ngành Lưu Trú', N'#9DE1F0', 36)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (190, N'361', 470, N'Giới Thiệu Nghiệp Vụ Nhà Hàng', N'#E6A5F1', 36)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (191, N'364', 472, N'Nghiệp Vụ Bàn', N'#B283EF', 36)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (192, N'401', 473, N'Quản Trị Nhà Hàng', N'#ECDF9D', 36)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (193, N'403', 479, N'Quản Trị Cơ Sở Vật Chất Khách Sạn', N'#F5A18B', 36)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (194, N'301', 326, N'Quản Trị Nhân Lực', N'#CBEEC7', 37)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (195, N'303', 305, N'Quản Trị Nhân Lực Trong Du Lịch', N'#80A3D9', 37)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (196, N'403', 1953, N'Quản Trị Chiến Lược Nguồn Nhân Lực', N'#95FBC8', 37)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (197, N'404', 1950, N'Đào Tạo & Phát Triển Nhân Lực', N'#90DBA8', 37)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (198, N'351', 340, N'Thương Mại Quốc Tế', N'#82C6D9', 38)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (199, N'301', 1733, N'Thiết Kế Logo', N'#8EDF9E', 39)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (200, N'354', 1734, N'Thiết Kế Sách', N'#C989DA', 39)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (201, N'434', 1120, N'Thiết Kế Bao Bì', N'#ACEE8A', 39)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (202, N'422', 443, N'Quan Hệ Quốc Tế Âu - Mỹ', N'#9D9BC5', 40)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (203, N'434', 447, N'Kỹ Năng Đàm Phán Quốc Tế', N'#B2DE87', 40)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (204, N'251', 323, N'Hệ Thống Thông Tin Quản Lý', N'#BFAFC4', 41)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (205, N'253', 1002, N'Hệ Thống Thông Tin Du Lịch', N'#E2B7E8', 41)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (206, N'301', 60, N'Cơ Sở Dữ Liệu', N'#B7F7DC', 41)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (207, N'381', 354, N'Thương Mại Điện Tử', N'#E6A584', 41)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (208, N'385', 2316, N'Kỹ Thuật Thương Mại Điện Tử', N'#CAC1A8', 41)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (209, N'401', 98, N'Hệ Quản Trị Cơ Sở Dữ Liệu', N'#C089FB', 41)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (210, N'402', 631, N'Hệ Hỗ Trợ Ra Quyết Định', N'#FFC9DB', 41)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (211, N'136', 2325, N'English for International School - Level 1', N'#BEA281', 42)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (212, N'186', 2327, N'English for International School - Level 3', N'#D5F1A5', 42)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (213, N'187', 2328, N'English for International School - Level 4', N'#9CFEFB', 42)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (214, N'236', 2329, N'English for International School - Level 5', N'#B4F299', 42)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (215, N'101', 5, N'Nhật Ngữ Sơ Cấp 1', N'#89C897', 43)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (216, N'201', 11, N'Nhật Ngữ Trung Cấp 1', N'#CF82E3', 43)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (217, N'202', 14, N'Nhật Ngữ Trung Cấp 2', N'#9184D8', 43)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (218, N'302', 20, N'Nhật Ngữ Cao Cấp 2', N'#959795', 43)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (219, N'102', 1914, N'Hàn Ngữ Sơ Cấp 2', N'#7FB5B0', 44)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (220, N'107', 2082, N'Viết 1', N'#CC96B5', 44)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (221, N'302', 1918, N'Hàn Ngữ Cao Cấp 2', N'#99FAEC', 44)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (222, N'371', 2086, N'Biên Dịch 1 (Tiếng Hàn)', N'#FBBC9B', 44)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (223, N'372', 2087, N'Phiên Dịch 1 (Tiếng Hàn)', N'#E3F7EC', 44)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (224, N'396', 2068, N'Tranh tài giải pháp PBL', N'#C0AD82', 44)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (225, N'417', 2628, N'Luyện Thi Năng Lực Tiếng Hàn 1 (Đọc - Viết)', N'#DF9FE6', 44)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (226, N'423', 2092, N'Tiếng Hàn trong Nhà Hàng Khách Sạn', N'#A687B7', 44)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (227, N'201', 39, N'Pháp Luật Đại Cương', N'#B8C1B1', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (228, N'230', 1254, N'Luật Hành Chính', N'#EDF1C1', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (229, N'261', 1248, N'Xây Dựng Văn Bản Pháp Luật', N'#C18096', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (230, N'308', 1271, N'Luật Dân Sự 2', N'#838ED7', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (231, N'323', 1260, N'Công Pháp Quốc Tế', N'#AFA1CF', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (232, N'326', 1724, N'Luật Tố Tụng Hành Chính', N'#A783BE', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (233, N'369', 1602, N'Luật Môi Trường', N'#B3B0AF', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (234, N'376', 1266, N'Luật Sở Hữu Trí Tuệ', N'#8CFCD3', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (235, N'403', 328, N'Cơ Sở Luật Kinh Tế', N'#8E8DA8', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (236, N'413', 434, N'Pháp Luật Du Lịch (Việt Nam)', N'#E6D8CF', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (237, N'425', 1594, N'Luật Chứng Khoán', N'#A49DE4', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (238, N'427', 1595, N'Luật Ngân Hàng', N'#E2F4C8', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (239, N'476', 1272, N'Luật Thương Mại Quốc tế', N'#C4D880', 45)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (240, N'316', 143, N'Cú Pháp Học (trong tiếng Anh)', N'#D2A5B6', 46)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (241, N'422', 144, N'Ngữ Nghĩa Học (trong tiếng Anh)', N'#97F6A2', 46)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (242, N'376', 148, N'Văn Học Anh', N'#D9E6B4', 47)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (243, N'378', 149, N'Văn Học Mỹ', N'#E298FE', 47)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (244, N'111', 2106, N'Vẽ Kỹ Thuật Cơ Khí', N'#E3B0BB', 48)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (245, N'201', 154, N'Cơ Lý Thuyết 1', N'#CE9DC8', 48)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (246, N'202', 221, N'Cơ Lý Thuyết 2', N'#B5F3BA', 48)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (247, N'205', 2111, N'Nguyên Lý Máy', N'#ACD3D5', 48)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (248, N'210', 2110, N'Sức Bền Vật Liệu', N'#BEDDC0', 48)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (249, N'213', 2478, N'Các Kỹ Năng Sơ Cấp Cứu', N'#D5D490', 49)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (250, N'268', 914, N'Y Đức', N'#9DC088', 49)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (251, N'362', 573, N'Y Học Cổ Truyền', N'#A5A4BE', 49)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (252, N'661', 1351, N'Y Học Gia Đình', N'#B9CABA', 49)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (253, N'705', 1358, N'Pháp Y', N'#F6EBF5', 49)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (254, N'709', 1357, N'Y Học Thảm Họa', N'#FEB2F8', 49)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (255, N'301', 316, N'Quản Trị Hoạt Động & Sản Xuất', N'#F99DBA', 50)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (256, N'403', 317, N'Các Mô Hình Ra Quyết Định', N'#CCE3F6', 50)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (257, N'201', 318, N'Quản Trị Học', N'#C3F6A3', 51)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (258, N'296', 985, N'Tranh Tài Giải Pháp PBL', N'#93AE83', 51)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (259, N'371', 343, N'Quản Trị Chất Lượng & Rủi Ro', N'#E2C9C5', 51)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (260, N'374', 344, N'Quản Trị Hành Chính Văn Phòng', N'#CF86BD', 51)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (261, N'402', 341, N'Quản Trị Dự Án Đầu Tư', N'#CF83FC', 51)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (262, N'403', 319, N'Quản Trị Chiến Lược', N'#9EA5F5', 51)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (263, N'406', 342, N'Khởi Sự Doanh Nghiệp', N'#E8F3D6', 51)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (264, N'251', 322, N'Tiếp Thị Căn Bản', N'#ADC1D7', 52)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (265, N'253', 303, N'Tiếp Thị Du Lịch', N'#94D5CF', 52)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (266, N'359', 360, N'Tiếp Thị Địa Phương', N'#EDBEA8', 52)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (267, N'401', 352, N'Quản Trị Thu Mua', N'#F1B8E2', 52)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (268, N'402', 353, N'Quản Trị Bán Hàng', N'#888FFD', 52)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (269, N'404', 356, N'Hành Vi Tiêu Dùng', N'#C488C6', 52)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (270, N'407', 1474, N'Quản Trị Bán Lẻ', N'#FFA8EA', 52)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (271, N'424', 304, N'Hành Vi Tiêu Dùng Trong Du Lịch', N'#ADC2B1', 52)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (272, N'425', 1969, N'Digital Marketing', N'#89D3AA', 52)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (273, N'400', 1312, N'Kỹ Thuật & Công Nghệ Y Dược', N'#BFACB3', 53)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (274, N'406', 1329, N'Y Học Hạt Nhân', N'#D881F2', 53)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (275, N'100', 24, N'Toán Cao Cấp C', N'#EABE87', 54)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (276, N'101', 25, N'Toán Cao Cấp C1', N'#928EA7', 54)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (277, N'102', 26, N'Toán Cao Cấp C2', N'#96F793', 54)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (278, N'103', 27, N'Toán Cao Cấp A1', N'#F491D2', 54)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (279, N'104', 28, N'Toán Cao Cấp A2', N'#CDA7D0', 54)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (280, N'135', 1283, N'Toán Giải Tích cho Y-Dược', N'#B89AAA', 54)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (281, N'254', 53, N'Toán Rời Rạc & Ứng Dụng', N'#B0B599', 54)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (282, N'341', 1509, N'Toán Ứng Dụng cho Công Nghệ Thông Tin 2', N'#C0ACB1', 54)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (283, N'151', 536, N'Dinh Dưỡng Học', N'#A5E2CB', 55)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (284, N'431', 803, N'Thực Phẩm Chức Năng', N'#DCDFC7', 55)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (285, N'306', 565, N'Điều Dưỡng cho Gia Đình có Người Già 1', N'#C9E2C0', 56)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (286, N'251', 325, N'Tổng Quan Hành Vi Tổ Chức', N'#9FCDC6', 57)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (287, N'253', 859, N'Tổng Quan Hành Vi Tổ Chức trong Du Lịch', N'#B0E9AC', 57)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (288, N'403', 348, N'Nghệ Thuật Lãnh Đạo', N'#81BC81', 57)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (289, N'251', 1286, N'Sinh Lý 1', N'#A2CFA5', 58)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (290, N'401', 788, N'Công Nghệ Sản Xuất Dược Phẩm 1', N'#B4D291', 59)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (291, N'100', 1, N'Phương Pháp Luận (gồm Nghiên Cứu Khoa Học)', N'#C2C5E2', 60)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (292, N'150', 1857, N'Triết Học Marx - Lenin', N'#82ECD6', 60)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (293, N'162', 45, N'Những Nguyên Lý Cơ Bản của Chủ Nghĩa Marx - Lenin 2', N'#CEB7EC', 60)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (294, N'461', 1317, N'Phương Pháp Nghiên Cứu Khoa Hoc Trong Y Học', N'#DDD1AF', 60)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (295, N'410', 799, N'Nhóm GP (GDP, GSP, GPP)', N'#8EA381', 61)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (296, N'101', 30, N'Vật Lý Đại Cương 1', N'#C688C6', 62)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (297, N'102', 31, N'Vật Lý Đại Cương 2', N'#A685FC', 62)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (298, N'443', 802, N'Mỹ Phẩm', N'#FCE4B9', 63)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (299, N'492', 1673, N'Process Control Instrumentation', N'#8DADB5', 64)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (300, N'495', 1684, N'Applications of Industrial Robots for Advanced Manufacturing', N'#C7FAE9', 64)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (301, N'151', 1858, N'Kinh Tế Chính Trị Marx - Lenin', N'#9AB584', 65)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (302, N'351', 1859, N'Chủ Nghĩa Xã Hội Khoa Học', N'#BBB19A', 65)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (303, N'361', 47, N'Tư Tưởng Hồ Chí Minh', N'#C6BAE9', 65)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (304, N'201', 646, N'Nguyên Lý Kế Toán 1', N'#8AF792', 66)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (305, N'202', 647, N'Nguyên Lý Kế Toán 2', N'#9B89FD', 66)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (306, N'301', 648, N'Kế Toán Quản Trị 1', N'#BEF790', 66)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (307, N'306', 857, N'Kế Toán Quản Trị trong Du Lịch', N'#B08089', 66)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (308, N'384', 658, N'Nghệ Thuật Đàm Phán', N'#F6F7DB', 67)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (309, N'151', 641, N'Căn Bản Kinh Tế Vi Mô', N'#C281C3', 68)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (310, N'130', 1094, N'Anh Văn Chuyên Ngành cho Sinh Viên PSU 1', N'#E09AEF', 69)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (311, N'301', 651, N'Quản Trị Tài Chính 1', N'#8BADD5', 70)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (312, N'373', 653, N'Giới Thiệu về Mô Hình Hóa Tài Chính', N'#7FC8C3', 70)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (313, N'401', 659, N'Các Tổ Chức Tài Chính', N'#D1A7B2', 70)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (314, N'413', 868, N'Quản Trị Tài Chính trong Du Lịch - Dịch Vụ', N'#F4A0F4', 70)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (315, N'374', 867, N'Nghiệp Vụ Khách Sạn', N'#F488A0', 71)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (316, N'402', 869, N'Quản Trị Khách Sạn', N'#F3A68C', 71)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (317, N'201', 644, N'Quản Trị Học', N'#D1D9D3', 72)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (318, N'403', 645, N'Quản Trị Chiến Lược', N'#D990A5', 72)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (319, N'403', 657, N'Nghệ Thuật Lãnh Đạo', N'#88C8AC', 73)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (320, N'442', 1911, N'Programming in Recreation Services and Tourism', N'#9FA9A9', 74)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (321, N'111', 1113, N'Nguyên Lý Thị Giác', N'#E67F82', 75)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (322, N'151', 391, N'Đại Cương Tâm Lý Học', N'#C694FB', 75)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (323, N'615', 1378, N'Bệnh Nghề Nghiệp', N'#EC9E83', 76)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (324, N'400', 575, N'Phục Hồi Chức Năng', N'#D88388', 77)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (325, N'400', 897, N'Quản Trị Kênh Phân Phối', N'#C980B9', 78)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (326, N'402', 2037, N'Logistics and Project Management', N'#868EDD', 78)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (327, N'405', 2038, N'Smart Factory & Operations Management', N'#F6F092', 78)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (328, N'347', 1966, N'Đồ Án CDIO', N'#C3BBC9', 79)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (329, N'397', 1967, N'Đồ Án CDIO', N'#97EDB0', 79)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (330, N'445', 908, N'Tích Hợp Hệ Thống', N'#D8D288', 79)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (331, N'447', 1968, N'Đồ Án CDIO', N'#A4CED4', 79)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (332, N'323', 545, N'Dân Số Học - Kế Hoạch Hóa Gia Đình - Sức Khỏe Gia Đình', N'#D8E7E1', 80)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (333, N'200', 780, N'Truyền Thông & Giáo Dục Sức Khỏe', N'#F2EFF8', 81)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (334, N'302', 542, N'Dịch Tễ Học', N'#F5D4C3', 81)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (335, N'303', 1377, N'Thực Hành Dịch Tễ Học', N'#E1F0A3', 81)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (336, N'616', 1993, N'Nha Khoa Công Cộng', N'#88ECAC', 81)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (337, N'151', 52, N'Lý Thuyết Xác Suất & Thống Kê Toán', N'#9E96AE', 82)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (338, N'155', 1284, N'Xác Suất Thống Kê Cho Y - Dược', N'#97E299', 82)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (339, N'271', 315, N'Nguyên Lý Thống Kê Kinh Tế (với SPSS)', N'#B5DBB0', 82)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (340, N'423', 482, N'Phân Tích Thống Kê Du Lịch', N'#ADA6B3', 82)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (341, N'251', 2113, N'Kỹ Thuật Nhiệt', N'#CD85CF', 83)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (342, N'404', 307, N'Quản Trị Kinh Doanh Lữ Hành', N'#BBA1A5', 84)
GO
INSERT [dbo].[Keyword] ([Id], [Keyword1], [CourseId], [SubjectName], [Color], [DisciplineId]) VALUES (343, N'411', 478, N'Quản Trị Sự Kiện', N'#A1E7F0', 84)
GO
SET IDENTITY_INSERT [dbo].[CwebizUser] ON
GO
INSERT [dbo].[CwebizUser] ([Id], [Username], [Password], [UserType], [StudentId]) VALUES (1, N'nguyennminhtrang1', N'$2a$11$AOQzGqB37CYJ2N.irpI6Funq0LdYnyEEzqMkfPOm1xW0V6sndB/wq', 1, N'26204335764')
GO
INSERT [dbo].[CwebizUser] ([Id], [Username], [Password], [UserType], [StudentId]) VALUES (2, N'nguyentthanhtam33', N'$2a$11$NXqw1O550a5IRbSKrqxwUOlyQ3C1qFgBj2hf4L5ogJJK74sNMyJKC', 1, N'24205205493')
GO
INSERT [dbo].[CwebizUser] ([Id], [Username], [Password], [UserType], [StudentId]) VALUES (3, N'vominhhieu3', N'$2a$11$7Z5C7zDZsFhsgWEMbiXzz.jZ3D7ATWUCslTq824yf7LzVbyHQO87u', 1, N'25211210412')
GO
SET IDENTITY_INSERT [dbo].[CwebizUser] OFF
GO


