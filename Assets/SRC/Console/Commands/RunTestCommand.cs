using System;
using UnityEngine;
namespace Command
{
    [CreateAssetMenu(fileName = "new Log Command", menuName = "Utilities/DeveloperConsole/Commands/Run Test Command")]
    public class RunTestCommand : ConsoleCommand
    {
        public override string Process(string[] args)
        {
            if (args == null)
                return "FAIL! NULLARGUMENT";
            int a = 0;
            int b = 0;
            bool c = false;
            long[] moveResult = null;
            DateTime beforeT = DateTime.Now;
            if (args.Length >= 3)
            {
                if ((int.TryParse(args[0], out a)))
                {
                    if (int.TryParse(args[1], out b))
                    {
                        if (bool.TryParse(args[2], out c))
                            moveResult = MoveTest.StandardMoveTest(a, b, c);
                        else
                            return "FAIL! ARG 3 Not boolean";

                    }
                    else
                        return "FAIL! ARG 2 Not numeric";
                }
                else
                    return "FAIL! ARG 1 Not numeric";
            }
            else if (args.Length >= 2)
            {
                if ((int.TryParse(args[0], out a)))
                {
                    if (int.TryParse(args[1], out b))
                        moveResult = MoveTest.StandardMoveTest(a, b, false);
                    else
                        return "FAIL! ARG 2 Not numeric";
                }
                else
                    return "FAIL! ARG 1 Not numeric";
            }
            else if (args.Length == 1)
            {
                if (int.TryParse(args[0], out a))
                {
                    moveResult = MoveTest.StandardMoveTest(a);
                }
                else
                    return "FAIL! ARG 2 Not numeric";

            }
            else
                return "FAIL! No argument";
            if (moveResult == null)
                return "FAIL! No such test fen index, must be between: { 0 , " + (MoveTest.TestFen.Length - 1) + " }";
            string s = "";
            TimeSpan span = DateTime.Now - beforeT;
            int ms = (int)span.TotalMilliseconds;
            float timeD = ((float)((float)ms) / 1000);
            s += "\n \t Test Compleated in " + timeD + "sec : \n";
            int i = 1;
            foreach (long l in moveResult)
            {
                s += "Ply " + i + ", Moves = " + l + "\n";
                i++;
            }
            return s;
        }
    }
}