namespace TrailBlazr.Models;

public abstract class ViewRegistration(Type viewType, Type viewModelType)
{
    public Type ViewType { get; } = viewType ?? throw new ArgumentNullException(nameof(viewType));
    public Type ViewModelType { get; } = viewModelType ?? throw new ArgumentNullException(nameof(viewModelType));
}

public sealed class ViewRegistration<TView, TViewModel>() : ViewRegistration(typeof(TView), typeof(TViewModel))
    where TView : Views.ViewBase<TView, TViewModel>
    where TViewModel : ViewModels.ViewModelBase<TViewModel, TView>
{
    public override string ToString() => $"({this.ViewType.Name} - {this.ViewModelType.Name})";
}
