using System.Reactive.Linq;
using System.Reactive.Subjects;
using SmartLock.Client.Models;

namespace SmartLock.UI.Features.Settings;

public class ClientSettingsMediator : IClientSettingsMediator, IObservable<ClientSettings>
{
    private ISubject<ClientSettings> _subject = new ReplaySubject<ClientSettings>(1);
    private IObservable<ClientSettings> Observable => _subject.AsObservable();
    public void Publish(ClientSettings settings) => _subject.OnNext(settings);

    public IDisposable Subscribe(IObserver<ClientSettings> observer) => 
        Observable.Subscribe(observer);

    IObservable<ClientSettings> IClientSettingsMediator.Observable => Observable;
}
public interface IClientSettingsMediator
{
    IObservable<ClientSettings> Observable { get; }
    
    void Publish(ClientSettings settings);
}