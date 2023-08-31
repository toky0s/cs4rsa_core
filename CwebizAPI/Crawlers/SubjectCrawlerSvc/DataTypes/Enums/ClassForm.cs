namespace CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes.Enums
{
    /**
    * Danh sách hình thức lớp
    * Tham khảo
    * http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listpolproc&infowebpartid=5 
    */
    public class ClassForm
    {
        public static readonly ClassForm CLC = new("CLC", "CLiniC", "Lâm Sàng", "Thực tập/thực hành trực tiếp với bệnh nhân");
        public static readonly ClassForm COL = new("COL", "COLloquium", "Hội Thảo Chuyên Đề", "Hội họp với sự góp mặt của nhiều giảng viên hay những người có nhiều kinh nghiệm thực tế");
        public static readonly ClassForm CON = new("CON", "CONversation", "Đối Thoại", "Thực hành đối thoại ngôn ngữ/ngoại ngữ");
        public static readonly ClassForm DEM = new("DEM", "DEMonstration", "Biểu Diễn", "Học qua quan sát biểu diễn hay mô phỏng của giảng viên");
        public static readonly ClassForm DIS = new("DIS", "DIScussion", "Thảo Luận", "Hỏi đáp giữa sinh viên/học viên với giảng viên");
        public static readonly ClassForm FLD = new("FLD", "FieLDwork", "Dã Ngoại", "Học qua tham quan/đi dã ngoại ngoài lớp học");
        public static readonly ClassForm GRP = new("GRP", "GRouP study", "Học Nhóm", "Học nhóm theo hướng dẫn của giảng viên");
        public static readonly ClassForm IND = new("IND", "INDependent study", "Tự Nghiên Cứu", "Tự nghiên cứu và phát triển");
        public static readonly ClassForm INT = new("INT", "INTernship", "Thực Tập", "Làm việc ở các doanh nghiệp, cơ quan, tổ chức thực tế");
        public static readonly ClassForm LAB = new("LAB", "LABoratory", "Thực Hành \\ Thí Nghiệm", "Thực hành trong phòng thí nghiệm");
        public static readonly ClassForm LEC = new("LEC", "LECture", "Giảng Lý Thuyết", "Nghe giảng lý thuyết");
        public static readonly ClassForm PRJ = new("PRJ", "PRoJect", "Đồ Án", "Học qua việc làm đồ án và/hoặc dự án");
        public static readonly ClassForm REA = new("REA", "REAding", "Đọc", "Học qua tự đọc hay đọc theo hướng dẫn");
        public static readonly ClassForm REC = new("REC", "RECitation", "Ôn Tập", "Ôn tập lại những kiến thức đã học trong các hình thức lớp khác (thường là lý thuyết)", "ôn tập (thường xuyên)");
        public static readonly ClassForm SEM = new("SEM", "SEMinar", "Seminar", "Hội họp");
        public static readonly ClassForm SES = new("SES", "SESsion", "Trình Bày \\ Thảo Luận", "Trình bày nội dung và ngay sau đó, thảo luận về nội dung đó");
        public static readonly ClassForm SLF = new("SLF", "SeLF-study", "Tự Học", "Tự học theo những yêu cầu cụ thể (ví dụ qua việc làm danh sách bài học)");
        public static readonly ClassForm STD = new("STD", "STuDio", "Studio", "Thực hành hoặc làm đồ án trong Studio(s)");
        public static readonly ClassForm SUP = new("SUP", "SUPplement", "Bổ Trợ", "Học thêm hoặc ôn tập thêm");
        public static readonly ClassForm TUT = new("TUT", "TUToring", "Phụ Đạo", "Một người phụ đạo cho một hoặc một số người khác");
        public static readonly ClassForm VOL = new("VOL", "VOLuntary work", "Tình Nguyện", "Tự nguyện tham gia");
        public static readonly ClassForm WOR = new("WOR", "WORkshop", "Workshop", "Thực hành qua việc trực tiếp làm một việc gì đó, với sự góp mặt của nhiều người khác");

        private static readonly ClassForm[] _classForms = {
            CLC,COL,CON,DEM,DIS,
            FLD,GRP,IND,INT,LAB,
            LEC,PRJ,REA,REC,SEM,
            SES,SLF,STD,SUP,TUT,VOL,WOR
        };

        public string Code { get; }
        public readonly string FullEn;
        public readonly string FullVn;
        public readonly string Description;
        public readonly string[] Patterns;

        private ClassForm(string code, string fullEn, string fullVn, string description, params string[] patterns)
        {
            Code = code;
            FullEn = fullEn;
            FullVn = fullVn;
            Description = description;
            Patterns = patterns;
        }

        public static bool operator ==(ClassForm left, ClassForm right)
        {
            return left.Code == right.Code;
        }

        public static bool operator !=(ClassForm left, ClassForm right)
        {
            return left.Code != right.Code;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            ClassForm other = (ClassForm)obj;
            return Code.Equals(other.Code);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public static ClassForm[] GetClassForms()
        {
            return _classForms;
        }

        public override string ToString()
        {
            return Code;
        }

        public static ClassForm Find(string code)
        {
            code = code.ToLower();
            foreach (ClassForm classForm in _classForms)
            {
                if (classForm.Code.ToLower() == code
                    || classForm.FullEn.ToLower() == code
                    || classForm.FullVn.ToLower() == code
                    || classForm.Patterns.ToList().Contains(code))
                {
                    return classForm;
                }
            }
            return null;
        }
    }
}
