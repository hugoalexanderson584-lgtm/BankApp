using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using BankApp.Enums;
namespace BankApp
{
    internal class TransactionService
    {
        private readonly List<Transactions> _Transactions = new();

        

        public Transactions CreateTransaction(string senderAccountId, string receiverAccountId, decimal amount, Currency currency)
        {
            var transaction = new Transactions(senderAccountId, receiverAccountId, amount, currency);

            _Transactions.Add(transaction);

            return transaction;
        }
        
    }
}
