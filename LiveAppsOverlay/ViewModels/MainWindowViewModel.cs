using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Entities;
using LiveAppsOverlay.Interfaces;
using LiveAppsOverlay.Messages;
using LiveAppsOverlay.Services;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Logging;
using NHotkey;
using NHotkey.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LiveAppsOverlay.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly ILogger _logger;
        private readonly IReleaseManager _releaseManager;
        private readonly ISettingsManager _settingsManager;

        private string _windowTitle = $"Live Apps Overlay v{Assembly.GetExecutingAssembly().GetName().Version}";

        // Start of Constructors region

        #region Constructors

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator, ILogger<MainWindowViewModel> logger, IReleaseManager releaseManager, ISettingsManager settingsManager)
        {
            // Init services
            _dialogCoordinator = dialogCoordinator;
            _logger = logger;
            _releaseManager = releaseManager;
            _settingsManager = settingsManager;

            // Init Messages
            WeakReferenceMessenger.Default.Register<ReleaseInfoUpdatedMessage>(this, HandleReleaseInfoUpdatedMessage);
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

        public string WindowTitle
        {
            get => _windowTitle;
            set => SetProperty(ref _windowTitle, value);
        }

        #endregion

        // Start of Event handlers region

        #region Event handlers

        private void ApplicationClosingExecute()
        {
            WeakReferenceMessenger.Default.Send(new ApplicationClosingMessage(new ApplicationClosingMessageParams()));
        }

        private void ApplicationLoadedExecute()
        {
            _logger.LogInformation(WindowTitle);

            WeakReferenceMessenger.Default.Send(new ApplicationLoadedMessage(new ApplicationLoadedMessageParams()));
        }


        private void HandleReleaseInfoUpdatedMessage(object recipient, ReleaseInfoUpdatedMessage message)
        {
            var current = Assembly.GetExecutingAssembly().GetName().Version;
            var releases = new List<Release>();
            releases.AddRange(_releaseManager.Releases);

            var release = releases.FirstOrDefault();
            if (release != null)
            {
                var latest = Version.Parse(release.Version[1..]);

                if (latest > current)
                {
                    _releaseManager.UpdateAvailable = true;
                    WindowTitle = $"Live Apps Overlay v{Assembly.GetExecutingAssembly().GetName().Version} ({release.Version} available)";

                    //TODO: Add logging new version
                    //_eventAggregator.GetEvent<InfoOccurredEvent>().Publish(new InfoOccurredEventParams
                    //{
                    //    Message = $"New version available: {release.Version}"
                    //});

                    // Open update dialog
                    if (File.Exists("LiveAppsOverlay.Updater.exe"))
                    {
                        _dialogCoordinator.ShowMessageAsync(this, $"Update", $"New version available, do you want to download {release.Version}?", MessageDialogStyle.AffirmativeAndNegative).ContinueWith(t =>
                        {
                            if (t.Result == MessageDialogResult.Affirmative)
                            {
                                string url = release.Assets.FirstOrDefault(a => a.ContentType.Equals("application/x-zip-compressed"))?.BrowserDownloadUrl ?? string.Empty;
                                if (!string.IsNullOrWhiteSpace(url))
                                {
                                    _logger.LogInformation($"Starting LiveAppsOverlay.Updater.exe. Launch arguments: --url \"{url}\"");
                                    Process.Start("LiveAppsOverlay.Updater.exe", $"--url \"{url}\"");

                                    Application.Current?.Dispatcher?.Invoke(() =>
                                    {
                                        _logger.LogInformation("Closing LiveAppsOverlay.exe");
                                        Application.Current.Shutdown();
                                    });
                                }
                            }
                            else
                            {
                                _logger.LogInformation($"Update process canceled by user.");
                            }
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Cannot update application, LiveAppsOverlay.Updater.exe not available.");
                    }
                }
            }
            else
            {
                _logger.LogInformation("No new version available.");
            }
        }

        private void HandleUpdateHotkeysRequestMessage(object recipient, UpdateHotkeysRequestMessage message)
        {
            UpdateHotkeysRequestMessageParams updateHotkeysRequestMessageParams = message.Value;

            InitKeyBindings();
        }

        private void HotkeyManager_HotkeyAlreadyRegistered(object? sender, HotkeyAlreadyRegisteredEventArgs hotkeyAlreadyRegisteredEventArgs)
        {
            _logger.LogWarning($"The hotkey {hotkeyAlreadyRegisteredEventArgs.Name} is already registered by another application.");
            // TODO: Add logging hotkeys
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
                _logger.LogError(exception, MethodBase.GetCurrentMethod()?.Name);
                // TODO: Add logging hotkeys
                //_eventAggregator.GetEvent<ErrorOccurredEvent>().Publish(new ErrorOccurredEventParams
                //{
                //    Message = $"The hotkey \"{exception.Name}\" is already registered by another application."
                //});
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, MethodBase.GetCurrentMethod()?.Name);
            }
        }

        #endregion
    }
}
