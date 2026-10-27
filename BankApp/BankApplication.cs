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

                Console.WriteLine("Fill in your information");

                // Authentication here

            }
        }

        private void Exit()
        {
            IsRunning = false;
            Console.WriteLine("Application closed");
        }
    }
}
