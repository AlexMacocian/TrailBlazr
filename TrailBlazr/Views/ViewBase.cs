using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using TrailBlazr.Components;
using TrailBlazr.ViewModels;

namespace TrailBlazr.Views;
public abstract class ViewBase<TView, TViewModel> : ComponentBase, IDisposable
    where TView : ViewBase<TView, TViewModel>
    where TViewModel : ViewModelBase<TViewModel, TView>
{
    public readonly static TimeSpan InitializationTimeout = TimeSpan.FromSeconds(5);
    private bool disposed = false;

    [Inject]
    public required TViewModel ViewModel { get; set; }
    [Inject]
    public required ILogger<TView> Logger { get; set; }
    [CascadingParameter]
    public required ViewContainer ViewContainer { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (!firstRender)
        {
            return;
        }

        if (this is not TView view)
        {
            throw new InvalidOperationException($"View is not of type {typeof(TView).Name}. Actual type: {this.GetType().Name}");
        }

        if (this.ViewContainer is null)
        {
            throw new InvalidOperationException("ViewContainer is not set. Ensure that this view is placed within a ViewContainer component");
        }

        this.ViewModel.SetView(view);
        using var initializationCts = new CancellationTokenSource(InitializationTimeout);
        _ = this.ViewModel ?? throw new InvalidOperationException("Cannot initialize ViewModel. ViewModel is null");
        try
        {
            await this.ViewModel.Initialize(initializationCts.Token);
        }
        catch (Exception ex)
        {
            this.Logger?.LogError(ex, "Failed to initialize ViewModel {ViewModelType}", typeof(TViewModel).Name);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (this is not TView view)
        {
            throw new InvalidOperationException($"View is not of type {typeof(TView).Name}. Actual type: {this.GetType().Name}");
        }

        using var initializationCts = new CancellationTokenSource(InitializationTimeout);
        await this.ViewModel.ParametersSet(view, initializationCts.Token);
    }

    internal void RefreshView()
    {
        _ = this.InvokeAsync(this.StateHasChanged);
    }

    internal async ValueTask RefreshViewAsync()
    {
        await this.InvokeAsync(this.StateHasChanged);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.ViewModel = null!;
                this.Logger = null!;
                this.ViewContainer = null!;
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}

