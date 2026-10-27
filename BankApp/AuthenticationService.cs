using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
}
