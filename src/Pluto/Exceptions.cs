using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Pluto
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AccountNotFoundException : InvalidOperationException
    {
        public AccountNotFoundException() { }
        public AccountNotFoundException(string message) : base(message) { }
        public AccountNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected AccountNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class InvoiceNotFoundException : InvalidOperationException
    {
        public InvoiceNotFoundException() { }
        public InvoiceNotFoundException(string message) : base(message) { }
        public InvoiceNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected InvoiceNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class TransactionNotFoundException : InvalidOperationException
    {
        public TransactionNotFoundException() { }
        public TransactionNotFoundException(string message) : base(message) { }
        public TransactionNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected TransactionNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class InvoiceItemNotFoundException : InvalidOperationException
    {
        public InvoiceItemNotFoundException() { }
        public InvoiceItemNotFoundException(string message) : base(message) { }
        public InvoiceItemNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected InvoiceItemNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
