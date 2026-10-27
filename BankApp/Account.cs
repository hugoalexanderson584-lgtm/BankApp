using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    public class Account
    {
        public string account { get; set; }
        public Guid accountName { get; } = Guid.NewGuid();

        public double salary { get; private set; }


        public Account(string Account, int Salary, Guid AccountName)
        {
            account = Account;
            salary = Salary;
        }

        public bool Deposit(double amount)
        {
            if (amount == 0)
            {
                return false;
            }
            else
            {
                salary += amount;
                return true;
            }
        }

        public bool Withdraw(double amount)
        {
            if (amount <= 0 || amount < salary) return false;

            salary -= amount;
            return true;
        }
    }
}
