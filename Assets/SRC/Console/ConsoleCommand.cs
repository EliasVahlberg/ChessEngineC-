using UnityEngine;
namespace Command
{
    public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
    {
        [SerializeField] private string commandWord = string.Empty;
        public string CommandWord => commandWord;
        public abstract string Process(string[] args);
    }
}