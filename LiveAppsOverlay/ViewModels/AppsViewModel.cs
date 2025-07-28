using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Entities;
using LiveAppsOverlay.Extensions;
using LiveAppsOverlay.Interfaces;
using LiveAppsOverlay.Messages;
using LiveAppsOverlay.ViewModels.Dialogs;
using LiveAppsOverlay.ViewModels.Entities;
using LiveAppsOverlay.Views;
using LiveAppsOverlay.Views.Dialogs;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Win32.Foundation;

namespace LiveAppsOverlay.ViewModels
{
    public class AppsViewModel : ObservableObject
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly ILogger _logger;
        private readonly ISettingsManager _settingsManager;
        private readonly IThumbnailManager _thumbnailManager;

        private ObservableCollection<FavoriteProcessEntryViewModel> _favoriteProcessEntries = new ObservableCollection<FavoriteProcessEntryViewModel>();
        private ObservableCollection<ProcessEntry> _processEntries = new ObservableCollection<ProcessEntry>();

        private ConfigWindow? _configWindow = null;
        private bool _isFavoriteProcessEntrySelected = false;
        private ProcessEntry _selectedProcessEntry = new ProcessEntry();
        private FavoriteProcessEntryViewModel? _selectedFavoriteProcessEntryViewModel = null;

        #region Constructors

        public AppsViewModel(IDialogCoordinator dialogCoordinator, ILogger<AppsViewModel> logger, ISettingsManager settingsManager, IThumbnailManager thumbnailManager)
        {
            // Init Services
            _dialogCoordinator = dialogCoordinator;
            _logger = logger;
            _settingsManager = settingsManager;
            _thumbnailManager = thumbnailManager;

            // Init Messages
            WeakReferenceMessenger.Default.Register<ApplicationClosingMessage>(this, HandleApplicationClosingMessage);
            WeakReferenceMessenger.Default.Register<ApplicationLoadedMessage>(this, HandleApplicationLoadedMessage);
            WeakReferenceMessenger.Default.Register<ThumbnailConfigModifiedMessage>(this, HandleThumbnailConfigModifiedMessage);
            WeakReferenceMessenger.Default.Register<ThumbnailWindowChangedMessage>(this, HandleThumbnailWindowChangedMessage);
            WeakReferenceMessenger.Default.Register<ThumbnailWindowEditModeChangedMessage>(this, HandleThumbnailWindowEditModeChangedMessage);
            WeakReferenceMessenger.Default.Register<ThumbnailWindowOpenConfigMessage>(this, HandleThumbnailWindowOpenConfigMessage);
            WeakReferenceMessenger.Default.Register<ToggleEditModeKeyBindingMessage>(this, HandleToggleEditModeKeyBindingMessage);

            // Init View commands
            AddProcessEntryCommand = new RelayCommand(AddProcessEntryExecute, CanAddProcessEntryExecute);
            NextSelectedFavoriteProcessEntryHandleCommand = new RelayCommand(NextSelectedFavoriteProcessEntryHandleExecute, CanNextSelectedFavoriteProcessEntryHandleExecute);
            RefreshProcessEntryListCommand = new RelayCommand(RefreshProcessEntryListExecute);
            RefreshSelectedFavoriteProcessEntryHandleCommand = new RelayCommand(RefreshSelectedFavoriteProcessEntryHandleExecute, CanRefreshSelectedFavoriteProcessEntryHandleExecute);
            RemoveFavoriteProcessEntryCommand = new RelayCommand<FavoriteProcessEntryViewModel>(RemoveFavoriteProcessEntryExecute, CanRemoveFavoriteProcessEntryExecute);
            SelectFavoriteProcessEntryCommand = new RelayCommand<FavoriteProcessEntryViewModel>(SelectFavoriteProcessEntryExecute);
            ThumbnailConfigAddCommand = new RelayCommand(ThumbnailConfigAddExecute);
            ThumbnailConfigRemoveCommand = new RelayCommand<ThumbnailConfigViewModel>(ThumbnailConfigRemoveExecute);
            ThumbnailConfigSetCommand = new RelayCommand<ThumbnailConfigViewModel>(ThumbnailConfigSetExecute);
            ThumbnailConfigToggleCommand = new RelayCommand<ThumbnailConfigViewModel>(ThumbnailConfigToggleExecute);
            ThumbnailConfigsToggleAllCommand = new RelayCommand(ThumbnailConfigsToggleAllExecute, CanThumbnailConfigsToggleAllExecute);
            ToggleFavoriteProcessEntryCommand = new RelayCommand<FavoriteProcessEntryViewModel>(ToggleFavoriteProcessEntryExecute, CanToggleFavoriteProcessEntryExecute);
        }

