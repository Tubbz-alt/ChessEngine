using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    class Pawn : Piece
    {
        private bool firstMove;
        
        public Pawn(string initPos, bool initColor, Board initBoard) : base(initPos, initColor, initBoard) 
        {
            firstMove = true;
        }

        public override void SetPos(string newPos)
        {
            pos = newPos;

            if(firstMove)
            {
                firstMove = false;
            }
        }

        public override List<string> GetPossibleMove()
        {
            char[,] boardArr = board.GetBoardTab();

            List<string> possibleMoves = new List<string>();

            List<int> ijCoord = board.CoordToIj(pos);

            if (color)
            {
                char nextElem = boardArr[ijCoord[0] -1, ijCoord[1]];

                if (nextElem == ' ')
                {
                    possibleMoves.Add(board.IjToCoord(ijCoord[0] - 1, ijCoord[1]));

                    if (firstMove)
                    {
                        possibleMoves.Add(board.IjToCoord(ijCoord[0] - 2, ijCoord[1]));
                    }
                }
                
                if(Char.IsLower(boardArr[ijCoord[0] - 1, ijCoord[1] - 1]))
                {
                    possibleMoves.Add(board.IjToCoord(ijCoord[0] - 1, ijCoord[1] - 1));
                }
                if(Char.IsLower(boardArr[ijCoord[0] - 1, ijCoord[1] + 1]))
                {
                    possibleMoves.Add(board.IjToCoord(ijCoord[0] - 1, ijCoord[1] + 1));
                }
            }
            else
            {
                char nextElem = boardArr[ijCoord[0] + 1, ijCoord[1]];

                if (nextElem == ' ')
                {
                    possibleMoves.Add(board.IjToCoord(ijCoord[0] + 1, ijCoord[1]));
                    
                    if (firstMove)
                    {
                        possibleMoves.Add(board.IjToCoord(ijCoord[0] + 2, ijCoord[1]));
                    }
                }

                if (Char.IsUpper(boardArr[ijCoord[0] + 1, ijCoord[1] - 1]))
                {
                    possibleMoves.Add(board.IjToCoord(ijCoord[0] + 1, ijCoord[1] - 1));
                }
                if (Char.IsUpper(boardArr[ijCoord[0] + 1, ijCoord[1] + 1]))
                {
                    possibleMoves.Add(board.IjToCoord(ijCoord[0] + 1, ijCoord[1] + 1));
                }

            }

            return possibleMoves;
        }
    }
}
