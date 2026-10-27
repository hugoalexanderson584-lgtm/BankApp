namespace BankApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Välkommen till banken, tryck '1' för att logga in:");
            int input = int.TryParse(Console.ReadLine(), out input);
        }
    }

}