using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveAppsOverlay.Messages
{
    public class ThumbnailWindowOpenConfigMessage(ThumbnailWindowOpenConfigMessageParams thumbnailWindowOpenConfigMessageParams) : ValueChangedMessage<ThumbnailWindowOpenConfigMessageParams>(thumbnailWindowOpenConfigMessageParams)
    {
    }

    public class ThumbnailWindowOpenConfigMessageParams
    {
        public object? ThumbnailConfigViewModel { get; set; }
    }
}
