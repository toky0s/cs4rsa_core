using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.DependencyInjection;

using Squirrel;
using Squirrel.Sources;

using System;
using System.Linq;
using System.Windows;

namespace Cs4rsa.Views
{
    public partial class Home : ScreenAbstract
    {
        //UpdateManager _manager;
        public Home()
        {
            InitializeComponent();
        }

        private void ScreenAbstract_Loaded(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    // Kiểm tra cập nhật
            //    UpdateManager _manager = new UpdateManager(new GithubSource(VmConstants.LinkProjectPage, string.Empty, true));
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(
            //        ex.Message
            //      , ViewConstants.Screen01.MenuName
            //      , MessageBoxButton.OK
            //      , MessageBoxImage.Error
            //    );
            //}
        }

        private async void Btn_Update_Click(object sender, RoutedEventArgs e)
        {
            using var mgr = await UpdateManager.GitHubUpdateManager(VmConstants.LinkProjectPage);
            if (mgr.IsInstalledApp)
            {
                var newVersion = await mgr.UpdateApp();
                // optionally restart the app automatically, or ask the user if/when they want to restart
                if (newVersion != null)
                {
                    var result = MessageBox.Show(
                        $"Phiên bản hiện tại của ứng dụng là {mgr.CurrentlyInstalledVersion()}, phiên bản mới nhất hiện tại là {newVersion.Version}. " +
                        $"Quá trình cập nhật sẽ tốn một ít thời gian và sẽ khởi động lại ứng dụng. Bạn có muốn tiếp tục?"
                      , ViewConstants.Screen01.MenuName
                      , MessageBoxButton.YesNoCancel
                      , MessageBoxImage.Information
                    );
                    if (result == MessageBoxResult.OK) UpdateManager.RestartApp();
                }
                else
                {
                    MessageBox.Show(
                        $"Phiên bản hiện tại của ứng dụng là {mgr.CurrentlyInstalledVersion()}, là PHIÊN BẢN MỚI NHẤT."
                      , ViewConstants.Screen01.MenuName
                      , MessageBoxButton.OK
                      , MessageBoxImage.Information
                    );
                }
            }
            else
            {
                MessageBox.Show(
                    $"[DEBUG] Ứng dụng chưa được cài đặt trên máy thật."
                  , ViewConstants.Screen01.MenuName
                  , MessageBoxButton.OK
                  , MessageBoxImage.Information
                );
            }


            //try
            //{
            //    UpdateInfo updateInfo = await _manager.CheckForUpdate();
            //    if (updateInfo.ReleasesToApply.Count > 0)
            //    {
            //        MessageBoxResult result = MessageBox.Show(
            //            $"Phiên bản mới nhất {updateInfo.ReleasesToApply.Last().Version} có sẵn, bạn có muốn thực hiện cập nhật?"
            //          , ViewConstants.Screen01.MenuName
            //          , MessageBoxButton.YesNoCancel
            //          , MessageBoxImage.Information
            //        );

            //        if (result == MessageBoxResult.OK)
            //        {
            //            var newVersion = await _manager.UpdateApp();
            //            if (newVersion != null) UpdateManager.RestartApp();
            //        }
            //        //Container.GetService<ISnackbarMessageQueue>()
            //        //    .Enqueue(
            //        //        "Cập nhật thành công hãy khởi động lại ứng dụng!"
            //        //      , "KHỞI ĐỘNG LẠI"
            //        //      , () =>
            //        //          {

            //        //          }
            //        //    );
            //    }
            //    else
            //    {
            //        Container.GetService<ISnackbarMessageQueue>()
            //            .Enqueue($"Đây là phiên bản mới nhất {_manager.CurrentlyInstalledVersion()}");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(
            //        CredizText.Common001("Kiểm tra cập nhật", ex.Message)
            //        , ViewConstants.Screen01.MenuName
            //        , MessageBoxButton.OK
            //        , MessageBoxImage.Error
            //    );
            //}
        }
    }
}
