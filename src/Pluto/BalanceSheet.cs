using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Pluto
{
    public class BalanceSheet
    {
        public ICollection<IPlutoAccount> Assets { get; set; }

        public ICollection<IPlutoAccount> Capital { get; set; }

        public ICollection<IPlutoAccount> Liabilities { get; set; }



        public static BalanceSheet PrepareBalanceSheet(IEnumerable<IPlutoAccount> accounts, bool clumpAccounts)
        {
            BalanceSheet result = new BalanceSheet()
            {
                Assets = new List<IPlutoAccount>(),
                Capital = new List<IPlutoAccount>(),
                Liabilities = new List<IPlutoAccount>(),
            };

            IPlutoAccount accountsReceiveable = null, accountsPayable = null;

            if (clumpAccounts)
            {
                accountsReceiveable = new PlutoAccount
                {
                    Id = Horus.Horus.GenerateId(Horus.IdKind.Standard),
                    Name = "Accounts Receiveable",
                    Transactions = new List<IPlutoTransaction>(),
                    Type = AccountType.Asset
                };

                accountsPayable = new PlutoAccount()
                {
                    Id = Horus.Horus.GenerateId(Horus.IdKind.Standard),
                    Name = "Accounts Payable",
                    Transactions = new List<IPlutoTransaction>(),
                    Type = AccountType.Liability
                };
            }

            foreach(IPlutoAccount account in accounts)
            {
                if (account.Type == AccountType.Asset)
                {
                    result.Assets.Add(account);
                }
                else if (account.Type == AccountType.Liability)
                {
                    result.Liabilities.Add(account);
                }
                else if (account.Type == AccountType.Expense)
                {

                }
                else if(account.Type == AccountType.Cash)
                {
                    result.Assets.Add(account);
                }
                else if (account.Type == AccountType.Payable)
                {
                    result.
                }
            }
        }
    }
}
