using System;
using System.Collections.Generic;
using System.Text;
using BlackHoleGame.Model;

namespace BlackHoleGame.Persistance
{
    public class GameLoadedEventArgs
    {   
        public GameLoadedEventArgs(int tableSize, int p1, int p2, List<List<Entity>> table, bool firstsTurn)
        {
            P1InTheHole = p1;
            P2InTheHole = p2;
            Table = table;
            TableSize = tableSize;
            FirstsTurn = firstsTurn;
        }
        public int TableSize { get; }
        public List<List<Entity>> Table { get;}
        public int P1InTheHole { get;}
        public int P2InTheHole { get; }

        public bool FirstsTurn { get;}
    }
}
