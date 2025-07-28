using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Entities;
using LiveAppsOverlay.Interfaces;
using LiveAppsOverlay.Messages;
using LiveAppsOverlay.ViewModels.Entities;
using LiveAppsOverlay.Views;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;

namespace LiveAppsOverlay.ViewModels
{
    public class ThumbnailWindowViewModel : ObservableObject
    {
        private HWND _handle = HWND.Null;
        private HWND _handleSource = HWND.Null;
        private RECT _regionSource = new RECT();
        private ThumbnailConfigViewModel _thumbnailConfigViewModel = new ThumbnailConfigViewModel(new ThumbnailConfig());
        private string _title = "ThumbnailWindow";
        private IntPtr _thumbnailHandle = IntPtr.Zero;

        #region Constructors

        public ThumbnailWindowViewModel(ISettingsManager settingsManager)
        {
            // Init View commands
            MenuCancelCommand = new RelayCommand(MenuCancelExecute);
            MenuDragModeCommand = new RelayCommand(MenuDragModeExecute);
            MenuRegionModeCommand = new RelayCommand(MenuRegionModeExecute);
            MenuResetCommand = new RelayCommand(MenuResetExecute);
            OpenConfigCommand = new RelayCommand(OpenConfigExecute);
        }

        #endregion

        #region Events

        public event EventHandler? CloseRequest;
        public event EventHandler? DrawRegionCanceled;

        #endregion

        #region Properties

        public ICommand MenuCancelCommand { get; }
        public ICommand MenuDragModeCommand { get; }
        public ICommand MenuRegionModeCommand { get; }
        public ICommand MenuResetCommand { get; }
        public ICommand OpenConfigCommand { get; }

        public double ActualHeight { get; set; }
        public double ActualHeightPixels { get; set; }
        public double ActualWidth { get; set; }
        public double ActualWidthPixels { get; set; }

        public int Height
        {
            get => ThumbnailConfigViewModel.Height;
            set
            {
                ThumbnailConfigViewModel.Height = value;
                OnPropertyChanged(nameof(Height));

                UpdateThumbnail();
            }
        }

        public HWND Handle
        {
            get => _handle;
            set
            {
                SetProperty(ref _handle, value);
                Title = $"ThumbnailWindow ({Handle})";
            }
        }

        public HWND HandleSource
        {
            get => _handleSource;
            set => SetProperty(ref _handleSource, value);
        }

        public bool IsDragModeEnabled
        {
            get => ThumbnailConfigViewModel.IsDragModeEnabled;
            set
            {
                ThumbnailConfigViewModel.IsDragModeEnabled = value;
                OnPropertyChanged(nameof(IsDragModeEnabled));
            }
        }

        public bool IsEditModeEnabled
        {
            get => ThumbnailConfigViewModel.IsEditModeEnabled;
            set
            {
                ThumbnailConfigViewModel.IsEditModeEnabled = value;
                OnPropertyChanged(nameof(IsEditModeEnabled));
            }
        }

        public bool IsRegionModeEnabled
        {
            get => ThumbnailConfigViewModel.IsRegionModeEnabled;
            set
            {
                ThumbnailConfigViewModel.IsRegionModeEnabled = value;
                OnPropertyChanged(nameof(IsRegionModeEnabled));
            }
        }

        public double Left
        {
            get => ThumbnailConfigViewModel.Left;
            set
            {
                ThumbnailConfigViewModel.Left = value;
                OnPropertyChanged(nameof(Left));
            }
        }

        public int Opacity
        {
            get => ThumbnailConfigViewModel.IsEditModeEnabled ? 150 : ThumbnailConfigViewModel.Opacity;
            set
            {
                ThumbnailConfigViewModel.Opacity = value;
                OnPropertyChanged(nameof(Opacity));

                UpdateThumbnail();
            }
        }

        public double Ratio
        {
            get
            {
                double xRatio = 1.0;
                if (!_regionSource.IsEmpty)
                {
                    xRatio = (double)_regionSource.Width / (double)_regionSource.Height;
                }
                return xRatio;
            }
        }

