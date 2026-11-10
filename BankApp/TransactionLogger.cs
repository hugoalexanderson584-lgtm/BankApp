namespace BankApp
{
  public class TransactionLogger
  {
    private readonly string _filePath = "transaction-history.log";

    public void Log(string message)
    {
      string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}";

      File.AppendAllText(
          _filePath,
          entry + Environment.NewLine);

      Console.WriteLine($"[LOG] {entry}");
    }
  }
}
