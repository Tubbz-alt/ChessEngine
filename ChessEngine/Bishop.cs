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
            List<string> possibleMove = new List<string>();

            possibleMove.AddRange(GetLineMove(-1,-1));
            possibleMove.AddRange(GetLineMove(1,-1));
            possibleMove.AddRange(GetLineMove(1,1));
            possibleMove.AddRange(GetLineMove(-1,1));

            return possibleMove;
        }
    }
}
