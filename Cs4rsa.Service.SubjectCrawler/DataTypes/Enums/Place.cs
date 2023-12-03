namespace Cs4rsa.Service.SubjectCrawler.DataTypes.Enums
{
    public enum Place
    {
        QuangTrung,
        VietTin,
        PhanThanh,
        HoaKhanh,
        Nvl254,
        Nvl137,
        Online
    }

    public static class PlaceExtensions
    {
        public static string ToActualPlace(this Place place)
        {
            switch (place)
            {
                case Place.QuangTrung:
                    return "03 Quang Trung";
                case Place.VietTin:
                    return "VietTin";
                case Place.HoaKhanh:
                    return "120 Hoàng Minh Thảo";
                case Place.Nvl137:
                    return "137 Nguyễn Văn Linh";
                case Place.Nvl254:
                    return "254 Nguyễn Văn Linh";
                case Place.PhanThanh:
                    return "Phan Thanh";
                case Place.Online:
                default:
                    return "Online";
            }
        }
    }
}
