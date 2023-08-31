﻿using Cs4rsa.BaseClasses;
using Cs4rsa.Views;
using Cs4rsa.Views.AutoScheduling;
using Cs4rsa.Views.Database;
using Cs4rsa.Views.ManualScheduling;
using Cs4rsa.Views.Profile;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Diagnostics;
using System.Windows;

namespace Cs4rsa.Constants
{
    public class CredizMenuItem
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
            Vm = vm;
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
        private string Vm { get; }

        /// <summary>
        /// Khi một màn hình kế thừa từ <see cref="ScreenAbstract"/>
        /// được điều hướng tới bằng phương thức <see cref="MainWindow.Goto(int)"/>
        /// , phương thức này sẽ được gọi để khởi tạo các thông tin ban đầu của màn hình.
        /// 
        /// Các <see cref="IScreenViewModel"/> là DataContext của
        /// <see cref="ScreenAbstract"/> sẽ lần lượt gọi các phương thức
        /// <see cref="IScreenViewModel.InitData"/>
        /// </summary>
        public void LoadSelfData()
        {
            if (Vm is null) return;
            Type viewModelType = Type.GetType(Vm);
            if (viewModelType is null)
            {
                Debug.WriteLine($"Can not found VM with name {Vm}");
                return;
            }
            object viewModel = ActivatorUtilities.GetServiceOrCreateInstance(((App)Application.Current).Container, viewModelType);
            if (viewModel is not IScreenViewModel vm) return;
            vm.InitData();
        }
    }

    public abstract class ViewConstants
    {
        private const string ViewModelNamespace = "Cs4rsa.ViewModels";

        /// <summary>
        /// Màn hình Trang chủ
        /// </summary>
        public static readonly CredizMenuItem Screen01 = new(
            "Trang chủ"
            , PackIconKind.HomeOutline
            , PackIconKind.Home
            , new Home()
            , ViewModelNamespace + ".HomeViewModel");

        /// <summary>
        /// Màn hình Xếp lịch thủ công
        /// </summary>
        public static readonly CredizMenuItem Screen02 = new(
            "Xếp lịch thủ công"
            , PackIconKind.CursorDefaultOutline
            , PackIconKind.CursorDefault
            , new MainScheduling()
            , ViewModelNamespace + ".ManualScheduling.MainSchedulingViewModel");

        /// <summary>
        /// Màn hình Xếp lịch tự động
        /// </summary>
        public static readonly CredizMenuItem Screen03 = new(
            "Xếp lịch tự động"
            , PackIconKind.LightbulbOutline
            , PackIconKind.Lightbulb
            , new Auto());

        /// <summary>
        /// Màn hình Hồ sơ
        /// </summary>
        public static readonly CredizMenuItem Screen04 = new(
            "Hồ sơ"
            , PackIconKind.ClipboardAccountOutline
            , PackIconKind.ClipboardAccount
            , new Profile()
            , ViewModelNamespace + ".Profile.ProfileViewModel");

        /// <summary>
        /// Màn hình Cơ sở dữ liệu
        /// </summary>
        public static readonly CredizMenuItem Screen05 = new(
            "Cơ sở dữ liệu"
            , PackIconKind.DatabaseOutline
            , PackIconKind.Database
            , new Db()
            , ViewModelNamespace + ".Database.DbViewModel");

        /// <summary>
        /// Màn hình Thông tin
        /// </summary>
        public static readonly CredizMenuItem Screen06 = new(
            "Thông tin"
            , PackIconKind.InformationOutline
            , PackIconKind.Information
            , new Info());

        public static CredizMenuItem[] CredizMenu =
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
