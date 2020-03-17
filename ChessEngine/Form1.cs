using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessEngine
{
    public partial class Form1 : Form
    {
        // Nombre de cellule pour un coté du plateau
        private int numOfCells = 8;
        // Taille des cellules (en px)
        private int cellSize = 100;

        private Board board;
        private bool boardIsInit;

        public Form1()
        {
            InitializeComponent();

            board = new Board();
            boardIsInit = false;
        }

        private void BoardPaint(object sender, PaintEventArgs e)
        {
            char[,] boardTab = board.GetBoardTab();

            if (!boardIsInit)
            {
                Size boardSize = boardDisp.Size;
                int nbBoxEdge = (int)Math.Sqrt(boardTab.Length);
                int boxSize = boardSize.Width / nbBoxEdge;

                Color lightColor = Color.FromArgb(238, 238, 210);
                Color darkColor = Color.FromArgb(118, 150, 86);

                SolidBrush brush = new SolidBrush(lightColor);

                for (int i=0; i<nbBoxEdge; i++)
                {
                    for(int j=0; j<nbBoxEdge; j++)
                    {
                        if((i%2 == 0 && j%2 == 0) || (i%2 != 0 && j%2 != 0))
                        {
                            brush.Color = lightColor;
                        }
                        else
                        {
                            brush.Color = darkColor;
                        }

                        Rectangle rect = new Rectangle(i*boxSize, j*boxSize, boxSize, boxSize);

                        e.Graphics.FillRectangle(brush, rect);
                    }
                }

                boardIsInit = true;
            }
        }
    }
}
