namespace BankApp
{
  internal class BankApplication
  {
    private bool IsRunning = true;

    private void DisplayAccounts(Customer customer)
    {
      if (customer.Accounts.Count == 0)
      {
        Console.WriteLine("You do not have any accounts yet.");
        return;
      }

      for (int i = 0; i < customer.Accounts.Count; i++)
      {
        Console.WriteLine($"{i + 1}. {customer.Accounts[i].accountName}");
      }
    }

    public void Run()
    {
      List<Customer> customers = new List<Customer>();
      Customer customerTest1 = new Customer("John Eriksson", "John", "2000");
      Customer customerTest2 = new Customer("Erika Johnsson", "Erika", "b25o");
      Customer customerTest3 = new Customer("Jack Holmes", "Jack", "tack");

      customers.Add(customerTest1);
      customers.Add(customerTest2);
      customers.Add(customerTest3);

      AuthenticationService authenticationService = new AuthenticationService(customers);

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
          Console.WriteLine("This account does not exist");
          continue;
        }

        bool loggedIn = true;
        while (loggedIn && IsRunning)
        {
          Console.WriteLine("1. View My Accounts");
          Console.WriteLine("2. Check Account Balance");
          Console.WriteLine("3. Deposit Money");
          Console.WriteLine("4. Withdraw Money");
          Console.WriteLine("5. Transfer Money");
          Console.WriteLine("6. Transaction History");
          Console.WriteLine("7. Create New Bank Account");
          Console.WriteLine("8. Apply for a Loan");
          Console.WriteLine("9. View My Loans");
          Console.WriteLine("10. Currency Exchange");
          Console.WriteLine("11. Logout");

          Console.Write("Choose action: ");
          string? userAction = Console.ReadLine();

          switch (userAction)
          {
            case "1":
              {
                DisplayAccounts(loggedCustomer);
                break;
              }

            case "2":
              {
                DisplayAccounts(loggedCustomer);

                Console.Write("Enter account number from the list above: ");

                string? balanceAccountId = Console.ReadLine();
                Account? balanceAccount = loggedCustomer.Accounts.FirstOrDefault(
                  item => item.accountName.ToString() == balanceAccountId);
                if (balanceAccount == null)
                {
                  Console.WriteLine("Account not found.");
                  break;
                }

                Console.WriteLine($"Balance: {balanceAccount.salary}");
                break;
              }

            case "3":
              {
                DisplayAccounts(loggedCustomer);

                Console.Write("Enter account number from the list above: ");
                string? accountId = Console.ReadLine();

                Account? ExistingAccount = loggedCustomer.Accounts.FirstOrDefault(item => item.accountName.ToString() == accountId);

                if (ExistingAccount == null)
                {
                  Console.WriteLine("Account not found.");
                  break;
                }

                Console.Write("Amount to deposit: ");
                if (double.TryParse(Console.ReadLine(), out double depositAmount) && depositAmount > 0 && ExistingAccount.Deposit(depositAmount))
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
                DisplayAccounts(loggedCustomer);

                Console.Write("Enter account number from the list above: ");
                string? accountId = Console.ReadLine();

                Account? account = loggedCustomer.Accounts.FirstOrDefault(item => item.accountName.ToString() == accountId);

                if (account == null)
                {
                  Console.WriteLine("Account not found.");
                  break;
                }

                Console.Write("Amount to withdraw: ");

                if (double.TryParse(Console.ReadLine(), out double withdrawalAmount) && account.Withdraw(withdrawalAmount))
                {
                  Console.WriteLine("Withdrawal successful.");
                }
                else
                {
                  Console.WriteLine("Withdrawal failed.");
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
                Console.Write("Enter a name for the new account: ");
                string? accountName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(accountName))
                {
                  Console.WriteLine("Account name cannot be empty.");
                  break;
                }

                Account newAccount = new Account(accountName, 0, Guid.NewGuid());
                loggedCustomer.AddAccount(newAccount);

                Console.WriteLine($"Account created. Account number: {newAccount.accountName}");

                break;
              }

            case "8":
              {
                Console.Write("Loan amount for an interest estimate: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal borrowAmount) || borrowAmount <= 0)
                {
                  Console.WriteLine("Enter a valid amount greater than zero.");
                  break;
                }

                Console.Write("Interest rate (for example, 0.05 for 5%): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal interestRate) || interestRate < 0)
                {
                  Console.WriteLine("Enter a valid non-negative interest rate.");
                  break;
                }

                Loan loan = new Loan(borrowAmount, interestRate);
                LoanService loanService = new LoanService();
                Console.WriteLine($"Estimated interest: {loanService.CalculateInterestRate(loan)}. This does not submit a loan application.");

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
                CurrencyService currencyService = new CurrencyService();
                currencyService.GetSupportedCurrencies();

                Console.Write("Amount: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount < 0)
                {
                  Console.WriteLine("Enter a valid non-negative amount.");
                  break;
                }

                Console.Write("From currency: ");
                if (!Enum.TryParse(Console.ReadLine(), true, out Enums.Currency from))
                {
                  Console.WriteLine("Unsupported currency.");
                  break;
                }

                Console.Write("To currency: ");
                if (!Enum.TryParse(Console.ReadLine(), true, out Enums.Currency to))
                {
                  Console.WriteLine("Unsupported currency.");
                  break;
                }

                try
                {
                  Console.WriteLine(
                    $"Converted amount: {currencyService.ConvertCurrency(amount, from, to)} {to}");
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

            default:
              {
                Console.WriteLine("Unknown action");
                break;
              }
          }
        }
      }
    }
  }
}
