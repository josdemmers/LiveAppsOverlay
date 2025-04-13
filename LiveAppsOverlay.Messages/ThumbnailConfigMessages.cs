using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiveAppsOverlay.Messages
{
    /// <summary>
    /// Message for updating CanExecute commands and saving thumbnail configuration changes.
    /// </summary>
    /// <param name="thumbnailConfigModifiedMessageParams"></param>
    public class ThumbnailConfigModifiedMessage(ThumbnailConfigModifiedMessageParams thumbnailConfigModifiedMessageParams) : ValueChangedMessage<ThumbnailConfigModifiedMessageParams>(thumbnailConfigModifiedMessageParams)
    {
    }

    public class ThumbnailConfigModifiedMessageParams
    {
    }
}
