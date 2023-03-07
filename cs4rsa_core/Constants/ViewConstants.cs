using Cs4rsa.BaseClasses;
using Cs4rsa.Views;
using Cs4rsa.Views.AutoScheduling;
using Cs4rsa.Views.Database;
using Cs4rsa.Views.ManualScheduling;
using Cs4rsa.Views.Profile;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Windows;

namespace Cs4rsa.Constants
{
    internal class CredizMenuItem
    {
        /// <summary>
        /// Khởi tạo Menu Item đi cùng với ViewModel.
        /// </summary>
        /// <param name="menuName">Tên Menu</param>
        /// <param name="iconKind">Icon khi chưa được select</param>
        /// <param name="iconKindOnSelected">Icon khi đang được select</param>
        /// <param name="screen">Màn hình</param>
        /// <param name="vm">ViewModel màn hình</param>
        public CredizMenuItem(
            string menuName,
            PackIconKind iconKind,
            PackIconKind iconKindOnSelected,
            ScreenAbstract screen,
            string vm)
        {
            MenuName = menuName;
            IconKind = iconKind;
            IconKindOnSelected = iconKindOnSelected;
            Screen = screen;
            _vm = vm;
        }

        /// <summary>
        /// Khởi tạo Menu Item không cần ViewModel.
        /// </summary>
        /// <param name="menuName">Tên Menu</param>
        /// <param name="iconKind">Icon khi chưa được select</param>
        /// <param name="iconKindOnSelected">Icon khi đang được select</param>
        /// <param name="screen">Màn hình</param>
        public CredizMenuItem(
            string menuName,
            PackIconKind iconKind,
            PackIconKind iconKindOnSelected,
            ScreenAbstract screen)
        {
            MenuName = menuName;
            IconKind = iconKind;
            IconKindOnSelected = iconKindOnSelected;
            Screen = screen;
        }

        public string MenuName { get; }
        public PackIconKind IconKind { get; }
        public PackIconKind IconKindOnSelected { get; }
        public ScreenAbstract Screen { get; }
        private readonly string _vm;

        /// <summary>
        /// Khi một màn hình kế thừa từ <see cref="Cs4rsa.BaseClasses.ScreenAbstract"/>
        /// được điều hướng tới bằng phương thức <see cref="Cs4rsa.Views.MainWindow.Goto(int)"/>
        /// , phương thức này sẽ được gọi để khởi tạo các thông tin ban đầu của màn hình.
        /// 
        /// Các <see cref="Cs4rsa.BaseClasses.IScreenViewModel"/> là DataContext của
        /// <see cref="Cs4rsa.BaseClasses.ScreenAbstract"/> sẽ lần lượt gọi các phương thức
        /// <see cref="Cs4rsa.BaseClasses.IScreenViewModel.InitData"/> và
        /// <see cref="Cs4rsa.BaseClasses.IScreenViewModel.InitDataAsync"/>.
        /// </summary>
        public void LoadSelfData()
        {
            if (_vm == null) return;
            Type viewModelType = Type.GetType(_vm);
            object viewModel = ActivatorUtilities.GetServiceOrCreateInstance(((App)Application.Current).Container, viewModelType);
            IScreenViewModel vm = (IScreenViewModel)viewModel;
            vm.InitData();
            App.Current.Dispatcher.InvokeAsync(async () => await vm.InitDataAsync());
        }
    }

    internal abstract class ViewConstants
    {
        private static readonly string VIEWMODEL_NAMESPACE = "Cs4rsa.ViewModels";

        public static readonly CredizMenuItem Screen01 = new(
            "Trang chủ"
            , PackIconKind.HomeOutline
            , PackIconKind.Home
            , new Home()
            , VIEWMODEL_NAMESPACE + ".HomeViewModel");

        public static readonly CredizMenuItem Screen02 = new(
            "Xếp lịch thủ công"
            , PackIconKind.CursorDefaultOutline
            , PackIconKind.CursorDefault
            , new MainScheduling()
            , VIEWMODEL_NAMESPACE + ".ManualScheduling.MainSchedulingViewModel");

        /// <summary>
        /// Màn hình Xếp lịch tự động
        /// </summary>
        public static readonly CredizMenuItem Screen03 = new(
            "Xếp lịch tự động"
            , PackIconKind.LightbulbOutline
            , PackIconKind.Lightbulb
            , new Auto());

        public static readonly CredizMenuItem Screen04 = new(
            "Hồ sơ"
            , PackIconKind.ClipboardAccountOutline
            , PackIconKind.ClipboardAccount
            , new Profile()
            , VIEWMODEL_NAMESPACE + ".Profile.ProfileViewModel");

        public static readonly CredizMenuItem Screen05 = new(
            "Cơ sở dữ liệu"
            , PackIconKind.DatabaseOutline
            , PackIconKind.Database
            , new Db()
            , VIEWMODEL_NAMESPACE + ".Database.DbViewModel");

        public static readonly CredizMenuItem Screen06 = new(
            "Thông tin"
            , PackIconKind.InformationOutline
            , PackIconKind.Information
            , new Info()
            , VIEWMODEL_NAMESPACE + ".InfoViewModel");

        public static CredizMenuItem[] CREDIZ_MENU_ITEMS =
        {
            Screen01,
            Screen02,
            Screen03,
            Screen04,
            Screen05,
            Screen06
        };
    }
}
