using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    public enum TransactionStatus
    {
        pending,
        completed,
        failed,

    }
    internal class Transactions
    {

        public string SenderAccountId { get; private set; }

        public string ReceiverAccountId { get; private set; }

        public decimal Amount { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public TransactionStatus Status { get; private set; }

        public Transactions(string senderAccountId, string receiverAccountId, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(senderAccountId))
                throw new ArgumentException("Sender is required.");

            if (string.IsNullOrWhiteSpace(receiverAccountId))
                throw new ArgumentException("Receiver is required.");

            if (senderAccountId == receiverAccountId)
                throw new ArgumentException(
                    "Sender and receiver must be different.");

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");
            SenderAccountId = senderAccountId;
            ReceiverAccountId = receiverAccountId;
            Amount = amount;
            CreatedAt = DateTime.Now;
            Status = TransactionStatus.pending;
        }
        
    }
}
            
