using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Pluto
{
    /// <summary>
    /// The transaction type determines how the transaction affects the account on which it is posted
    /// </summary>
    public enum TransactionType { Debit, Credit }

    /// <summary>
    /// Determines how a transaction affects an account, and how the account affects statements
    /// </summary>
    public enum AccountType
    {
        Asset,
        Liability,
        Equity,
        Payable,
        Receiveable,
        Income,
        Expense,
        Cash,
        Bank
    }
}
