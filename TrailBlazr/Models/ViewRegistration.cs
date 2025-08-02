using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing.Template;
using System.Reflection;

namespace TrailBlazr.Models;

public abstract class ViewRegistration(Type viewType, Type viewModelType, IReadOnlyList<TemplateMatcher> routes)
{
    public Type ViewType { get; } = viewType ?? throw new ArgumentNullException(nameof(viewType));
    public Type ViewModelType { get; } = viewModelType ?? throw new ArgumentNullException(nameof(viewModelType));
    public IReadOnlyList<TemplateMatcher> Routes { get; } = routes ?? throw new ArgumentNullException(nameof(routes));
}

public sealed class ViewRegistration<TView, TViewModel>()
    : ViewRegistration(
        typeof(TView),
        typeof(TViewModel),
        [.. typeof(TView).GetCustomAttributes<RouteAttribute>().Select(r => r.Template).Select(r => new TemplateMatcher(TemplateParser.Parse(r), []))])
    where TView : Views.ViewBase<TView, TViewModel>
    where TViewModel : ViewModels.ViewModelBase<TViewModel, TView>
{
    public override string ToString() => $"({this.ViewType.Name} - {this.ViewModelType.Name} ({string.Join(", ", this.Routes.Select(t => t.Template))}))";
}
