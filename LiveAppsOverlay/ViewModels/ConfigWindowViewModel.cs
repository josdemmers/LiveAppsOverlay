using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Entities;
using LiveAppsOverlay.Interfaces;
using LiveAppsOverlay.Messages;
using LiveAppsOverlay.ViewModels.Entities;
using System.Windows.Input;

namespace LiveAppsOverlay.ViewModels
{
    public class ConfigWindowViewModel : ObservableObject
    {
        private ThumbnailConfigViewModel _thumbnailConfigViewModel = new ThumbnailConfigViewModel(new ThumbnailConfig());
        private string _title = "Config";

        #region Constructors

        public ConfigWindowViewModel()
        {
            // Init View commands
            WindowClosingCommand = new RelayCommand(WindowClosingExecute);
        }

        #endregion

        #region Events

        #endregion

        #region Properties

        public bool IsDragModeEnabled
        {
            get => ThumbnailConfigViewModel.IsDragModeEnabled;
            set
            {
                ThumbnailConfigViewModel.IsDragModeEnabled = value;
                OnPropertyChanged(nameof(IsDragModeEnabled));
            }
        }

        public bool IsDragModeButtonEnabled => IsEditModeEnabled;

        public bool IsEditModeEnabled
        {
            get => ThumbnailConfigViewModel.IsEditModeEnabled;
            set
            {
                ThumbnailConfigViewModel.IsEditModeEnabled = value;

                OnPropertyChanged(nameof(IsEditModeEnabled));
                OnPropertyChanged(nameof(IsDragModeButtonEnabled));
                OnPropertyChanged(nameof(IsLockAspectRatioButtonEnabled));
                OnPropertyChanged(nameof(IsOpacityEditEnabled));
                OnPropertyChanged(nameof(IsRegionModeButtonEnabled));
            }
        }

        public bool IsLockAspectRatioEnabled
        {
            get => ThumbnailConfigViewModel.IsAspectRatioLocked;
            set
            {
                ThumbnailConfigViewModel.IsAspectRatioLocked = value;
                OnPropertyChanged(nameof(IsLockAspectRatioEnabled));
            }
        }

        public bool IsLockAspectRatioButtonEnabled => IsEditModeEnabled;

        public bool IsOpacityEditEnabled => !IsEditModeEnabled;

        public bool IsRegionModeEnabled
        {
            get => ThumbnailConfigViewModel.IsRegionModeEnabled;
            set
            {
                ThumbnailConfigViewModel.IsRegionModeEnabled = value;
                OnPropertyChanged(nameof(IsRegionModeEnabled));
            }
        }

        public bool IsRegionModeButtonEnabled => IsEditModeEnabled;

        public ThumbnailConfigViewModel ThumbnailConfigViewModel
        {
            get => _thumbnailConfigViewModel;
            set
            {
                _thumbnailConfigViewModel = value;
                _thumbnailConfigViewModel.PropertyChanged += ThumbnailConfigViewModel_PropertyChanged;

                OnPropertyChanged(nameof(IsDragModeEnabled));
                OnPropertyChanged(nameof(IsLockAspectRatioButtonEnabled));
                OnPropertyChanged(nameof(IsLockAspectRatioEnabled));
                OnPropertyChanged(nameof(IsRegionModeEnabled));
                OnPropertyChanged(nameof(Opacity));
                OnPropertyChanged(nameof(Title));
            }
        }

        public int Opacity
        {
            get => ThumbnailConfigViewModel.Opacity;
            set
            {
                ThumbnailConfigViewModel.Opacity = value;
                OnPropertyChanged(nameof(Opacity));
            }
        }

        public string Title
        {
            get
            {
                string title = "Config";
                title = string.IsNullOrWhiteSpace(ThumbnailConfigViewModel.AppNameDisplay) ? title : ThumbnailConfigViewModel.AppNameDisplay;
                title = string.IsNullOrWhiteSpace(ThumbnailConfigViewModel.Name) ? title : $"{title} - {ThumbnailConfigViewModel.Name}";
                return title;
            }

            set => SetProperty(ref _title, value);
        }

        public ICommand WindowClosingCommand { get; }

        #endregion

        #region Event handlers

        private void ThumbnailConfigViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsDragModeEnabled));
            OnPropertyChanged(nameof(IsLockAspectRatioEnabled));
            OnPropertyChanged(nameof(IsRegionModeEnabled));
        }

        private void WindowClosingExecute()
        {
            _thumbnailConfigViewModel.PropertyChanged -= ThumbnailConfigViewModel_PropertyChanged;
        }

        #endregion

        #region Methods

        #endregion
    }
}
