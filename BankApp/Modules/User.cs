using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    internal class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Salary { get; set; }
        public string Password { get; set; }

        public User(string name, string email, int salary, string password)
        {
            Name = name;
            Email = email;
            Salary = salary;
            Password = password;
        }
    }
}
