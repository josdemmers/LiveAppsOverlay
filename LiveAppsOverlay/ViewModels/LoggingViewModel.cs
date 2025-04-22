using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LiveAppsOverlay.ViewModels
{
    public class LoggingViewModel : ObservableObject
    {

        #region Constructors

        public LoggingViewModel()
        {
            // Init Messages
            WeakReferenceMessenger.Default.Register<InfoOccurredMessage>(this, HandleInfoOccurredMessage);
            WeakReferenceMessenger.Default.Register<WarningOccurredMessage>(this, HandleWarningOccurredMessage);
            WeakReferenceMessenger.Default.Register<ErrorOccurredMessage>(this, HandleErrorOccurredMessage);
            WeakReferenceMessenger.Default.Register<ExceptionOccurredMessage>(this, HandleExceptionOccurredMessage);

            // Init View commands
            ClearLogMessagesCommand = new RelayCommand(ClearLogMessagesExecute);
        }

        #endregion

        #region Events

        #endregion

        #region Properties

        public ICommand ClearLogMessagesCommand { get; }

        public ObservableCollection<string> LogMessages { get; set; } = new ObservableCollection<string>();

        #endregion

        #region Event handlers

        private void ClearLogMessagesExecute()
        {
            LogMessages.Clear();
        }

        private void HandleInfoOccurredMessage(object recipient, InfoOccurredMessage message)
        {
            InfoOccurredMessageParams infoOccurredMessageParams = message.Value;

            Application.Current?.Dispatcher?.Invoke(() =>
            {
                string previousMessage = LogMessages.Any() ? LogMessages.Last() : string.Empty;
                if (!previousMessage.Equals(infoOccurredMessageParams.Message))
                {
                    LogMessages.Add(infoOccurredMessageParams.Message);
                }
            });
        }

        private void HandleWarningOccurredMessage(object recipient, WarningOccurredMessage message)
        {
            WarningOccurredMessageParams warningOccurredMessageParams = message.Value;

            Application.Current?.Dispatcher?.Invoke(() =>
            {
                string previousMessage = LogMessages.Any() ? LogMessages.Last() : string.Empty;
                if (!previousMessage.Equals(warningOccurredMessageParams.Message))
                {
                    LogMessages.Add(warningOccurredMessageParams.Message);
                }
            });
        }

        private void HandleErrorOccurredMessage(object recipient, ErrorOccurredMessage message)
        {
            ErrorOccurredMessageParams errorOccurredMessageParams = message.Value;

            Application.Current?.Dispatcher?.Invoke(() =>
            {
                string previousMessage = LogMessages.Any() ? LogMessages.Last() : string.Empty;
                if (!previousMessage.Equals(errorOccurredMessageParams.Message))
                {
                    LogMessages.Add(errorOccurredMessageParams.Message);
                }
            });
        }

        private void HandleExceptionOccurredMessage(object recipient, ExceptionOccurredMessage message)
        {
            ExceptionOccurredMessageParams exceptionOccurredMessageParams = message.Value;

            Application.Current?.Dispatcher?.Invoke(() =>
            {
                string previousMessage = LogMessages.Any() ? LogMessages.Last() : string.Empty;
                if (!previousMessage.Equals(exceptionOccurredMessageParams.Message))
                {
                    LogMessages.Add(exceptionOccurredMessageParams.Message);
                }
            });
        }

        #endregion

        #region Methods

        #endregion
    }
}
