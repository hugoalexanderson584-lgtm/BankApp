using BankApp.Enums;

namespace BankApp;

public class AccountService
{
    // Stores accounts for this service instance. Reuse the instance to keep the same accounts.
    private readonly List<BankAccount> accounts = new();

    // Creates an account and adds it to the list.
    public BankAccount CreateAccount(string accountNumber, int ownerId, Currency currency)
    {
        // Reject missing, empty or whitespace-only account numbers.
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException("Account number is required.", nameof(accountNumber));
        }

        // Ignore surrounding spaces when checking and storing account numbers.
        accountNumber = accountNumber.Trim();

        // Check that the account number is not already in use.
        foreach (BankAccount existingAccount in accounts)
        {
            if (existingAccount.AccountNumber == accountNumber)
            {
                throw new ArgumentException("Account number already exists.", nameof(accountNumber));
            }
        }

        // The constructor also validates the account number and supported currency.
        BankAccount account = new BankAccount(accountNumber, ownerId, currency);

        accounts.Add(account);

        // Return the newly created account to the caller.
        return account;
    }

    // Returns all accounts belonging to the specified user.
    public List<BankAccount> GetAccountsForUser(int ownerId)
    {
        List<BankAccount> userAccounts = new();

        foreach (BankAccount account in accounts)
        {
            // Include only accounts whose owner ID matches the user's ID.
            if (account.OwnerId == ownerId)
            {
                userAccounts.Add(account);
            }
        }

        // Returns an empty list if the user has no accounts.
        return userAccounts;
    }
}
