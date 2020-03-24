using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public static class Globals
    {
        public static string[,] coordTab;

        public static List<int> CoordToIj(string coord)
        {
            List<int> ij = new List<int>();

            ij.Add(((int)Math.Sqrt(coordTab.Length) - (coord[1] - '1')) - 1);
            ij.Add(coord[0] - 'a');

            return ij;
        }
    }
}
