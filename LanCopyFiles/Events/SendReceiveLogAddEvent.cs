using System.Collections.Generic;
using LanCopyFiles.Models;
using Prism.Events;

namespace LanCopyFiles.Events;

public class SendReceiveLogAddEvent: PubSubEvent<List<SendReceiveLogItem>>
{
    
}