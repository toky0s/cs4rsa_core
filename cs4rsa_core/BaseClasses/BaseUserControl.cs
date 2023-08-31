using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.BaseClasses
{
    public abstract class BaseUserControl : UserControl
    {
        protected IServiceProvider Container { get; private set; }

        protected BaseUserControl()
        {
            Loaded += BaseUserControl_Loaded;
        }

        protected void BaseUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Container = ((App)Application.Current).Container;
        }

        protected void OpenHtmlCacheFile(string courseId)
        {
            IOpenInBrowser browser = Container.GetRequiredService<IOpenInBrowser>();
            IUnitOfWork unitOfWork = Container.GetRequiredService<IUnitOfWork>();
            string filePath = CredizText.PathHtmlCacheFile(courseId);

            if (!File.Exists(filePath))
            {
                string cache = unitOfWork.Keywords.GetCache(courseId);
                File.WriteAllText(filePath, StringHelper.CacheGenAddStyle(cache));
            }
            browser.Open(filePath);
        }
    }
}
