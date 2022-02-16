using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Thismaker.Goro
{
    public class BindingTrigger : INotifyPropertyChanged
    {

        public BindingTrigger()
            => Binding = new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(Value))
            };

        public event PropertyChangedEventHandler? PropertyChanged;

        public Binding Binding { get; }

        public void Refresh()
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));

        public object? Value { get; }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member