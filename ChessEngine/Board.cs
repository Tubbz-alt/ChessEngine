using System;
using System.Collections.Generic;
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
        protected bool colorTurn;

        private List<Piece> listPieces;

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
            listPieces = new List<Piece>();
            
            int len = (int)Math.Sqrt(boardTab.Length);

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    switch ((PieceEnum) boardTab[i,j])
                    {
                        case PieceEnum.WhiteKing:
                            listPieces.Add(new King(coordTab[i,j], true, this));
                            break;
                        case PieceEnum.WhiteQueen:
                            listPieces.Add(new Queen(coordTab[i, j], true, this));
                            break;
                        case PieceEnum.WhiteRook:
                            listPieces.Add(new Rook(coordTab[i, j], true, this));
                            break;
                        case PieceEnum.WhiteKnight:
                            listPieces.Add(new Knight(coordTab[i, j], true, this));
                            break;
                        case PieceEnum.WhiteBishop:
                            listPieces.Add(new Bishop(coordTab[i, j], true, this));
                            break;
                        case PieceEnum.WhitePawn:
                            listPieces.Add(new Pawn(coordTab[i, j], true, this));
                            break;
                        case PieceEnum.BlackKing:
                            listPieces.Add(new King(coordTab[i, j], false, this));
                            break;
                        case PieceEnum.BlackQueen:
                            listPieces.Add(new Queen(coordTab[i, j], false, this));
                            break;
                        case PieceEnum.BlackRook:
                            listPieces.Add(new Rook(coordTab[i, j], false, this));
                            break;
                        case PieceEnum.BlackKnight:
                            listPieces.Add(new Knight(coordTab[i, j], false, this));
                            break;
                        case PieceEnum.BlackBishop:
                            listPieces.Add(new Bishop(coordTab[i, j], false, this));
                            break;
                        case PieceEnum.BlackPawn:
                            listPieces.Add(new Pawn(coordTab[i, j], false, this));
                            break;

                    }
                }
            }
        }

        private void StartInterface()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(this));
        }

        private void StartGame()
        {
            while(true)
            {   
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
        }
        
        public char[,] GetBoardTab()
        {
            return boardTab;
        }

        public List<string> GetMove(string coord)
        {
            List<string> possibleMove = new List<string>();

            foreach(Piece piece in listPieces)
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

            foreach (Piece piece in listPieces)
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
            foreach (Piece piece in listPieces)
            {
                if (piece.GetPos() == pieceCoord)
                {
                    List<int> oldPos = CoordToIj(pieceCoord);
                    List<int> newPos = CoordToIj(newCoord);

                    char pieceLetter = boardTab[oldPos[0], oldPos[1]];
                    boardTab[oldPos[0], oldPos[1]] = ' ';

                    if (boardTab[newPos[0], newPos[1]] != ' ')
                    {
                        foreach (Piece pieceToKill in listPieces)
                        {
                            if (pieceToKill.GetPos() == newCoord)
                            {
                                listPieces.Remove(pieceToKill);
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
            List<int> ij = new List<int>();

            ij.Add(((int)Math.Sqrt(coordTab.Length) - (coord[1] - '1')) - 1);
            ij.Add(coord[0] - 'a');

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
    }
}
