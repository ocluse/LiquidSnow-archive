namespace System.Windows
{
    /// <summary>
    /// Extensions for System.Windows
    /// </summary>
    public static class SystemWindows
    {
        /// <summary>
        /// Converts a bool to a visibility value.
        /// </summary>
        /// <remarks>
        /// True becomes <see cref="Visibility.Visible"/> and false becomes <see cref="Visibility.Collapsed"/>.
        /// </remarks>
        public static Visibility ToVisibility(this bool state)
        {
            return state ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
