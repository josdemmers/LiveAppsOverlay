using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Entities;
using LiveAppsOverlay.Interfaces;
using LiveAppsOverlay.Messages;
using LiveAppsOverlay.Views.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LiveAppsOverlay.ViewModels.Dialogs
{
    public class HotkeysConfigViewModel : ObservableObject
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly ISettingsManager _settingsManager;

        // Start of Constructors region

        #region Constructors

        public HotkeysConfigViewModel(Action<HotkeysConfigViewModel?> closeHandler)
        {
            // Init services
            _dialogCoordinator = App.Current.Services.GetRequiredService<IDialogCoordinator>();
            _settingsManager = App.Current.Services.GetRequiredService<ISettingsManager>();

            // Init View commands
            CloseCommand = new RelayCommand<HotkeysConfigViewModel>(closeHandler);
            KeyBindingConfigToggleEditModeCommand = new RelayCommand<object>(KeyBindingConfigExecute);
            SetDoneCommand = new RelayCommand(SetDoneExecute);
            ToggleKeybindingEditModeCommand = new RelayCommand(ToggleKeybindingExecute);
        }

        #endregion

        // Start of Events region

        #region Events

        #endregion

        // Start of Properties region

        #region Properties

        public ICommand CloseCommand { get; }
        public ICommand KeyBindingConfigToggleEditModeCommand { get; }
        public ICommand SetDoneCommand { get; }
        public ICommand ToggleKeybindingEditModeCommand { get; }

        public KeyBindingConfig KeyBindingConfigToggleEditMode
        {
            get => _settingsManager.Settings.KeyBindingConfigToggleEditMode;
            set
            {
                if (value != null)
                {
                    _settingsManager.Settings.KeyBindingConfigToggleEditMode = value;
                    OnPropertyChanged(nameof(KeyBindingConfigToggleEditMode));

                    _settingsManager.SaveSettings();
                }
            }
        }

        #endregion

        // Start of Event handlers region

        #region Event handlers

        private void HotkeysConfigDoneExecute()
        {
            CloseCommand.Execute(this);
        }

        private async void KeyBindingConfigExecute(object? obj)
        {
            if (obj == null) return;

            var hotkeyConfigDialog = new CustomDialog() { Title = "Hotkey config" };
            var dataContext = new HotkeyConfigViewModel(async instance =>
            {
                await hotkeyConfigDialog.WaitUntilUnloadedAsync();
            }, (KeyBindingConfig)obj);
            hotkeyConfigDialog.Content = new HotkeyConfigView() { DataContext = dataContext };
            await _dialogCoordinator.ShowMetroDialogAsync(this, hotkeyConfigDialog);
            await hotkeyConfigDialog.WaitUntilUnloadedAsync();

            _settingsManager.SaveSettings();
            OnPropertyChanged(nameof(KeyBindingConfigToggleEditMode));

            UpdateHotkeys();
        }

        private void SetDoneExecute()
        {
            CloseCommand.Execute(this);
        }


        private void ToggleKeybindingExecute()
        {
            _settingsManager.SaveSettings();
            UpdateHotkeys();
        }


        #endregion

        // Start of Methods region

        #region Methods

        private void UpdateHotkeys()
        {
            WeakReferenceMessenger.Default.Send(new UpdateHotkeysRequestMessage(new UpdateHotkeysRequestMessageParams()));
        }

        #endregion
    }
}
