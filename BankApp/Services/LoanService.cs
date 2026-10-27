
namespace BankApp
{
    internal class LoanService
    {
        // Calculates the interest amount for a loan.
        public Decimal CalculateInterestRate(Loan loan)

        {
            return loan.BorrowAmount * loan.InterestRate;

        }

        public bool CanBorrow(decimal availableFunds, decimal borrowAmount)
        {
            return availableFunds >= borrowAmount *5 ;
        }
    }
}

