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

        public User (string Name, string Email, int Salary, string Password)
        {
            this.Name = Name;
            this.Email = Email;
            this.Salary = Salary;
            this.Password = Password;
        }



        public void Login()
        {

        }
    }

    internal class Administrator : User
    {
        public Administrator(string Name, string Email, int Salary, string Password)
            : base(Name, Email, Salary, Password)
        {

        }
    }
}
