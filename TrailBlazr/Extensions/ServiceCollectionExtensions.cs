using Microsoft.Extensions.DependencyInjection;
using TrailBlazr.Models;
using TrailBlazr.Services;
using TrailBlazr.ViewModels;
using TrailBlazr.Views;

namespace TrailBlazr.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTrailBlazr(this IServiceCollection services)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.AddSingleton<IViewManager, ViewManager>();
        return services;
    }

    public static IServiceCollection RegisterView<TView, TViewModel>(this IServiceCollection services, bool isSingleton = false)
        where TView : ViewBase<TView, TViewModel>
        where TViewModel : ViewModelBase<TViewModel, TView>
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        if (isSingleton)
        {
            services.AddSingleton<TView>();
            services.AddSingleton<TViewModel>();
        }
        else
        {
            services.AddScoped<TView>();
            services.AddScoped<TViewModel>();
        }

        services.AddSingleton<ViewRegistration>(new ViewRegistration<TView, TViewModel>());
        return services;
    }
}
