using System.ComponentModel;
using TrailBlazr.Views;

namespace TrailBlazr.ViewModels;

public abstract class ViewModelBase<TViewModel, TView> : INotifyPropertyChanged, IDisposable
    where TViewModel : ViewModelBase<TViewModel, TView>
    where TView : ViewBase<TView, TViewModel>
{
    private bool disposed = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected TView? View
    {
        get => field;
        private set
        {
            field = value;
            this.NotifyPropertyChanged(nameof(this.View));
        }
    }

    internal void SetView(TView view)
    {
        this.View = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void RefreshView()
    {
        this.View?.RefreshView();
    }

    public ValueTask RefreshViewAsync()
    {
        return this.View?.RefreshViewAsync() ?? ValueTask.CompletedTask;
    }

    public void NotifyPropertyChanged(string propertyName)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual ValueTask Initialize(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public virtual ValueTask ParametersSet(TView view, CancellationToken cancellationToken) => ValueTask.CompletedTask;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.View = null;
                this.PropertyChanged = null;
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
