using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

        private string baseFen;
        private string fen;

        private Player whitePlayer;
        private Player blackPlayer;

        // true=white, false=black
        private bool colorTurn;

        private bool isUpdated;

        private List<Piece> piecesList;

        public Board()
        {
            boardTab = new char[8,8];
            
            InitCoord();
            InitPiece();

            baseFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            fen = baseFen;

            LoadBoardWithFen(fen);


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
                    AddPiece(boardTab[i, j], coordTab[i, j]);
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
                //Console.WriteLine("Before :");
                //PrintBoard();
                
                if(colorTurn)
                {
                    if (whitePlayer.GetType() == typeof(Human))
                        isUpdated = true;


                    whitePlayer.GetNextMove();
                    colorTurn = false;
                }
                else
                {
                    if (blackPlayer.GetType() == typeof(Human))
                        isUpdated = true;

                    blackPlayer.GetNextMove();
                    colorTurn = true;
                }


                UpdateFen();
                Console.WriteLine(fen);
                //Console.WriteLine("After :");
                // PrintBoard();
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
                    if (piece.GetType() == typeof(King) && GetDistance(pieceCoord, newCoord) == 2 && oldPos[0]==newPos[0])
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

                    // promotion
                    if (piece.GetType() == typeof(Pawn) && (newPos[0] == 0 || newPos[0] == GetBoardEdgeLen() - 1))
                    {
                        piecesList.Add(new Queen(newCoord, piece.GetColor(), this));

                        boardTab[newPos[0], newPos[1]] = 'Q';

                        piecesList.Remove(piece);
                    }
                    else
                    {
                        piece.SetPos(newCoord);
                        boardTab[newPos[0], newPos[1]] = pieceLetter;
                    }

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

            if (!IsChecked(color) && !kingPiece.HasBeenMoved())
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

        public int GetHeuristicValue()
        {
            int value = 0;

            foreach(Piece piece in piecesList)
            {
                if (piece.GetColor()) 
                {
                    value += piece.GetValue();               
                }
                else
                {
                    value -= piece.GetValue();
                }
            }

            return value;
        }

        public string GetFen()
        {
            return fen;
        }

        public void UpdateFen()
        {
            string newFen = "";
            int cptVoid = 0;

            for(int i=0; i<GetBoardEdgeLen(); i++)
            {
                for(int j=0; j<GetBoardEdgeLen(); j++)
                {
                    if(boardTab[i,j] != ' ')
                    {
                        if(cptVoid > 0)
                        {
                            newFen += cptVoid.ToString();
                        }

                        cptVoid = 0;
                        
                        newFen += boardTab[i, j];
                    }
                    else
                    {
                        cptVoid++;
                    }
                }

                if (cptVoid > 0)
                {
                    newFen += cptVoid.ToString();
                }

                cptVoid = 0;

                newFen += '/';
            }

            newFen = newFen.Remove(newFen.Length - 1);

            if (colorTurn)
            {
                newFen += " w";
            }
            else
            {
                newFen += " b";
            }

            string canCastling = "";

            King whiteKing = null;
            King blackKing = null;
            List<Rook> whiteRooks = new List<Rook>();
            List<Rook> blackRooks = new List<Rook>();

            foreach (Piece piece in piecesList)
            {
                if (piece.GetType() == typeof(King))
                {
                    if (piece.GetColor() == true)
                    {
                        whiteKing = (King) piece;
                    }
                    else
                    {
                        blackKing = (King) piece;
                    }
                }
                else if (piece.GetType() == typeof(Rook))
                {
                    if (piece.GetColor() == true)
                    {
                        whiteRooks.Add((Rook) piece);
                    }
                    else
                    {
                        blackRooks.Add((Rook) piece);
                    }
                }
            }

            canCastling += CastlingFen(whiteKing, whiteRooks);
            canCastling += CastlingFen(blackKing, blackRooks);

            if(canCastling == "")
            {
                canCastling = "-";
            }

            newFen += " " + canCastling;

            fen = newFen;
        }

        private string CastlingFen(King king, List<Rook> rooks)
        {
            string canCstling = "";

            if(king != null)
            {
                List<int> kingPos = CoordToIj(king.GetPos());

                if (!king.HasBeenMoved())
                {
                    foreach (Rook rook in rooks)
                    {
                        if (!rook.HasBeenMoved())
                        {
                            List<int> rookPos = CoordToIj(rook.GetPos());

                            if (rookPos[1] > kingPos[1])
                            {
                                if (king.GetColor())
                                {
                                    canCstling += "K";
                                }
                                else
                                {
                                    canCstling += "k";
                                }
                            }
                            else
                            {
                                if (king.GetColor())
                                {
                                    canCstling += "Q";
                                }
                                else
                                {
                                    canCstling += "q";
                                }
                            }
                        }
                    }
                }
            }

            char[] charArray = canCstling.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public void LoadBoardWithFen(string fen)
        {
            piecesList.Clear();

            string[] splitFen = fen.Split(' ');

            int i=0;
            int j=0;

            foreach(string row in splitFen[0].Split('/'))
            {
                foreach(char letter in row.ToCharArray())
                {
                    if (Char.IsDigit(letter))
                    {
                        int nbVoid = (int)(letter - '0');

                        for(int k=j; k<nbVoid+j; k++)
                        {
                            boardTab[i, k] = ' ';
                        }

                        j += nbVoid;
                    }
                    else
                    {
                        boardTab[i, j] = letter;
                        AddPiece(letter, coordTab[i, j]);
                        j++;
                    }
                }

                j = 0;
                
                i++;
            }

            if(splitFen[1] == "w")
            {
                colorTurn = true;
            }
            else
            {
                colorTurn = false;
            }

            SetCastlingFenInfo(splitFen[2],true);
            SetCastlingFenInfo(splitFen[2], false);

            CheckPawnMove();
        }

        private void CheckPawnMove()
        {
            foreach(Piece piece in piecesList)
            {
                if(piece.GetType() == typeof(Pawn))
                {
                    Pawn pawn = (Pawn)piece;
                    
                    List<int> pawnPos = CoordToIj(piece.GetPos());
                    
                    if(piece.GetColor() && pawnPos[0] < GetBoardEdgeLen() - 2)
                    {
                        pawn.IsMoved();
                    }
                    if (!piece.GetColor() && pawnPos[0] > 1)
                    {
                        pawn.IsMoved();
                    }
                }
            }
        }

        private void SetCastlingFenInfo(string castlingInfo, bool color)
        {
            Tuple<King, List<Rook>> castlingPiece = GetCastlingPiece(color);

            if(castlingPiece.Item1 != null)
            {
                List<int> kingPos = CoordToIj(castlingPiece.Item1.GetPos());

                if(castlingInfo == "-" || (color && kingPos[0] != GetBoardEdgeLen() - 1) || (!color && kingPos[0] != 0) || kingPos[1] != 4)
                {
                    castlingPiece.Item1.SetCantCastling();
                }
            }

            SetRookCastling(castlingInfo, castlingPiece.Item2, color);
        }

        private void SetRookCastling(string castlingInfo, List<Rook> rooks, bool color)
        {
            Rook kingSideRook = null;
            Rook queenSideRook = null;

            foreach (Rook rook in rooks)
            {
                List<int> rookPos = CoordToIj(rook.GetPos());

                if(rookPos[1] < GetBoardEdgeLen() / 2)
                {
                    queenSideRook = rook;
                }
                else
                {
                    kingSideRook = rook;
                }
            }
            
            if (color)
            {
                if (!castlingInfo.Contains('K') && kingSideRook != null)
                {
                   kingSideRook.SetCantCastling();
                }
                if (!castlingInfo.Contains('Q') && queenSideRook != null)
                {
                   queenSideRook.SetCantCastling();
                }

            }
            else
            {
                if (!castlingInfo.Contains('k') && kingSideRook != null)
                {
                    kingSideRook.SetCantCastling();
                }
                if (!castlingInfo.Contains('q') && queenSideRook != null)
                {
                    queenSideRook.SetCantCastling();
                }
            }
        }

        private void AddPiece(char letter, string coord)
        {
            switch ((PieceEnum)letter)
            {
                case PieceEnum.WhiteKing:
                    piecesList.Add(new King(coord, true, this));
                    break;
                case PieceEnum.WhiteQueen:
                    piecesList.Add(new Queen(coord, true, this));
                    break;
                case PieceEnum.WhiteRook:
                    piecesList.Add(new Rook(coord, true, this));
                    break;
                case PieceEnum.WhiteKnight:
                    piecesList.Add(new Knight(coord, true, this));
                    break;
                case PieceEnum.WhiteBishop:
                    piecesList.Add(new Bishop(coord, true, this));
                    break;
                case PieceEnum.WhitePawn:
                    piecesList.Add(new Pawn(coord, true, this));
                    break;
                case PieceEnum.BlackKing:
                    piecesList.Add(new King(coord, false, this));
                    break;
                case PieceEnum.BlackQueen:
                    piecesList.Add(new Queen(coord, false, this));
                    break;
                case PieceEnum.BlackRook:
                    piecesList.Add(new Rook(coord, false, this));
                    break;
                case PieceEnum.BlackKnight:
                    piecesList.Add(new Knight(coord, false, this));
                    break;
                case PieceEnum.BlackBishop:
                    piecesList.Add(new Bishop(coord, false, this));
                    break;
                case PieceEnum.BlackPawn:
                    piecesList.Add(new Pawn(coord, false, this));
                    break;

            }
        }

        private Tuple<King, List<Rook>> GetCastlingPiece(bool color)
        {
            King king = null;
            List<Rook> rooks = new List<Rook>();

            foreach (Piece piece in piecesList)
            {
                if (piece.GetColor() == color)
                {
                    if (piece.GetType() == typeof(King))
                    {
                        king = (King)piece;
                    }
                    else if (piece.GetType() == typeof(Rook))
                    {
                        rooks.Add((Rook)piece);
                    }
                }
            }

            return Tuple.Create(king, rooks);
        }
    }
}
