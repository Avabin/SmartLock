using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;
using Splat;
using WebcamYolo.ViewModels;

namespace WebcamYolo;

public class ViewLocator : IDataTemplate
{
    public IControl? Build(object? data)
    {
        if (data is null)
            return null;

        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            var maybeView = Locator.Current.GetService(type);
            if (maybeView is IViewFor view)
            {
                view.ViewModel = data;
                return (Control) view;
            }
                
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}