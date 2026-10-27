using BankApp.Enums;

namespace BankApp
{
  internal class CurrencyService
  {
    public Currency Currency { get; set; }
    private readonly Dictionary<(Currency, Currency), decimal> _exchangeRates = new Dictionary<(Currency, Currency), decimal>
    {
      {
        (Currency.SEK, Currency.EUR), 0.090m
      },
      {
        (Currency.SEK, Currency.USD), 0.10m
      },
      {
        (Currency.EUR, Currency.USD), 1.12m
      }
    };

    /// <summary>
    /// Calculates currency rates
    /// </summary>
    /// <param name="from">The currency that will be converted</param>
    /// <param name="to">The new currency rate</param>
    /// <returns>Currency rate</returns>
    /// <exception cref="NotSupportedException"></exception>
    decimal GetExchangeRate(Currency from, Currency to)
    {

      if (from == to)
      {
        return 1m;
      }

      if (_exchangeRates.TryGetValue((Currency, Currency), out decimal rate))
      {
        return rate;
      }

      if (_exchangeRates.TryGetValue((Currency, Currency), out decimal reverseRate))
      {
        return 1m / reverseRate;
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

    /// <summary>
    /// Converts currencies
    /// </summary>
    /// <param name="amount">The money amount</param>
    /// <param name="from">The currency to convert from</param>
    /// <param name="to">The currency to convert to</param>
    /// <returns>Converted amount as the new currency</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public decimal ConvertCurrency(decimal amount, Currency from, Currency to)
    {
      if (amount < 0)
      {
        throw new ArgumentOutOfRangeException(
          nameof(amount),
          "Amount cannot be negative"
        );
      }

      decimal rate = GetExchangeRate(from, to);

      return Math.Round(amount * rate, 2);
    }

    /// <summary>
    /// View which currencies are supported in the application.
    /// </summary>
    /// <returns>List of the only supported currencies.</returns>
    public List<Currency> GetSupportedCurrencies()
    {
      return Enum.GetValues<Currency>().ToList();
    }

    /// <summary>
    /// Available only for admins to update exchange rate.
    /// </summary>
    /// <param name="from">The old currency to converted</param>
    /// <param name="to">The new converted currency</param>
    /// <param name="newRate">The updated rate</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    public void UpdateExchangeRate(Currency from, Currency to, decimal newRate)
    {
      if (newRate <= 0)
      {
        throw new ArgumentOutOfRangeException(
          nameof(newRate),
          "Rate cannot be 0 or negative"
        );
      }

      if (_exchangeRates.ContainsKey((from, to)))
      {
        _exchangeRates[(from, to)] = newRate;
        return;
      }

      if (_exchangeRates.ContainsKey((from, to)))
      {
        _exchangeRates[(from, to)] = 1m / newRate;
      }

      throw new KeyNotFoundException("Currency KEY was not found");
    }
  }
}
