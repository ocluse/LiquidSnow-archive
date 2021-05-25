using System;
using System.Diagnostics.Contracts;
using System.Windows;

namespace Thismaker.Goro
{
    /// <summary>
    /// A message box that is styled according to the current Goro styles.
    /// </summary>
    public partial class GoroMessageBox : Window
    {
        #region Static Methods

        /// <summary>
        /// Shows a simple message window with the provided message.
        /// </summary>
        public static void Show(string message)
        {
            Show(null, message);
        }

        /// <summary>
        /// Shows a simple message window with the provided message and title.
        /// </summary>
        public static void Show(string title, string message)
        {
            Show(title, message, MessageBoxButton.OK);
        }

        /// <summary>
        /// Shows a message window with the provided buttons and status icons, returning the result of the dialog
        /// </summary>
        /// <param name="message">The message body of the window</param>
        /// <param name="button">The buttons to show on the window</param>
        /// <param name="status">The status icon to show</param>
        public static bool? Show(string message, MessageBoxButton button, StatusInfo? status = null)
        {
            return Show(null, message, button, status);
        }

        /// <summary>
        /// Shows a message window, attaching a checkbox for other options
        /// </summary>
        /// <param name="message">The message body of the window</param>
        /// <param name="button">The buttons to show on the window</param>
        /// <param name="optionalText">The text of the optional checkbox</param>
        /// <param name="optionalResult">Returns the result of the check state of the checkbox</param>
        /// <param name="status">The status icon to show</param>
        public static bool? Show(string message,  MessageBoxButton button, string optionalText, out bool? optionalResult, StatusInfo? status=null)
        {
            return Show(null, message, button, optionalText, out optionalResult, status);
        }

        /// <summary>
        /// Shows a message window with the provided title and buttons
        /// </summary>
        /// <param name="title">The title of the window</param>
        /// <param name="message">The message body of the window</param>
        /// <param name="button">The buttons to show on the window</param>
        /// <param name="status">The status icon to show</param>
        public static bool? Show(string title, string message, MessageBoxButton button, StatusInfo? status=null)
        {
            var showSecondary= button != MessageBoxButton.OK;
            var showTertiary = button == MessageBoxButton.YesNoCancel;

            var primaryText = button == MessageBoxButton.OKCancel || button == MessageBoxButton.OK ? "OK" : "Yes";
            var secondaryText= button == MessageBoxButton.OKCancel ? "Cancel" : "No";
            var tertiaryText = "Cancel";
            return ShowPrivate(title, message, primaryText, secondaryText, tertiaryText, showSecondary, showTertiary, null, false, out _, status);
        }

        /// <summary>
        /// Shows a message window, attaching an optional checkbox.
        /// </summary>
        /// <param name="title">The title of the window</param>
        /// <param name="message">The message body of the window</param>
        /// <param name="button">The buttons to show on the window</param>
        /// <param name="optionalText">The text of the optional checkbox</param>
        /// <param name="optionalResult">Returns the IsChecked state of the optional checkbox</param>
        /// <param name="status">The status icon</param>
        public static bool? Show(string title, string message, MessageBoxButton button, string optionalText, out bool? optionalResult, StatusInfo? status = null)
        {
            var showSecondary = button != MessageBoxButton.OK;
            var showTertiary = button == MessageBoxButton.YesNoCancel;

            var primaryText = button == MessageBoxButton.OKCancel || button == MessageBoxButton.OK ? "OK" : "Yes";
            var secondaryText = button == MessageBoxButton.OKCancel ? "Cancel" : "No";
            var tertiaryText = "Cancel";
            return ShowPrivate(title, message, primaryText, secondaryText, tertiaryText, showSecondary, showTertiary, optionalText, true, out optionalResult, status);
        }

        /// <summary>
        /// Shows a message window with only a primary button.
        /// </summary>
        /// <param name="title">The title of the message window</param>
        /// <param name="message">The message body of the window</param>
        /// <param name="primaryText">The text to show on the primary button</param>
        /// <param name="status">The status icon</param>
        public static void Show(string title, string message, string primaryText, StatusInfo? status = null)
        {
            ShowPrivate(title, message, primaryText, null, null, false, false, null, false, out _, status);
        }

        /// <summary>
        /// Shows a message window with only a primary button and attached optional checkbox
        /// </summary>
        /// <param name="title">The title of the window</param>
        /// <param name="message">The message body of the window</param>
        /// <param name="primaryText">The text of the primary button</param>
        /// <param name="optionalText">The text of optional checkbox</param>
        /// <param name="optionalResult">Returns the IsChecked state of the optional checkbox</param>
        /// <param name="status">The status icon</param>
        public static void Show(string title, string message, string primaryText, string optionalText, out bool? optionalResult, StatusInfo? status = null)
        {
            ShowPrivate(title, message, primaryText, null, null, false, false, optionalText, true, out optionalResult, status);
        }

