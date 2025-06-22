using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveAppsOverlay.Messages
{
    public class UpdateHotkeysRequestMessage(UpdateHotkeysRequestMessageParams updateHotkeysRequestMessageParams) : ValueChangedMessage<UpdateHotkeysRequestMessageParams>(updateHotkeysRequestMessageParams)
    { 
    }

    public class UpdateHotkeysRequestMessageParams
    {
    }
}
