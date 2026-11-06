
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
    }
}