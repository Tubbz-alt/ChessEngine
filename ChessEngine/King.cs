using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    class King : Piece
    {
        private bool isMoved;
        public King(string initPos, bool initColor, Board initBoard) : base(initPos, initColor, initBoard) 
        {
            isMoved = false;
        }

        public override List<string> GetPossibleMove()
        {
            possibleMoves = new List<string>();

            List<int> IjCoord = board.CoordToIj(pos);

            for(int i = IjCoord[0]-1; i<=IjCoord[0]+1; i++)
            {
                for(int j = IjCoord[1]-1; j<=IjCoord[1]+1; j++)
                {
                    if (board.InBoard(i,j))
                    {
                        if (board.IsVoid(i, j) || board.IsKillable(board.IjToCoord(i, j), color))
                        {
                            possibleMoves.Add(board.IjToCoord(i, j));
                        }
                    }  
                }
            }

            if(board.GetColorTurn() == color)
            {
                possibleMoves.AddRange(board.ChecksCastling(color));
            }

            possibleMoves = base.GetPossibleMove();

            return possibleMoves;
        }

        public override void SetPos(string newPos)
        {
            if (!isMoved) { isMoved = true; }
            base.SetPos(newPos);
        }

        public bool HasBeenMoved()
        {
            return isMoved;
        }
    }
}
