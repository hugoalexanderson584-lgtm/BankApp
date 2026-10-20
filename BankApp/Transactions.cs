using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BankApp.Enums;
namespace BankApp
{
    
    public enum TransactionStatus
    {
        pending,
        completed,
        failed,

    }
    public class Transactions
    {

        public Guid Id { get; private set; }
        public string SenderAccountId { get; private set; }

        public string ReceiverAccountId { get; private set; }

        public decimal Amount { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public TransactionStatus Status { get; private set; }

        public Currency Currency { get; private set; }

        public Transactions(string senderAccountId, string receiverAccountId, decimal amount, Currency currency, Guid id)
        {
            // Arguments to make sure the user doesnt do something stupid 
            if (string.IsNullOrWhiteSpace(senderAccountId))
            {
                throw new ArgumentException("Sender is required.");
            }

            if (string.IsNullOrWhiteSpace(receiverAccountId))
            {
                throw new ArgumentException("Receiver is required.");
            }

            if (senderAccountId == receiverAccountId)
            {
                throw new ArgumentException("Sender and receiver must be different.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.");
            }

            SenderAccountId = senderAccountId;
            ReceiverAccountId = receiverAccountId;
            Amount = amount;
            CreatedAt = DateTime.Now;
            Status = TransactionStatus.pending;
            Currency = currency;
            Id = id;
        }
        public void MarkCompleted()
        {
            if (Status != TransactionStatus.pending)
                throw new InvalidOperationException(
                    "Only pending transactions can be completed.");

            Status = TransactionStatus.completed;
        }

        public void MarkFailed()
        {
            if (Status != TransactionStatus.pending)
                throw new InvalidOperationException(
                    "Only pending transactions can be failed.");

            Status = TransactionStatus.failed;
        }

    }
}
            
