using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{
    /*
    @File IAI.cs
    @author Elias Vahlberg 
    @Date 2021-07 
    */
    public interface IAI
    {
        Move SelectMove(Board board);
    }
}

