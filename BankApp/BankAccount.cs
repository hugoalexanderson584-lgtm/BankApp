using BankApp.Enums;

namespace BankApp;

public class BankAccount
{
    public string AccountNumber { get; set; }
    public int OwnerId { get; set; }
    public decimal Balance { get; private set; }
    public Currency Currency { get; set; }

    public BankAccount(string accountNumber, int ownerId, Currency currency)
    {
        AccountNumber = accountNumber;
        OwnerId = ownerId;
        Currency = currency;
        Balance = 0m;
    }

    public bool Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        Balance += amount;
        return true;
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0 || amount > Balance)
        {
            return false;
        }

        Balance -= amount;
        return true;
    }
}

