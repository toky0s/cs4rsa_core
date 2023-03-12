using Cs4rsa.BaseClasses;

using System.Windows;

namespace Cs4rsa.Views
{
    public partial class Home : ScreenAbstract
    {
        //private UpdateManager _manager;
        public Home()
        {
            InitializeComponent();
        }

        private void ScreenAbstract_Loaded(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    // Kiểm tra cập nhật
            //    _manager = await UpdateManager.GitHubUpdateManager(VmConstants.LinkProjectPage);
            //    Debug.WriteLine($"Current version {_manager.CurrentlyInstalledVersion()}");
            //}
            //catch (InvalidOperationException ex)
            //{
            //    Debug.WriteLine("Have no release in root repo.");
            //    MessageBox.Show(
            //        ex.Message
            //      , ViewConstants.Screen01.MenuName
            //      , MessageBoxButton.OK
            //      , MessageBoxImage.Error
            //    );
            //}
        }

        private void Btn_Update_Click(object sender, RoutedEventArgs e)
        {
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
            //            await _manager.UpdateApp();
            //        }
            //        Container.GetService<ISnackbarMessageQueue>()
            //            .Enqueue(
            //                "Cập nhật thành công hãy khởi động lại ứng dụng!"
            //              , "KHỞI ĐỘNG LẠI"
            //              , () =>
            //                  {
            //                      UpdateManager.RestartAppWhenExited();
            //                  }
            //            );
            //    }
            //    else
            //    {
            //        Container.GetService<ISnackbarMessageQueue>()
            //            .Enqueue("Đây là phiên bản mới nhất");
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
