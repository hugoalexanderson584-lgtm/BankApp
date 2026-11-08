
using BankApp;
using BankApp.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class TransferService
{
    private readonly List<Transactions> _transactions = new();
    private readonly List<BankAccount> _bankAccounts;
    private readonly TransactionLogger _logger = new();

    public TransferService(List<BankAccount> accounts)
    {
        _bankAccounts = accounts;
    }

    public Transactions CreateTransaction(
        string senderAccountId,
        string receiverAccountId,
        decimal amount,
        Currency currency,
        Guid id)
    {
        var transaction = new Transactions(
            senderAccountId,
            receiverAccountId,
            amount,
            currency,
            id);

        _transactions.Add(transaction);
        return transaction;
    }

    public Transactions GetTransaction(Guid id)
    {
        return _transactions.FirstOrDefault(t => t.Id == id);
    }

    // Queue the transfer. Do not move money here.
    public void TransferMoney(
        string senderAccountId,
        string receiverAccountId,
        decimal amount,
        Currency currency,
        Guid id)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        if (senderAccountId == receiverAccountId)
            throw new ArgumentException(
                "Sender and receiver must be different accounts.");

        if (_transactions.Any(t => t.Id == id))
            throw new ArgumentException("Transaction ID already exists.");

        var sender = _bankAccounts.FirstOrDefault(
            a => a.AccountNumber == senderAccountId);

        var receiver = _bankAccounts.FirstOrDefault(
            a => a.AccountNumber == receiverAccountId);

        if (sender == null)
            throw new ArgumentException("Sender account not found.");

        if (receiver == null)
            throw new ArgumentException("Receiver account not found.");

        if (sender.Currency != currency ||
            receiver.Currency != currency)
            throw new ArgumentException(
                "Currency must match both accounts.");

        var transaction = CreateTransaction(senderAccountId, receiverAccountId, amount, currency, id);

        _logger.Log($"Transfer created | ID: {id} | " + $"{senderAccountId} -> {receiverAccountId} | " +  $"{amount} {currency} | Status: pending");
    }

    // Called only when the timer ticks.
    public void ProcessPendingTransactions()
    {
        var pendingTransactions = _transactions.Where(t => t.Status == TransactionStatus.pending).ToList();

        _logger.Log($"Timer tick: processing {pendingTransactions.Count} pending transfer(s).");

        foreach (var transaction in pendingTransactions)
        {
            try
            {
                var sender = _bankAccounts.FirstOrDefault(
                    a => a.AccountNumber == transaction.SenderAccountId);

                var receiver = _bankAccounts.FirstOrDefault(
                    a => a.AccountNumber == transaction.ReceiverAccountId);

                if (sender == null || receiver == null)
                    throw new InvalidOperationException(
                        "An account could not be found.");

                if (sender.Currency != transaction.Currency ||
                    receiver.Currency != transaction.Currency)
                    throw new InvalidOperationException(
                        "Account currency does not match transaction currency.");

                if (sender.Balance < transaction.Amount)
                    throw new InvalidOperationException(
                        "Insufficient funds.");

                // Withdraw the money first.
                if (!sender.Withdraw(transaction.Amount))
                    throw new InvalidOperationException(
                        "Withdrawal failed.");

                // Deposit into the receiver.
                if (!receiver.Deposit(transaction.Amount))
                {
                    // Reverse the withdrawal if the deposit fails.
                    sender.Deposit(transaction.Amount);

                    throw new InvalidOperationException(
                        "Deposit failed; withdrawal reversed.");
                }

                transaction.MarkCompleted();

                _logger.Log(
                    $"Transfer completed | ID: {transaction.Id} | " +
                    $"{transaction.Amount} {transaction.Currency}");
            }
            catch (Exception ex)
            {
                transaction.MarkFailed();

                _logger.Log( $"Transfer failed | ID: {transaction.Id} | " + $"Reason: {ex.Message}");
            }
        }
    }

    // Runs continuously until cancellation.
    public async Task RunProcessingLoopAsync(
        CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));

        _logger.Log("15-minute transaction processor started.");

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                ProcessPendingTransactions();
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown.
        }

        _logger.Log("Transaction processor stopped.");
    }

    public void LogAction(string message)
    {
        _logger.Log(message);
    }
    // made a menu for the transfers but it wasnt needed but i decided to leave it as a comment incase we need it.
    //public void ShowTransferMenu()
   /* {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n=== BANK APP ===");
            Console.WriteLine("1. Transfer money");
            Console.WriteLine("2. View account balances");
            Console.WriteLine("3. View transaction history");
            Console.WriteLine("4. Exit");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    try
                    {
                        Console.Write("Sender account number: ");
                        string sender = Console.ReadLine() ?? "";

                        Console.Write("Receiver account number: ");
                        string receiver = Console.ReadLine() ?? "";

                        Console.Write("Amount: ");

                        if (!decimal.TryParse(
                            Console.ReadLine(), out decimal amount))
                        {
                            Console.WriteLine("Invalid amount.");
                            break;
                        }

                        TransferMoney(
                            sender,
                            receiver,
                            amount,
                            Currency.SEK,
                            Guid.NewGuid());

                        Console.WriteLine(
                            "Transfer queued! It will execute on the next timer tick.");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Transfer error: {ex.Message}");
                    }

                    break;

                case "2":
                    Console.WriteLine("\n--- Account Balances ---");

                    foreach (var account in _bankAccounts)
                    {
                        Console.WriteLine(
                            $"{account.AccountNumber}: " +
                            $"{account.Balance} {account.Currency}");
                    }

                    break;

                case "3":
                    Console.WriteLine("\n--- Transaction History ---");

                    if (File.Exists("transaction-history.log"))
                    {
                        Console.WriteLine(
                            File.ReadAllText("transaction-history.log"));
                    }
                    else
                    {
                        Console.WriteLine("No history available.");
                    }

                    break;

                case "4":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }*/


}


