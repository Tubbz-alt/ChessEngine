using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    class Rook : Piece
    {
        private bool isMoved;
        public Rook(string initPos, bool initColor, Board initBoard) : base(initPos, initColor, initBoard) 
        {
            isMoved = false;
        }

        public override List<string> GetPossibleMove()
        {
            List<string> possibleMove = new List<string>();

            possibleMove.AddRange(GetLineMove(vertInc : -1));
            possibleMove.AddRange(GetLineMove(vertInc: 1));
            possibleMove.AddRange(GetLineMove(horInc: -1));
            possibleMove.AddRange(GetLineMove(horInc: 1));

            return possibleMove;
        }

        public override void SetPos(string newPos)
        {
            if (!isMoved) { isMoved = true; }
            base.SetPos(newPos);
        }
    }
}
