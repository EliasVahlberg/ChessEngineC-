using UnityEngine;
namespace Command
{
    /*
    @File LogCommand.cs
    @author Elias Vahlberg 
    @Date 2021-07
    */
    [CreateAssetMenu(fileName = "new Log Command", menuName = "Utilities/DeveloperConsole/Commands/Log Command")]
    public class LogCommand : ConsoleCommand
    {
        public override string Process(string[] args)
        {
            if (args == null)
                return "FAIL! NULLARGUMENT";
            string logText = string.Join(" ", args);
            Debug.Log(logText);
            return "Sent to console:\" " + logText + " \"";
        }
    }
}