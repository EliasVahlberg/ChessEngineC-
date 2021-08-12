using UnityEngine;
namespace Command
{
    /*
    @File ConsoleCommand.cs
    @author Elias Vahlberg 
    @Date 2021-07
    */
    public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
    {
        [SerializeField] private string commandWord = string.Empty;
        public string CommandWord => commandWord;
        public abstract string Process(string[] args);
    }
}