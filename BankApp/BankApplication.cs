using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    internal class BankApplication
    {
        private bool IsRunning;

        public void Run()
        {
            User user = new User("", "", 0, "");

            while (IsRunning)
            {
                Console.WriteLine("Välkommen till banken, tryck '1' för att logga in:");

                string? userInput = Console.ReadLine();

                if (userInput != "1")
                {
                    Console.WriteLine("Invalid input, please try again");
                    Exit();
                }

                Console.WriteLine("Skriv in dina inloggningsuppgifter:");
            }
        }

        public void Exit()
        {
            IsRunning = false;
            Console.WriteLine("Application closed");
            return;
        }
    }
}
