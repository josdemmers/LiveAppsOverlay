using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Entities;
using LiveAppsOverlay.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32.Foundation;

namespace LiveAppsOverlay.ViewModels.Entities
{
    public class ThumbnailConfigBaseViewModel : ObservableObject
    {
    }

    public class ThumbnailConfigAddViewModel : ThumbnailConfigBaseViewModel
    {
    }

    public class ThumbnailConfigViewModel : ThumbnailConfigBaseViewModel
    {
        private string _appName = string.Empty;
        private bool _isDragModeEnabled = true;
        private bool _isRegionModeEnabled = false;
        private ThumbnailConfig _thumbnailConfig = new ThumbnailConfig();


        #region Constructors

        public ThumbnailConfigViewModel(ThumbnailConfig thumbnailConfig)
        {
            _thumbnailConfig = thumbnailConfig;
        }

        #endregion

        #region Events

        #endregion

        #region Properties

        public string AppName
        {
            get => _appName;
            set
            {
                _appName = value;
                SetProperty(ref _appName, value);
            }
        }

        public int Height
        {
            get => Model.Height;
            set
            {
                Model.Height = value;
                OnPropertyChanged(nameof(Height));

                WeakReferenceMessenger.Default.Send(new ThumbnailConfigModifiedMessage(new ThumbnailConfigModifiedMessageParams()));
            }
        }

        public bool IsActive
        {
            get => Model.IsActive;
            set
            {
                Model.IsActive = value;
                OnPropertyChanged(nameof(IsActive));

                WeakReferenceMessenger.Default.Send(new ThumbnailConfigModifiedMessage(new ThumbnailConfigModifiedMessageParams()));
            }
        }

        public bool IsDragModeEnabled
        {
            get => _isDragModeEnabled;
            set
            {
                SetProperty(ref _isDragModeEnabled, value);
                _isRegionModeEnabled = !_isDragModeEnabled;
                OnPropertyChanged(nameof(IsRegionModeEnabled));
            }
        }

        public bool IsEnabled
        {
            get => Model.IsEnabled;
            set
            {
                Model.IsEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));

                WeakReferenceMessenger.Default.Send(new ThumbnailConfigModifiedMessage(new ThumbnailConfigModifiedMessageParams()));
            }
        }

        public bool IsRegionModeEnabled
        {
            get => _isRegionModeEnabled;
            set
            {
                SetProperty(ref _isRegionModeEnabled, value);
                _isDragModeEnabled = !_isRegionModeEnabled;
                OnPropertyChanged(nameof(IsDragModeEnabled));
            }
        }

        public double Left
        {
            get => Model.Left;
            set
            {
                Model.Left = value;
                OnPropertyChanged(nameof(Left));

                WeakReferenceMessenger.Default.Send(new ThumbnailConfigModifiedMessage(new ThumbnailConfigModifiedMessageParams()));
            }
        }

        public ThumbnailConfig Model
        {
            get => _thumbnailConfig;
        }

        public string Name
        {
            get => Model.Name;
            set
            {
                Model.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public int Opacity
        {
            get => Model.Opacity;
            set
            {
                Model.Opacity = value;
                OnPropertyChanged(nameof(Opacity));

                WeakReferenceMessenger.Default.Send(new ThumbnailConfigModifiedMessage(new ThumbnailConfigModifiedMessageParams()));
            }
        }

        public RegionSourceRECT RegionSource
        {
            get => Model.RegionSource;
            set
            {
                Model.RegionSource = value;
                OnPropertyChanged(nameof(RegionSource));

                WeakReferenceMessenger.Default.Send(new ThumbnailConfigModifiedMessage(new ThumbnailConfigModifiedMessageParams()));
            }
        }

        public double Top
        {
            get => Model.Top;
            set
            {
                Model.Top = value;
                OnPropertyChanged(nameof(Top));

                WeakReferenceMessenger.Default.Send(new ThumbnailConfigModifiedMessage(new ThumbnailConfigModifiedMessageParams()));
            }
        }

        public int Width
        {
            get => Model.Width;
            set
            {
                Model.Width = value;
                OnPropertyChanged(nameof(Width));

                WeakReferenceMessenger.Default.Send(new ThumbnailConfigModifiedMessage(new ThumbnailConfigModifiedMessageParams()));
            }
        }

        #endregion

        #region Event handlers

        #endregion

        #region Methods

        #endregion
    }

    public class ThumbnailConfigCustomSort : IComparer
    {
        public int Compare(object? x, object? y)
        {
            int result = -1;

            if ((x.GetType() == typeof(ThumbnailConfigAddViewModel)) && !(y.GetType() == typeof(ThumbnailConfigAddViewModel))) return -1;
            if ((y.GetType() == typeof(ThumbnailConfigAddViewModel)) && !(x.GetType() == typeof(ThumbnailConfigAddViewModel))) return 1;

            if ((x.GetType() == typeof(ThumbnailConfigViewModel)) && (y.GetType() == typeof(ThumbnailConfigViewModel)))
            {
                var itemX = (ThumbnailConfigViewModel)x;
                var itemY = (ThumbnailConfigViewModel)y;

                result = itemX.Name.CompareTo(itemY.Name);
            }

            return result;
        }
    }
}
