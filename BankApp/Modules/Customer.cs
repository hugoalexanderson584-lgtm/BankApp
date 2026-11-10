using BankApp.Enums;

namespace BankApp
{
  internal class Customer
  {
    private const int MaxAttempts = 3;

    public int OwnerId { get; set; }
    public string Name { get; }
    public string UserName { get; }

    public UserRole Role { get; }

    private string _pin;

    public List<BankAccount> Accounts { get; } = new();
    public int FailedAttempts { get; private set; }

    public bool IsLocked => FailedAttempts >= MaxAttempts;

    public Customer(string name, string userName, string pin, UserRole role = UserRole.Customer)
    {
      Name = name;
      UserName = userName;
      _pin = pin;
      Role = role;
    }

    public bool ControlPin(string pin)
    {
      if (IsLocked)
      {
        return false;
      }

      if (pin == _pin)
      {
        FailedAttempts = 0;
        return true;
      }

      FailedAttempts++;
      return false;
    }
    public void AddAccount(BankAccount account)
    {
      Accounts.Add(account);
    }
  }
}
