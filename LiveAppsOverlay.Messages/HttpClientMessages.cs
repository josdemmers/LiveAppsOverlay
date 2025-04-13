using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiveAppsOverlay.Messages
{
    public class DownloadCompletedMessage(DownloadCompletedMessageParams downloadCompletedMessageParams) : ValueChangedMessage<DownloadCompletedMessageParams>(downloadCompletedMessageParams)
    {
    }

    public class DownloadCompletedMessageParams
    {
        public string FileName { get; set; } = string.Empty;
    }

    public class DownloadProgressUpdatedMessage(DownloadProgressUpdatedMessageParams downloadProgressUpdatedMessageParams) : ValueChangedMessage<DownloadProgressUpdatedMessageParams>(downloadProgressUpdatedMessageParams)
    {
    }

    public class DownloadProgressUpdatedMessageParams
    {
        public HttpProgress? HttpProgress { get; set; }
    }

    public class UploadProgressUpdatedMessage(UploadProgressUpdatedMessageParams uploadProgressUpdatedMessageParams) : ValueChangedMessage<UploadProgressUpdatedMessageParams>(uploadProgressUpdatedMessageParams)
    {
    }

    public class UploadProgressUpdatedMessageParams
    {
        public HttpProgress? HttpProgress { get; set; }
    }

    public class HttpProgress
    {
        public long Bytes { get; set; }
        public int Progress { get; set; }
    }
}
