using BankApp.Enums;

namespace BankApp
{
  internal class AdminService
  {
    private readonly CurrencyService _currency;

    public AdminService(CurrencyService currency)
    {
      _currency = currency;
    }

    public void UpdateExchangeRate(Currency from, Currency to, decimal newRate)
    {
      _currency.UpdateExchangeRate(from, to, newRate);
    }
  }
}
