using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Command
{
    public class DevConsole
    {
        private readonly string prefix;
        private readonly IEnumerable<IConsoleCommand> commands;
        public DevConsole(string prefix, IEnumerable<IConsoleCommand> commands)
        {
            this.prefix = prefix;
            this.commands = commands;
        }
        public string ProcessCommand(string commandInput, string[] args)
        {
            foreach (var command in commands)
            {
                if (!commandInput.Equals(command.CommandWord, System.StringComparison.OrdinalIgnoreCase))
                    continue;
                else
                { return command.Process(args); }
            }
            return "FAIL! NoSuchCommand: { " + commandInput + " }";
        }
        public string ProcessCommand(string inputValue)
        {
            if (inputValue == null || inputValue.Length < prefix.Length)
                return "FAIL! NULLCOMMAND";
            inputValue = inputValue.Remove(0, prefix.Length);
            string[] inputSplit = inputValue.Split(' ');
            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();
            return ProcessCommand(commandInput, args);
        }
    }
}