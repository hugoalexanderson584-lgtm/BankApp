
namespace BankApp
{
    internal class LoanService
    {
        public Decimal CalculateInterestRate(Loan loan)

        {
            return loan.BorrowAmount * loan.InterestRate;

        }

        public bool CanBorraw(decimal availableFunds, decimal borrowAmount)
        {
            return availableFunds >= borrowAmount *5 ;
        }
    }
}

