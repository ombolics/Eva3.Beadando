using System;
using System.Collections.Generic;
using System.Text;

namespace BlackHoleGame.Model
{
    public class GameUpdatedEventArgs
    {
        public GameUpdatedEventArgs(List<List<Entity>> table)
        {
            Table = table;
        }
        public List<List<Entity>> Table { get;}
    }
}
