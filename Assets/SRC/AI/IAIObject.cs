using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{

    public abstract class IAIObject : ScriptableObject, IAI
    {
        [SerializeField] private string AIName = string.Empty;
        public abstract Move SelectMove(Board board);
    }
}