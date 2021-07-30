
using UnityEngine;
namespace ChessAI
{

    public class AIUtillsManager : MonoBehaviour
    {

        public static AIUtillsManager instance;
        private BoardScoreGenerator boardScoreGenerator;
        public BoardScoreGenerator BoardScoreGen
        {
            get
            {
                if (boardScoreGenerator == null)
                    boardScoreGenerator = new BoardScoreGenerator();
                return boardScoreGenerator;
            }
        }
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("SAMEINSTACE ");
                Destroy(this);
            }
        }
        private void Start()
        {

        }
    }
}