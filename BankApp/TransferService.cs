using BankApp.Enums;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Transactions;
namespace BankApp
{
    public class TransferService
    {
        private readonly List<Transactions> _Transactions = new();
        private readonly List<BankAccount> _BankAccounts = new();
        public TransferService(List<BankAccount> accounts)
        {
            _BankAccounts = accounts;
        }



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

        public void TransferMoney(string senderAccountId, string receiverAccountId, decimal amount, Currency currency, Guid id)
        {
            
        }

    }
}
