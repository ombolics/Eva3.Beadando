using System;
using System.Collections.Generic;
using System.Text;

namespace BlackHoleGame.Model
{
    public class FieldChangedEventArgs
    {
        private int x;
        private int y;
        private Entity entity;


        public int X { get { return x; } }
        public int Y { get { return y; } }
        public Entity Entity { get { return entity; } }

        public FieldChangedEventArgs(int x, int y, Entity entity)
        {
            this.x = x;
            this.y = y;
            this.entity = entity;
        }
    }
}
