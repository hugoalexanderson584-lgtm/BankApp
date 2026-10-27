using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using BankApp.Enums;
namespace BankApp
{
    public class TransferService
    {
        private readonly List<Transactions> _Transactions = new();

        

        public Transactions CreateTransaction(string senderAccountId, string receiverAccountId, decimal amount, Currency currency, Guid id)
        {
            var transaction = new Transactions(senderAccountId, receiverAccountId, amount, currency, id);

            _Transactions.Add(transaction);

            return transaction;
        }
        public void CompleteTransaction(Transactions transaction)
        {
            transaction.MarkCompleted();
        }
        public void FailTransaction(Transactions transaction)
        {
            transaction.MarkFailed();
        }

        public Transactions GetTransaction(Guid id)
        {
            return _Transactions.FirstOrDefault(t => t.Id == id);
        }

    }
}
