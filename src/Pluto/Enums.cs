using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Pluto
{
    /// <summary>
    /// The transaction type determines how the transaction affects the account on which it is posted
    /// </summary>
    public enum TransactionType 
    {
        /// <summary>
        /// Debit transactions, written on the debit side of accounts
        /// </summary>
        Debit, 
        /// <summary>
        /// Credit transactions, written on the credit side of accounts
        /// </summary>
        Credit 
    }

    /// <summary>
    /// Determines how a transaction affects an account, and how the account affects statements
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// Accounts for assets, such as Motor Vehicle.
        /// </summary>
        Asset,
        /// <summary>
        /// Accounts for liabilities, such as loans.
        /// </summary>
        Liability,
        /// <summary>
        /// Equity account.
        /// </summary>
        Equity,
        /// <summary>
        /// Accounts recording the business's creditors.
        /// </summary>
        Payable,
        /// <summary>
        /// Accounts recording the business's debtors.
        /// </summary>
        Receiveable,
        /// <summary>
        /// Income accounts, such as profit.
        /// </summary>
        Income,
        /// <summary>
        /// Expense accounts, such as rent.
        /// </summary>
        Expense,
        /// <summary>
        /// Account for money held as cash.
        /// </summary>
        Cash,
        /// <summary>
        /// Account for money held in the bank.
        /// </summary>
        Bank
    }
}
