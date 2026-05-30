using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace Cs4rsa.Module.Shared
{
    public sealed class ToastService
    {
        // ── Singleton ────────────────────────────────────────────────────────
        private static readonly Lazy<ToastService> _instance =
            new Lazy<ToastService>(() => new ToastService());

        public static ToastService Instance
        {
            get { return _instance.Value; }
        }

        private ToastService() { }

        // ── Config ───────────────────────────────────────────────────────────
        private const int DisplayMs = 2000;
        private const double ToastGap = 8;
        private const double MarginRight = 16;
        private const double MarginBottom = 32;
        private const double ToastWidth = 320; // Khớp với MaxWidth trong XAML
        private const double ToastHeight = 75; // Khớp với MaxWidth trong XAML

        // ── State ────────────────────────────────────────────────────────────
        private readonly List<ToastNotification> _visible = new List<ToastNotification>();
        private Window _owner;

        // ── Public API ───────────────────────────────────────────────────────
        public void SetOwner(Window owner)
        {
            _owner = owner;
            _owner.LocationChanged += (s, e) => ReflowToasts();
            _owner.SizeChanged += (s, e) => ReflowToasts();
        }

        public void Show(string title, string message = "", ToastType type = ToastType.Info)
        {
            Dispatch(() => ShowOnUiThread(title, message, type));
        }

        public void Info(string title, string message = "")
        {
            Show(title, message, ToastType.Info);
        }

        public void Success(string title, string message = "")
        {
            Show(title, message, ToastType.Success);
        }

        public void Warning(string title, string message = "")
        {
            Show(title, message, ToastType.Warning);
        }

        public void Error(string title, string message = "")
        {
            Show(title, message, ToastType.Error);
        }

        // ── Internal ─────────────────────────────────────────────────────────
        private void Dispatch(Action action)
        {
            Dispatcher dispatcher = Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
                dispatcher.BeginInvoke(action);
            else
                action();
        }

        private void ShowOnUiThread(string title, string message, ToastType type)
        {
            var toast = new ToastNotification(title, message, type);

            toast.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            toast.Arrange(new Rect(toast.DesiredSize));

            PositionToast(toast);
            _visible.Add(toast);

            toast.Closed += (s, e) =>
            {
                _visible.Remove(toast);
                ReflowToasts();
            };

            toast.BeginShowSequence(DisplayMs);
        }

        private void PositionToast(ToastNotification toast)
        {
            double baseRight, baseBottom;

            if (_owner != null)
            {
                baseRight = _owner.Left + _owner.ActualWidth;
                baseBottom = _owner.Top + _owner.ActualHeight;
            }
            else
            {
                baseRight = SystemParameters.WorkArea.Right;
                baseBottom = SystemParameters.WorkArea.Bottom;
            }

            double usedHeight = 0;
            foreach (var t in _visible)
                usedHeight += t.ActualHeight + ToastGap;

            // Dùng ToastWidth cố định thay vì DesiredSize.Width (chưa render = 0)
            // ToastHeight ước tính cho lần đầu, sau Show() ActualHeight sẽ đúng

            toast.Left = baseRight - ToastWidth - MarginRight;
            toast.Top = baseBottom - MarginBottom - usedHeight - ToastHeight;
        }

        private void ReflowToasts()
        {
            if (_owner == null) return;

            double baseRight = _owner.Left + _owner.ActualWidth;
            double baseBottom = _owner.Top + _owner.ActualHeight;
            double usedHeight = 0;

            for (int i = _visible.Count - 1; i >= 0; i--)
            {
                var t = _visible[i];
                t.Left = baseRight - ToastWidth - MarginRight;
                t.Top = baseBottom - MarginBottom - usedHeight - ToastHeight;
                usedHeight += ToastHeight + ToastGap;
            }
        }
    }
}