        public ThumbnailConfigViewModel ThumbnailConfigViewModel
        {
            get => _thumbnailConfigViewModel;
            set
            {
                _thumbnailConfigViewModel = value;
                _thumbnailConfigViewModel.PropertyChanged += ThumbnailConfigViewModel_PropertyChanged;

                _regionSource = new RECT(
                    _thumbnailConfigViewModel.RegionSource.Left,
                    _thumbnailConfigViewModel.RegionSource.Top,
                    _thumbnailConfigViewModel.RegionSource.Right,
                    _thumbnailConfigViewModel.RegionSource.Bottom);
            }
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public double Top
        {
            get => ThumbnailConfigViewModel.Top;
            set
            {
                ThumbnailConfigViewModel.Top = value;
                OnPropertyChanged(nameof(Top));
            }
        }

        public int Width
        {
            get => ThumbnailConfigViewModel.Width;
            set
            {
                ThumbnailConfigViewModel.Width = value;
                OnPropertyChanged(nameof(Width));

                UpdateThumbnail();
            }
        }

        #endregion

        #region Event handlers

        private void MenuCancelExecute()
        {
            DrawRegionCanceled?.Invoke(this, EventArgs.Empty);
        }

        private void MenuDragModeExecute()
        {
            IsDragModeEnabled = true;
            IsRegionModeEnabled = false;
        }

        private void MenuRegionModeExecute()
        {
            IsDragModeEnabled = false;
            IsRegionModeEnabled = true;
        }

        private void MenuResetExecute()
        {
            ResetRegion();

            Height = 450;
            Left = 0;
            Top = 0;
            Width = 800;
        }

        private void OpenConfigExecute()
        {
            WeakReferenceMessenger.Default.Send(new ThumbnailWindowOpenConfigMessage(new ThumbnailWindowOpenConfigMessageParams { ThumbnailConfigViewModel = ThumbnailConfigViewModel }));
        }

        private void ThumbnailConfigViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs eventArgs)
        {
            if (!ThumbnailConfigViewModel.IsActive)
            {
                UnRegisterThumbnail();
                _thumbnailConfigViewModel.PropertyChanged -= ThumbnailConfigViewModel_PropertyChanged;
                CloseRequest?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // Specific properties
                if (eventArgs.PropertyName?.Equals(nameof(Opacity)) ?? false)
                {
                    OnPropertyChanged(nameof(Opacity));
                    UpdateThumbnail();
                }
                else if (eventArgs.PropertyName?.Equals(nameof(IsEditModeEnabled)) ?? false)
                {
                    WeakReferenceMessenger.Default.Send(new ThumbnailWindowEditModeChangedMessage(new ThumbnailWindowEditModeChangedMessageParams 
                    {
                        HandleSource = HandleSource,
                        ThumbnailConfigViewModel = ThumbnailConfigViewModel
                    }));
                }

                // All properties
                WeakReferenceMessenger.Default.Send(new ThumbnailWindowChangedMessage(new ThumbnailWindowChangedMessageParams
                {
                    DisplayName = ThumbnailConfigViewModel.AppNameDisplay,
                    HandleSource = HandleSource
                }));
            }
        }

        #endregion

        #region Methods

        public void Init()
        {
            RegisterThumbnail(HandleSource);
        }

        //unsafe static RECT GetExtendedFrameBounds(HWND hwnd)
        //{
        //    // Note: Returns incorrect data when Windows scaling is changed while source app is running.

        //    // https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmgetwindowattribute
        //    RECT rect = default;
        //    PInvoke.DwmGetWindowAttribute(hwnd, DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, &rect, (uint)sizeof(RECT)).ThrowOnFailure();
        //    return rect;
        //}

        private System.Drawing.Size? GetSourceSize()
        {
            Debug.WriteLine($"GetSourceSize");

            // https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmquerythumbnailsourcesize
            HRESULT result = PInvoke.DwmQueryThumbnailSourceSize(_thumbnailHandle, out var sourceSize);
            if (result.Failed) return null;

            return new System.Drawing.Size(sourceSize.Width, sourceSize.Height);
        }

        private void RegisterThumbnail(HWND handleSource)
        {
            // https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmregisterthumbnail
            HRESULT result = PInvoke.DwmRegisterThumbnail(Handle, handleSource, out _thumbnailHandle);
            if (result.Failed) return;

            bool isRegionNotSet = _regionSource.IsEmpty;

            UpdateThumbnail();

            // Update DwmUpdateThumbnailProperties again to apply fSourceClientAreaOnly
            System.Drawing.Point sourcePoint = new System.Drawing.Point(0, 0);
            System.Drawing.Size? sourceSize = GetSourceSize();
            if (sourceSize == null) return;

            if (isRegionNotSet)
            {
                _regionSource = new RECT(sourcePoint, sourceSize.Value);
                ThumbnailConfigViewModel.RegionSource = new RegionSourceRECT
                {
                    Left = _regionSource.left,
                    Top = _regionSource.top,
                    Right = _regionSource.right,
                    Bottom = _regionSource.bottom
                };
            }

            SetThumbnailProperties(0, 0, (int)ActualWidthPixels, (int)ActualHeightPixels);
        }

        private void ResetRegion()
        {
            System.Drawing.Point sourcePoint = new System.Drawing.Point(0, 0);
            System.Drawing.Size? sourceSize = GetSourceSize();
            if (sourceSize == null) return;
            _regionSource = new RECT(sourcePoint, sourceSize.Value);
            ThumbnailConfigViewModel.RegionSource = new RegionSourceRECT
            {
                Left = _regionSource.left,
                Top = _regionSource.top,
                Right = _regionSource.right,
                Bottom = _regionSource.bottom
            };
        }

