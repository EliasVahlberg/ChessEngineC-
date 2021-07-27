namespace Command
{
    public interface IConsoleCommand
    {
        string CommandWord { get; }
        string Process(string[] args);
    }
}