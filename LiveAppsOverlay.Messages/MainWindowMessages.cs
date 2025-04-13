using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiveAppsOverlay.Messages
{
    public class ApplicationClosingMessage(ApplicationClosingMessageParams applicationClosingMessageParams) : ValueChangedMessage<ApplicationClosingMessageParams>(applicationClosingMessageParams)
    {
    }

    public class ApplicationClosingMessageParams
    {
    }

    public class ApplicationLoadedMessage(ApplicationLoadedMessageParams applicationLoadedMessageParams) : ValueChangedMessage<ApplicationLoadedMessageParams>(applicationLoadedMessageParams)
    {
    }

    public class ApplicationLoadedMessageParams
    {
    }
}
