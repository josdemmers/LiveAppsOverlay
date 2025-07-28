using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Entities;
using LiveAppsOverlay.Extensions;
using LiveAppsOverlay.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LiveAppsOverlay.ViewModels.Entities
{
    public class FavoriteProcessEntryViewModel : ObservableObject
    {
        private ObservableCollection<ThumbnailConfigBaseViewModel> _thumbnailConfigs = new ObservableCollection<ThumbnailConfigBaseViewModel>();

        private FavoriteProcessEntry _favoriteProcessEntry = new FavoriteProcessEntry();

        private nint _handle = 0;
        private bool _isSelected = false;

        #region Constructors

        public FavoriteProcessEntryViewModel(FavoriteProcessEntry favoriteProcessEntry)
        {
            // Init Messages
            WeakReferenceMessenger.Default.Register<ThumbnailConfigModifiedMessage>(this, HandleThumbnailConfigModifiedMessage);

            _favoriteProcessEntry = favoriteProcessEntry;
            _thumbnailConfigs.Add(new ThumbnailConfigAddViewModel());
            _thumbnailConfigs.AddRange(_favoriteProcessEntry.ThumbnailConfigs.Select(t => new ThumbnailConfigViewModel(t)));

            CreateThumbnailConfigSortedView();
        }

        #endregion

        #region Events

        #endregion

        #region Properties

        public ObservableCollection<ThumbnailConfigBaseViewModel> ThumbnailConfigs { get => _thumbnailConfigs; set => _thumbnailConfigs = value; }
        public ListCollectionView? ThumbnailConfigsSorted { get; private set; }

        public string DisplayName
        {
            get => Model.DisplayName;
            set
            {
                Model.DisplayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public nint Handle
        {
            get => _handle;
            set
            {
                SetProperty(ref _handle, value);
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        public bool IsRunning
        {
            get => Handle != 0;
        }

        public bool IsAnyThumbnailActive
        {
            get => _thumbnailConfigs.OfType<ThumbnailConfigViewModel>().Any(t => t.IsActive);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                SetProperty(ref _isSelected, value);
            }
        }

        public FavoriteProcessEntry Model
        {
            get => _favoriteProcessEntry;
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

        #endregion

        #region Event handlers

        private void HandleThumbnailConfigModifiedMessage(object recipient, ThumbnailConfigModifiedMessage message)
        {
            OnPropertyChanged(nameof(IsAnyThumbnailActive));
        }

        #endregion

        #region Methods

        public void AddThumbnailConfig(ThumbnailConfigViewModel thumbnailConfigViewModel)
        {
            _thumbnailConfigs.Add(thumbnailConfigViewModel);
            Model.ThumbnailConfigs.Add(thumbnailConfigViewModel.Model);
        }

        private void CreateThumbnailConfigSortedView()
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                ThumbnailConfigsSorted = new ListCollectionView(ThumbnailConfigs)
                {
                    Filter = FilterThumbnailConfigs
                };

                ThumbnailConfigsSorted.CustomSort = new ThumbnailConfigCustomSort();
            });
        }

        private bool FilterThumbnailConfigs(object thumbnailConfigObj)
        {
            if (thumbnailConfigObj == null) return false;
            if (thumbnailConfigObj.GetType() == typeof(ThumbnailConfigAddViewModel)) return true;

            return true;
        }

        public void RemoveTumbnailConfig(ThumbnailConfigViewModel? thumbnailConfigViewModel)
        {
            if (thumbnailConfigViewModel == null) return;

            _thumbnailConfigs.Remove(thumbnailConfigViewModel);
            Model.ThumbnailConfigs.Remove(thumbnailConfigViewModel.Model);
        }

        #endregion
    }
}