        #endregion

        #region Events

        #endregion

        #region Properties

        public ICommand AddProcessEntryCommand { get; }
        public ICommand NextSelectedFavoriteProcessEntryHandleCommand { get; }
        public ICommand RefreshProcessEntryListCommand { get; }
        public ICommand RefreshSelectedFavoriteProcessEntryHandleCommand { get; }
        public ICommand RemoveFavoriteProcessEntryCommand { get; }
        public ICommand SelectFavoriteProcessEntryCommand { get; }
        public ICommand ThumbnailConfigAddCommand { get; }
        public ICommand ThumbnailConfigRemoveCommand { get; }
        public ICommand ThumbnailConfigSetCommand { get; }
        public ICommand ThumbnailConfigToggleCommand { get; }
        public ICommand ThumbnailConfigsToggleAllCommand { get; }
        public ICommand ToggleFavoriteProcessEntryCommand { get; }

        public ObservableCollection<FavoriteProcessEntryViewModel> FavoriteProcessEntries { get => _favoriteProcessEntries; set => _favoriteProcessEntries = value; }
        public ObservableCollection<ProcessEntry> ProcessEntries { get => _processEntries; set => _processEntries = value; }

        public bool IsFavoriteProcessEntrySelected
        {
            get => _isFavoriteProcessEntrySelected;
            set
            {
                SetProperty(ref _isFavoriteProcessEntrySelected, value);
            }
        }

        public ProcessEntry SelectedProcessEntry
        {
            get => _selectedProcessEntry;
            set
            {
                SetProperty(ref _selectedProcessEntry, value);
                ((RelayCommand)AddProcessEntryCommand).NotifyCanExecuteChanged();
            }
        }

        public FavoriteProcessEntryViewModel? SelectedFavoriteProcessEntryViewModel 
        {
            get => _selectedFavoriteProcessEntryViewModel;
            set
            {
                SetProperty(ref _selectedFavoriteProcessEntryViewModel, value);
                IsFavoriteProcessEntrySelected = SelectedFavoriteProcessEntryViewModel != null;
                ((RelayCommand)NextSelectedFavoriteProcessEntryHandleCommand).NotifyCanExecuteChanged();
                ((RelayCommand)RefreshSelectedFavoriteProcessEntryHandleCommand).NotifyCanExecuteChanged();
                ((RelayCommand)ThumbnailConfigsToggleAllCommand).NotifyCanExecuteChanged();
            }
        }

        #endregion

        #region Event handlers

        private void HandleApplicationClosingMessage(object recipient, ApplicationClosingMessage message)
        {
            ApplicationClosingMessageParams applicationClosingMessageParams = message.Value;

            _configWindow?.Close();
            foreach (var favoriteProcessEntry in FavoriteProcessEntries)
            {
                foreach (var thumbnailConfig in favoriteProcessEntry.ThumbnailConfigs.OfType<ThumbnailConfigViewModel>())
                {
                    thumbnailConfig.IsActive = false;
                }
            }
        }

        private void HandleApplicationLoadedMessage(object recipient, ApplicationLoadedMessage message)
        {
            ApplicationLoadedMessageParams applicationLoadedMessageParams = message.Value;

            LoadFavoriteProcessEntries();
            UpdateProcessEntries();
        }

        private void HandleThumbnailConfigModifiedMessage(object recipient, ThumbnailConfigModifiedMessage message)
        {
            ((RelayCommand<FavoriteProcessEntryViewModel>)RemoveFavoriteProcessEntryCommand).NotifyCanExecuteChanged();
            ((RelayCommand)RefreshSelectedFavoriteProcessEntryHandleCommand).NotifyCanExecuteChanged();
        }

        private void HandleThumbnailWindowChangedMessage(object recipient, ThumbnailWindowChangedMessage message)
        {
            ThumbnailWindowChangedMessageParams thumbnailWindowChangedMessageParams = message.Value;
            var handleSource = thumbnailWindowChangedMessageParams.HandleSource;

            SelectedFavoriteProcessEntryViewModel = FavoriteProcessEntries.FirstOrDefault(f => f.Handle.Equals(handleSource));
        }

