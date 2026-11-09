using System;
using System.Collections.Generic;
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
            var customer = _customers.FirstOrDefault(c => c.UserName == userName);
            if (customer == null) return null;

            return customer.ControlPin(pin) ? customer : null;
        }
    }
}



























//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;

//namespace BankApp
//{
//    internal class AuthenticationService : Customer
//    {
//        private readonly List<Customer> _customers;

//        public AuthenticationService(List<Customer> customers)
//        {
//            _customers = customers;
//        }

//        public Customer? Login(string userName, string pin)
//        {
//            var Customer = _customers.FirstOrDefault(c => c.UserName == userName);
//            if (Customer == null) return null;

//            return Customer.ControlPin(pin) ? Customer : null;

//        }

//        public bool ControlPin(string Pin)
//        {
//            if (IsLocked) return false;

//            if (Pin == _pin)
//            {
//                FailedAttempts = 0;
//                return true;
//            }

//            FailedAttempts++;
//            return false;
//        }

//        public void AddAccount(BankAccount account) => Accounts.Add(account);
//    }
//}

