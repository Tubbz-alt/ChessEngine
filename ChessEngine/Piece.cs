using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    class Piece
    {
        protected string pos;
        
        // true=white, false=black
        protected bool color;

        protected Board board;

        public Piece(string initPos, bool initColor, Board initBoard)
        {
            pos = initPos;
            color = initColor;
            board = initBoard;
        }

        public string GetPos()
        {
            return pos;
        }

        public virtual void SetPos(string newPos)
        {
            pos = newPos;
        }

        public bool GetColor()
        {
            return color;
        }

        public virtual List<string> GetPossibleMove() 
        {
            return new List<string>();
        }
    }
}
