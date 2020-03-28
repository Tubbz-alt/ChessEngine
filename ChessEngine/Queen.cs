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
            List<string> possibleMoves = new List<string>();

            Bishop fakeBishop = new Bishop(pos, color, board);
            Rook fakeRook = new Rook(pos,color,board);
            
            possibleMoves.AddRange(fakeBishop.GetPossibleMove());
            possibleMoves.AddRange(fakeRook.GetPossibleMove());

            return possibleMoves;
        }
    }
}
