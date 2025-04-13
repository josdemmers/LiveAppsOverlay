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
    /// Interaction logic for ThumbnailWindowEdit.xaml
    /// </summary>
    public partial class ThumbnailWindowEdit : Window
    {
        private DrawingGroup _drawingGroup = new DrawingGroup();
        private bool _mouseDown = false;
        private Point _mousePosition = new Point(0, 0);
        private Point _mousePositionClick = new Point(0, 0);


        #region Constructors

        public ThumbnailWindowEdit(HWND handleSource, ThumbnailConfigViewModel thumbnailConfigViewModel)
        {
            InitializeComponent();

            DataContext = App.Current.Services.GetRequiredService<ThumbnailWindowViewModel>();

            ((ThumbnailWindowViewModel)DataContext).CloseRequest += ThumbnailWindow_CloseRequest;
            ((ThumbnailWindowViewModel)DataContext).DrawRegionCanceled += ThumbnailWindow_DrawRegionCanceled;
            ((ThumbnailWindowViewModel)DataContext).HandleSource = handleSource;
            ((ThumbnailWindowViewModel)DataContext).ThumbnailConfigViewModel = thumbnailConfigViewModel;

            this.Top = thumbnailConfigViewModel.Top;
            this.Left = thumbnailConfigViewModel.Left;
        }

        #endregion

        #region Events

        #endregion

        #region Properties

        #endregion

        #region Event handlers

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawDrawing(_drawingGroup);
        }

        private void ThumbnailWindow_CloseRequest(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void ThumbnailWindow_DrawRegionCanceled(object? sender, EventArgs e)
        {
            _mouseDown = false;
            UpdateRender();
        }

        private void UpdateActualSize()
        {
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

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            ((ThumbnailWindowViewModel)DataContext).Top = this.Top;
            ((ThumbnailWindowViewModel)DataContext).Left = this.Left;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mousePositionClick = e.GetPosition(this);
            _mouseDown = true;

            base.OnMouseLeftButtonDown(e);

            if (((ThumbnailWindowViewModel)DataContext).IsDragModeEnabled)
            {
                this.DragMove();
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_mouseDown)
            {
                _mouseDown = false;
                UpdateRender();

                ((ThumbnailWindowViewModel)DataContext).UpdateRegion(new Rect(_mousePositionClick, _mousePosition));
            }
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                _mouseDown = false;
                UpdateRender();

                ((ThumbnailWindowViewModel)DataContext).UpdateRegion(new Rect(_mousePositionClick, _mousePosition));
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            _mousePosition = e.GetPosition(this);
            UpdateActualSize();
            UpdateRender();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateActualSize();
        }

        #endregion

        #region Methods

        private void UpdateRender()
        {
            _drawingGroup.Children.Clear();

            if (((ThumbnailWindowViewModel)DataContext).IsRegionModeEnabled)
            {
                //double height = ((ThumbnailWindowViewModel)DataContext).ActualHeight;
                //double width = ((ThumbnailWindowViewModel)DataContext).ActualWidth;
                //_drawingGroup.Children.Add(new GeometryDrawing(null, new Pen(Brushes.Orange, 2), new RectangleGeometry(new Rect(0, 0, width, height))));

                _drawingGroup.Children.Add(new GeometryDrawing(null, new Pen(Brushes.Red, 2), new LineGeometry(new Point(0, _mousePosition.Y), new Point(ActualWidth, _mousePosition.Y))));
                _drawingGroup.Children.Add(new GeometryDrawing(null, new Pen(Brushes.Red, 2), new LineGeometry(new Point(_mousePosition.X, 0), new Point(_mousePosition.X, ActualHeight))));

                if (_mouseDown)
                {
                    _drawingGroup.Children.Add(new GeometryDrawing(null, new Pen(Brushes.Red, 2), new RectangleGeometry(new Rect(_mousePositionClick, _mousePosition))));
                }
            }
        }

        #endregion
    }
}
