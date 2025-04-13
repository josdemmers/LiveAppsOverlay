using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Entities;
using LiveAppsOverlay.Interfaces;
using LiveAppsOverlay.Messages;
using MahApps.Metro.Controls.Dialogs;
using NHotkey;
using NHotkey.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiveAppsOverlay.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly ISettingsManager _settingsManager;

        // Start of Constructors region

        #region Constructors

        public MainWindowViewModel(ISettingsManager settingsManager)
        {
            // Init services
            _settingsManager = settingsManager;

            // Init Messages
            WeakReferenceMessenger.Default.Register<UpdateHotkeysRequestMessage>(this, HandleUpdateHotkeysRequestMessage);

            // Init View commands
            ApplicationClosingCommand = new RelayCommand(ApplicationClosingExecute);
            ApplicationLoadedCommand = new RelayCommand(ApplicationLoadedExecute);

            // Init Key bindings
            InitKeyBindings();
        }

        #endregion

        // Start of Events region

        #region Events

        #endregion

        // Start of Properties region

        #region Properties

        public ICommand ApplicationClosingCommand { get; }
        public ICommand ApplicationLoadedCommand { get; }

        #endregion

        // Start of Event handlers region

        #region Event handlers

        private void ApplicationClosingExecute()
        {
            WeakReferenceMessenger.Default.Send(new ApplicationClosingMessage(new ApplicationClosingMessageParams()));
        }

        private void ApplicationLoadedExecute()
        {
            WeakReferenceMessenger.Default.Send(new ApplicationLoadedMessage(new ApplicationLoadedMessageParams()));
        }

        private void HandleUpdateHotkeysRequestMessage(object recipient, UpdateHotkeysRequestMessage message)
        {
            UpdateHotkeysRequestMessageParams updateHotkeysRequestMessageParams = message.Value;

            InitKeyBindings();
        }

        private void HotkeyManager_HotkeyAlreadyRegistered(object? sender, HotkeyAlreadyRegisteredEventArgs hotkeyAlreadyRegisteredEventArgs)
        {
            // TODO: Add logging hotkeys
            //_logger.LogWarning($"The hotkey {hotkeyAlreadyRegisteredEventArgs.Name} is already registered by another application.");
            //_eventAggregator.GetEvent<WarningOccurredEvent>().Publish(new WarningOccurredEventParams
            //{
            //    Message = $"The hotkey \"{hotkeyAlreadyRegisteredEventArgs.Name}\" is already registered by another application."
            //});
        }

        private void ToggleEditModeKeyBindingExecute(object? sender, HotkeyEventArgs hotkeyEventArgs)
        {
            hotkeyEventArgs.Handled = true;
            WeakReferenceMessenger.Default.Send(new ToggleEditModeKeyBindingMessage(new ToggleEditModeKeyBindingMessageParams()));
        }

        #endregion

        // Start of Methods region

        #region Methods

        private void InitKeyBindings()
        {
            try
            {
                KeyBindingConfig keyBindingConfigToggleEditMode = _settingsManager.Settings.KeyBindingConfigToggleEditMode;

                HotkeyManager.HotkeyAlreadyRegistered += HotkeyManager_HotkeyAlreadyRegistered;

                KeyGesture keyGestureToggleEditMode = new KeyGesture(keyBindingConfigToggleEditMode.KeyGestureKey, keyBindingConfigToggleEditMode.KeyGestureModifier);

                if (keyBindingConfigToggleEditMode.IsEnabled)
                {
                    HotkeyManager.Current.AddOrReplace(keyBindingConfigToggleEditMode.Name, keyGestureToggleEditMode, ToggleEditModeKeyBindingExecute);
                }
                else
                {
                    HotkeyManager.Current.Remove(keyBindingConfigToggleEditMode.Name);
                }
            }
            catch (HotkeyAlreadyRegisteredException exception)
            {
                // TODO: Add logging hotkeys
                //_logger.LogError(exception, MethodBase.GetCurrentMethod()?.Name);
                //_eventAggregator.GetEvent<ErrorOccurredEvent>().Publish(new ErrorOccurredEventParams
                //{
                //    Message = $"The hotkey \"{exception.Name}\" is already registered by another application."
                //});
            }
            catch (Exception exception)
            {
                //_logger.LogError(exception, MethodBase.GetCurrentMethod()?.Name);
            }
        }

        #endregion
    }
}
