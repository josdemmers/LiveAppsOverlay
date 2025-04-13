using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Entities;
using LiveAppsOverlay.Interfaces;
using LiveAppsOverlay.Localization;
using LiveAppsOverlay.Messages;
using LiveAppsOverlay.ViewModels.Dialogs;
using LiveAppsOverlay.Views.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiveAppsOverlay.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly ISettingsManager _settingsManager;

        private ObservableCollection<AppLanguage> _appLanguages = new ObservableCollection<AppLanguage>();

        private AppLanguage _selectedAppLanguage = new AppLanguage();

        #region Constructors

        public SettingsViewModel(IDialogCoordinator dialogCoordinator, ISettingsManager settingsManager)
        {
            // Init services
            _dialogCoordinator = dialogCoordinator;
            _settingsManager = settingsManager;
            _settingsManager.SettingsChanged += SettingsManager_SettingsChanged;

            // Init Messages
            WeakReferenceMessenger.Default.Register<ToggleEditModeKeyBindingMessage>(this, HandleToggleEditModeKeyBindingMessage);

            // Init View commands
            SetHotkeysCommand = new RelayCommand(SetHotkeysExecute);

            InitApplanguages();
        }

        #endregion

        #region Events

        #endregion

        #region Properties
        public ICommand SetHotkeysCommand { get; }

        public ObservableCollection<AppLanguage> AppLanguages { get => _appLanguages; set => _appLanguages = value; }

        public bool IsCheckForUpdatesEnabled
        {
            get => _settingsManager.Settings.IsCheckForUpdatesEnabled;
            set
            {
                _settingsManager.Settings.IsCheckForUpdatesEnabled = value;
                OnPropertyChanged(nameof(IsCheckForUpdatesEnabled));

                _settingsManager.SaveSettings();
            }
        }

        public bool IsEditModeEnabled
        {
            get => _settingsManager.Settings.IsEditModeEnabled;
            set
            {
                _settingsManager.Settings.IsEditModeEnabled = value;
                OnPropertyChanged(nameof(IsEditModeEnabled));

                _settingsManager.SaveSettings();

                WeakReferenceMessenger.Default.Send(new ThumbnailEditModeChangedMessage(new ThumbnailEditModeChangedMessageParams()));
            }
        }

        public AppLanguage SelectedAppLanguage
        {
            get => _selectedAppLanguage;
            set
            {
                _selectedAppLanguage = value;
                OnPropertyChanged(nameof(SelectedAppLanguage));
                if (value != null)
                {
                    _settingsManager.Settings.SelectedAppLanguage = value.Id;
                    _settingsManager.SaveSettings();

                    TranslationSource.Instance.CurrentCulture = new System.Globalization.CultureInfo(SelectedAppLanguage.Id);
                }
            }
        }

        #endregion

        #region Event handlers

        private void HandleToggleEditModeKeyBindingMessage(object recipient, ToggleEditModeKeyBindingMessage message)
        {
            ToggleEditModeKeyBindingMessageParams toggleEditModeKeyBindingMessageParams = message.Value;

            IsEditModeEnabled = !_settingsManager.Settings.IsEditModeEnabled;
        }

        private async void SetHotkeysExecute()
        {
            var hotkeysConfigDialog = new CustomDialog() { Title = "Hotkeys config" };
            var dataContext = new HotkeysConfigViewModel(async instance =>
            {
                await hotkeysConfigDialog.WaitUntilUnloadedAsync();
            });
            hotkeysConfigDialog.Content = new HotkeysConfigView() { DataContext = dataContext };
            await _dialogCoordinator.ShowMetroDialogAsync(this, hotkeysConfigDialog);
            await hotkeysConfigDialog.WaitUntilUnloadedAsync();
        }

        private void SettingsManager_SettingsChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsEditModeEnabled));
        }

        #endregion

        #region Methods

        private void InitApplanguages()
        {
            _appLanguages.Clear();
            _appLanguages.Add(new AppLanguage("en-US", "English"));

            var language = _appLanguages.FirstOrDefault(language => language.Id.Equals(_settingsManager.Settings.SelectedAppLanguage));
            if (language != null)
            {
                SelectedAppLanguage = language;
            }
        }

        #endregion
    }
}
