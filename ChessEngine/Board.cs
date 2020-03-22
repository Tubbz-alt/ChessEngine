using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    class Board
    {
        private char[,] boardTab;
        private string fen;

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
        }

        private void InitCoord()
        { 
            int len = (int) Math.Sqrt(boardTab.Length);

            Globals.coordTab = new string[len,len];

            for (int i = 0; i < len; i++)
            {
                for(int j = 0; j < len; j++)
                {
                    char[] coord = { (char)(j + 'a'), (char)((len-i) + '0') };
                    Globals.coordTab[i, j] = new string(coord);
                }
            }
        }

        private void InitPiece()
        {
            int len = (int)Math.Sqrt(boardTab.Length);

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    switch ((PieceEnum) boardTab[i,j])
                    {
                        case PieceEnum.WhiteKing:
                            listPieces.Add(new King(Globals.coordTab[i,j], true));
                            break;
                        case PieceEnum.WhiteQueen:
                            listPieces.Add(new Queen(Globals.coordTab[i, j], true));
                            break;
                        case PieceEnum.WhiteRook:
                            listPieces.Add(new Rook(Globals.coordTab[i, j], true));
                            break;
                        case PieceEnum.WhiteKnight:
                            listPieces.Add(new Knight(Globals.coordTab[i, j], true));
                            break;
                        case PieceEnum.WhiteBishop:
                            listPieces.Add(new Bishop(Globals.coordTab[i, j], true));
                            break;
                        case PieceEnum.WhitePawn:
                            listPieces.Add(new Pawn(Globals.coordTab[i, j], true));
                            break;
                        case PieceEnum.BlackKing:
                            listPieces.Add(new King(Globals.coordTab[i, j], false));
                            break;
                        case PieceEnum.BlackQueen:
                            listPieces.Add(new Queen(Globals.coordTab[i, j], false));
                            break;
                        case PieceEnum.BlackRook:
                            listPieces.Add(new Rook(Globals.coordTab[i, j], false));
                            break;
                        case PieceEnum.BlackKnight:
                            listPieces.Add(new Knight(Globals.coordTab[i, j], false));
                            break;
                        case PieceEnum.BlackBishop:
                            listPieces.Add(new Bishop(Globals.coordTab[i, j], false));
                            break;
                        case PieceEnum.BlackPawn:
                            listPieces.Add(new Pawn(Globals.coordTab[i, j], false));
                            break;

                    }
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
                    possibleMove = piece.GetPossibleMove(boardTab);
                    break;
                }
            }

            return possibleMove;
        }
    }
}
