using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Client.Transfers
{
    public class TransferManagerException : Exception
    {
        public TransferManagerException(string message) : base(message)
        {

        }

        public TransferManagerException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public TransferManagerException()
        {

        }
    }
}
