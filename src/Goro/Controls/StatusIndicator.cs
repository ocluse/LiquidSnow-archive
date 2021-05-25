using System.Windows;
using System.Windows.Controls;

namespace Thismaker.Goro
{
    /// <summary>
    /// A control that can be used to show statuses as icons
    /// </summary>
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

        /// <summary>
        /// The status state of the indicator
        /// </summary>
        public StatusInfo Status
        {
            get { return (StatusInfo)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        /// <summary>
        /// The icon design of the status indicator
        /// </summary>
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