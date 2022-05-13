using System;

namespace Thismaker.Aba.Client.Transfers
{
    /// <summary>
    /// An esception thrown when a transfer fails.
    /// </summary>
    public class TransferException : Exception
    {
        /// <summary>
        /// The transfer that invoked the failure.
        /// </summary>
        public Transfer Transfer { get; set; }
        /// <summary>
        /// Creates a new instance of the <see cref="TransferException"/>
        /// </summary>
        public TransferException(string message, Transfer transfer) : base(message)
        {
            Transfer = transfer;
        }
        /// <summary>
        /// Creates a new instance of the <see cref="TransferException"/>
        /// </summary>
        public TransferException(string message, Transfer transfer, Exception innerException) : base(message, innerException)
        {
            Transfer = transfer;
        }
        /// <summary>
        /// Creates a new instance of the <see cref="TransferException"/>
        /// </summary>
        public TransferException(Transfer transfer)
        {
            Transfer = transfer;
        }
    }
}
