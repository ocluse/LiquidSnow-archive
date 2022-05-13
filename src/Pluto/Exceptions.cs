using System;

namespace Thismaker.Pluto
{
    /// <summary>
    /// Thrown when an account is not found.
    /// </summary>
    [Serializable]
    public class AccountNotFoundException : InvalidOperationException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AccountNotFoundException() { }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AccountNotFoundException(string message) : base(message) { }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AccountNotFoundException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        protected AccountNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an invoice is not found.
    /// </summary>
    [Serializable]
    public class InvoiceNotFoundException : InvalidOperationException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public InvoiceNotFoundException() { }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public InvoiceNotFoundException(string message) : base(message) { }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public InvoiceNotFoundException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        protected InvoiceNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when a transaction is not found.
    /// </summary>
    [Serializable]
    public class TransactionNotFoundException : InvalidOperationException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public TransactionNotFoundException() { }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public TransactionNotFoundException(string message) : base(message) { }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public TransactionNotFoundException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        protected TransactionNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an invoice item is not found.
    /// </summary>
    [Serializable]
    public class InvoiceItemNotFoundException : InvalidOperationException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public InvoiceItemNotFoundException() { }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public InvoiceItemNotFoundException(string message) : base(message) { }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public InvoiceItemNotFoundException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        protected InvoiceItemNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
