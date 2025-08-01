using TrailBlazr.Models;
using TrailBlazr.ViewModels;
using TrailBlazr.Views;

namespace TrailBlazr.Services;
public interface IViewManager
{
    event EventHandler<ViewRequest>? ShowViewRequested;

    void ShowView<TView, TViewModel>(object? dataContext = null)
        where TView : ViewBase<TView, TViewModel>
        where TViewModel : ViewModelBase<TViewModel, TView>;

    void ShowView(Type viewType, object? dataContext = null);
}
