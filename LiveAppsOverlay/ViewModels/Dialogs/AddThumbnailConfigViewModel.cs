using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveAppsOverlay.Entities;
using LiveAppsOverlay.ViewModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiveAppsOverlay.ViewModels.Dialogs
{
    public class AddThumbnailConfigViewModel : ObservableObject
    {
        private ThumbnailConfigViewModel _thumbnailConfigViewModel;

        #region Constructors

        public AddThumbnailConfigViewModel(Action<AddThumbnailConfigViewModel?> closeHandler, ThumbnailConfigViewModel thumbnailConfigViewModel)
        {
            _thumbnailConfigViewModel = thumbnailConfigViewModel;

            // Init View commands
            CloseCommand = new RelayCommand<AddThumbnailConfigViewModel>(closeHandler);
            SetCancelCommand = new RelayCommand(SetCancelExecute);
            SetDoneCommand = new RelayCommand(SetDoneExecute, CanSetDoneExecute);
        }

        #endregion

        #region Events

        #endregion

        #region Properties

        public ICommand CloseCommand { get; }
        public ICommand SetCancelCommand { get; }
        public ICommand SetDoneCommand { get; }

        public bool IsCanceled { get; set; } = false;

        public string Name
        {
            get => _thumbnailConfigViewModel.Name;
            set
            {
                _thumbnailConfigViewModel.Name = value;
                OnPropertyChanged(nameof(Name));
                ((RelayCommand)SetDoneCommand).NotifyCanExecuteChanged();
            }
        }

        #endregion

        #region Event handlers

        private void SetCancelExecute()
        {
            IsCanceled = true;
            CloseCommand.Execute(this);
        }

        private bool CanSetDoneExecute()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        private void SetDoneExecute()
        {
            CloseCommand.Execute(this);
        }

        #endregion

        #region Methods

        #endregion
    }
}
