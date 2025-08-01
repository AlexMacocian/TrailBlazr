using System.ComponentModel;
using TrailBlazr.Views;

namespace TrailBlazr.ViewModels;

public abstract class ViewModelBase<TViewModel, TView> : INotifyPropertyChanged
    where TViewModel : ViewModelBase<TViewModel, TView>
    where TView : ViewBase<TView, TViewModel>
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public object? DataContext
    {
        get => field;
        set
        {
            field = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.DataContext)));
        }
    }

    protected TView? View
    {
        get => field;
        private set
        {
            field = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.View)));
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

    public virtual ValueTask Initialize(CancellationToken cancellationToken) => ValueTask.CompletedTask;
}
