using Microsoft.AspNetCore.Routing;

namespace TrailBlazr.Models;
public sealed record ViewRequest(Type ViewType, Type ViewModelType, RouteValueDictionary RouteValues)
{
}
