///HADAL AHBEK

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Anubis
{
    public abstract class Jector
    {
        /// <summary>
        /// The depth of the Least Significant Bit, increasing the value may allow storing bigger files,
        /// but may lead to noticable artifacts in the produced data. Value cannot be greater than 8
        /// </summary>
        public int LsbDepth { get; set; } = 2;

        /// <summary>
        /// The string that will be used to signify an End of File, if not assigned, 
        /// a default value will be used.
        /// When calling <see cref="Inject(Bitmap, byte[])"/> the string is added at the
        /// end of the file before infusing the data
        /// During <see cref="Eject(Bitmap)"/> the string is checked at the end of the obtained data, 
        /// the returned data is truncated at where the EOF was found.
        /// If the value is null, the EOF will not be added at the end nor will it be checked.
        /// </summary>
        public string EOF { get; set; }
        = "#$%-";

        /// <summary>
        /// The EOF as a byte array, using UTF8 Encoding
        /// </summary>
        protected byte[] Sign
        {
            get { return Encoding.UTF8.GetBytes(EOF); }
        }

        /// <summary>
        /// When <see cref="true"/> throws <see cref="InvalidOperationException"/> 
        /// when the EOF cannot be written or was not read.
        /// </summary>
        public bool EnsureSuccess { get; set; } = false;

        /// <summary>
        /// Injects data to a source stream, saving the data to the specified destination stream.
        /// </summary>
        /// <param name="source">The stream of the plain data</param>
        /// <param name="destination">The stream where the written data will be saved to</param>
        /// <param name="data">The stream with the data to be written</param>
        /// <param name="progress">Where to report the <b>data writing</b> progress,</param>
        /// <param name="cancellationToken">A way to cancel the write process</param>
        /// <returns></returns>
        public abstract Task InjectAsync(Stream source, Stream destination, Stream data, IProgress<float> progress=null, CancellationToken cancellationToken=default);

        /// <summary>
        /// Seeks data hidden in a stream, saving it to the destination.
        /// </summary>
        /// <param name="source">The source stream with hidden data</param>
        /// <param name="destination">The stream where the data will be written to</param>
        /// <param name="progress">The progress of <b>data reading</b> progress</param>
        /// <param name="cancellationToken">A way to cancel the read process</param>
        /// <returns></returns>
        public abstract Task EjectAsync(Stream source, Stream destination, IProgress<float> progress=null, CancellationToken cancellationToken=default);

        protected bool IsSignature(byte[] input, int index)
        {
            try
            {
                foreach (var b in Sign)
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