        private void HandleThumbnailWindowEditModeChangedMessage(object recipient, ThumbnailWindowEditModeChangedMessage message)
        {
            ThumbnailWindowEditModeChangedMessageParams thumbnailWindowEditModeChangedMessageParams = message.Value;
            var handleSource = thumbnailWindowEditModeChangedMessageParams.HandleSource;
            var thumbnailConfigViewModel = thumbnailWindowEditModeChangedMessageParams.ThumbnailConfigViewModel as ThumbnailConfigViewModel;
            if (thumbnailConfigViewModel == null) return;

            thumbnailConfigViewModel.IsActive = false;
            thumbnailConfigViewModel.IsActive = true;
            if (thumbnailConfigViewModel.IsActive)
            {
                if (thumbnailConfigViewModel.IsEditModeEnabled)
                {
                    ThumbnailWindowEdit thumbnailWindow = new ThumbnailWindowEdit((HWND)handleSource, thumbnailConfigViewModel);
                    thumbnailWindow.Show();
                }
                else
                {
                    ThumbnailWindow thumbnailWindow = new ThumbnailWindow((HWND)handleSource, thumbnailConfigViewModel);
                    thumbnailWindow.Show();
                }
            }
        }

        private void HandleThumbnailWindowOpenConfigMessage(object recipient, ThumbnailWindowOpenConfigMessage message)
        {
            ThumbnailWindowOpenConfigMessageParams thumbnailConfigModifiedMessageParams = message.Value;

            if (_configWindow == null || _configWindow.IsClosed)
            {
                _configWindow = new ConfigWindow();
                _configWindow.Owner = App.Current.MainWindow;
            }

            ((ConfigWindowViewModel)_configWindow.DataContext).ThumbnailConfigViewModel = thumbnailConfigModifiedMessageParams.ThumbnailConfigViewModel as ThumbnailConfigViewModel ??
                new ThumbnailConfigViewModel(new ThumbnailConfig());

            _configWindow.Show();
        }

        private void HandleToggleEditModeKeyBindingMessage(object recipient, ToggleEditModeKeyBindingMessage message)
        {
            if (SelectedFavoriteProcessEntryViewModel == null)
            {
                SelectedFavoriteProcessEntryViewModel = FavoriteProcessEntries.FirstOrDefault();
            }
            if (SelectedFavoriteProcessEntryViewModel == null) return;
            if (!SelectedFavoriteProcessEntryViewModel.IsAnyThumbnailActive) return;

            // Close all active thumbnails
            foreach (var thumbnailConfigViewModel in SelectedFavoriteProcessEntryViewModel.ThumbnailConfigs.OfType<ThumbnailConfigViewModel>())
            {
                thumbnailConfigViewModel.IsActive = false;
            }

            // Open all enabled thumbnails with the correct edit mode view
            foreach (var thumbnailConfigViewModel in SelectedFavoriteProcessEntryViewModel.ThumbnailConfigs.OfType<ThumbnailConfigViewModel>())
            {                
                thumbnailConfigViewModel.IsActive = thumbnailConfigViewModel.IsEnabled;
                thumbnailConfigViewModel.IsEditModeEnabled = !thumbnailConfigViewModel.IsEditModeEnabled;
                thumbnailConfigViewModel.AppName = SelectedFavoriteProcessEntryViewModel.Name;
                if (thumbnailConfigViewModel.IsActive)
                {
                    if (thumbnailConfigViewModel.IsEditModeEnabled)
                    {
                        ThumbnailWindowEdit thumbnailWindow = new ThumbnailWindowEdit((HWND)SelectedFavoriteProcessEntryViewModel?.Handle, thumbnailConfigViewModel);
                        thumbnailWindow.Show();
                    }
                    else
                    {
                        ThumbnailWindow thumbnailWindow = new ThumbnailWindow((HWND)SelectedFavoriteProcessEntryViewModel?.Handle, thumbnailConfigViewModel);
                        thumbnailWindow.Show();
                    }
                }
            }
        }

