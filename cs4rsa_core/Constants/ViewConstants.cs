using Cs4rsa.Views;

using MaterialDesignThemes.Wpf;

using System.Windows.Controls;

namespace Cs4rsa.Constants
{
    /// <summary>
    /// Chỉ mục màn hình
    /// 
    /// Các bước thêm một màn hình vào menu
    /// - Thêm mới một CredizScreen
    /// - Tạo View tương ứng và đặt vào MainWindow
    /// - Tạo ViewModel tương ứng cho View được kế thừa từ ViewModelBase
    /// </summary>
    internal enum ScreenIndex
    {
        /// <summary>
        /// Trang chủ
        /// </summary>
        HOME,

        /// <summary>
        /// Màn hình tài khoản
        /// </summary>
        ACCOUNT,

        /// <summary>
        /// Màn hình xếp lịch thủ công
        /// </summary>
        HAND,

        /// <summary>
        /// Màn hình xếp lịch tự động
        /// </summary>
        AUTO,

        /// <summary>
        /// Màn hình danh sách giảng viên
        /// </summary>
        TEACHER,

        /// <summary>
        /// Màn hình thông tin ứng dụng
        /// </summary>
        INFO,
    }

    internal readonly struct CredizScreen
    {
        public readonly UserControl Screen { get; init; }
        public readonly ScreenIndex Index { get; init; }
    }

    internal readonly struct CredizMenuItem
    {
        public readonly string MenuName { get; }
        public readonly PackIconKind IconKind { get; }
        public readonly PackIconKind IconKindOnSelected { get; }
        public readonly CredizScreen CredizScreen;
        public CredizMenuItem(
            string menuName,
            PackIconKind iconKind,
            PackIconKind iconKindOnSelected,
            CredizScreen credizScreen
        )
        {
            MenuName = menuName;
            IconKind = iconKind;
            IconKindOnSelected = iconKindOnSelected;
            CredizScreen = credizScreen;
        }
    }

    internal abstract class ViewConstants
    {
        public static readonly string APP_NAME = "Crediz";
        public static readonly string WELCOME_TEXT = "Chào mừng đến với CS4RSA";

        public static readonly CredizScreen Home = new() { Screen = new Home(), Index = ScreenIndex.HOME };
        public static readonly CredizScreen Account = new() { Screen = new Login(), Index = ScreenIndex.ACCOUNT };
        public static readonly CredizScreen Hand = new() { Screen = new MainScheduling(), Index = ScreenIndex.HAND };
        public static readonly CredizScreen Auto = new() { Screen = new AutoSchedule(), Index = ScreenIndex.AUTO };
        public static readonly CredizScreen Teacher = new() { Screen = new Teacher(), Index = ScreenIndex.TEACHER };
        public static readonly CredizScreen Info = new() { Screen = new Info(), Index = ScreenIndex.INFO };

        public static CredizMenuItem[] CREDIZ_MENU_ITEMS =
        {
            new CredizMenuItem("Trang chủ",             PackIconKind.HomeOutline,               PackIconKind.Home,              Home),
            new CredizMenuItem("Danh sách tài khoản",   PackIconKind.AccountOutline,            PackIconKind.Account,           Account),
            new CredizMenuItem("Xếp lịch thủ công",     PackIconKind.CursorDefaultOutline,      PackIconKind.CursorDefault,     Hand),
            new CredizMenuItem("Xếp lịch tự động",      PackIconKind.LightbulbOutline,          PackIconKind.Lightbulb,         Auto),
            new CredizMenuItem("Danh sách giảng viên",  PackIconKind.ClipboardAccountOutline,   PackIconKind.ClipboardAccount,  Teacher),
            new CredizMenuItem("Thông tin",             PackIconKind.InformationOutline,        PackIconKind.Information,       Info),
        };
    }
}
