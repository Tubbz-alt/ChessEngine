using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public class Piece : ICloneable
    {
        protected string pos;
        
        // true=white, false=black
        protected bool color;

        protected List<string> possibleMoves;

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
            if(board.GetColorTurn() == color)
            {
                possibleMoves = board.FilterMove(possibleMoves, pos, color);
            }
            
            return possibleMoves;
        }

        protected List<string> GetLineMove(int vertInc = 0, int horInc = 0)
        {
            List<int> IjCoord = board.CoordToIj(pos);

            List<string> lineMove = new List<string>();

            int i, j;

            for (int n = 1; n < board.GetBoardEdgeLen(); n++)
            {
                i = IjCoord[0] + n * vertInc;
                j = IjCoord[1] + n * horInc;

                if (board.InBoard(i, j))
                {
                    if (board.IsVoid(i, j))
                    {
                        lineMove.Add(board.IjToCoord(i, j));
                    }
                    else
                    {
                        if (board.IsKillable(board.IjToCoord(i, j), color))
                        {
                            lineMove.Add(board.IjToCoord(i, j));
                        }

                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return lineMove;
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
