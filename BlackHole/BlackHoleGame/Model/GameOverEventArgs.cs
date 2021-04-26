using System;
using System.Collections.Generic;
using System.Text;

namespace BlackHoleGame.Model
{
    public class GameOverEventArgs
    {
        public Entity Player { get; }
        public GameOverEventArgs(Entity player)
        {
            this.Player = player;
        }
    }
}
