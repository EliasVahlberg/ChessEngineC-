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

    public interface IAIPlayer
    {
        string Name();
        void Initialize(Board board, AISettings settings);

        //Start search
        void RequestMove();

        //If time is up
        void SearchTimeEnded();

        //If checkmate found
        void SearchCompleted(Move move);

        //Submit move to the game manager
        void SubmitMove(Move move);

        IAIPlayer GetInstance();
    }
}

