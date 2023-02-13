using SmartLock.Client.Models;

namespace SmartLock.UI.Features.Buildings;

public class BuildingViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public BuildingViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public BuildingViewModel Create(LocationModel location)
    {
        var vm = _serviceProvider.GetRequiredService<BuildingViewModel>();
        vm.Location = location.Value;
        return vm;
    }
}