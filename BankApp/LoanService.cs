
namespace BankApp
{
    internal class LoanService
    {
        public Decimal CalculateInterestRate(Loan loan)

        {
            return loan.BorrowAmount * loan.InterestRate;

        }
    }
}

