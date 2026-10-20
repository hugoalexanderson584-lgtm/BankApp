using BankApp.Enums;
namespace BankApp;

public class AccountService
{
    private readonly List<BankAccount> accounts = new();

    public BankAccount CreateAccount(string accountNumber, int ownerId, Currency currency)
    {
        BankAccount account = new BankAccount(accountNumber, ownerId, currency); 
        
        accounts.Add(account);

        return account; 
    }
}