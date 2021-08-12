using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{
    /*
    @File IAIObject.cs
    @author Elias Vahlberg 
    @Date 2021-07 
    */
    public abstract class IAIObject : ScriptableObject, IAI
    {
        public static readonly Move PENDING_SEARCH_MOVE = new Move(32, 32, 4);
        [SerializeField] private string AIName = string.Empty;
        public string Name { get { return AIName; } }
        public abstract Move SelectMove(Board board);
    }
}