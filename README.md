# BankApp

BankApp is a C# console banking application targeting **.NET 10**. It brings together customer login, bank accounts, deposits and withdrawals, queued transfers, loan interest estimates, savings interest estimates, and currency conversion.

The project is a work in progress. Some services are implemented independently but are not connected to the main menu, and the main menu currently contains references to an older account model that must be updated before the application can compile.

## Contents

- [Requirements and setup](#requirements-and-setup)
- [Login and navigation](#login-and-navigation)
- [Features and current status](#features-and-current-status)
- [How the code works](#how-the-code-works)
- [Project structure](#project-structure)
- [Data and logging](#data-and-logging)
- [Known limitations and troubleshooting](#known-limitations-and-troubleshooting)
- [Development and verification](#development-and-verification)

## Requirements and setup

You need the .NET 10 SDK and a terminal. JetBrains Rider or another C# IDE is optional. The project has no explicit third-party NuGet package dependencies, database requirement, API keys, or environment-variable configuration.

From the repository root:

```sh
dotnet --list-sdks
dotnet restore BankApp.slnx
dotnet build BankApp.slnx
dotnet run --project BankApp/BankApp.csproj
```

Run the final command after resolving the source compatibility issues described below. In Rider, open `BankApp.slnx` and select the `BankApp` project to run.

The project file enables nullable reference analysis and implicit using directives. Its output type is a console executable.

## Login and navigation

`Program.Main` creates a `BankApplication` and calls `Run()`. That method creates three demonstration customers at startup:

| Customer | Username | PIN |
| --- | --- | --- |
| John Eriksson | `John` | `2000` |
| Erika Johnsson | `Erika` | `b25o` |
| Jack Holmes | `Jack` | `tack` |

At the `Name:` prompt, enter the **username**, such as `John`, rather than the customer's full name. Usernames and PINs are case-sensitive. PINs are strings and may contain letters.

Each customer starts without bank accounts. Three incorrect PIN attempts for an existing customer lock that customer for the rest of the process. A successful login resets the failed-attempt counter. Restarting the app recreates all customers and resets the lockouts.

After login, the intended main menu is:

```text
1. View My Accounts
2. Check Account Balance
3. Deposit Money
4. Withdraw Money
5. Transfer Money
6. Transaction History
7. Create New Bank Account
8. Apply for a Loan
9. View My Loans
10. Currency Exchange
11. Logout
```

Enter the option number and follow the prompts. Amount parsing uses the current system culture, so the accepted decimal separator depends on the machine's regional settings. Logout returns to the login prompt. There is no main-menu exit option; use `Ctrl+C` to stop the process.

## Features and current status

| Feature | Current implementation |
| --- | --- |
| Login | Demonstration users, PIN verification, and a three-attempt lockout are implemented. |
| Account creation and lookup | `AccountService` creates zero-balance accounts and retrieves accounts by owner ID. Main-menu integration still uses an older account model. |
| Deposits and withdrawals | Implemented in `BankAccount`; main-menu calls need updating to the current API. |
| Transfers | Queuing, processing, validation, and logging exist in `TransferService`. Main-menu option 5 is a TODO. |
| Transaction history | The main menu reads the shared `transaction-history.log` file if it exists. |
| Loans | Option 8 calculates an interest estimate only. It does not submit an application or add money to an account. |
| Loan records | Option 9 reports that applications and records are not stored yet. |
| Currency exchange | Option 10 calculates a conversion using fixed rates; it does not change account balances. |
| Savings | `SavingsAccount` estimates interest, but is not exposed through the main menu. |

## How the code works

### Customers and authentication

`AuthenticationService` searches an in-memory customer list by username, then calls `Customer.ControlPin`. The `Customer` class owns its account list, PIN, failed-attempt count, and lockout state. `Customer.OwnerId` is settable but is not assigned distinct values in the demonstration setup.

`Modules/User.cs` contains a separate `User` model with name, email, salary, and password properties. Its static `Login()` method is empty; the application uses `Customer` and `AuthenticationService` instead.

### Bank accounts

`BankAccount` holds:

- `AccountNumber`: a nonempty account identifier, trimmed at construction.
- `OwnerId`: the customer's numeric identifier.
- `Balance`: a `decimal`, initially zero, changed only through account methods.
- `Currency`: `SEK`, `EUR`, or `USD`, fixed at creation.

`Deposit(decimal amount)` returns `false` for zero, negative, or overflowing amounts. Otherwise, it increases the balance and returns `true`.

`Withdraw(decimal amount)` returns `false` for zero, negative, or unaffordable amounts. Otherwise, it reduces the balance and returns `true`. Overdrafts are not supported.

`AccountService.CreateAccount(accountNumber, ownerId, currency)` validates account numbers, rejects duplicates within that service instance, stores the account, and returns it. `GetAccountsForUser(ownerId)` returns a new list containing the matching accounts. Reuse the same service instance to retain its account collection. The service does not automatically add accounts to `Customer.Accounts`.

### Transfers and transaction states

`Transactions` stores a GUID, sender and receiver account identifiers, amount, currency, creation time, and status. New transactions start as `pending`. Only pending transactions may become `completed` or `failed`.

`TransferService` receives a list of bank accounts. Its transfer flow is:

1. `TransferMoney(...)` checks that the amount is positive, the accounts differ and exist, the transaction ID is unused, and both account currencies match the transfer currency.
2. It creates a pending transaction and logs the request. No money moves at this stage.
3. `ProcessPendingTransactions()` checks funds, withdraws from the sender, and deposits into the receiver.
4. A successful transfer becomes completed. A processing failure becomes failed and is logged. If the receiver deposit fails, the code attempts to restore the sender's withdrawal.

`RunProcessingLoopAsync(CancellationToken)` processes pending transfers every **30 seconds** until cancellation. Its startup log says “15-minute transaction processor,” but the actual timer interval is 30 seconds.

The separate `ShowTransferMenu()` offers transfer requests, balance viewing, history viewing, and exit. That menu submits transfers in SEK. It does not start the processing loop itself, and neither it nor the loop is started by the application entry point. Code integrating this service must start processing explicitly or call `ProcessPendingTransactions()` directly.

Transfers require matching account currencies. They do not perform currency conversion, reserve funds when queued, or persist pending transactions.

### Loans and savings interest

`Loan` stores a borrowed amount and an interest rate. `LoanService.CalculateInterestRate(loan)` returns the **interest amount** despite its method name:

```text
interest = borrowed amount × interest rate
10,000 × 0.05 = 500
```

This is a simple estimate without a duration, repayment schedule, compounding, fees, or loan approval process.

The separate `CanBorraw(availableFunds, borrowAmount)` method returns whether available funds are at least five times the requested loan. That method is not used by the main menu.

`SavingsAccount` inherits from `BankAccount`, adds an editable `InterestRate`, and calculates `Balance × InterestRate` through `EstimateInterest()`. It does not credit that interest to the balance automatically.

### Currency conversion

`CurrencyService` supports these fixed base rates:

| Conversion | Multiplier |
| --- | --- |
| SEK → EUR | 0.090 |
| SEK → USD | 0.10 |
| EUR → USD | 1.12 |

Reverse conversions use the reciprocal of the corresponding rate. Converting a currency to itself uses 1. Results are rounded to two decimal places, and negative amounts are rejected. These are demonstration values embedded in the source, not live market rates.

The menu accepts currency names without regard to case, such as `SEK`, `eur`, or `USD`. For example, converting `100` SEK to EUR yields `9.00` numerically; the displayed decimal formatting may vary by culture.

`UpdateExchangeRate(...)` exists as a service method but has no menu or authentication enforcement. The rate lookup and update code have limitations described below.

## Project structure

```text
BankApp.slnx                       Solution file
BankApp/
├── BankApp.csproj                 .NET 10 console project configuration
├── Program.cs                     Application entry point
├── BankApplication.cs             Login loop and main console menu
├── BankAccount.cs                 Account identity, balance, and money operations
├── SavingsAccount.cs              Savings interest estimate
├── Transactions.cs                Transaction data and status transitions
├── TransactionLogger.cs           Timestamped file and console logging
├── Enums/
│   └── Currency.cs                SEK, EUR, and USD
├── Modules/
│   ├── Loan.cs                    Loan amount and rate
│   ├── User.cs                    Separate, currently unused user model
│   └── readme.txt                 Folder guidance
└── Services/
    ├── AccountService.cs          Account creation and owner lookup
    ├── AuthenticationService.cs   Login logic and Customer model
    ├── CurrencyService.cs         Conversion rates and calculations
    ├── LoanService.cs             Interest and eligibility calculations
    ├── TransferService.cs         Queued transfers, processor, and transfer menu
    └── readme.txt                 Folder guidance
```

Most types use the `BankApp` namespace, even when stored in subfolders. `TransferService` is currently in the global namespace.

## Data and logging

Customers, balances, account lists, loans, and queued transactions have no database or serialized storage. In-memory state is lost when the process ends. Logging does not restore this state on startup.

`TransactionLogger.Log(...)` appends entries to `transaction-history.log` in the **current working directory** and also prints them to the console:

```text
yyyy-MM-dd HH:mm:ss | message
```

The log records transfer creation, processing ticks, completion, failure, and processor startup/shutdown when those service paths are invoked. Account deposits and withdrawals do not automatically write log entries.

History is a shared file, not filtered by customer. Its location can differ between terminal and IDE runs if their working directories differ. Deleting the file removes the recorded history; it does not reset balances in a running process.

## Known limitations and troubleshooting

### SDK compatibility

If a build reports `NETSDK1045`, check `dotnet --list-sdks` and select/install a .NET 10 SDK. The project targets `net10.0`; a .NET 9 SDK cannot build it.

During README preparation, the installed SDK was `9.0.300`, and `dotnet build BankApp.slnx --no-restore` stopped with `NETSDK1045`. Application execution could not be verified in that environment.

### Main-menu account API mismatch

`BankApplication.cs` still references the missing `Account` type and old members named `accountName` and `salary`. The current model is `BankAccount`, with `AccountNumber` and `Balance`, and its constructor requires an account number, owner ID, and currency. The menu also parses deposit and withdrawal amounts as `double`, while `BankAccount` expects `decimal`.

These source mismatches must be resolved in addition to selecting a compatible SDK. Account-selection prompts currently ask for a list number while the lookup code compares an account name; that interaction also needs alignment when updating the menu.

### Other unfinished behavior

- Main-menu transfers and loan records are unfinished, and savings accounts have no menu integration.
- Currency dictionary lookups mistakenly use `(Currency, Currency)` from the service property instead of `(from, to)`. Explicit fallback branches currently provide the listed conversion rates, but dictionary rate updates do not reliably affect conversions.
- `UpdateExchangeRate` repeats the forward-key check instead of handling a reverse key, so reverse updates are not implemented correctly.
- Credentials are stored as plain strings, entered visibly, and embedded in source for demonstration. There is no registration, PIN reset, or administrative workflow.
- There is no synchronization for concurrent access to accounts and transaction lists. Integrating the background processor with menu actions needs concurrency handling.
- File read/write failures are not handled throughout the logging and history code.
- The main loop has no end-of-input handling or graceful shutdown path.

## Development and verification

There is currently no automated test project in the repository. Build with:

```sh
dotnet build BankApp.slnx
```

After fixing the account integration, useful manual checks include successful and failed logins, lockout after three incorrect PINs, account creation, rejected invalid deposits, insufficient-funds withdrawals, loan interest estimates, and conversions in both directions.

When connecting transfers, verify that queuing leaves balances unchanged, processing moves funds once, failures leave appropriate balances and statuses, and cancellation stops the processor. Check the shared log separately from in-memory balances.
