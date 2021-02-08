using System.Windows;
using System.Windows.Controls;

namespace Thismaker.Goro
{
    public class StatusIndicator : Control
    {
        static StatusIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusIndicator), new FrameworkPropertyMetadata(typeof(StatusIndicator)));
        }

        public static readonly DependencyProperty StatusProperty
            = DependencyProperty.Register(nameof(Status), typeof(StatusInfo), typeof(StatusIndicator), new PropertyMetadata(StatusInfo.Information));

        public static readonly DependencyProperty DesignProperty
            = DependencyProperty.Register(nameof(Design), typeof(IconDesign), typeof(StatusIndicator), new PropertyMetadata(IconDesign.Segoe));

        public StatusInfo Status
        {
            get { return (StatusInfo)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public IconDesign Design
        {
            get { return (IconDesign)GetValue(DesignProperty); }
            set { SetValue(DesignProperty, value); }
        }
    }
    
    public enum StatusInfo
    {
        Success,
        Warning,
        Information,
        Error
    }

}