        private bool CanAddProcessEntryExecute()
        {
            return SelectedProcessEntry != null;

            // No duplicates allowed
            //return !FavoriteProcessEntries.Any(f => f.Name.Equals(SelectedProcessEntry.Name));
        }

        private void AddProcessEntryExecute()
        {
            var favoriteProcessEntry = new FavoriteProcessEntry
            {
                DisplayName = SelectedProcessEntry.Name,
                Name = SelectedProcessEntry.Name,
            };

            FavoriteProcessEntries.Add(new FavoriteProcessEntryViewModel(favoriteProcessEntry));
            _thumbnailManager.AddFavoriteProcessEntry(favoriteProcessEntry);

            ((RelayCommand)AddProcessEntryCommand).NotifyCanExecuteChanged();
        }

        private bool CanNextSelectedFavoriteProcessEntryHandleExecute()
        {
            return SelectedFavoriteProcessEntryViewModel != null;

            //if (SelectedFavoriteProcessEntryViewModel == null) return false;

            //return !SelectedFavoriteProcessEntryViewModel.IsAnyThumbnailActive;
        }

        //private void NextSelectedFavoriteProcessEntryHandleExecute()
        //{
        //    var processes = Process.GetProcessesByName(SelectedFavoriteProcessEntryViewModel?.Name).Where(p => p.MainWindowHandle != 0).ToList();

        //    if (processes.Count == 0)
        //    {
        //        SelectedFavoriteProcessEntryViewModel.Handle = 0;
        //    }
        //    else if (!processes.Any(p => p.MainWindowHandle == SelectedFavoriteProcessEntryViewModel.Handle))
        //    {
        //        SelectedFavoriteProcessEntryViewModel.Handle = processes[0].MainWindowHandle;
        //    }
        //    else
        //    {
        //        int currentIndex = processes.FindIndex(p => p.MainWindowHandle == SelectedFavoriteProcessEntryViewModel.Handle);
        //        int newIndex = currentIndex + 1 > processes.Count - 1 ? 0 : currentIndex + 1;
        //        SelectedFavoriteProcessEntryViewModel.Handle = processes[newIndex].MainWindowHandle;
        //    }

        //    ((RelayCommand)ThumbnailConfigsToggleAllCommand).NotifyCanExecuteChanged();
        //}

        private void NextSelectedFavoriteProcessEntryHandleExecute()
        {
            // First disable all active thumbnails before changing the process handle.
            if (SelectedFavoriteProcessEntryViewModel?.IsAnyThumbnailActive ?? false)
            {
                foreach (var thumbnailConfigViewModel in SelectedFavoriteProcessEntryViewModel.ThumbnailConfigs.OfType<ThumbnailConfigViewModel>())
                {
                    thumbnailConfigViewModel.IsActive = false;
                }
            }

            // Select the next available process handle.
            var processes = Process.GetProcessesByName(SelectedFavoriteProcessEntryViewModel?.Name).Where(p => p.MainWindowHandle != 0).ToList();

            if (processes.Count == 0)
            {
                SelectedFavoriteProcessEntryViewModel.Handle = 0;
            }
            else if (!processes.Any(p => p.MainWindowHandle == SelectedFavoriteProcessEntryViewModel.Handle))
            {
                SelectedFavoriteProcessEntryViewModel.Handle = processes[0].MainWindowHandle;
            }
            else
            {
                int currentIndex = processes.FindIndex(p => p.MainWindowHandle == SelectedFavoriteProcessEntryViewModel.Handle);
                int newIndex = currentIndex + 1 > processes.Count - 1 ? 0 : currentIndex + 1;
                SelectedFavoriteProcessEntryViewModel.Handle = processes[newIndex].MainWindowHandle;
            }

            ((RelayCommand)RefreshSelectedFavoriteProcessEntryHandleCommand).NotifyCanExecuteChanged();
            ((RelayCommand)ThumbnailConfigsToggleAllCommand).NotifyCanExecuteChanged();

            if (SelectedFavoriteProcessEntryViewModel.Handle == 0) return;

            // Create thumbnails for the new process handle.
            foreach (var thumbnailConfigViewModel in SelectedFavoriteProcessEntryViewModel.ThumbnailConfigs.OfType<ThumbnailConfigViewModel>())
            {
                thumbnailConfigViewModel.IsActive = thumbnailConfigViewModel.IsEnabled;
                thumbnailConfigViewModel.AppName = SelectedFavoriteProcessEntryViewModel.Name;
                if (thumbnailConfigViewModel.IsActive)
                {
                    if (thumbnailConfigViewModel.IsEditModeEnabled)
                    {
                        ThumbnailWindowEdit thumbnailWindow = new ThumbnailWindowEdit((HWND)SelectedFavoriteProcessEntryViewModel?.Handle, thumbnailConfigViewModel);
                        thumbnailWindow.Show();
                    }
                    else
                    {
                        ThumbnailWindow thumbnailWindow = new ThumbnailWindow((HWND)SelectedFavoriteProcessEntryViewModel?.Handle, thumbnailConfigViewModel);
                        thumbnailWindow.Show();
                    }
                }
            }
        }

