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

    public void ShowView<TView>(params (string, object)[] routeValues)
        where TView : ComponentBase
    {
        var viewType = typeof(TView);
        this.ShowViewByType(viewType, routeValues);
    }

    public void ShowView(Type viewType, params (string, object)[] routeValues)
    {
        this.ShowViewByType(viewType, routeValues);
    }

    public void ShowView(string path, params (string, object)[] routeValues)
    {
        this.ShowViewByPath(path, routeValues);
    }

    private void ShowViewByType(Type viewType, params (string, object)[] routeValues)
    {
        var viewRegistrations = this.serviceProvider.GetServices<ViewRegistration>();
        routeValues ??= [];
        if (this.navigationManager is null)
        {
            return;
        }

        if (viewRegistrations.FirstOrDefault(r => r.ViewType == viewType) is not ViewRegistration registration)
        {
            throw new InvalidOperationException($"View type not registered: {viewType.FullName}");
        }

        if (registration.Routes.Count == 0)
        {
            throw new InvalidOperationException($"View type not registered with any routes: {viewType.FullName}");
        }

        var template = registration.Routes[0].Template.TemplateText;
        var parameters = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in routeValues)
        {
            var placeholder = $"{{{kvp.Item1}}}";

            if (template.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
            {
                template = template.Replace(placeholder, Uri.EscapeDataString(kvp.Item2?.ToString() ?? string.Empty));
            }
            else
            {
                parameters[kvp.Item1] = kvp.Item2;
            }
        }

        if (template.Contains('{'))
        {
            throw new InvalidOperationException($"Missing route values for template '{template}'");
        }

        var uri = this.navigationManager.GetUriWithQueryParameters(template, parameters);
        this.navigationManager.NavigateTo(uri);
    }

    private void ShowViewByPath(string path, (string, object)[]? routeValues)
    {
        var uriObj = path.StartsWith('/') ? new Uri(path, UriKind.Relative) : new Uri(path, UriKind.Absolute);
        var parameters = new RouteValueDictionary(StringComparer.OrdinalIgnoreCase);
        foreach(var param in routeValues ?? [])
        {
            parameters[param.Item1] = param.Item2;
        }

        var query = uriObj.IsAbsoluteUri 
            ? uriObj.Query
            : path.Contains('?')
                ? path[(path.IndexOf('?') + 1)..]
                : string.Empty;
        var finalPath = uriObj.IsAbsoluteUri ? uriObj.PathAndQuery.Split('?').FirstOrDefault() ?? string.Empty : path.Split('?').FirstOrDefault() ?? string.Empty;
        foreach (var kvp in QueryHelpers.ParseNullableQuery(query) ?? [])
        {
            parameters[kvp.Key] = kvp.Value.Count == 1 ? kvp.Value[0] : kvp.Value;
        }

        var viewRegistrations = this.serviceProvider.GetServices<ViewRegistration>();
        if (viewRegistrations.FirstOrDefault(r => r.Routes.FirstOrDefault(t => {
                var parsedRouteValues = new RouteValueDictionary();
                if (t.TryMatch(finalPath, parsedRouteValues))
                {
                    foreach(var kvp in parsedRouteValues)
                    {
                        parameters[kvp.Key] = kvp.Value;
                    }

                    return true;
                }

                return false;
            }) is not null) is not ViewRegistration registration)
        {
            throw new InvalidOperationException($"Path not found: {finalPath}");
        }

        this.ShowViewInner(registration.ViewType, registration.ViewModelType, parameters);
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
