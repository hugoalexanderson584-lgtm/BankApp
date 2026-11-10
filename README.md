# BankApp

BankApp is a C# console banking application targeting **.NET 10**. It supports customer and administrator login, bank accounts, deposits, withdrawals, queued transfers with currency conversion, loans, and exchange-rate management.

## Requirements and setup

Install the .NET 10 SDK. JetBrains Rider or another C# IDE is optional. No database, API keys, or explicit third-party NuGet packages are required.

From the repository root:

```sh
dotnet restore BankApp.slnx
dotnet build BankApp.slnx
dotnet run --project BankApp/BankApp.csproj
```

In Rider, open `BankApp.slnx` and run the `BankApp` project. If you see `NETSDK1045`, check `dotnet --list-sdks` and select a .NET 10 SDK.

## Login and navigation

Two demonstration users are created each time the application starts:

| Role | Username | PIN |
| --- | --- | --- |
| Administrator | `admin` | `2000` |
| Customer | `customer` | `pass` |

Enter the **username** at the `Name:` prompt. Usernames and PINs are case-sensitive. Three incorrect PIN attempts lock an existing user until the application restarts; a successful login resets the failed-attempt counter.

The demonstration customer starts without bank accounts. Select **7. Create New Bank Account** before depositing money or using other account operations. Accounts start with a zero balance and use SEK, EUR, or USD. Account numbers are generated sequentially and are used to select accounts.

### Customer menu

```text
1. View My Accounts
2. Check Account Balance
3. Deposit Money
4. Withdraw Money
5. Transfer Money
6. Transaction History
7. Create New Bank Account
8. Apply for a Loan
9. Currency Exchange
10. Logout
11. Exit
```

Amount parsing uses the machine's current culture, so the decimal separator depends on regional settings. Logout returns to the login prompt. Exit stops the application and cancels the transfer processor.

### Administrator menu

```text
1. Create User
2. Update Exchange Rates
3. Logout
4. Exit
```

Creating a user requires a full name, unique username, and PIN. Blank fields and duplicate usernames (compared without regard to case) are rejected. The new customer receives a unique owner ID and a zero-balance SEK account automatically.

Administrators can update the rate between two different supported currencies. Rates must be greater than zero. Updates affect conversions and transfers processed during the current session.

## Banking behavior

### Accounts, deposits, and withdrawals

`BankAccount` stores an account number, owner ID, decimal balance, and currency. `AccountService` creates accounts and retrieves them by owner ID. The application registers new accounts with the service, their customer, and the shared transfer account list.

Deposits must be positive and must not overflow the balance. Withdrawals must be positive and cannot exceed the available balance. Customer menu operations select accounts belonging to the logged-in customer.

### Queued transfers

The transfer menu checks the sender's ownership, amount, and current balance before submitting a request to `TransferService`. The service validates account existence, distinct sender and receiver accounts, a positive amount, a unique transaction ID, and a transaction currency matching the sender.

Transfers begin as `pending`: queuing does not move or reserve money. The application starts a background processor that processes pending transfers **every 15 minutes**, starting 15 minutes after startup. Keep the application running until a processing tick; exiting does not flush pending transfers.

At processing time, the service checks funds again, withdraws the original amount, converts it to the receiver's currency, and deposits the converted amount. The current exchange rate at processing time is used. Successful transfers become `completed`; errors mark them `failed`. A rejected receiver deposit triggers an attempt to refund the sender. Conversion exceptions after withdrawal do not currently trigger a refund.

For example, at the default rate, transferring 100 SEK to an EUR account credits 9.00 EUR. Transfers are logged with their IDs and statuses. `ProcessPendingTransactions()` is also available for explicit processing by code.

### Loans and savings

The loan menu selects an existing account, asks for a positive loan amount and a non-negative interest percentage, then calls `LoanService.TakeLoan`. The requested amount must be **at most five times the account's current balance**. An account with 1,000 can borrow up to 5,000; an empty account cannot borrow.

