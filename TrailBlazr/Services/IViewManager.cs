using Microsoft.AspNetCore.Components;
using TrailBlazr.Models;

namespace TrailBlazr.Services;
public interface IViewManager
{
    event EventHandler<ViewRequest>? ShowViewRequested;

    void ShowView<TView>(params (string, object)[] routeValues)
        where TView : ComponentBase;

    void ShowView(Type viewType, params (string, object)[] routeValues);

    void ShowView(string path, params (string, object)[] routeValues);
}
