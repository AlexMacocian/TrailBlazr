using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using TrailBlazr.Models;

namespace TrailBlazr.Services;
public sealed class ViewManager(
    IServiceProvider serviceProvider) : IViewManager, IDisposable
{
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    private NavigationManager? navigationManager;

    public event EventHandler<ViewRequest>? ShowViewRequested;

    internal void ContainerInitialized(NavigationManager navigationManager)
    {
        this.navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        this.navigationManager.LocationChanged += this.NavigationManager_LocationChanged;
    }

    public void ShowView<TView>(RouteValueDictionary? routeValues = default)
        where TView : ComponentBase
    {
        var viewType = typeof(TView);
        this.ShowViewByType(viewType, routeValues);
    }

    public void ShowView(Type viewType, RouteValueDictionary? routeValues = default)
    {
        this.ShowViewByType(viewType, routeValues);
    }

    public void ShowView(string path, RouteValueDictionary? routeValues = default)
    {
        this.ShowViewByPath(path, routeValues);
    }

    private void ShowViewByType(Type viewType, RouteValueDictionary? routeValues)
    {
        var viewRegistrations = this.serviceProvider.GetServices<ViewRegistration>();
        routeValues ??= [];
        if (this.navigationManager is null)
        {
            throw new InvalidOperationException("NavigationManager is not initialized. Ensure that the ViewManager is properly registered in the service container and that the NavigationManager is available.");
        }

        if (viewRegistrations.FirstOrDefault(r => r.ViewType == viewType) is not ViewRegistration registration)
        {
            throw new InvalidOperationException($"View type not registered: {viewType.FullName}");
        }

        if (registration.Routes.Count == 0)
        {
            throw new InvalidOperationException($"View type not registered with any routes: {viewType.FullName}");
        }

        var route = registration.Routes[0];
        var url = route.Template.TemplateText;
        if (url.Contains('?'))
        {
            url = url[..url.IndexOf('?')];
        }

        if (url.Contains('#'))
        {
            url = url[..url.IndexOf('#')];
        }

        var uri = this.navigationManager.GetUriWithQueryParameters(url, routeValues.ToDictionary());
        this.navigationManager.NavigateTo(uri);
    }

    private void ShowViewByPath(string path, RouteValueDictionary? routeValues)
    {
        var uriObj = path.StartsWith('/') ? new Uri(path, UriKind.Relative) : new Uri(path, UriKind.Absolute);
        routeValues ??= [];
        var query = uriObj.IsAbsoluteUri ? uriObj.Query : path[(path.IndexOf('?') + 1)..];
        var finalPath = uriObj.IsAbsoluteUri ? uriObj.PathAndQuery.Split('?').FirstOrDefault() ?? string.Empty : path.Split('?').FirstOrDefault() ?? string.Empty;
        foreach (var kvp in QueryHelpers.ParseNullableQuery(query) ?? [])
        {
            routeValues[kvp.Key] = kvp.Value.Count == 1 ? kvp.Value[0] : kvp.Value;
        }

        var viewRegistrations = this.serviceProvider.GetServices<ViewRegistration>();
        routeValues ??= [];
        if (viewRegistrations.FirstOrDefault(r => r.Routes.FirstOrDefault(t => {
                var parsedRouteValues = new RouteValueDictionary();
                if (t.TryMatch(finalPath, parsedRouteValues))
                {
                    foreach(var kvp in parsedRouteValues)
                    {
                        routeValues[kvp.Key] = kvp.Value;
                    }

                    return true;
                }

                return false;
            }) is not null) is not ViewRegistration registration)
        {
            throw new InvalidOperationException($"Path not found: {finalPath}");
        }

        this.ShowViewInner(registration.ViewType, registration.ViewModelType, routeValues);
    }

    private void ShowViewInner(Type viewType, Type viewModelType, RouteValueDictionary routeValues)
    {
        var viewRequest = new ViewRequest(viewType, viewModelType, routeValues);
        this.ShowViewRequested?.Invoke(this, viewRequest);
    }

    public void Dispose()
    {
        this.navigationManager?.LocationChanged -= this.NavigationManager_LocationChanged;
    }

    private void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        this.ShowViewByPath(e.Location, default);
    }
}
