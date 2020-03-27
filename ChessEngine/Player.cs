using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public class Player
    {
        private bool color;
        private Board board;

        public Player(bool initColor, Board initBoard)
        {
            color = initColor;
            board = initBoard;
        }

        public virtual void GetNextMove(){}

        public bool GetColor()
        {
            return color;
        }
    }
}
