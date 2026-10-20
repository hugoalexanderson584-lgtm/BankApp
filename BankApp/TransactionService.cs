using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace BankApp
{
    internal class TransactionService
    {
        private readonly List<Transactions> _Transactions = new();

        

        public Transactions CreateTransaction(string senderAccountId, string receiverAccountId, decimal amount)
        {
            var transaction = new Transactions(senderAccountId, receiverAccountId, amount);

            _Transactions.Add(transaction);

            return transaction;
        }
        
    }
}
