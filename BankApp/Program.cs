namespace BankApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StartProgram();
        }
        public static void StartProgram()
        {
            bool runProgram = true;
            while (runProgram)
            {
                Console.WriteLine("Welcome to the bank!");
                Console.WriteLine("Press 1 to log in");
                Console.WriteLine("Press 2 to exit program");
                string userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1":
                        
                        break;
                    case "2":
                        runProgram = false;
                        break;
                }
            }
        }
    }
}
