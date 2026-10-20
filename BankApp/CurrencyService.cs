using BankApp.Enums;

namespace BankApp
{
  internal class CurrencyService
  {
    public Currency Currency { get; set; }

    public decimal GetExchangeRate(Currency from, Currency to)
    {

      if (from == to)
      {
        return 1m;
      }

      // SEK
      if (from == Currency.SEK && to == Currency.EUR)
      {
        return 0.090m;
      }
      if (from == Currency.SEK && to == Currency.USD)
      {
        return 0.10m;
      }

      // EUR
      if (from == Currency.EUR && to == Currency.SEK)
      {
        return 1m / 0.090m;
      }
      if (from == Currency.EUR && to == Currency.USD)
      {
        return 1.12m;
      }

      // USD
      if (from == Currency.USD && to == Currency.SEK)
      {
        return 1m / 0.10m;
      }
      if (from == Currency.USD && to == Currency.EUR)
      {
        return 1m / 1.12m;
      }

      throw new NotSupportedException("Currency not supported");
    }
  }
}
