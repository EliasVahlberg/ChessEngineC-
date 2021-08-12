using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Command
{
    /*
    @File ConnectToServerCommand.cs
    @author Elias Vahlberg 
    @Date 2021-07
    */
    [CreateAssetMenu(fileName = "ConnectToServer", menuName = "Utilities/DeveloperConsole/Commands/Connect To Server")]
    public class ConnectToServerCommand : ConsoleCommand
    {
        public override string Process(string[] args)
        {
            if (args == null)
                return "FAIL! NULLARGUMENT";
            if (args.Length == 0)
                return "FAIL! NULLARGUMENT";
            else if (args.Length > 1)
                return "FAIL! TOOMANYARGUMENTS";
            else
            {
                if (args[0].ToLower() == "localhost")
                {
                    NetworkUIManager.instance.ConnectToServer("127.0.0.1");
                    return ("Connecting to: " + "127.0.0.1");
                }
                else
                {
                    NetworkUIManager.instance.ConnectToServer(args[0]);
                    return ("Connecting to: " + args[0]);
                }
            }
        }
    }
}
