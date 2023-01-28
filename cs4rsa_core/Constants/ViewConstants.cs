using Cs4rsa.Views;
using Cs4rsa.Views.AutoScheduling;
using Cs4rsa.Views.Database;
using Cs4rsa.Views.ManualScheduling;

using MaterialDesignThemes.Wpf;

using System.Windows.Controls;

namespace Cs4rsa.Constants
{
    internal class CredizMenuItem
    {
        public CredizMenuItem(
            string menuName,
            PackIconKind iconKind,
            PackIconKind iconKindOnSelected,
            UserControl screen)
        {
            MenuName = menuName;
            IconKind = iconKind;
            IconKindOnSelected = iconKindOnSelected;
            Screen = screen;
        }

        public string MenuName { get; }
        public PackIconKind IconKind { get; }
        public PackIconKind IconKindOnSelected { get; }
        public UserControl Screen { get; }
    }

    internal abstract class ViewConstants
    {
        public static readonly string APP_NAME = "Crediz";
        public static readonly string WELCOME_TEXT = "Chào mừng đến với CS4RSA";

        public static CredizMenuItem[] CREDIZ_MENU_ITEMS =
        {
            new CredizMenuItem("Trang chủ"              ,PackIconKind.HomeOutline               ,PackIconKind.Home              ,new Home()           ),
            new CredizMenuItem("Xếp lịch thủ công"      ,PackIconKind.CursorDefaultOutline      ,PackIconKind.CursorDefault     ,new MainScheduling() ),
            new CredizMenuItem("Xếp lịch tự động"       ,PackIconKind.LightbulbOutline          ,PackIconKind.Lightbulb         ,new Auto()           ),
            new CredizMenuItem("Danh sách giảng viên"   ,PackIconKind.ClipboardAccountOutline   ,PackIconKind.ClipboardAccount  ,new Teacher()        ),
            new CredizMenuItem("Cơ sở dữ liệu"          ,PackIconKind.DatabaseOutline           ,PackIconKind.Database          ,new Db()             ),
            new CredizMenuItem("Thông tin"              ,PackIconKind.InformationOutline        ,PackIconKind.Information       ,new Info()           ),
        };
    }
}
