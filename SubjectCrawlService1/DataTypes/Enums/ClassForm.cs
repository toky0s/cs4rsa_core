namespace SubjectCrawlService1.DataTypes.Enums {

    /**
    * Tham khảo
    * http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listpolproc&infowebpartid=5 
    */
    public class ClassForm {
        
        public static ClassForm CLC = new ClassForm("CLC", "CLiniC", "Lâm Sàng", "Thực tập/thực hành trực tiếp với bệnh nhân");
        public static ClassForm COL = new ClassForm("COL", "COLloquium", "Hội Thảo Chuyên Đề", "Hội họp với sự góp mặt của nhiều giảng viên hay những người có nhiều kinh nghiệm thực tế");
        public static ClassForm CON = new ClassForm("CON", "CONversation", "Đối Thoại", "Thực hành đối thoại ngôn ngữ/ngoại ngữ");
        
        public static ClassForm DEM = new ClassForm("DEM", "DEMonstration", "Biểu Diễn", "Học qua quan sát biểu diễn hay mô phỏng của giảng viên");
        public static ClassForm DIS = new ClassForm("DIS", "DIScussion", "Thảo Luận", "Hỏi đáp giữa sinh viên/học viên với giảng viên");
        public static ClassForm FLD = new ClassForm("FLD", "FieLDwork", "Dã Ngoại", "Học qua tham quan/đi dã ngoại ngoài lớp học");
        
        public static ClassForm GRP = new ClassForm("GRP", "GRouP study", "Học Nhóm", "Học nhóm theo hướng dẫn của giảng viên");
        public static ClassForm IND = new ClassForm("IND", "INDependent study", "Tự Nghiên Cứu", "Tự nghiên cứu và phát triển");
        public static ClassForm INT = new ClassForm("INT", "INTernship", "Thực Tập", "Làm việc ở các doanh nghiệp, cơ quan, tổ chức thực tế");
        public static ClassForm LAB = new ClassForm("LAB", "LABoratory", "Thực Hành \\ Thí Nghiệm", "Thực hành trong phòng thí nghiệm");

        public static ClassForm LEC = new ClassForm("LEC", "LECture", "Giảng Lý Thuyết", "Nghe giảng lý thuyết");
        public static ClassForm PRJ = new ClassForm("PRJ", "PRoJect", "Đồ Án", "Học qua việc làm đồ án và/hoặc dự án");
        public static ClassForm REA = new ClassForm("REA", "REAding", "Đọc", "Học qua tự đọc hay đọc theo hướng dẫn");

        public static ClassForm REC = new ClassForm("REC", "RECitation", "Ôn Tập", "Ôn tập lại những kiến thức đã học trong các hình thức lớp khác (thường là lý thuyết)");
        public static ClassForm SEM = new ClassForm("SEM", "SEMinar", "Seminar", "Hội họp");
        public static ClassForm SES = new ClassForm("SES", "SESsion", "Trình Bày \\ Thảo Luận", "Trình bày nội dung và ngay sau đó, thảo luận về nội dung đó");

        public static ClassForm SLF = new ClassForm("SLF", "SeLF-study", "Tự Học", "Tự học theo những yêu cầu cụ thể (ví dụ qua việc làm danh sách bài học)");
        public static ClassForm STD = new ClassForm("STD", "STuDio", "Studio", "Thực hành hoặc làm đồ án trong Studio(s)");
        public static ClassForm SUP = new ClassForm("SUP", "SUPplement", "Bổ Trợ", "Học thêm hoặc ôn tập thêm");
        public static ClassForm TUT = new ClassForm("TUT", "TUToring", "Phụ Đạo", "Một người phụ đạo cho một hoặc một số người khác");
        public static ClassForm VOL = new ClassForm("VOL", "VOLuntary work", "Tình Nguyện", "Tự nguyện tham gia");
        public static ClassForm WOR = new ClassForm("WOR", "WORkshop", "Workshop", "Thực hành qua việc trực tiếp làm một việc gì đó, với sự góp mặt của nhiều người khác");

        private string _code;
        public string Code { get => _code; }
        private string _fullEn;
        public string FullEn { get => _fullEn; } 
        private string _fullVn;
        public string FullVn { get => _fullVn; }
        private string _description;
        public string Description { get => _description;}
        
        private ClassForm(string code, string fullEn, string fullVn, string description) {
            _code = code;
            _fullEn = fullEn;
            _fullVn = fullVn;
            _description = description;
        }
    }
}