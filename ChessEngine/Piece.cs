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

        public Piece(string initPos, bool initColor)
        {
            pos = initPos;
            color = initColor;
        }

        public string GetPos()
        {
            return pos;
        }

        public void SetPos(string newPos)
        {
            pos = newPos;
        }

        public virtual List<string> GetPossibleMove(char[,] boardArr) 
        {
            return new List<string>();
        }
    }
}
