using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Client.Transfers
{
    public class TransferException : Exception
    {
        public TransferException(string message) : base(message)
        {

        }

        public TransferException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public TransferException()
        {

        }
    }
}
