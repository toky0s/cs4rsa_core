using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;

using Squirrel;

using System.Windows;

namespace Cs4rsa.Views
{
    public partial class Home : ScreenAbstract
    {
        public Home()
        {
            InitializeComponent();
        }

        private async void Btn_Update_Click(object sender, RoutedEventArgs e)
        {
            using var mgr = await UpdateManager.GitHubUpdateManager(VmConstants.LinkProjectPage);
            if (mgr.IsInstalledApp)
            {
                ReleaseEntry newVersion = await mgr.UpdateApp();
                // optionally restart the app automatically, or ask the user if/when they want to restart
                if (newVersion != null)
                {
                    MessageBoxResult result = MessageBox.Show(
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
        }
    }
}
