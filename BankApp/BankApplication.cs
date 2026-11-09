using BankApp.Enums;

namespace BankApp
{
  internal class BankApplication
  {
    private bool IsRunning = true;

    private bool DisplayAccounts(Customer customer, AccountService accountService)
    {
      List<BankAccount> accounts = accountService.GetAccountsForUser(customer.OwnerId);

      if (accounts.Count == 0)
      {
        Console.WriteLine("You do not have any accounts yet.");
        return false;
      }

      foreach (BankAccount account in accounts)
      {
        Console.WriteLine($"Account {account.AccountNumber} - Balance: {account.Balance:N2} {account.Currency}");
      }

      return true;
    }

    private void WriteColored(string text, ConsoleColor color)
    {
      ConsoleColor previousColor = Console.ForegroundColor;
      Console.ForegroundColor = color;
      Console.WriteLine(text);
      Console.ForegroundColor = previousColor;
    }

    private void AdminMenu(Customer loggedCustomer, CurrencyService currencyService, List<Customer> customers, ref bool loggedIn)
    {
      AdminService adminService = new AdminService(currencyService, customers);

      WriteColored("<============ BANK MENU ============>", ConsoleColor.Yellow);

      Console.WriteLine("\n[1] Create User.");
      Console.WriteLine("[2] Update Exchange Rates.");
      Console.WriteLine("[3] Logout.");
      Console.WriteLine("[4] Exit.");

      Console.Write("\nEnter option: ");
      string? userAction = Console.ReadLine();

      switch (userAction)
      {
        case "1":
          {
            Console.Write("Full name: ");
            string? name = Console.ReadLine() ?? "";

            Console.Write("Username: ");
            string? username = Console.ReadLine() ?? "";

            Console.Write("PIN: ");
            string pin = Console.ReadLine() ?? "";

            try
            {
              Customer customer = adminService.CreateCustomer(name, username, pin);

              Console.WriteLine($"Created user {customer.UserName} with ID {customer.OwnerId}.");
            }
            catch (ArgumentException ex)
            {
              Console.WriteLine(ex.Message);
            }

            break;
          }

        case "2":
          {
            currencyService.GetSupportedCurrencies();

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

            if (from == to)
            {
              Console.WriteLine("Choose two different currencies.");
              break;
            }

            Console.Write($"How many {to} equals 1 {from}? ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal rate)
                || rate <= 0)
            {
              Console.WriteLine("Enter a valid rate greater than zero.");
              break;
            }

            try
            {
              currencyService.UpdateExchangeRate(from, to, rate);
              Console.WriteLine($"Exchange rate updated: 1 {from} = {rate} {to}.");
            }
            catch (ArgumentException ex)
            {
              Console.WriteLine(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
              Console.WriteLine(ex.Message);
            }

            break;
          }

        case "3":
          {
            loggedIn = false;
            break;
          }

        case "4":
          {
            loggedIn = false;
            IsRunning = false;
            break;
          }
      }
    }

    private void CustomerMenu(
        Customer loggedCustomer,
        AccountService accountService,
        TransferService transferService,
        CurrencyService currencyService,
        LoanService loanService,
        List<BankAccount> bankAccounts,
        ref int nextAccountNumber,
        ref bool loggedIn)
    {
      WriteColored("<============ BANK MENU ============>", ConsoleColor.Yellow);

      Console.WriteLine("\n[1] View My Accounts.");
      Console.WriteLine("[2] Check Account Balance.");
      Console.WriteLine("[3] Deposit Money.");
      Console.WriteLine("[4] Withdraw Money.");
      Console.WriteLine("[5] Transfer Money.");
      Console.WriteLine("[6] Transaction History.");
      Console.WriteLine("[7] Create New Bank Account.");
      Console.WriteLine("[8] Apply for a Loan.");
      Console.WriteLine("[9] Currency Exchange.");

      WriteColored("\n[10] Logout   [11] Exit", ConsoleColor.Yellow);

      Console.Write("\nEnter option: ");
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
            bool isDisplayed = DisplayAccounts(loggedCustomer, accountService);
            if (!isDisplayed) break;

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
            bool isDisplayed = DisplayAccounts(loggedCustomer, accountService);
            if (!isDisplayed) break;

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
            bool isDisplayed = DisplayAccounts(loggedCustomer, accountService);
            if (!isDisplayed) break;

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
            bool isDisplayed = DisplayAccounts(loggedCustomer, accountService);
            if (!isDisplayed) break;

            Console.Write("Sender account number: ");
            string? senderNumber = Console.ReadLine();

            BankAccount? sender = accountService
                .GetAccountsForUser(loggedCustomer.OwnerId)
                .FirstOrDefault(account => account.AccountNumber == senderNumber);

            if (sender == null)
            {
              Console.WriteLine("Sender account not found.");
              break;
            }

            Console.Write("Receiver account number: ");
            string receiverNumber = (Console.ReadLine() ?? "").Trim();

            Console.Write("Amount to transfer: ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal amount)
                || amount <= 0)
            {
              Console.WriteLine("Enter a valid amount greater than zero.");
              break;
            }

            if (amount > sender.Balance)
            {
              Console.WriteLine("Insufficient funds.");
              break;
            }

            try
            {
              transferService.TransferMoney(
                  sender.AccountNumber,
                  receiverNumber,
                  amount,
                  sender.Currency,
                  Guid.NewGuid());

              Console.WriteLine("Transfer queued for processing.");
            }
            catch (ArgumentException ex)
            {
              Console.WriteLine($"Transfer failed: {ex.Message}");
            }

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
            bool isDisplayed = DisplayAccounts(loggedCustomer, accountService);
            if (!isDisplayed) break;

            Console.Write("Account number to receive the loan: ");
            string? accountNumber = Console.ReadLine();

            BankAccount? account = accountService
              .GetAccountsForUser(loggedCustomer.OwnerId)
              .FirstOrDefault(account => account.AccountNumber == accountNumber);

            if (account == null)
            {
              Console.WriteLine("Account not found.");
              break;
            }

            Console.Write("Loan amount: ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal borrowAmount) || borrowAmount <= 0)
            {
              Console.WriteLine("Enter a valid amount greater than zero.");
              break;
            }

            Console.Write("Interest rate in percent (enter 5 for 5%): ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal interestPercent) || interestPercent < 0)
            {
              Console.WriteLine("Enter a valid non-negative interest rate.");
              break;
            }

            var result = loanService.TakeLoan(account, borrowAmount, interestPercent / 100m);

            if (result == null)
            {
              Console.WriteLine("Loan rejected. Check the borrowing limit and amount.");
              break;
            }

            Console.WriteLine($"Loan received: {result.Value.LoanAmount:N2} {account.Currency}");

            Console.WriteLine($"Interest cost: {result.Value.InterestCost:N2} {account.Currency}");

            break;
          }

        case "9":
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

        case "10":
          {
            loggedIn = false;
            Console.WriteLine("Logged out.");
            break;
          }

        case "11":
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

    public void Run()
    {
      List<Customer> customers = new List<Customer>();
      Customer adminTest1 = new Customer("Admin", "admin", "2000", UserRole.Admin)
      {
        OwnerId = 1
      };
      Customer customertest1 = new Customer("Customer", "customer", "pass")
      {
        OwnerId = 2
      };

      customers.Add(adminTest1);
      customers.Add(customertest1);

      AuthenticationService authenticationService = new AuthenticationService(customers);

      AccountService accountService = new();
      List<BankAccount> bankAccounts = new();
      CurrencyService currencyService = new();
      TransferService transferService = new(bankAccounts, currencyService);
      LoanService loanService = new LoanService();

      int nextAccountNumber = 1;

      using CancellationTokenSource cancellation = new CancellationTokenSource();
      Task processor = transferService.RunProcessingLoopAsync(cancellation.Token);

      try
      {

        while (IsRunning)
        {
          Console.Clear();
          WriteColored(@"
  ██████╗  █████╗ ███╗   ██╗██╗  ██╗
  ██╔══██╗██╔══██╗████╗  ██║██║ ██╔╝
  ██████╔╝███████║██╔██╗ ██║█████╔╝
  ██╔══██╗██╔══██║██║╚██╗██║██╔═██╗
  ██████╔╝██║  ██║██║ ╚████║██║  ██╗
  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝
        ", ConsoleColor.Yellow);

          Console.WriteLine("Fill in your information.");

          Console.Write("Name: ");
          string? userName = Console.ReadLine();

          Console.Write("Pin: ");
          string? userPin = Console.ReadLine();

          Customer? loggedCustomer = authenticationService.Login(userName, userPin);

          if (loggedCustomer == null)
          {
            Customer? customer = customers.FirstOrDefault(customer => customer.UserName == userName);

            if (customer != null && customer.IsLocked)
            {
              Console.WriteLine("Your account is locked after three failed attempts.");
            }
            else
            {
              Console.WriteLine("Incorrect username or PIN.");
            }

            Console.Write("\nPress Enter to return to the menu...");
            Console.ReadLine();

            continue;
          }

          bool loggedIn = true;
          while (loggedIn && IsRunning)
          {
            bool hasAccounts = accountService.GetAccountsForUser(loggedCustomer.OwnerId).Count > 0;

            Console.Clear();

            if (loggedCustomer.Role == UserRole.Admin)
            {
              AdminMenu(loggedCustomer, currencyService, customers, ref loggedIn);
            }

            else
            {
              CustomerMenu(
                loggedCustomer,
                accountService,
                transferService,
                currencyService,
                loanService,
                bankAccounts,
                ref nextAccountNumber,
                ref loggedIn
              );
            }

            if (loggedIn && IsRunning)
            {
              Console.Write("\nPress Enter to return to the menu...");
              Console.ReadLine();
            }
          }
        }
      }
      finally
      {
        cancellation.Cancel();
        processor.GetAwaiter().GetResult();
      }
    }
  }
}

