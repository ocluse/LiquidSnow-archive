using System.Windows;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Thismaker.Goro
{
    public class BindingProxy : Freezable
    {

        public BindingProxy() { }
        public BindingProxy(object value)
            => Value = value;

        protected override Freezable CreateInstanceCore()
            => new BindingProxy();

        #region Value Property

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(object),
            typeof(BindingProxy),
            new FrameworkPropertyMetadata(default));

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion Value Property
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member