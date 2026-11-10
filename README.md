# BankApp

BankApp is a console-based banking application made in C# using .NET 10.

The project was created as a group assignment where we worked with object-oriented programming, Git/GitHub and different parts of a banking system.

The application has two types of users: customers and administrators.

## Features

As a customer you can:

- Log in to the bank
- View your bank accounts
- Check your balance
- Deposit money
- Withdraw money
- Transfer money between accounts
- Transfer money to another customer
- Create new bank accounts
- Use different currencies
- View transaction history
- Apply for a loan
- Calculate currency exchange

As an administrator you can:

- Log in as administrator
- Create new customers
- Update currency exchange rates
- Log out or exit the application

## Login

The application creates two test users when it starts.

| User | Username | PIN |
| --- | --- | --- |
| Administrator | admin | 2000 |
| Customer | customer | pass |

The user has three attempts to enter the correct PIN. After three failed attempts, the user is locked until the application is restarted.

## Getting started

The project requires the .NET 10 SDK.

Open the solution in an IDE or run the project from the terminal.

```sh
dotnet restore BankApp.slnx
dotnet build BankApp.slnx
dotnet run --project BankApp/BankApp.csproj
```

The customer starts without a bank account, so a new account needs to be created before using deposits, withdrawals or transfers.

## Bank accounts

A bank account contains:

- Account number
- Owner ID
- Balance
- Currency

The supported currencies are:

- SEK
- EUR
- USD

New accounts start with a balance of 0.

Deposits must be greater than 0.

Withdrawals must also be greater than 0 and the customer cannot withdraw more money than the available balance.

## Transfers

Transfers are created through `TransferService`.

Before a transfer is accepted, the application checks:

- That both accounts exist
- That the sender and receiver are different accounts
- That the amount is greater than 0
- That the sender owns the account being used
- That the transaction ID is unique
- That the correct currency is used

Transfers are first saved as `pending`.

A background process checks pending transfers every 15 minutes. When the transfer is processed, the money is withdrawn from the sender and deposited into the receiver's account.

If the accounts use different currencies, the amount is converted before being deposited.

A transaction can have the status:

- pending
- completed
- failed

## Currency exchange

The application supports exchange between SEK, EUR and USD.

The default exchange rates are demonstration values and are not connected to a real banking API.

Example rates:

| Conversion | Rate |
| --- | --- |
| SEK → EUR | 0.090 |
| SEK → USD | 0.10 |
| EUR → USD | 1.12 |

Reverse conversions use the corresponding reversed rate.

Administrators can update exchange rates while the application is running.

The currency exchange option in the customer menu only calculates the converted amount. It does not move money between accounts.

## Loans

Customers can apply for a loan using one of their existing accounts.

The loan amount must be positive and cannot be more than five times the current account balance.

For example:

If the account balance is:

```text
1000 SEK
```

the maximum loan is:

```text
5000 SEK
```

If the loan is accepted, the loan amount is deposited directly into the selected account.

The application also calculates an estimated interest cost based on the interest percentage entered by the user.

## Transaction history

Transfers are written to:

```text
transaction-history.log
```

The log contains information about created, completed and failed transfers.

It is also used to log when the transfer processor starts, stops and checks pending transactions.

## Project structure

```text
BankApp.slnx
BankApp/
├── BankApp.csproj
├── Program.cs
├── BankApplication.cs
├── TransactionLogger.cs
├── Enums/
│   ├── Currency.cs
│   └── UserRole.cs
├── Modules/
│   ├── BankAccount.cs
│   ├── Customer.cs
│   ├── Loan.cs
│   ├── SavingsAccount.cs
│   └── Transactions.cs
└── Services/
    ├── AccountService.cs
    ├── AdminService.cs
    ├── AuthenticationService.cs
    ├── CurrencyService.cs
    ├── LoanService.cs
    └── TransferService.cs
```

## Testing

The project does not currently contain automated tests.

We tested the application manually during development.

Some of the things we tested were:

- Customer login
- Administrator login
- Incorrect PIN attempts
- Creating customers
- Creating bank accounts
- Deposits
- Withdrawals
- Transfers
- Currency conversion
- Updating exchange rates
- Loan limits
- Transaction history

The project can also be checked for compilation errors with:

```sh
dotnet build BankApp.slnx
```

## Limitations

The application currently stores most information in memory.

This means that accounts, customers, balances, exchange rates and pending transactions are reset when the application restarts.

The project also does not currently include:

- A database
- Persistent customer data
- Hidden PIN input
- Loan repayment
- Automatic savings interest
- Live currency exchange rates

These would be possible areas for future development.
