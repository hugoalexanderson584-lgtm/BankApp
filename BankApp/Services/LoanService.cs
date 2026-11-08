
using System;

namespace BankApp
{
    internal class LoanService
    {
        // Calculates the interest amount for a loan.
        public decimal CalculateInterestRate(Loan loan)
        {
            if (loan == null)
            {
                throw new ArgumentNullException(nameof(loan));
            }

            return loan.BorrowAmount * loan.InterestRate;
        }

        // Checks if the customer meets the borrowing limit.
        public bool CanBorrow(decimal availableFunds, decimal borrowAmount)
        {
            if (availableFunds < 0 || borrowAmount <= 0)
            {
                return false;
            }

            return borrowAmount <= availableFunds * 5;
        }

        // Creates a loan and deposits the money into the customer's account.
        public (decimal LoanAmount, decimal InterestCost)? TakeLoan(
            BankAccount account, decimal borrowAmount, decimal interestRate)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (!CanBorrow(account.Balance, borrowAmount) || interestRate < 0)
            {
                return null;
            }

            Loan loan = new Loan(borrowAmount, interestRate);

            decimal interestCost = CalculateInterestRate(loan);

            if (!account.Deposit(loan.BorrowAmount))
            {
                return null;
            }

            return (loan.BorrowAmount, interestCost);
        }
    }
}
