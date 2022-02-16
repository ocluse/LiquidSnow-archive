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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty StatusProperty
            = DependencyProperty.Register(nameof(Status), typeof(StatusInfo), typeof(StatusIndicator), new PropertyMetadata(StatusInfo.Information));

        public static readonly DependencyProperty DesignProperty
            = DependencyProperty.Register(nameof(Design), typeof(IconDesign), typeof(StatusIndicator), new PropertyMetadata(IconDesign.Segoe));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// The status state of the indicator
        /// </summary>
        public StatusInfo Status
        {
            get => (StatusInfo)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        /// <summary>
        /// The icon design of the status indicator
        /// </summary>
        public IconDesign Design
        {
            get => (IconDesign)GetValue(DesignProperty);
            set => SetValue(DesignProperty, value);
        }
    }
    
    /// <summary>
    /// Represents the state of Status
    /// </summary>
    public enum StatusInfo
    {
        /// <summary>
        /// Used when an operation is succesful
        /// </summary>
        Success,
        
        /// <summary>
        /// Used when there is a warning on an operation
        /// </summary>
        Warning,
        
        /// <summary>
        /// Used when there is information on an operation
        /// </summary>
        Information,
        
        /// <summary>
        /// Used when an operation failss
        /// </summary>
        Error
    }

}