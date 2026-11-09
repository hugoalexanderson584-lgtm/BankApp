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
      var customer = _customers.FirstOrDefault(c => c.UserName == userName);
      if (customer == null) return null;
      return customer.ControlPin(pin) ? customer : null;
    }
  }
}