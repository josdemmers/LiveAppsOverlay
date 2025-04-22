using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveAppsOverlay.Messages
{
    public class InfoOccurredMessage(InfoOccurredMessageParams infoOccurredMessageParams) : ValueChangedMessage<InfoOccurredMessageParams>(infoOccurredMessageParams)
    {
    }

    public class InfoOccurredMessageParams
    {
        public string Message { get; set; } = string.Empty;
    }

    public class WarningOccurredMessage(WarningOccurredMessageParams warningOccurredMessageParams) : ValueChangedMessage<WarningOccurredMessageParams>(warningOccurredMessageParams)
    {
    }

    public class WarningOccurredMessageParams
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ErrorOccurredMessage(ErrorOccurredMessageParams errorOccurredMessageParams) : ValueChangedMessage<ErrorOccurredMessageParams>(errorOccurredMessageParams)
    {
    }

    public class ErrorOccurredMessageParams
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ExceptionOccurredMessage(ExceptionOccurredMessageParams exceptionOccurredMessageParams) : ValueChangedMessage<ExceptionOccurredMessageParams>(exceptionOccurredMessageParams)
    {
    }

    public class ExceptionOccurredMessageParams
    {
        public string Message { get; set; } = string.Empty;
    }
}