        private void SetThumbnailProperties(int left, int top, int right, int bottom)
        {
            // https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmupdatethumbnailproperties
            // https://learn.microsoft.com/en-us/windows/win32/dwm/dwm-tnp-constants
            DWM_THUMBNAIL_PROPERTIES DwmThumbnailProperties = new DWM_THUMBNAIL_PROPERTIES()
            {
                dwFlags = PInvoke.DWM_TNP_RECTDESTINATION | PInvoke.DWM_TNP_RECTSOURCE | PInvoke.DWM_TNP_OPACITY | PInvoke.DWM_TNP_VISIBLE | PInvoke.DWM_TNP_SOURCECLIENTAREAONLY,
                fSourceClientAreaOnly = true,
                fVisible = true,
                opacity = (byte)Opacity,
                rcDestination = new RECT
                {
                    left = left,
                    top = top,
                    right = right,
                    bottom = bottom,
                },
                rcSource = _regionSource
            };

            // https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmupdatethumbnailproperties
            HRESULT result = PInvoke.DwmUpdateThumbnailProperties(_thumbnailHandle, DwmThumbnailProperties);
            if (result.Failed) return;
        }

        private void UnRegisterThumbnail()
        {
            if (_thumbnailHandle == IntPtr.Zero) return;

            // https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmunregisterthumbnail
            HRESULT result = PInvoke.DwmUnregisterThumbnail(_thumbnailHandle);
            if (result.Failed) return;
        }

        public void UpdateRegion(Rect rect)
        {
            if (IsDragModeEnabled || !IsRegionModeEnabled) return;
            if (rect.Size.Width == 0 || rect.Size.Height == 0) return;

            // Convert region to percentages
            double leftPercentage = rect.Left / ActualWidth;
            double topPercentage = rect.Top / ActualHeight;
            double rightPercentage = rect.Right / ActualWidth;
            double bottomPercentage = rect.Bottom / ActualHeight;

            Debug.WriteLine($"{leftPercentage},{topPercentage},{rightPercentage},{bottomPercentage}");

            // Create source region in pixels
            System.Drawing.Point sourcePoint = new System.Drawing.Point(0, 0);
            System.Drawing.Size? sourceSize = GetSourceSize();
            if (sourceSize == null) return;
            RECT windowSource = new RECT(sourcePoint, sourceSize.Value);

            int leftRegion = (int)(leftPercentage * _regionSource.Width) + _regionSource.left;
            int topRegion = (int)(topPercentage * _regionSource.Height) + _regionSource.top;
            int rightRegion = (int)(rightPercentage * _regionSource.Width) + _regionSource.left;
            int bottomRegion = (int)(bottomPercentage * _regionSource.Height) + _regionSource.top;

            Debug.WriteLine($"{_regionSource.left},{_regionSource.top},{_regionSource.right},{_regionSource.bottom}");

            _regionSource = new RECT(leftRegion, topRegion, rightRegion, bottomRegion);
            ThumbnailConfigViewModel.RegionSource = new RegionSourceRECT
            {
                Left = leftRegion,
                Top = topRegion,
                Right = rightRegion,
                Bottom = bottomRegion
            };

            Debug.WriteLine($"{_regionSource.left},{_regionSource.top},{_regionSource.right},{_regionSource.bottom}");

            UpdateThumbnail();
        }

        private void UpdateThumbnail()
        {
            if (_thumbnailHandle == IntPtr.Zero) return;

            //RECT rectDestination = GetExtendedFrameBounds(Handle);
            System.Drawing.Point sourcePoint = new System.Drawing.Point(0, 0);
            System.Drawing.Size? sourceSize = GetSourceSize();
            if (sourceSize == null) return;

            if (_regionSource.IsEmpty)
            {
                _regionSource = new RECT(sourcePoint, sourceSize.Value);
                ThumbnailConfigViewModel.RegionSource = new RegionSourceRECT
                {
                    Left = _regionSource.left,
                    Top = _regionSource.top,
                    Right = _regionSource.right,
                    Bottom = _regionSource.bottom
                };
            }

            //double heightFactor = (double)_regionSource.Height / (double)rectDestination.Height;
            //double widthFactor = (double)_regionSource.Width / (double)rectDestination.Width;
            //double resizeFactor = Math.Max(widthFactor, heightFactor);
            //int height = (int)(_regionSource.Height / resizeFactor);
            //int width = (int)(_regionSource.Width / resizeFactor);
            //height = (int)(_regionSource.Height / heightFactor);
            //width = (int)(_regionSource.Width / widthFactor);
            //width = rectDestination.Width;
            //height = rectDestination.Height;
            //width = (int)ActualWidthPixels;
            //height = (int)ActualHeightPixels;

            SetThumbnailProperties(0, 0, (int)ActualWidthPixels, (int)ActualHeightPixels);

            Debug.WriteLine($"UpdateThumbnail");
            Debug.WriteLine($"- Window: {Width}x{Height}");
            Debug.WriteLine($"- Window(Render): {ActualWidth}x{ActualHeight}");
            Debug.WriteLine($"- Window(Render pixels): {ActualWidthPixels}x{ActualHeightPixels}");
            Debug.WriteLine($"- Window(Source pixels): {sourceSize?.Width}x{sourceSize?.Height}");
        }

        #endregion
    }
}
