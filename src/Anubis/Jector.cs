//HADAL AHBEK

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Anubis
{
    /// <summary>
    /// An abstract class that outlines the methods used for performing steganographic operations.
    /// </summary>
    /// <remarks>
    /// The <see cref="Jector"/> implementations typically hides data in the input by manipulating the least significat bits of the input media.
    /// </remarks>
    public abstract class Jector
    {
        private int _lsbDepth = 2;

        /// <summary>
        /// The default, hardcoded end of file string.
        /// </summary>
        public const string EOF_STRING = "#$%-";

        ///<summary>
        ///Gets or sets the Least Significant Bit writing depth.
        ///</summary>
        /// <remarks>
        /// By default, the value is 2. Increasing the value may allow hiding more data, but may lead to noticable artifacts in the produced output.
        /// Setting a value greater than 8 throws an <see cref="ArgumentException"/>.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if value is greater than 8</exception>
        public int LsbDepth
        {
            get => _lsbDepth;
            set
            {
                if (value > 8)
                {
                    throw new ArgumentException("Lsb Depth cannot be greater than 8", nameof(value));
                }
                else
                {
                    _lsbDepth = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the end of file indicator
        /// </summary>
        /// <remarks>
        /// When injecting data into an input source, the value is added to indicate the correct end of the data.
        /// When ejecting, the value will be checked against the output and the output truncated where the end of file started.
        /// The default end of file used <see cref="EOF_STRING"/> as UTF8 bytes.
        /// It is important to select a unique byte sequence that does not exist in the input data, as this may otherwise lead to returning incomplete data during the Eject operation.
        /// </remarks>
        public byte[] Eof { get; set; } = EOF_STRING.GetBytes<UTF8Encoding>();

        /// <summary>
        /// Gets or sets a value indicating whether the steganographic process must be successful.
        /// </summary>
        /// <remarks>
        /// In a successful Injection, all the data, including the <see cref="Eof"/> if provided, must be completely written to the target input media.
        /// A successful Ejection requires that the <see cref="Eof"/> sequence, if provided, must exist in the data at which point the data is truncated.
        /// </remarks>
        public bool EnsureSuccess { get; set; }

        /// <summary>
        /// Injects the <paramref name="data"/> into the input media <paramref name="source"/>, writing the resulting output in the <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The input media, where the data will be hidden</param>
        /// <param name="destination">The stream where the output media, containing the hidden data will be written</param>
        /// <param name="data">The data to be hidden in the <paramref name="source"/></param>
        /// <param name="progress">The subscriber notified of the progress of the operation</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <remarks>
        /// The <paramref name="source"/> will remain unchanged after the operation. It is merely read from.
        /// </remarks>
        public abstract Task InjectAsync(Stream source, Stream destination, Stream data, IProgress<double> progress=null, CancellationToken cancellationToken=default);

        /// <summary>
        /// Ejects or extracts data hidden in the <paramref name="source"/> and writes it to the <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The input media containing the hidden data</param>
        /// <param name="destination">The stream where the extracted data will be written</param>
        /// <param name="progress">The subscriber notified of the progress of the operation</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        public abstract Task EjectAsync(Stream source, Stream destination, IProgress<double> progress=null, CancellationToken cancellationToken=default);

        /// <summary>
        /// Checks if the byte sequence of <paramref name="input"/> starting from <paramref name="index"/> matches the <see cref="Eof"/> byte sequence
        /// </summary>
        /// <param name="input">The input to check</param>
        /// <param name="index">The starting index along the input</param>
        /// <returns>True if it matches the <see cref="Eof"/>. Otherwise returns false</returns>
        protected bool IsEndOfFileSequence(byte[] input, int index)
        {
            try
            {
                foreach (byte b in Eof)
                {
                    if (b != input[index]) return false;
                    index++;
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

    }
}
