
namespace System.Windows
{
    public static class SystemWindows
    {
        public static Visibility ToVisibility(this bool state)
        {
            return state ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
