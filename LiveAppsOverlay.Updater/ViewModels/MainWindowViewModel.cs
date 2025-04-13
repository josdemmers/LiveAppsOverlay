using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Messages;
using LiveAppsOverlay.Updater.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LiveAppsOverlay.Updater.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly ILogger _logger;
        private readonly IDownloadManager _downloadManager;

        private DispatcherTimer _applicationTimer = new();

        private Dictionary<string, string> _arguments = new Dictionary<string, string>();
        private int _downloadProgress;
        private long _downloadProgressBytes;
        private string _statusText = string.Empty;
        private string _windowTitle = $"LiveAppsOverlay.Updater v{Assembly.GetExecutingAssembly().GetName().Version}";

        // Start of Constructors region

        #region Constructors

        public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IDownloadManager downloadManager)
        {
            // Init services
            _downloadManager = downloadManager;
            _logger = logger;

            // Init messages
            WeakReferenceMessenger.Default.Register<DownloadCompletedMessage>(this, HandleDownloadCompletedMessage);
            WeakReferenceMessenger.Default.Register<DownloadProgressUpdatedMessage>(this, HandleDownloadProgressUpdatedMessage);
            WeakReferenceMessenger.Default.Register<ReleaseExtractedMessage>(this, HandleReleaseExtractedMessage);

            // Read command line arguments
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                for (int index = 1; index < args.Length; index += 2)
                {
                    string arg = args[index].Replace("--", "");
                    _arguments.Add(arg, args[index + 1]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Invalid arguments.");
            }

            Task.Factory.StartNew(() =>
            {
                bool valid = _arguments.TryGetValue("url", out string? url);
                if (!valid || string.IsNullOrWhiteSpace(url))
                {
                    _logger.LogWarning($"Url argument missing.");
                }
            });

            // Start timer to check if LiveAppsOverlay is closed.
            _applicationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(2500),
                IsEnabled = true
            };
            _applicationTimer.Tick += ApplicationTimerTick;
        }

        #endregion

        // Start of Events region

        #region Events

        #endregion

        // Start of Properties region

        #region Properties

        public string CurrentVersion
        {
            get
            {
                Version? version = Assembly.GetExecutingAssembly().GetName().Version;
                return version?.ToString() ?? string.Empty;
            }
        }

        public int DownloadProgress
        {
            get => _downloadProgress;
            set
            {
                SetProperty(ref _downloadProgress, value);
            }
        }

        public long DownloadProgressBytes
        {
            get => _downloadProgressBytes;
            set
            {
                SetProperty(ref _downloadProgressBytes, value);
            }
        }

        public string StatusText
        {
            get => _statusText;
            set
            {
                SetProperty(ref _statusText, value);
            }
        }

        public string WindowTitle { get => _windowTitle; set => _windowTitle = value; }

        #endregion

        // Start of Event handlers region

        #region Event handlers

        private void ApplicationTimerTick(object? sender, EventArgs e)
        {
            (sender as DispatcherTimer)?.Stop();

            Process[] proc = Process.GetProcessesByName("LiveAppsOverlay");
            if (proc.Length == 0)
            {
                // Not running
                Task.Factory.StartNew(() =>
                {
                    bool valid = _arguments.TryGetValue("url", out string? url);
                    if (valid && !string.IsNullOrWhiteSpace(url))
                    {
                        _downloadManager.DeleteReleases();
                        _downloadManager.DownloadRelease(url);
                    }
                    else
                    {
                        _logger.LogInformation($"Starting LiveAppsOverlay.exe");
                        Process.Start("LiveAppsOverlay.exe");

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            _logger.LogInformation("Closing LiveAppsOverlay.Updater.exe");
                            Application.Current.Shutdown();
                        });
                    }
                });
            }
            else
            {
                // Running
                StatusText = "Closing LiveAppsOverlay.";
                _logger.LogInformation("Closing LiveAppsOverlay.exe");

                proc[0].Kill();
                (sender as DispatcherTimer)?.Start();
            }
        }

        private void HandleDownloadProgressUpdatedMessage(object recipient, DownloadProgressUpdatedMessage message)
        {
            DownloadProgressUpdatedMessageParams downloadProgressUpdatedMessageParams = message.Value;
            if (downloadProgressUpdatedMessageParams.HttpProgress == null) return;

            DownloadProgress = downloadProgressUpdatedMessageParams.HttpProgress.Progress;
            DownloadProgressBytes = downloadProgressUpdatedMessageParams.HttpProgress.Bytes;
            StatusText = $"Downloading: {DownloadProgressBytes} ({DownloadProgress}%)";
        }

        private void HandleDownloadCompletedMessage(object recipient, DownloadCompletedMessage message)
        {
            DownloadCompletedMessageParams downloadCompletedMessageParams = message.Value;

            StatusText = $"Finished downloading: {downloadCompletedMessageParams.FileName}";
            Task.Factory.StartNew(() =>
            {
                _downloadManager.ExtractRelease(downloadCompletedMessageParams.FileName);
            });
            StatusText = $"Extracting: {downloadCompletedMessageParams.FileName}";
        }

        private void HandleReleaseExtractedMessage(object recipient, ReleaseExtractedMessage message)
        {
            StatusText = $"Extracted";

            _logger.LogInformation($"Starting LiveAppsOverlay.exe");
            Process.Start("LiveAppsOverlay.exe");

            Application.Current?.Dispatcher?.Invoke(() =>
            {
                _logger.LogInformation("Closing LiveAppsOverlay.Updater.exe");
                Application.Current.Shutdown();
            });
        }

        #endregion

        // Start of Methods region

        #region Methods

        #endregion
    }
}
