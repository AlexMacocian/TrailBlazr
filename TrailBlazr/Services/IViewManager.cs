using Microsoft.AspNetCore.Components;
using TrailBlazr.Models;

namespace TrailBlazr.Services;
public interface IViewManager
{
    event EventHandler<ViewRequest>? ShowViewRequested;

    void ShowView<TView>(object? dataContext = null)
        where TView : ComponentBase;

    void ShowView(Type viewType, object? dataContext = null);
}
