using TrailBlazr.Models;
using TrailBlazr.ViewModels;
using TrailBlazr.Views;

namespace TrailBlazr.Services;
public sealed class ViewManager(
    IEnumerable<ViewRegistration> viewRegistrations) : IViewManager
{
    private readonly List<ViewRegistration> viewRegistrations = [.. viewRegistrations];

    public event EventHandler<ViewRequest>? ShowViewRequested;

    public void ShowView<TView, TViewModel>(object? dataContext = null)
        where TView : ViewBase<TView, TViewModel>
        where TViewModel : ViewModelBase<TViewModel, TView>
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
        if (this.viewRegistrations.FirstOrDefault(r => r.ViewType == viewType) is not ViewRegistration registration)
        {
            throw new InvalidOperationException("View type not registered: " + viewType.FullName);
        }

        var viewRequest = new ViewRequest(registration.ViewType, registration.ViewModelType, dataContext);
        this.ShowViewRequested?.Invoke(this, viewRequest);
    }
}
