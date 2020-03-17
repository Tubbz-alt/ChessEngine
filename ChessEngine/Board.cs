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

            fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        }

        public char[,] GetBoardTab()
        {
            return boardTab;
        }
    }
}
