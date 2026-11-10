namespace BankApp
{
  internal class AdminService
  {
    private readonly CurrencyService _currency;
    private readonly List<Customer> _customers;

    public AdminService(CurrencyService currency, List<Customer> customers)
    {
      _currency = currency;
      _customers = customers;
    }

    public Customer CreateCustomer(string name, string userName, string pin)
    {
      // Deny empty information.
      if (string.IsNullOrWhiteSpace(name)
          || string.IsNullOrWhiteSpace(userName)
          || string.IsNullOrWhiteSpace(pin))
      {
        throw new ArgumentException("All fields are required.");
      }

      userName = userName.Trim();


      bool usernameExists = _customers.Any(customer =>
        customer.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
      );

      if (usernameExists)
      {
        throw new ArgumentException("Username already exists.");
      }

      int ownerId = _customers.Count == 0
          ? 1
          : _customers.Max(customer => customer.OwnerId) + 1;

      Customer customer = new Customer(name.Trim(), userName, pin)
      {
        OwnerId = ownerId
      };

      _customers.Add(customer);

      return customer;
    }
  }
}
