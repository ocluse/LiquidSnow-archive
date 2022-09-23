using System.Windows.Media;
using Thismaker.Goro.Utilities;

namespace Thismaker.Goro
{
    /// <summary>
    /// A list of predefined static colors that can be used in Goro
    /// </summary>
    public static class DefaultColors
    {
        static DefaultColors()
        {
            SpectreOriginal = ColorUtility.Parse("#7F47DD");
            Briliet = ColorUtility.Parse("#39B54A");
            LaSectur = ColorUtility.Parse("#00E2D1");
            Hibiscus = ColorUtility.Parse("#ED0F64");
            Oros = ColorUtility.Parse("#F4A800");
            ThismakerOfficial = ColorUtility.Parse("#0681FC");
        }

        /// <summary>
        /// The Official shade of Thismaker. It's a very mute blue
        /// </summary>
        public static Color ThismakerOfficial { get; private set; }

        /// <summary>
        /// The default color of the Spectre Series. Purplish.
        /// </summary>
        public static Color SpectreOriginal { get; private set; }

        /// <summary>
        /// The colors of Briliet, and Omondi, and is therefore green.
        /// </summary>
        public static Color Briliet { get; private set; }

        /// <summary>
        /// A pronounced troquise/cyan color of the LaSectur
        /// </summary>
        public static Color LaSectur { get; private set; }

        /// <summary>
        /// Warm and beutiful, a magenta color. It would smell noice too.
        /// </summary>
        public static Color Hibiscus { get; private set; }

        /// <summary>
        /// Gold. Gold. Gold.
        /// </summary>
        public static Color Oros { get; private set; }
    }
}