        /// <summary>
        /// Shows a message window.
        /// </summary>
        /// <param name="title">The title of the message window</param>
        /// <param name="message">The message body</param>
        /// <param name="primaryText">The text on the primary button</param>
        /// <param name="secondaryText">The text on the secondary button</param>
        /// <param name="status">The status icon</param>
        public static bool? Show(string title, string message, string primaryText, string secondaryText, StatusInfo? status = null)
        {
            return ShowPrivate(title, message, primaryText, secondaryText, null, true, false, null, false, out _, status);
        }

        /// <summary>
        /// Shows a message window with an optional checkbox
        /// </summary>
        /// <param name="title">The title of the message window</param>
        /// <param name="message">The message body</param>
        /// <param name="primaryText">The text on the primary button</param>
        /// <param name="secondaryText">The text on the secondary button</param>
        /// <param name="optionalText">The text of optional checkbox</param>
        /// <param name="optionalResult">Returns the IsChecked state of the optional checkbox</param>
        /// <param name="status">The status icon</param>
        public static bool? Show(string title, string message, string primaryText, string secondaryText, string optionalText, out bool? optionalResult, StatusInfo? status = null)
        {
            return ShowPrivate(title, message, primaryText, secondaryText, null, true, false, optionalText, true, out optionalResult, status);
        }

        /// <summary>
        /// Shows a message window.
        /// </summary>
        /// <param name="title">The title of the message window</param>
        /// <param name="message">The message body</param>
        /// <param name="primaryText">The text on the primary button</param>
        /// <param name="secondaryText">The text on the secondary button</param>
        /// <param name="tetiaryText">The text on the tertiary button</param>
        /// <param name="status">The status icon</param>
        public static bool? Show(string title, string message, string primaryText, string secondaryText, string tetiaryText, StatusInfo? status=null)
        {
            return ShowPrivate(title, message, primaryText, secondaryText, tetiaryText, true, true, null, false, out _, status);
        }

        /// <summary>
        /// Shows a message window with an optional checkbox
        /// </summary>
        /// <param name="title">The title of the message window</param>
        /// <param name="message">The message body</param>
        /// <param name="primaryText">The text on the primary button</param>
        /// <param name="secondaryText">The text on the secondary button</param>
        /// <param name="tetiaryText">The text on the tertiary button</param>
        /// <param name="optionalText">The text of optional checkbox</param>
        /// <param name="optionalResult">Returns the IsChecked state of the optional checkbox</param>
        /// <param name="status">The status icon</param>
        public static bool? Show(string title, string message, string primaryText, string secondaryText, string tetiaryText, string optionalText, out bool? optionalResult, StatusInfo? status = null)
        {
            return ShowPrivate(title, message, primaryText, secondaryText, tetiaryText, true, true, optionalText, true, out optionalResult, status);
        }

        //The private method that can be configured any which way
        private static bool? ShowPrivate(string title, string message, string primaryText, string secondaryText, string tertiaryText, bool showSecondary, bool showTertiary, string optionalText, bool showOptional, out bool? optionalResult, StatusInfo? status)
        {
            if (string.IsNullOrEmpty(title)) title = " ";
            var dlg = new GoroMessageBox
            {
                Title = title,
            };

            dlg.ContentText.Text = message;
            dlg.BtnPrimary.Content = primaryText;
            dlg.BtnSecondary.Content = secondaryText;
            dlg.BtnTertiary.Content = tertiaryText;
            dlg.OptionalParameter.Content = optionalText;

            dlg.BtnSecondary.Visibility = showSecondary.ToVisibility();
            dlg.BtnTertiary.Visibility = showTertiary.ToVisibility();
            dlg.OptionalParameter.Visibility = showOptional.ToVisibility();
            
            if (status.HasValue)
            {
                dlg.Status.Visibility = Visibility.Visible;
                dlg.Status.Status = status.Value;
            }
            else
            {
                dlg.Status.Visibility = Visibility.Collapsed;
            }

            var result= dlg.ShowDialog();

            if(showTertiary && !dlg.SecondaryClosure && result.HasValue && !result.Value)
            {
                result = null;
            }

            optionalResult = dlg.OptionalParameter.IsChecked;
            return result; 
        }

        #endregion

        #region Instance Methods

        bool SecondaryClosure { get; set; } = false;

        private GoroMessageBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnBtnPrimary(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnBtnSecondary(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            SecondaryClosure = true;
            Close();
        }

        private void OnBtnTertiary(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            Close();
        }

        #endregion
    }
}
