namespace TrailBlazr.Models;
public sealed record ViewRequest(Type ViewType, Type ViewModelType, object? DataContext)
{
}
