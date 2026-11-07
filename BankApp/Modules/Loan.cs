using System;
namespace BankApp
{
    internal class Loan
    {
        public decimal BorrowAmount { get; private set; }

        public decimal InterestRate { get; private set; }

        public Loan(decimal borrowAmount, decimal interestRate)
        {
            if (borrowAmount <= 0)
            {
                throw new ArgumentException("Loan amount must be greater than zero.");
            }

            if (interestRate < 0)
            {
                throw new ArgumentException("Interest rate cannot be negative.");
            }

            BorrowAmount = borrowAmount;
            InterestRate = interestRate;
        }
    }
}


