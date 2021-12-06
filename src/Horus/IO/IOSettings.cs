using System.Text.Json;
using Thismaker.Horus.Symmetrics;

namespace Thismaker.Horus.IO
{
    /// <summary>
    /// A static class for specifying the cryptographic and serialization settings to be used by the <see cref="ICryptoFile"/> and <see cref="ICryptoContainer"/> instances
    /// </summary>
    public static class IOSettings
    {
        private static ISerializer _serializer;
        
        private static ISymmetric _algorithm;

        private readonly static ISymmetric _defaultAlgorithm = SymmetricBuilder.CreateAesFixed();

        private readonly static InternalSerializer _defaultSerializer = new InternalSerializer();

        /// <summary>
        /// The serializer that is used to convert an <see cref="object"/> to a data stream that will be stored and reverse.
        /// </summary>
        /// <remarks>
        /// If one is not specified, the default <see cref="JsonSerializer"/> from the <see cref="System.Text.Json"/> library is used.
        /// </remarks>
        public static ISerializer Serializer
        {
            internal get
            {
                return _serializer ?? _defaultSerializer;
            }
            set
            {
                _serializer = value;
            }
        }

        /// <summary>
        /// The <see cref="ISymmetric"/> algorithm that will be used 
        /// to encrypt and decrypt by the <see cref="ICryptoFile"/> and <see cref="ICryptoContainer"/> instances
        /// </summary>
        /// <remarks>
        /// If one is not manually assigned, the default algorithm is created using <see cref="SymmetricBuilder.CreateAesFixed"/> method
        /// </remarks>
        public static ISymmetric Algorithm
        {
            internal get
            {
                return _algorithm ?? _defaultAlgorithm;
            }
            set
            {
                _algorithm = value;
            }
        }
    }
}
