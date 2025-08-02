using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using TrailBlazr.Models;

namespace TrailBlazr.Services;
public interface IViewManager
{
    event EventHandler<ViewRequest>? ShowViewRequested;

    void ShowView<TView>(RouteValueDictionary? routeValues = default)
        where TView : ComponentBase;

    void ShowView(Type viewType, RouteValueDictionary? routeValues = default);

    void ShowView(string path, RouteValueDictionary? routeValues = default);
}
