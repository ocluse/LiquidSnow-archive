using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace Thismaker.Goro
{
    public static class ControlExtensions
    {
        #region Header
        public static readonly DependencyProperty HeaderProperty=
            DependencyProperty.RegisterAttached("Header", typeof(string), typeof(ControlExtensions), new PropertyMetadata());


        public static void SetHeader(UIElement element, string header)
        {
            element.SetValue(HeaderProperty, header);
        }

        public static string GetHeader(UIElement element)
        {
            return (string)element.GetValue(HeaderProperty);
        }
        #endregion

        #region Placeholder
        public static readonly DependencyProperty PlaceholderTextProperty =
           DependencyProperty.RegisterAttached("PlaceholderText", typeof(string), typeof(ControlExtensions), new PropertyMetadata());


        public static void SetPlaceholderText(UIElement element, string header)
        {
            element.SetValue(PlaceholderTextProperty, header);
        }

        public static string GetPlaceholderText(UIElement element)
        {
            return (string)element.GetValue(PlaceholderTextProperty);
        }
        #endregion

        #region PasswordLength
        internal static readonly DependencyPropertyKey PasswordLengthKey =
            DependencyProperty.RegisterAttachedReadOnly("PasswordLength", typeof(int), typeof(ControlExtensions), new PropertyMetadata(0));

        public static readonly DependencyProperty PasswordLengthProperty =
            PasswordLengthKey.DependencyProperty;

        public static int GetPasswordLength(UIElement element)
        {
            return ((PasswordBox)element).Password.Length;
        }
        #endregion

        #region Icon
        public static readonly DependencyProperty IconProperty =
           DependencyProperty.RegisterAttached("Icon", typeof(Icon), typeof(ControlExtensions), new PropertyMetadata(Icon.None));

        public static void SetIcon(UIElement element, Icon icon)
        {
            element.SetValue(IconProperty, icon);
        }

        public static Icon GetIcon(UIElement element)
        {
            return (Icon)element.GetValue(IconProperty);
        }
        #endregion

        #region Design
        public static readonly DependencyProperty DesignProperty =
           DependencyProperty.RegisterAttached("Design", typeof(IconDesign), typeof(ControlExtensions), new PropertyMetadata(IconDesign.Segoe));

        public static void SetDesign(UIElement element, IconDesign Design)
        {
            element.SetValue(DesignProperty, Design);
        }

        public static IconDesign GetDesign(UIElement element)
        {
            return (IconDesign)element.GetValue(DesignProperty);
        }
        #endregion

    }

    public class InputMonitor : DependencyObject
    {
        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(InputMonitor), new UIPropertyMetadata(false, OnIsMonitoringChanged));



        public static bool GetIsEmpty(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEmptyProperty);
        }

        public static void SetIsEmpty(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEmptyProperty, value);
        }

        public static readonly DependencyProperty IsEmptyProperty =
            DependencyProperty.RegisterAttached("IsEmpty", typeof(bool), typeof(InputMonitor), new UIPropertyMetadata(true));

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
            {
                return;
            }
            if ((bool)e.NewValue)
            {
                if (d.GetType() == typeof(RichTextBox))
                {
                    ((RichTextBox)d).TextChanged += InputChanged;
                }
                else if (d.GetType() == typeof(TextBox))
                {
                    ((TextBox)d).TextChanged += InputChanged;
                }
                else if(d.GetType()==typeof(PasswordBox))
                {
                    ((PasswordBox)d).PasswordChanged += InputChanged;
                }
            }
            else
            {

                if (d.GetType() == typeof(RichTextBox))
                {
                    ((RichTextBox)d).TextChanged -= InputChanged;
                }
                else if (d.GetType() == typeof(TextBox))
                {
                    ((TextBox)d).TextChanged -= InputChanged;
                }
                else if (d.GetType() == typeof(PasswordBox))
                {
                    ((PasswordBox)d).PasswordChanged -= InputChanged;
                }
            }
        }

        static void InputChanged(object sender, RoutedEventArgs e)
        {
            bool isEmpty;


            if (sender.GetType() == typeof(TextBox))
            {
                isEmpty = string.IsNullOrEmpty(((TextBox)sender).Text);
            }
            else if (sender.GetType() == typeof(RichTextBox))
            {
                isEmpty = ((RichTextBox)sender).Document.IsEmpty();
            }
            else if(sender.GetType()==typeof(PasswordBox))
            {
                isEmpty = string.IsNullOrEmpty(((PasswordBox)sender).Password);
            }
            else
            {
                return;
            }

            SetIsEmpty((DependencyObject)sender, isEmpty);

        }
    }
}