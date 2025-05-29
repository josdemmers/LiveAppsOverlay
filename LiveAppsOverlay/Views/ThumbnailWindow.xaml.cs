using LiveAppsOverlay.ViewModels;
using LiveAppsOverlay.ViewModels.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace LiveAppsOverlay.Views
{
    /// <summary>
    /// Interaction logic for ThumbnailWindow.xaml
    /// </summary>
    public partial class ThumbnailWindow : Window
    {

        #region Constructors

        public ThumbnailWindow(HWND handleSource, ThumbnailConfigViewModel thumbnailConfigViewModel)
        {
            DataContext = App.Current.Services.GetRequiredService<ThumbnailWindowViewModel>();
            InitializeComponent();

            ((ThumbnailWindowViewModel)DataContext).CloseRequest += ThumbnailWindow_CloseRequest;
            ((ThumbnailWindowViewModel)DataContext).HandleSource = handleSource;
            ((ThumbnailWindowViewModel)DataContext).ThumbnailConfigViewModel = thumbnailConfigViewModel;

            this.Top = thumbnailConfigViewModel.Top;
            this.Left = thumbnailConfigViewModel.Left;
            this.Height = thumbnailConfigViewModel.Height;
            this.Width = thumbnailConfigViewModel.Width;
        }

        #endregion

        #region Events

        #endregion

        #region Properties

        #endregion

        #region Event handlers

        private void ThumbnailWindow_CloseRequest(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateActualSize()
        {
            if (double.IsNaN(Height) || double.IsNaN(Width)) return;

            IEnumerable children = LogicalTreeHelper.GetChildren(this);
            double height = 0;
            double width = 0;

            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    height = ((FrameworkElement)depChild).ActualHeight;
                    width = ((FrameworkElement)depChild).ActualWidth;
                }
            }

            ((ThumbnailWindowViewModel)DataContext).ActualHeight = height;
            ((ThumbnailWindowViewModel)DataContext).ActualWidth = width;
            ((ThumbnailWindowViewModel)DataContext).ActualHeightPixels = this.PointToScreen(new Point(width, height)).Y - this.PointToScreen(new Point(0, 0)).Y;
            ((ThumbnailWindowViewModel)DataContext).ActualWidthPixels = this.PointToScreen(new Point(width, height)).X - this.PointToScreen(new Point(0, 0)).X;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hWnd = windowInteropHelper.Handle;
            ((ThumbnailWindowViewModel)DataContext).Handle = (HWND)hWnd;
            ((ThumbnailWindowViewModel)DataContext).Init();

            var extendedStyle = PInvoke.GetWindowLong((HWND)hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
            int result = PInvoke.SetWindowLong((HWND)hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, extendedStyle | (int)WINDOW_EX_STYLE.WS_EX_TOOLWINDOW);

            UpdateActualSize();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateActualSize();
        }

        #endregion

        #region Methods

        #endregion
    }
}
