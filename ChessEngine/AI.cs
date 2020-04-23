using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessEngine
{
    class AI : Player
    {
        private int depth;
        private bool revMaxPlayer;

        private int nodeMinValue = -999;
        private int nodeMaxValue = 999;

        public AI(bool initColor, Board initBoard) : base(initColor, initBoard) { }

        public override void GetNextMove()
        {
            Init();
        }

        public void RandomMove()
        {
            Dictionary<string, List<string>> allPossibleMoves = board.GetAllMove(color);
            List<string> keyList = new List<string>(allPossibleMoves.Keys);

            Random rand = new Random();

            if (allPossibleMoves.Count > 0)
            {
                int nbPiece = rand.Next(allPossibleMoves.Count);
                string coordPiece = keyList[nbPiece];

                int nbMove = rand.Next(allPossibleMoves[coordPiece].Count);

                Console.WriteLine("{0} to {1}", coordPiece, allPossibleMoves[coordPiece][nbMove]);

                board.SetPieceCoord(coordPiece, allPossibleMoves[coordPiece][nbMove]);
            }
        }

        public void Init()
        {
            int depth = 2;

            TreeNode<int> rootNode = new TreeNode<int>(-99, board.GetFen());

            if(color == false)
            {
                revMaxPlayer = true;
            }
            else
            {
                revMaxPlayer = false;
            }

            int rootNodeValue = Minimax(rootNode, depth, nodeMinValue,  nodeMaxValue , true);

            Console.WriteLine(rootNodeValue);

            foreach (TreeNode<int> childNode in rootNode.Children)
            {
                
                if(childNode.Value == rootNodeValue)
                {
                    board.LoadBoardWithFen(childNode.Edge);
                    break;
                }
            }
        }

        public int Minimax(TreeNode<int> node, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            AddChildren(node, maximizingPlayer);            
            
            if(depth==0 || node.Children.Count == 0)
            {
                return node.Value;
            }

            if (maximizingPlayer)
            {
                node.Value = nodeMinValue;

                foreach(TreeNode<int> childNode in node.Children)
                {
                    childNode.Value = Minimax(childNode, depth - 1, alpha, beta, !maximizingPlayer);

                    if(childNode.Value > node.Value)
                    {
                        node.Value = childNode.Value;
                    }

                    if (node.Value > alpha)
                    {
                        alpha = node.Value;
                    }

                    if (alpha >= beta)
                    {
                        break;
                    }
                }

                return node.Value;
            }
            else
            {
                node.Value = nodeMaxValue;

                foreach (TreeNode<int> childNode in node.Children)
                {
                    childNode.Value = Minimax(childNode, depth - 1, alpha, beta, !maximizingPlayer);

                    if (childNode.Value < node.Value)
                    {
                        node.Value = childNode.Value;
                    }

                    if (node.Value < beta)
                    {
                        beta = node.Value;
                    }

                    if (alpha >= beta)
                    {
                        break;
                    }
                }

                return node.Value;
            }
        }

        public void AddChildren(TreeNode<int> node, bool maximizingPlayer)
        {
            bool colorToMove = maximizingPlayer;
            int heuristicVal;

            if (revMaxPlayer)
            {
                colorToMove = !colorToMove;
            }
            
            foreach (KeyValuePair<string, List<string>> pieceMoves in board.GetAllMove(colorToMove))
            {
                foreach(string move in pieceMoves.Value)
                {
                    board.LoadBoardWithFen(node.Edge);

                    board.SetPieceCoord(pieceMoves.Key, move);

                    board.UpdateFen();

                    if (revMaxPlayer)
                    {
                        heuristicVal = board.GetHeuristicValue() * (-1);
                    }
                    else
                    {
                        heuristicVal = board.GetHeuristicValue();
                    }

                    node.AddChild(heuristicVal, board.GetFen());
                }
            }
        }
    }
}
