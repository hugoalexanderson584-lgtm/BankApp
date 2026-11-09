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

    public Customer(string name, string userName, string pin, UserRole role)
    {
      Name = name;
      UserName = userName;
      _pin = pin;
      Role = role;
    }

    public enum UserRole
    {
      Customer,
      Admin
    }

    internal bool ControlPin(string pin)
    {
      throw new NotImplementedException();
    }
  }
}
