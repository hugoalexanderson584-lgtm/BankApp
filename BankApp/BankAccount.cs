using BankApp.Enums;

namespace BankApp;

public class BankAccount
{
    // Account number and owner ID can only be assigned during creation.
    public string AccountNumber { get; }
    public int OwnerId { get; }

    // The balance can only be changed from inside this class.
    public decimal Balance { get; private set; }

    // The currency used by this account.
    public Currency Currency { get; set; }

    // Creates an account with a starting balance of zero.
    public BankAccount(string accountNumber, int ownerId, Currency currency)
    {
        AccountNumber = accountNumber;
        OwnerId = ownerId;
        Currency = currency;
        Balance = 0m;
    }

    // Adds money to the account. Returns true if successful.
    public bool Deposit(decimal amount)
    {
        // Reject zero or negative amounts.
        if (amount <= 0)
        {
            return false;
        }

        Balance += amount;
        return true;
    }

    // Removes money from the account. Returns true if successful.
    public bool Withdraw(decimal amount)
    {
        // Reject invalid amounts or withdrawals exceeding the balance.
        if (amount <= 0 || amount > Balance)
        {
            return false;
        }

        Balance -= amount;
        return true;
    }
}