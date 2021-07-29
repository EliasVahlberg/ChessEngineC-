using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{
    public interface IAI
    {
        Move SelectMove(Board board);
    }
}

