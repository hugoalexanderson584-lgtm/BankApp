using BankApp.Enums;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
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
            // Validate the amount
            if (amount <= 0)
                throw new ArgumentException(
                    "Transfer amount must be greater than zero.");

            // Find both accounts
            var sender = _BankAccounts.FirstOrDefault(
                a => a.AccountNumber == senderAccountId);

            var receiver = _BankAccounts.FirstOrDefault(
                a => a.AccountNumber == receiverAccountId);

            if (sender == null)
                throw new ArgumentException("Sender account not found.");

            if (receiver == null)
                throw new ArgumentException("Receiver account not found.");

            if (senderAccountId == receiverAccountId)
                throw new ArgumentException(
                    "Sender and receiver must be different accounts.");

            if (sender.Currency != currency || receiver.Currency != currency)
            {
                throw new ArgumentException(
                    "The currency must match both accounts.");
            }

            // Create a transaction record
            var transaction = CreateTransaction(
                senderAccountId,
                receiverAccountId,
                amount,
                currency,
                id);

            try
            {
                // Withdraw from sender
                sender.Withdraw(amount);

                // Deposit into receiver
                receiver.Deposit(amount);

                // Mark the transaction successful
                CompleteTransaction(transaction);
            }
            catch
            {
                FailTransaction(transaction);
                throw;
            }



        }
        public class TransferMenu
        {
            private readonly TransferService _transferService;

            public TransferMenu(TransferService transferService)
            {
                _transferService = transferService;
            }

            public void ShowTransferMenu()
            {
                Console.WriteLine("\n=== Money Transfer ===");

                Console.Write("Enter sender account number: ");
                string senderAccountId = Console.ReadLine() ?? "";

                Console.Write("Enter receiver account number: ");
                string receiverAccountId = Console.ReadLine() ?? "";

                Console.Write("Enter amount to transfer: ");

                if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                {
                    Console.WriteLine("Invalid amount.");
                    return;
                }

                Console.WriteLine("Select currency:");

                // Change these options to match your Currency enum.
                foreach (Currency currencyOption in Enum.GetValues(typeof(Currency)))
                {
                    Console.WriteLine($"{(int)currencyOption}. {currencyOption}");
                }

                Console.Write("Enter currency number: ");

                if (!int.TryParse(Console.ReadLine(), out int currencyNumber) ||
                    !Enum.IsDefined(typeof(Currency), currencyNumber))
                {
                    Console.WriteLine("Invalid currency.");
                    return;
                }

                Currency currency = (Currency)currencyNumber;

                try
                {
                    _transferService.TransferMoney(
                        senderAccountId,
                        receiverAccountId,
                        amount,
                        currency,
                        Guid.NewGuid());

                    Console.WriteLine("\nTransfer completed successfully!");
                    Console.WriteLine($"Amount transferred: {amount} {currency}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Transfer error: {ex.Message}");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Transfer failed: {ex.Message}");
                }
            }
        }
    }
}
