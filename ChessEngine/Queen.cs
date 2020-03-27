using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    class Queen : Piece
    {
        public Queen(string initPos, bool initColor, Board initBoard) : base(initPos, initColor, initBoard) { }

        public override List<string> GetPossibleMove()
        {
            return new List<string>();
        }
    }
}
