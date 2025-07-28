using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveAppsOverlay.Messages
{
    public class ThumbnailWindowChangedMessage(ThumbnailWindowChangedMessageParams thumbnailWindowChangedMessageParams) : ValueChangedMessage<ThumbnailWindowChangedMessageParams>(thumbnailWindowChangedMessageParams)
    {
    }

    public class ThumbnailWindowChangedMessageParams
    {
        public string DisplayName { get; set; } = string.Empty;
        public nint HandleSource { get; set; }
    }

    public class ThumbnailWindowEditModeChangedMessage(ThumbnailWindowEditModeChangedMessageParams thumbnailWindowEditModeChangedMessageParams) : ValueChangedMessage<ThumbnailWindowEditModeChangedMessageParams>(thumbnailWindowEditModeChangedMessageParams)
    {
    }

    public class ThumbnailWindowEditModeChangedMessageParams
    {
        public nint HandleSource { get; set; }
        public object? ThumbnailConfigViewModel { get; set; }
    }

    public class ThumbnailWindowOpenConfigMessage(ThumbnailWindowOpenConfigMessageParams thumbnailWindowOpenConfigMessageParams) : ValueChangedMessage<ThumbnailWindowOpenConfigMessageParams>(thumbnailWindowOpenConfigMessageParams)
    {
    }

    public class ThumbnailWindowOpenConfigMessageParams
    {
        public object? ThumbnailConfigViewModel { get; set; }
    }
}
