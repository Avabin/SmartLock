using Orleans.Runtime;
using SmartLock.Client.Models;
using SmartLock.Orleans.Core;

namespace SmartLock.GrainInterfaces;

public static class LocationExtensions
{
    public static StreamId ToNotificationsStreamId(this LocationModel locationModel) => StreamId.Create(StreamProviderConstants.NotificationsNamespace, locationModel.Value);
    
}