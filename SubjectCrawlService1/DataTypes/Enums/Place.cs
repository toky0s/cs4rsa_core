namespace SubjectCrawlService1.DataTypes.Enums
{
    public enum Place
    {
        QUANGTRUNG,
        VIETTIN,
        PHANTHANH,
        HOAKHANH,
        NVL_254,
        NVL_137
    }

    public static class PlaceExtensions
    {
        public static string ToActualPlace(this Place place)
        {
            return place switch
            {
                Place.QUANGTRUNG => "03 Quang Trung",
                Place.VIETTIN => "VietTin",
                Place.HOAKHANH => "120 Hoàng Minh Thảo",
                Place.NVL_137 => "137 Nguyễn Văn Linh",
                _ => "254 Nguyễn Văn Linh"
            };
        }
    }
}
