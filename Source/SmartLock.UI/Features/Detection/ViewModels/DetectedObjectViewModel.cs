using CommunityToolkit.Mvvm.ComponentModel;
using SmartLock.Client.Models;

namespace SmartLock.UI.Features.Detection.ViewModels;

public class DetectedObjectViewModel : ObservableObject
{
    public  DetectedObjectModel Model { get; }
    [field: ObservableProperty]
    public string ClassName { get; set; } = "";
    [field: ObservableProperty]
    public double Confidence { get; set; } = 0f;
    public DetectedObjectViewModel(DetectedObjectModel model)
    {
        Model = model;
        ClassName = model.Class;
        Confidence = model.Confidence;
    }
}