using SmartLock.Client.Models;
using SmartLock.UI.ViewModels.Buildings;

namespace SmartLock.UI.ViewModels;

public class LockViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public LockViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public LockViewModel Create(LockModel status)
    {
        return new LockViewModel(status, _serviceProvider.GetRequiredService<IObservable<Notification<IEvent>>>());
    }
}