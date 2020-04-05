using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessEngine
{
    public class Board
    {
        private char[,] boardTab;
        private string[,] coordTab;
        private string fen;

        private Player whitePlayer;
        private Player blackPlayer;

        // true=white, false=black
        private bool colorTurn;

        private bool isUpdated;

        private List<Piece> piecesList;

        public Board()
        {
            boardTab = new char[,] {{'r','n','b','q','k','b','n','r'},
                                    {'p','p','p','p','p','p','p','p'},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {'P','P','P','P','P','P','P','P'},
                                    {'R','N','B','Q','K','B','N','R'}};

            InitCoord();
            InitPiece();

            fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

            colorTurn = true;

            whitePlayer = new Human(true, this);
            blackPlayer = new AI(false, this);

            Thread interfaceThread = new Thread(new ThreadStart(StartInterface));
            interfaceThread.Start();

            isUpdated = false;

            StartGame();
        }

        private void InitCoord()
        { 
            int len = (int) Math.Sqrt(boardTab.Length);

            coordTab = new string[len,len];

            for (int i = 0; i < len; i++)
            {
                for(int j = 0; j < len; j++)
                {
                    char[] coord = { (char)(j + 'a'), (char)((len-i) + '0') };
                    coordTab[i, j] = new string(coord);
                }
            }
        }

        private void InitPiece()
        {
            piecesList = new List<Piece>();
            
            int len = (int)Math.Sqrt(boardTab.Length);

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    switch ((PieceEnum) boardTab[i,j])
                    {
                        case PieceEnum.WhiteKing:
                            piecesList.Add(new King(coordTab[i,j], true, this));
                            break;
                        case PieceEnum.WhiteQueen:
                            piecesList.Add(new Queen(coordTab[i, j], true, this));
                            break;
                        case PieceEnum.WhiteRook:
                            piecesList.Add(new Rook(coordTab[i, j], true, this));
                            break;
                        case PieceEnum.WhiteKnight:
                            piecesList.Add(new Knight(coordTab[i, j], true, this));
                            break;
                        case PieceEnum.WhiteBishop:
                            piecesList.Add(new Bishop(coordTab[i, j], true, this));
                            break;
                        case PieceEnum.WhitePawn:
                            piecesList.Add(new Pawn(coordTab[i, j], true, this));
                            break;
                        case PieceEnum.BlackKing:
                            piecesList.Add(new King(coordTab[i, j], false, this));
                            break;
                        case PieceEnum.BlackQueen:
                            piecesList.Add(new Queen(coordTab[i, j], false, this));
                            break;
                        case PieceEnum.BlackRook:
                            piecesList.Add(new Rook(coordTab[i, j], false, this));
                            break;
                        case PieceEnum.BlackKnight:
                            piecesList.Add(new Knight(coordTab[i, j], false, this));
                            break;
                        case PieceEnum.BlackBishop:
                            piecesList.Add(new Bishop(coordTab[i, j], false, this));
                            break;
                        case PieceEnum.BlackPawn:
                            piecesList.Add(new Pawn(coordTab[i, j], false, this));
                            break;

                    }
                }
            }
        }

        private void StartInterface()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BoardForm(this));
        }

        private void StartGame()
        {
            while(GetAllMove(true).Count>0 && GetAllMove(false).Count>0)
            {
                //PrintBoard();
                isUpdated = true;
                
                if(colorTurn)
                {
                    whitePlayer.GetNextMove();
                    colorTurn = false;
                }
                else
                {
                    blackPlayer.GetNextMove();
                    colorTurn = true;
                }
            }

            isUpdated = true;

            if(IsChecked(true))
            {
                Console.WriteLine("Black win by checkmate");
            }
            else if (IsChecked(false))
            {
                Console.WriteLine("White win by checkmate");
            }
            else
            {
                Console.WriteLine("Draw");
            }
        }
        
        public char[,] GetBoardTab()
        {
            return boardTab;
        }

        public List<string> GetMove(string coord)
        {
            List<string> possibleMove = new List<string>();

            foreach(Piece piece in ClonePiecesList(piecesList))
            {
                if(piece.GetPos() == coord)
                {
                    possibleMove = piece.GetPossibleMove();
                    break;
                }
            }

            return possibleMove;
        }

        public Dictionary<string,List<string>> GetAllMove(bool color)
        {
            Dictionary<string, List<string>> possibleMove = new Dictionary<string, List<string>>();

            foreach (Piece piece in ClonePiecesList(piecesList))
            {
                if(piece.GetColor() == color && piece.GetPossibleMove().Count > 0)
                {
                    possibleMove.Add(piece.GetPos(), piece.GetPossibleMove());
                }
            }

            return possibleMove;
        }

        public List<Player> GetHumanPlayer()
        {
            List<Player> humanPlayerList = new List<Player>();

            if(whitePlayer.GetType() == typeof(Human))
            {
                humanPlayerList.Add(whitePlayer);
            }

            if(blackPlayer.GetType() == typeof(Human))
            {
                humanPlayerList.Add(blackPlayer);
            }

            return humanPlayerList;
        }

        public bool GetColorTurn()
        {
            return colorTurn;
        }

        public void SetPieceCoord(string pieceCoord, string newCoord)
        {
            foreach (Piece piece in piecesList)
            {
                if (piece.GetPos() == pieceCoord)
                {
                    List<int> oldPos = CoordToIj(pieceCoord);
                    List<int> newPos = CoordToIj(newCoord);

                    char pieceLetter = boardTab[oldPos[0], oldPos[1]];
                    boardTab[oldPos[0], oldPos[1]] = ' ';

                    if (boardTab[newPos[0], newPos[1]] != ' ')
                    {
                        foreach (Piece pieceToKill in piecesList)
                        {
                            if (pieceToKill.GetPos() == newCoord)
                            {
                                piecesList.Remove(pieceToKill);
                                break;
                            }
                        }
                    }

                    // castling
                    if (piece.GetType() == typeof(King) && GetDistance(pieceCoord, newCoord) == 2)
                    {
                        string rookPos = "";
                        string newRookPos = "";
                        
                        if(newPos[1] < oldPos[1])
                        {
                            rookPos = IjToCoord(oldPos[0], 0);
                            newRookPos = IjToCoord(newPos[0], newPos[1] + 1);
                        }
                        else
                        {
                            rookPos = IjToCoord(oldPos[0], GetBoardEdgeLen()-1);
                            newRookPos = IjToCoord(newPos[0], newPos[1] - 1);
                        }

                        foreach (Piece pieceCastling in piecesList)
                        {
                            if (pieceCastling.GetPos() == rookPos)
                            {
                                List<int> rookPosIj = CoordToIj(rookPos);
                                List<int> newRookPosIj = CoordToIj(newRookPos);

                                pieceCastling.SetPos(newRookPos);
                                boardTab[newRookPosIj[0], newRookPosIj[1]] = boardTab[rookPosIj[0], rookPosIj[1]];
                                boardTab[rookPosIj[0], rookPosIj[1]] = ' ';

                                break;
                            }
                        }
                    }

                    piece.SetPos(newCoord);
                    boardTab[newPos[0], newPos[1]] = pieceLetter;

                    break;
                }
            }
        }

        public List<int> CoordToIj(string coord)
        {
            List<int> ij = new List<int>
            {
                ((int)Math.Sqrt(coordTab.Length) - (coord[1] - '1')) - 1,
                coord[0] - 'a'
            };

            return ij;
        }

        public string IjToCoord(int i, int j)
        {
            return coordTab[i,j];
        }

        public bool InBoard(int i, int j)
        {
            int lenBoard = GetBoardEdgeLen();

            if(i<0 || j<0 || i>=lenBoard || j>=lenBoard)
            {
                return false;
            }

            return true;
        }

        public int GetBoardEdgeLen()
        {
            return (int)Math.Sqrt(boardTab.Length);
        }

        public bool IsKillable(string pieceToKillCoord, bool pieceToMoveColor)
        {
            List<int> ijCoord = CoordToIj(pieceToKillCoord);

            char toKill = boardTab[ijCoord[0], ijCoord[1]];

            if((Char.IsUpper(toKill) && pieceToMoveColor == false) || (Char.IsLower(toKill) && pieceToMoveColor == true))
            {
                return true;
            }

            return false;
        }

        public bool IsVoid(int i, int j)
        {
            if(boardTab[i,j] == ' ')
            {
                return true;
            }

            return false;
        }

        public bool IsChecked(bool kingColor)
        {
            string kingPos = "";

            foreach(Piece piece in piecesList)
            {
                if(piece.GetType() == typeof(King) && piece.GetColor() == kingColor)
                {
                    kingPos = piece.GetPos();
                }
            }

            foreach (KeyValuePair<string, List<string>> pieceMoves in GetAllMove(!kingColor))
            {
                foreach(string move in pieceMoves.Value)
                {
                    if(move==kingPos)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<string> FilterMove(List<string> currentMoves, string pieceCoord, bool color)
        {
            List<string> filterMoves = new List<string>(currentMoves);

            char[,] oldBoardTab = (char[,]) boardTab.Clone();
            List<Piece> oldPiecesList = ClonePiecesList(piecesList);
                
            foreach(string move in currentMoves)
            {
                SetPieceCoord(pieceCoord, move);

                if(IsChecked(color))
                {
                    filterMoves.Remove(move);
                }

                piecesList = ClonePiecesList(oldPiecesList);
                boardTab = (char[,])oldBoardTab.Clone();
            }

            return filterMoves;
        }

        public List<Piece> ClonePiecesList(List<Piece> piecesListToClone)
        {
            List<Piece> newPiecesList = new List<Piece>();

            foreach(Piece piece in piecesListToClone)
            {
                newPiecesList.Add((Piece) piece.Clone());
            }

            return newPiecesList;
        }

        public void PrintBoard()
        {
            for(int i=0; i<GetBoardEdgeLen(); i++)
            {
                for(int j=0; j<GetBoardEdgeLen(); j++)
                {
                    Console.Write(boardTab[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public bool IsUpdated()
        {
            return isUpdated;
        }

        public void IsRefreshed()
        {
            isUpdated = false;
        }

        public List<string> ChecksCastling(bool color)
        {
            List<string> castlingMoves = new List<string>();
            King kingPiece = null;
            List<Rook> rookPieces = new List<Rook>();
            
            foreach(Piece piece in piecesList)
            {
                if(piece.GetColor() == color)
                {
                    if (piece.GetType() == typeof(King))
                    {
                        kingPiece = (King)piece;
                    }
                    else if (piece.GetType() == typeof(Rook))
                    {
                        rookPieces.Add((Rook) piece);
                    }
                }
            }

            if (!(IsChecked(color)) && !(kingPiece.HasBeenMoved()))
            {
                List<int> kingPos = CoordToIj(kingPiece.GetPos());
                Dictionary<string, List<string>> ennemyMoves = GetAllMove(!color);

                bool queenSide = false;

                foreach (Rook rook in rookPieces)
                {
                    bool canCastling = true;

                    if (!(rook.HasBeenMoved()))
                    {
                        int i = 0;
                        
                        if(color)
                        {
                            i = GetBoardEdgeLen() - 1;   
                        }


                        List<int> rookPos = CoordToIj(rook.GetPos());
                        

                        int minj, maxj;

                        if (rookPos[1] < kingPos[1] )
                        {
                            minj = rookPos[1] + 1;
                            maxj = kingPos[1] - 1;
                            queenSide = true;
                        }
                        else
                        {
                            minj = kingPos[1] + 1;
                            maxj = rookPos[1] - 1;
                            queenSide = false;
                        }

                        for (int j = minj; j <= maxj; j++)
                        {
                            if (boardTab[i,j] == ' ')
                            { 
                                if((queenSide && j >= kingPos[1]-2) || (!queenSide))
                                {
                                    foreach (KeyValuePair<string, List<string>> pieceMoves in ennemyMoves)
                                    {
                                        foreach (string move in pieceMoves.Value)
                                        {
                                            if (move == IjToCoord(i,j))
                                            {
                                                canCastling = false;
                                                break;
                                            }
                                        }
                                        if(!canCastling) { break; }
                                    }
                                }
                            }
                            else 
                            {
                                canCastling = false;
                                break; 
                            }

                            if (!canCastling)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        canCastling = false;
                    }

                    if(canCastling)
                    {
                        char[] kingPosStr = kingPiece.GetPos().ToCharArray();
                        
                        if(queenSide)
                        {
                            kingPosStr[0] -= (char) 2;
                        }
                        else
                        {
                            kingPosStr[0] += (char)2;
                        }

                        castlingMoves.Add(new string(kingPosStr));
                    }
                }
            }

            return castlingMoves;
        }

        public int GetDistance(string coord1, string coord2)
        {
            List<int> src = CoordToIj(coord1);
            List<int> dst = CoordToIj(coord2);

            return Math.Abs(src[0] - dst[0]) + Math.Abs(src[1] - dst[1]);
        }
    }
}
