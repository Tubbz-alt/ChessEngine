using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    class Rook : Piece
    {
        public Rook(string initPos, bool initColor) : base(initPos, initColor) { }

        public override List<string> GetPossibleMove(char[,] boardArr)
        {
            return new List<string>();
        }
    }
}
