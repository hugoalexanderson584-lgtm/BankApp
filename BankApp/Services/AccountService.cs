using BankApp.Enums;

namespace BankApp;

public class AccountService
{
    private readonly List<BankAccount> accounts = new();

    public BankAccount CreateAccount(string accountNumber, int ownerId, Currency currency)
    {
        foreach (BankAccount existingAccount in accounts)
        {
            if (existingAccount.AccountNumber == accountNumber)
            {
                throw new ArgumentException("Account number already exists.");
            }
        }

        BankAccount account = new BankAccount(accountNumber, ownerId, currency);

        accounts.Add(account);

        return account;
    }

    public List<BankAccount> GetAccountsForUser(int ownerId)
    {
        List<BankAccount> userAccounts = new();

        foreach (BankAccount account in accounts)
        {
            if (account.OwnerId == ownerId)
            {
                userAccounts.Add(account);
            }
        }

        return userAccounts;
    }
}