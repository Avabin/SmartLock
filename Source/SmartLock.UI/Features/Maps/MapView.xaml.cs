using Mapsui.Tiling;

namespace SmartLock.UI.Features.Maps;

public partial class MapView : ContentView
{
    public MapView()
    {
        InitializeComponent();
        
        MapControl.Map?.Layers.Add(OpenStreetMap.CreateTileLayer());
    }
}