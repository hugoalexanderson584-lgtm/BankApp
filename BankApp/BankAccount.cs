using BankApp.Enums;

namespace BankApp;

public class BankAccount
{
    // Account number and owner ID can only be assigned during creation.
    public string AccountNumber { get; }
    public int OwnerId { get; }

    // The balance can only be changed from inside this class.
    public decimal Balance { get; private set; }

    // Currency is fixed at creation so the balance cannot be relabeled without conversion.
    public Currency Currency { get; }

    // Creates an account with a starting balance of zero.
    public BankAccount(string accountNumber, int ownerId, Currency currency)
    {
        // Validate here too, because accounts can be created without AccountService.
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException("Account number is required.", nameof(accountNumber));
        }

        // An enum can contain a numeric value that is not a supported currency.
        if (!Enum.IsDefined(currency))
        {
            throw new ArgumentOutOfRangeException(nameof(currency), "Currency is not supported.");
        }

        AccountNumber = accountNumber.Trim();
        OwnerId = ownerId;
        Currency = currency;
        Balance = 0m;
    }

    // Adds money to the account. Returns true if successful.
    public bool Deposit(decimal amount)
    {
        // Reject zero or negative amounts.
        // Reject amounts that would overflow decimal as well as nonpositive amounts.
        if (amount <= 0 || amount > decimal.MaxValue - Balance)
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
