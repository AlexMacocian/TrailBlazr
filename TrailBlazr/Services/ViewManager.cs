using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using TrailBlazr.Models;

namespace TrailBlazr.Services;
public sealed class ViewManager(
    IServiceProvider serviceProvider) : IViewManager
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public event EventHandler<ViewRequest>? ShowViewRequested;

    public void ShowView<TView>(object? dataContext = null)
        where TView : ComponentBase
    {
        var viewType = typeof(TView);
        this.ShowViewInner(viewType, dataContext);
    }

    public void ShowView(Type viewType, object? dataContext = null)
    {
        this.ShowViewInner(viewType, dataContext);
    }

    private void ShowViewInner(Type viewType, object? dataContext = null)
    {
        var viewRegistrations = this.serviceProvider.GetServices<ViewRegistration>();
        if (viewRegistrations.FirstOrDefault(r => r.ViewType == viewType) is not ViewRegistration registration)
        {
            throw new InvalidOperationException("View type not registered: " + viewType.FullName);
        }

        var viewRequest = new ViewRequest(registration.ViewType, registration.ViewModelType, dataContext);
        this.ShowViewRequested?.Invoke(this, viewRequest);
    }
}
