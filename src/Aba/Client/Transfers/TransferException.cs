using System;

namespace Thismaker.Aba.Client.Transfers
{
    public class TransferException : Exception
    {
        public Transfer Transfer { get; set; }

        public TransferException(string message, Transfer transfer) : base(message)
        {
            Transfer = transfer;
        }

        public TransferException(string message, Transfer transfer, Exception innerException) : base(message, innerException)
        {
            Transfer = transfer;
        }

        public TransferException(Transfer transfer)
        {
            Transfer = transfer;
        }
    }
}
