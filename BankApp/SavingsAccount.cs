namespace BankApp
{
  internal class SavingsAccount : BankAccount
  {
    public decimal InterestRate { get; set; }

    public SavingsAccount(int salary, decimal interestRate) : base(salary)
    {
      InterestRate = interestRate;
    }

    public decimal EstimateInterest()
    {
      return salary * InterestRate;
    }
  }
}
