using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace BankApp
{
    internal class Customer
    {
        private const int MaxAttempts = 3;

        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; }
        public string UserName { get; }
        private string _pin;

        public List<Account> Accounts { get; } = new();
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

        public void AddAccount(Account account) => Accounts.Add(account);
    }
}
