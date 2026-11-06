namespace BankApp
{
  internal class BankApplication
  {
    private bool IsRunning = true;

    private void DisplayAccounts(Customer customer, AccountService accountService)
    {
      List<BankAccount> accounts = accountService.GetAccountsForUser(customer.OwnerId);

      if (accounts.Count == 0)
      {
        Console.WriteLine("You do not have any accounts yet.");
        return;
      }

      for (int i = 0; i < accounts.Count; i++)
      {
        BankAccount account = accounts[i];
        Console.WriteLine($"{i + 1}. {account.AccountNumber} - {account.Balance} {account.Currency}");
      }
    }

    public void Run()
    {
      List<Customer> customers = new List<Customer>();
      Customer customerTest1 = new Customer("John Eriksson", "John", "2000")
      {
        OwnerId = 1
      };
      Customer customerTest2 = new Customer("Erika Johnsson", "Erika", "b25o")
      {
        OwnerId = 2
      };
      Customer customerTest3 = new Customer("Thomas Holmes", "Thomas", "coffe4me")
      {
        OwnerId = 3
      };

      customers.Add(customerTest1);
      customers.Add(customerTest2);
      customers.Add(customerTest3);

      AuthenticationService authenticationService = new AuthenticationService(customers);

      AccountService accountService = new();
      List<BankAccount> bankAccounts = new();
      TransferService transferService = new(bankAccounts);
      CurrencyService currencyService = new();
      LoanService loanService = new LoanService();

      int nextAccountNumber = 1;

      while (IsRunning)
      {
        Console.WriteLine("Fill in your information.");

        Console.Write("Name: ");
        string? userName = Console.ReadLine();

        Console.Write("Pin: ");
        string? userPin = Console.ReadLine();

        Customer? loggedCustomer = authenticationService.Login(userName, userPin);

        if (loggedCustomer == null)
        {
          Console.WriteLine("Login failed. Incorrect username or PIN.");
          continue;
        }


        bool loggedIn = true;
        while (loggedIn && IsRunning)
        {
          Console.WriteLine("1. View My Accounts.");
          Console.WriteLine("2. Check Account Balance.");
          Console.WriteLine("3. Deposit Money.");
          Console.WriteLine("4. Withdraw Money.");
          Console.WriteLine("5. Transfer Money.");
          Console.WriteLine("6. Transaction History.");
          Console.WriteLine("7. Create New Bank Account.");
          Console.WriteLine("8. Estimate Loan Interest.");
          Console.WriteLine("9. View My Loans.");
          Console.WriteLine("10. Currency Exchange.");
          Console.WriteLine("11. Logout.");
          Console.WriteLine("12. Exit.");

          Console.Write("Choose action: ");
          string? userAction = Console.ReadLine();

          switch (userAction)
          {
            case "1":
              {
                DisplayAccounts(loggedCustomer, accountService);
                break;
              }

            case "2":
              {
                DisplayAccounts(loggedCustomer, accountService);

                Console.Write("Enter account number: ");
                string? accountNumber = Console.ReadLine();

                BankAccount? account = accountService
                  .GetAccountsForUser(loggedCustomer.OwnerId)
                  .FirstOrDefault(account => account.AccountNumber == accountNumber);

                if (account == null)
                {
                  Console.WriteLine("Account not found.");
                  break;
                }

                Console.WriteLine($"Balance: {account.Balance} {account.Currency}");
                break;
              }

            case "3":
              {
                DisplayAccounts(loggedCustomer, accountService);

                Console.Write("Enter account number: ");
                string? accountNumber = Console.ReadLine();

                BankAccount? account = accountService
                    .GetAccountsForUser(loggedCustomer.OwnerId)
                    .FirstOrDefault(account => account.AccountNumber == accountNumber);

                if (account == null)
                {
                  Console.WriteLine("Account not found.");
                  break;
                }

                Console.Write("Amount to deposit: ");

                if (decimal.TryParse(Console.ReadLine(), out decimal amount)
                    && account.Deposit(amount))
                {
                  Console.WriteLine("Deposit successful.");
                }
                else
                {
                  Console.WriteLine("Invalid deposit amount.");
                }

                break;
              }

            case "4":
              {
                DisplayAccounts(loggedCustomer, accountService);

                Console.Write("Enter account number: ");
                string? accountNumber = Console.ReadLine();

                BankAccount? account = accountService
                    .GetAccountsForUser(loggedCustomer.OwnerId)
                    .FirstOrDefault(account => account.AccountNumber == accountNumber);

                if (account == null)
                {
                  Console.WriteLine("Account not found.");
                  break;
                }

                Console.Write("Amount to withdraw: ");

                if (decimal.TryParse(Console.ReadLine(), out decimal amount)
                    && account.Withdraw(amount))
                {
                  Console.WriteLine("Withdrawal successful.");
                }
                else
                {
                  Console.WriteLine("Withdrawal failed. Check the amount and balance.");
                }

                break;
              }

            case "5":
              {
                // TODO
                break;
              }

            case "6":
              {
                if (File.Exists("transaction-history.log"))
                {
                  Console.WriteLine(File.ReadAllText("transaction-history.log"));
                }
                else
                {
                  Console.WriteLine("No transaction history is available.");
                }

                break;
              }

            case "7":
              {
                Console.Write("Currency (SEK, EUR, USD): ");

                if (!Enum.TryParse(Console.ReadLine(), true, out Enums.Currency currency)
                  || !Enum.IsDefined(currency))
                {
                  Console.WriteLine("Unsupported currency.");
                  break;
                }

                BankAccount newAccount = accountService.CreateAccount(
                    nextAccountNumber.ToString(),
                    loggedCustomer.OwnerId,
                    currency
                );

                nextAccountNumber++;

                loggedCustomer.AddAccount(newAccount);
                bankAccounts.Add(newAccount);

                Console.WriteLine($"Created account {newAccount.AccountNumber} in {newAccount.Currency}.");

                break;
              }

            case "8":
              {
                Console.Write("Loan amount for an interest estimate: ");

                if (!decimal.TryParse(Console.ReadLine(), out decimal borrowAmount)
                    || borrowAmount <= 0)
                {
                  Console.WriteLine("Enter a valid amount greater than zero.");
                  break;
                }

                Console.Write("Interest rate in percent (enter 5 for 5%): ");

                if (!decimal.TryParse(Console.ReadLine(), out decimal interestPercent)
                    || interestPercent < 0)
                {
                  Console.WriteLine("Enter a valid non-negative interest rate.");
                  break;
                }

                Loan loan = new Loan(borrowAmount, interestPercent / 100m);

                Console.WriteLine(
                    $"Estimated interest: {loanService.CalculateInterestRate(loan)}.");

                Console.WriteLine("This does not submit a loan application.");
                break;
              }

            case "9":
              {
                Console.WriteLine("Loan applications and loan records are not stored yet.");
                // TODO
                break;
              }

            case "10":
              {
                currencyService.GetSupportedCurrencies();

                Console.Write("Amount to convert: ");

                if (!decimal.TryParse(Console.ReadLine(), out decimal amount)
                    || amount < 0)
                {
                  Console.WriteLine("Enter a valid non-negative amount.");
                  break;
                }

                Console.Write("From currency: ");

                if (!Enum.TryParse(Console.ReadLine(), true, out Enums.Currency from)
                    || !Enum.IsDefined(from))
                {
                  Console.WriteLine("Unsupported currency.");
                  break;
                }

                Console.Write("To currency: ");

                if (!Enum.TryParse(Console.ReadLine(), true, out Enums.Currency to)
                    || !Enum.IsDefined(to))
                {
                  Console.WriteLine("Unsupported currency.");
                  break;
                }

                try
                {
                  decimal convertedAmount = currencyService.ConvertCurrency(amount, from, to);

                  Console.WriteLine($"Converted amount: {convertedAmount} {to}");
                }
                catch (NotSupportedException ex)
                {
                  Console.WriteLine(ex.Message);
                }

                break;
              }

            case "11":
              {
                loggedIn = false;
                Console.WriteLine("Logged out.");
                break;
              }

            case "12":
              {
                loggedIn = false;
                IsRunning = false;
                break;
              }

            default:
              {
                Console.WriteLine("Unknown action.");
                break;
              }
          }
        }
      }
    }
  }
}
