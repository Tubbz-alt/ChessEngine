using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    class Bishop : Piece
    {
        public Bishop(string initPos, bool initColor, Board initBoard) : base(initPos, initColor, initBoard) { }

        public override List<string> GetPossibleMove()
        {
            return new List<string>();
        }
    }
}