An accepted loan immediately deposits the principal into the selected account. Interest is displayed as a simple estimate:

```text
interest cost = loan amount × (entered percentage / 100)
5,000 at 5% = 250
```

There is no stored loan record, repayment schedule, debt tracking, or automatic interest deduction. `CalculateInterestRate` returns the interest amount despite its name.

`SavingsAccount` inherits from `BankAccount` and estimates `Balance × InterestRate`. It does not automatically credit interest and has no main-menu integration.

### Currency exchange

Supported currencies are SEK, EUR, and USD. Initial rates are embedded demonstration values, not live market rates:

| Conversion | Multiplier |
| --- | --- |
| SEK → EUR | 0.090 |
| SEK → USD | 0.10 |
| EUR → USD | 1.12 |

Reverse conversions use the reciprocal rate; same-currency conversions use 1. Results are rounded to two decimal places. The menu accepts currency names without regard to case and rejects negative amounts.

Customer menu option 9 calculates a conversion only; it does not change account balances. Administrator rate changes work in both directions and are lost when the application restarts.

## Project structure

```text
BankApp.slnx                       Solution file
BankApp/
├── BankApp.csproj                 .NET 10 console project
├── Program.cs                     Application entry point
├── BankApplication.cs             Login, role-based menus, processor lifecycle
├── TransactionLogger.cs           Timestamped file and console logging
├── Enums/
│   ├── Currency.cs                SEK, EUR, USD
│   └── UserRole.cs                Administrator and customer roles
├── Modules/
│   ├── BankAccount.cs             Account identity and money operations
│   ├── Customer.cs                Credentials, roles, lockout, account list
│   ├── Loan.cs                    Loan amount and rate validation
│   ├── SavingsAccount.cs          Savings interest estimate
│   └── Transactions.cs            Transaction data and status transitions
└── Services/
    ├── AccountService.cs          Account creation and owner lookup
    ├── AdminService.cs            Customer creation and validation
    ├── AuthenticationService.cs   Username and PIN login
    ├── CurrencyService.cs         Conversion and exchange-rate updates
    ├── LoanService.cs             Loan eligibility, interest, and deposit
    └── TransferService.cs         Transfer queue and 15-minute processor
```

Most types use the `BankApp` namespace, including types stored in subfolders. `TransferService` is in the global namespace.

## Data, logging, and limitations

Customers, accounts, balances, loans, exchange-rate updates, and pending transfers have no persistent storage. Restarting recreates the demonstration users and clears in-memory state.

`TransactionLogger` appends timestamped entries to `transaction-history.log` in the **current working directory**, and also prints them to the console. The log covers processor startup/shutdown, processing ticks, and transfer creation, completion, and failure. Deposits and withdrawals do not automatically write log entries. The history menu reads the entire shared file; it is not filtered by customer. Log entries survive restarts but do not restore balances or pending transfers.

Current limitations include:

- Credentials are embedded demonstration strings and PIN input is visible. There is no PIN reset or persistent user management.
- Accounts and transaction collections have no synchronization between the console menu and background processor.
- Pending transfers do not reserve funds, so later withdrawals or other transfers can cause processing to fail.
- Conversion errors after withdrawal can leave the sender debited despite a failed transfer.
- File I/O errors and end-of-input are not handled consistently.
- Loans have no repayment or debt tracking, and savings are not integrated into the menu.

## Development and verification

There is no automated test project. Run `dotnet build BankApp.slnx` to check compilation.

Manual checks should cover both demonstration logins, failed PIN lockout, administrator user creation and rate updates, account creation, valid and invalid deposits/withdrawals, and loans within and beyond the five-times-balance limit. Check currency conversions in both directions after a rate update.

For transfers, verify that queuing leaves balances unchanged, the 15-minute tick moves funds once with currency conversion, insufficient funds produce a failed transaction, and Exit stops the processor. Check the shared log separately from in-memory balances.
