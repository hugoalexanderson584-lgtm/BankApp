using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    internal class BankApplication
    {

        bool Isrunning;

        
        public void Run()
        {

            User user = new User("", "", 0, "");


            while(Isrunning == true)
            {
                Console.WriteLine("Välkommen till banken, tryck '1' för att logga in:");

                string? text = Console.ReadLine();

                if (!int.TryParse(Console.ReadLine(), out int input) || input != 1)
                {
                    Console.WriteLine("Invalid input, please try again:");
                    continue;
                }

                Console.WriteLine("Skriv in dina inloggningsuppgifter:");
            }
        }
    }
}
