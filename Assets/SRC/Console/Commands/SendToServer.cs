using System;
using UnityEngine;
namespace Command
{
    /*
    @File SendToServer.cs
    @author Elias Vahlberg 
    @Date 2021-07
    */

    [CreateAssetMenu(fileName = "new Send to server Command", menuName = "Utilities/DeveloperConsole/Commands/Send To Server")]
    public class SendToServer : ConsoleCommand
    {
        public override string Process(string[] args)
        {
            if (args == null)
                return "FAIL! NULLARGUMENT";
            switch (args[0].ToLower())
            {
                case "move": return processMove(args);
                case "fen": return processFEN(args);
                default: return "FAIL! UNKNOWNPACKAGETYPE";
            }
        }
        private string processMove(string[] args)
        {
            try
            {


                if (args.Length != 4)
                {
                    return "FAIL! To few arguments for complete move";
                }
                int a1 = 0;
                int a2 = 0;
                int a3 = 0;
                if (!int.TryParse(args[1], out a1))
                    return "FAIL! PARSEFAIL for arg 1";
                if (!int.TryParse(args[2], out a2))
                    return "FAIL! PARSEFAIL for arg 2";
                if (!int.TryParse(args[3], out a3))
                    return "FAIL! PARSEFAIL for arg 3";
                ushort _move = (ushort)((a1 & (0b111111)) | ((a2 << 6) & 0b111111000000) | (a3 >> 12) & 0b111000000000000);
                ClientSend.ChessMove(_move);
                return "Move sent to server:\" " + _move + " \"";
            }
            catch (Exception _ex)
            {
                return $"FAIL! Exception during processMove : {_ex}";
            }

        }
        private string processFEN(string[] args)
        {
            if (args.Length < 3)
                return "FAIL! To few arguments, no fen";
            if (args[1] == null)
                return "FAIL! Arg 1 is NULL";
            if (args[2] == null)
                return "FAIL! Arg 2 is NULL";
            int a = 0;
            bool isWhite = false;
            if (int.TryParse(args[2], out a))
                isWhite = a != 0;
            else if (bool.TryParse(args[2].ToLower(), out isWhite))
            { }
            else
            { return "FAIL! PARSEFAIL for arg 2"; }

            ClientSend.FenSelect(args[1], isWhite);
            return "FEN sent to server:\" " + args[1] + " \". Playing as: \"" + (isWhite ? "WHITE" : "BLACK") + "\".";

        }
    }
}