        private void RefreshProcessEntryListExecute()
        {
            UpdateProcessEntries();
        }

        private bool CanRefreshSelectedFavoriteProcessEntryHandleExecute()
        {
            if (SelectedFavoriteProcessEntryViewModel == null) return false;

            return !SelectedFavoriteProcessEntryViewModel.IsAnyThumbnailActive;
        }

        private void RefreshSelectedFavoriteProcessEntryHandleExecute()
        {
            var process = Process.GetProcessesByName(SelectedFavoriteProcessEntryViewModel?.Name).Where(p => p.MainWindowHandle != 0).FirstOrDefault();
            if(process != null)
            {
                SelectedFavoriteProcessEntryViewModel.Handle = process.MainWindowHandle;
            }
            else
            {
                SelectedFavoriteProcessEntryViewModel.Handle = 0;
            }
            ((RelayCommand)ThumbnailConfigsToggleAllCommand).NotifyCanExecuteChanged();
        }

        private bool CanRemoveFavoriteProcessEntryExecute(FavoriteProcessEntryViewModel? favoriteProcessEntryViewModel)
        {
            if(favoriteProcessEntryViewModel == null) return false;

            return !favoriteProcessEntryViewModel.IsAnyThumbnailActive;
        }

        private void RemoveFavoriteProcessEntryExecute(FavoriteProcessEntryViewModel? favoriteProcessEntryViewModel)
        {
            if (favoriteProcessEntryViewModel == null) return;

            _dialogCoordinator.ShowMessageAsync(this, $"Remove", $"Are you sure you want to remove \"{favoriteProcessEntryViewModel.DisplayName}\"", MessageDialogStyle.AffirmativeAndNegative).ContinueWith(t =>
            {
                if (t.Result == MessageDialogResult.Affirmative)
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        _logger.LogInformation($"Removed \"{favoriteProcessEntryViewModel.DisplayName}\"");

                        FavoriteProcessEntries.Remove(favoriteProcessEntryViewModel);
                        _thumbnailManager.RemoveFavoriteProcessEntry(favoriteProcessEntryViewModel.Model);
                        SelectedFavoriteProcessEntryViewModel = null;

                        ((RelayCommand)AddProcessEntryCommand).NotifyCanExecuteChanged();
                    });
                }
            });
        }

        private void SelectFavoriteProcessEntryExecute(FavoriteProcessEntryViewModel? favoriteProcessEntryViewModel)
        {
            if (favoriteProcessEntryViewModel == null) return;

            SelectedFavoriteProcessEntryViewModel = favoriteProcessEntryViewModel;
            if (CanRefreshSelectedFavoriteProcessEntryHandleExecute())
            {
                RefreshSelectedFavoriteProcessEntryHandleExecute();
            }
        }

        private async void ThumbnailConfigAddExecute()
        {
            ThumbnailConfigViewModel thumbnailConfigViewModel = new ThumbnailConfigViewModel(new ThumbnailConfig());

            var addThumbnailConfigDialog = new CustomDialog() { Title = "Add" };
            var dataContext = new AddThumbnailConfigViewModel(async instance =>
            {
                await addThumbnailConfigDialog.WaitUntilUnloadedAsync();
            }, thumbnailConfigViewModel);
            addThumbnailConfigDialog.Content = new AddThumbnailConfigView() { DataContext = dataContext };
            await _dialogCoordinator.ShowMetroDialogAsync(this, addThumbnailConfigDialog);
            await addThumbnailConfigDialog.WaitUntilUnloadedAsync();

            // Add confirmed ThumbnailConfig.
            if (!dataContext.IsCanceled)
            {
                SelectedFavoriteProcessEntryViewModel?.AddThumbnailConfig(thumbnailConfigViewModel);
                _thumbnailManager.SaveFavorites();

                // Create new thumbnail window
                thumbnailConfigViewModel.IsActive = true;
                thumbnailConfigViewModel.AppName = SelectedFavoriteProcessEntryViewModel.Name;
                ThumbnailWindowEdit thumbnailWindow = new ThumbnailWindowEdit((HWND)SelectedFavoriteProcessEntryViewModel?.Handle, thumbnailConfigViewModel);
                thumbnailWindow.Show();

                // Show config window for newly created thumbnail window.
                WeakReferenceMessenger.Default.Send(new ThumbnailWindowOpenConfigMessage(new ThumbnailWindowOpenConfigMessageParams { ThumbnailConfigViewModel = thumbnailConfigViewModel }));
            }
        }

        private void ThumbnailConfigRemoveExecute(ThumbnailConfigViewModel? thumbnailConfigViewModel)
        {
            _dialogCoordinator.ShowMessageAsync(this, $"Remove", $"Are you sure you want to remove \"{thumbnailConfigViewModel?.Name}\"", MessageDialogStyle.AffirmativeAndNegative).ContinueWith(t =>
            {
                if (t.Result == MessageDialogResult.Affirmative)
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        _logger.LogInformation($"Removed \"{thumbnailConfigViewModel?.Name}\"");

                        SelectedFavoriteProcessEntryViewModel?.RemoveTumbnailConfig(thumbnailConfigViewModel);
                        _thumbnailManager.SaveFavorites();
                    });
                }
            });
        }

        private async void ThumbnailConfigSetExecute(ThumbnailConfigViewModel? thumbnailConfigViewModel)
        {
            if (SelectedFavoriteProcessEntryViewModel == null) return;
            if (thumbnailConfigViewModel == null) return;

            var updateThumbnailConfigDialog = new CustomDialog() { Title = "Config" };
            var dataContext = new UpdateThumbnailConfigViewModel(async instance =>
            {
                await updateThumbnailConfigDialog.WaitUntilUnloadedAsync();
            }, thumbnailConfigViewModel);
            updateThumbnailConfigDialog.Content = new UpdateThumbnailConfigView() { DataContext = dataContext };
            await _dialogCoordinator.ShowMetroDialogAsync(this, updateThumbnailConfigDialog);
            await updateThumbnailConfigDialog.WaitUntilUnloadedAsync();

            _thumbnailManager.SaveFavorites();
        }

        /// <summary>
        /// Open / closes thumbnail window, clicked from the current shown favorite app overview.
        /// </summary>
        /// <param name="thumbnailConfigViewModel"></param>
        private void ThumbnailConfigToggleExecute(ThumbnailConfigViewModel? thumbnailConfigViewModel)
        {
            if (thumbnailConfigViewModel == null) return;

            thumbnailConfigViewModel.IsActive = !thumbnailConfigViewModel.IsActive;
            thumbnailConfigViewModel.AppName = SelectedFavoriteProcessEntryViewModel.Name;

            if (thumbnailConfigViewModel.IsActive)
            {
                if (thumbnailConfigViewModel.IsEditModeEnabled)
                {
                    ThumbnailWindowEdit thumbnailWindow = new ThumbnailWindowEdit((HWND)SelectedFavoriteProcessEntryViewModel?.Handle, thumbnailConfigViewModel);
                    thumbnailWindow.Show();
                }
                else
                {
                    ThumbnailWindow thumbnailWindow = new ThumbnailWindow((HWND)SelectedFavoriteProcessEntryViewModel?.Handle, thumbnailConfigViewModel);
                    thumbnailWindow.Show();
                }
            }
        }

        private bool CanThumbnailConfigsToggleAllExecute()
        {
            return SelectedFavoriteProcessEntryViewModel?.IsRunning ?? false;
        }

        private void ThumbnailConfigsToggleAllExecute()
        {
            if (SelectedFavoriteProcessEntryViewModel == null) return;

            if(SelectedFavoriteProcessEntryViewModel.IsAnyThumbnailActive)
            {
                foreach (var thumbnailConfigViewModel in SelectedFavoriteProcessEntryViewModel.ThumbnailConfigs.OfType<ThumbnailConfigViewModel>())
                {
                    thumbnailConfigViewModel.IsActive = false;
                }
            }
            else
            {
                foreach (var thumbnailConfigViewModel in SelectedFavoriteProcessEntryViewModel.ThumbnailConfigs.OfType<ThumbnailConfigViewModel>())
                {
                    thumbnailConfigViewModel.IsActive = thumbnailConfigViewModel.IsEnabled;
                    thumbnailConfigViewModel.AppName = SelectedFavoriteProcessEntryViewModel.Name;
                    if (thumbnailConfigViewModel.IsActive)
                    {
                        if (thumbnailConfigViewModel.IsEditModeEnabled)
                        {
                            ThumbnailWindowEdit thumbnailWindow = new ThumbnailWindowEdit((HWND)SelectedFavoriteProcessEntryViewModel?.Handle, thumbnailConfigViewModel);
                            thumbnailWindow.Show();
                        }
                        else
                        {
                            ThumbnailWindow thumbnailWindow = new ThumbnailWindow((HWND)SelectedFavoriteProcessEntryViewModel?.Handle, thumbnailConfigViewModel);
                            thumbnailWindow.Show();
                        }
                    }
                }
            }
        }

        private bool CanToggleFavoriteProcessEntryExecute(FavoriteProcessEntryViewModel? favoriteProcessEntryViewModel)
        {
            return true;
        }

        private void ToggleFavoriteProcessEntryExecute(FavoriteProcessEntryViewModel? favoriteProcessEntryViewModel)
        {
            if (favoriteProcessEntryViewModel == null) return;

            // Get handle if not set
            var process = Process.GetProcessesByName(favoriteProcessEntryViewModel?.Name).Where(p => p.MainWindowHandle != 0).FirstOrDefault();
            if (process == null) return;

            favoriteProcessEntryViewModel.Handle = process.MainWindowHandle;
            if (favoriteProcessEntryViewModel.IsAnyThumbnailActive)
            {
                foreach (var thumbnailConfigViewModel in favoriteProcessEntryViewModel.ThumbnailConfigs.OfType<ThumbnailConfigViewModel>())
                {
                    thumbnailConfigViewModel.IsActive = false;
                }
            }
            else
            {
                foreach (var thumbnailConfigViewModel in favoriteProcessEntryViewModel.ThumbnailConfigs.OfType<ThumbnailConfigViewModel>())
                {
                    thumbnailConfigViewModel.IsActive = thumbnailConfigViewModel.IsEnabled;
                    thumbnailConfigViewModel.AppName = favoriteProcessEntryViewModel.Name;
                    if (thumbnailConfigViewModel.IsActive)
                    {
                        if (thumbnailConfigViewModel.IsEditModeEnabled)
                        {
                            ThumbnailWindowEdit thumbnailWindow = new ThumbnailWindowEdit((HWND)favoriteProcessEntryViewModel?.Handle, thumbnailConfigViewModel);
                            thumbnailWindow.Show();
                        }
                        else
                        {
                            ThumbnailWindow thumbnailWindow = new ThumbnailWindow((HWND)favoriteProcessEntryViewModel?.Handle, thumbnailConfigViewModel);
                            thumbnailWindow.Show();
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods

        private void LoadFavoriteProcessEntries()
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                FavoriteProcessEntries.Clear();
                FavoriteProcessEntries.AddRange(_thumbnailManager.FavoriteProcessEntries.Select(f => new FavoriteProcessEntryViewModel(f)));
            });
        }

        private void UpdateProcessEntries()
        {
            ImageSource ConvertIcon(System.Drawing.Icon icon)
            {
                if (icon == null) return null;

                ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                return imageSource;
            }

            Application.Current?.Dispatcher.Invoke(() =>
            {
                ProcessEntries.Clear();

                var processes = Process.GetProcesses().Where(p => p.MainWindowHandle != 0).Select(p => new ProcessEntry
                {
                    Name = p.ProcessName,
                    Icon = ConvertIcon(p.GetIcon())
                    
                }).ToList();

                processes.Sort((x, y) =>
                {
                    return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
                });

                ProcessEntries.AddRange(processes);
                if (ProcessEntries.Count > 0)
                {
                    SelectedProcessEntry = ProcessEntries.First();
                }
            });
        }

        #endregion
    }
}
