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
    public class UpdateThumbnailConfigViewModel : ObservableObject
    {
        private ThumbnailConfigViewModel _thumbnailConfigViewModel;

        #region Constructors

        public UpdateThumbnailConfigViewModel(Action<UpdateThumbnailConfigViewModel?> closeHandler, ThumbnailConfigViewModel thumbnailConfigViewModel)
        {
            _thumbnailConfigViewModel = thumbnailConfigViewModel;

            // Init View commands
            CloseCommand = new RelayCommand<UpdateThumbnailConfigViewModel>(closeHandler);
            SetDoneCommand = new RelayCommand(SetDoneExecute, CanSetDoneExecute);
        }

        #endregion

        #region Events

        #endregion

        #region Properties

        public ICommand CloseCommand { get; }
        public ICommand SetDoneCommand { get; }

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
