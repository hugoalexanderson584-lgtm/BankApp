namespace BankApp
{
  internal class AuthenticationService
  {
    private readonly List<Customer> _customers;

    public AuthenticationService(List<Customer> customers)
    {
      _customers = customers;
    }

    public Customer? Login(string userName, string pin)
    {
      var Customer = _customers.FirstOrDefault(c => c.UserName == userName);
      if (Customer == null) return null;

      return Customer.ControlPin(pin) ? Customer : null;

    }
  }

  internal class Customer
  {
    private const int MaxAttempts = 3;

    public int OwnerId { get; set; }
    public string Name { get; }
    public string UserName { get; }
    private string _pin;

    public List<BankAccount> Accounts { get; } = new();
    public int FailedAttempts { get; private set; }

    public bool IsLocked => FailedAttempts >= MaxAttempts;

    public Customer(string name, string userName, string pin)
    {
      Name = name;
      UserName = userName;
      _pin = pin;
    }

    public bool ControlPin(string Pin)
    {
      if (IsLocked) return false;

      if (Pin == _pin)
      {
        FailedAttempts = 0;
        return true;
      }

      FailedAttempts++;
      return false;
    }

    public void AddAccount(BankAccount account) => Accounts.Add(account);
  }
}

