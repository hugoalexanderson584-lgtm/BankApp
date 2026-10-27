using BankApp.Enums;
using static BankApp.TransferService;

namespace BankApp;

internal class Program
{
    private static void Main(string[] args)
    {
        // BankApplication app = new BankApplication();
        // app.Run();
        var alice = new BankAccount("ACC001", 1, Currency.SEK);
        var bob = new BankAccount("ACC002", 2, Currency.SEK);

        alice.Deposit(1000m);
        bob.Deposit(200m);

        var accounts = new List<BankAccount>
{
    alice,
    bob
};

        var transferService = new TransferService(accounts);
        var transferMenu = new TransferMenu(transferService);

        transferMenu.ShowTransferMenu();

        Console.WriteLine($"Alice's balance: {alice.Balance}");
        Console.WriteLine($"Bob's balance: {bob.Balance}");
    }
}