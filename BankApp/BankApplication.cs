using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    internal class BankApplication
    {
        private bool IsRunning = true;

        public void Run()
        {
            List<Customer> customers = new List<Customer>();
            Customer customerTest1 = new Customer("John Eriksson", "John", "2000");
            Customer customerTest2 = new Customer("Erika Johnsson", "Erika", "b25o");
            Customer customerTest3 = new Customer("Jack Holmes", "Jack", "tack");

            customers.Add(customerTest1);
            customers.Add(customerTest2);
            customers.Add(customerTest3);

            AuthenticationService authenticationService = new AuthenticationService(customers);

            while (IsRunning)
            {
                Console.Write("Welcome to our bank, press '1' to login: ");
                string? userInput = Console.ReadLine();

                if (userInput != "1")
                {
                    Console.WriteLine("Invalid input, please try again");
                    Exit();
                    return;
                }

                Console.WriteLine("Fill in your information.");

                Console.Write("Name: ");
                string? userName = Console.ReadLine();

                Console.Write("Pin: ");
                string? userPin = Console.ReadLine();

                Customer loggedCustomer = authenticationService.Login(userName, userPin);
                if (loggedCustomer == null)
                {
                    Console.WriteLine("this account does not exist");
                }
                else
                {
                    Console.WriteLine("Login was successful!");
                }
            }
        }

        private void Exit()
        {
            IsRunning = false;
            Console.WriteLine("Application closed");
        }
    }
}
