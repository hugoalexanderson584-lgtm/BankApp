using BankApp.Enums;

namespace BankApp
{
  internal class SavingsAccount : BankAccount
  {
    public decimal InterestRate { get; set; }

    public SavingsAccount(string balance, decimal interestRate, Currency currency, int ownerId) : base(balance, ownerId, currency)
    {
      InterestRate = interestRate;
    }

    public decimal EstimateInterest()
    {
      return Balance * InterestRate;
    }
  }
}
