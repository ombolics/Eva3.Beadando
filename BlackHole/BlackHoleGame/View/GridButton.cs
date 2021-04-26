using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BlackHoleGame.View
{
    public sealed class GridButton : Button
    {

        public int X { get; internal set; }
        public int Y { get; internal set; }

        public GridButton(Control parent, int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
