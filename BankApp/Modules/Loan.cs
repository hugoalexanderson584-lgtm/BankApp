
namespace BankApp
{
    internal class Loan
    {
        public decimal  BorrowAmount { get; private set; } 
        
        public decimal InterestRate { get; private set; }

        public Loan(decimal borrowAmount, decimal interestRate)
        {
            BorrowAmount = borrowAmount;
            InterestRate = interestRate;
        }
    }
}



