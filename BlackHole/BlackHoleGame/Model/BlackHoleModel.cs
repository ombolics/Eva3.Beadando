using System;
using System.Collections.Generic;
using BlackHoleGame.Persistance;

namespace BlackHoleGame.Model
{
    public enum Entity
    {
        PLAYER1 = 1, PLAYER2 = 2, BLACK_HOLE  = 3, NONE = 4
    }

    public class BlackHoleModel
    {
        #region Fields

        private int tableSize;
        private List<List<Entity>> table;
        private short p1InTheHole;
        private short p2InTheHole;
        private BlackHoleDataAccess dataAccess;
        private bool firstsTurn;
        private int winnigCondition;
        #endregion

        #region Properties
        /// <value>
        /// The count of Player1's ships in the hole
        /// </value>
        public int P1InTheHole { get => p1InTheHole; }

        /// <value>
        /// The count of Playe2's ships in the hole
        /// </value>
        public int P2InTheHole { get => p2InTheHole; }

        /// <value>
        /// Whose turn it is 
        /// (<c>true</c> = Player1, 
        /// <c>false</c> = Player2)
        /// </value>
        public bool FirstsTurn 
        { 
            get => firstsTurn;
            set => firstsTurn = value;
        }

        /// <value>
        /// The width and height of the table
        /// </value>
        public int TableSize { get { return tableSize; } }

        /// <value>
        /// The count of ships that must be in the black hole to win
        /// </value>
        public int WinningCondition { get => winnigCondition; }
        #endregion

        #region Events
        public event EventHandler<FieldChangedEventArgs> FieldChanged;
        public event EventHandler<GameOverEventArgs> GameOver;
        public event EventHandler<GameUpdatedEventArgs> GameUpdated;
        #endregion

        #region EventHandlers
        /// <summary>
        /// Eventhandler of <see cref="BlackHoleDataAccess.LoadGame(string)"/>
        /// </summary>
        /// <param name="sneder">Sender object</param>
        /// <param name="e">Gamedata loaded from text</param>
        private void OnPersistance_GameLoaded(object sneder, GameLoadedEventArgs e)
        {
            this.tableSize = e.TableSize;
            this.table = e.Table;
            this.firstsTurn = e.FirstsTurn;
            this.p1InTheHole = (short)e.P1InTheHole;
            this.p2InTheHole = (short)e.P2InTheHole;
            GameUpdated(this, new GameUpdatedEventArgs(table));
        }
        #endregion

        #region Constructors
        public BlackHoleModel()
        {
            table = new List<List<Entity>>();
            p1InTheHole = 0;
            p2InTheHole = 0;
            dataAccess = new BlackHoleDataAccess();
            dataAccess.GameLoaded += this.OnPersistance_GameLoaded;
            firstsTurn = true;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Generates a new table with a specified size
        /// </summary>
        /// <param name="size">The widht and height of the new table</param>
        private void GenerateTable(int size)
        {
            table.Clear();
            for (int j = 0; j < size; j++)
            {
                List<Entity> sup = new List<Entity>();
                for (int i = 0; i < size; i++)
                {
                    if (i == size / 2 && j == i) sup.Add(Entity.BLACK_HOLE);
                    else if (i == j || i == size - 1 - j)
                    {
                        if (i < size / 2)
                            sup.Add(Entity.PLAYER1);
                        else
                            sup.Add(Entity.PLAYER2);
                    }
                    else
                        sup.Add(Entity.NONE);
                }
                table.Add(sup);
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Start a new game with a new table with a specified size
        /// </summary>
        /// <param name="size">The width and height of the new table</param>
        public void NewGame(int size)
        {
            tableSize = size;
            GenerateTable(size);
            winnigCondition = (tableSize - 1) / 2;
            p1InTheHole = 0;
            p2InTheHole = 0;
            firstsTurn = true;
        }

        /// <summary>
        /// Checks if <c>coordinate</c> is valid compared to the current <c>tableSize</c>
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public bool IsValidCoordinate((int X, int Y) coordinate)
        {
            return 0 <= coordinate.X && coordinate.X < tableSize && 0 <= coordinate.Y && coordinate.Y < tableSize; 
        }

        /// <summary>
        /// Move a players ship to another location
        /// </summary>
        /// <param name="from"> Starting coordinate</param>
        /// <param name="to">Target coordinate</param>
        /// <param name="player">
        ///The player that should be moved</param>
        ///<exception cref="ArgumentOutOfRangeException">Thrown when coordinates are out of the table </exception>
        public void Move((int X, int Y) from, (int X, int Y) to, Entity player)
        {
            if (!IsValidCoordinate(from) || !(IsValidCoordinate(to)))
            {
                throw new ArgumentOutOfRangeException("Coordinates are out of the table!");
            }

            //check if to is the black hole
            if (to.X == tableSize / 2 && to.Y == tableSize / 2)
            {
                table[from.X][from.Y] = Entity.NONE;
                 _ = player == Entity.PLAYER1 ? ++p1InTheHole : ++p2InTheHole;

                if (p1InTheHole == winnigCondition)
                {
                    GameOver(this, new GameOverEventArgs(Entity.PLAYER1));
                }
                else if (p2InTheHole == winnigCondition)
                {
                    GameOver(this, new GameOverEventArgs(Entity.PLAYER2));
                }
            }
            else
            {
                table[from.X][from.Y] = Entity.NONE;
                table[to.X][to.Y] = player;
            }
        }

        /// <summary>
        /// Save the current standings to plain text
        /// </summary>
        /// <param name="file">Name of the saving file</param>
        /// <returns>True if the saving was successful, false otherwise</returns>
        /// <exception cref="ArgumentNullException">File name is null or empty</exception>
        public bool SaveGame(string fileName)
        {
            if(String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("File name is null or empty!");
            }
            return dataAccess.SaveGame(fileName, table, p1InTheHole, p2InTheHole, firstsTurn);
        }

        /// <summary>
        /// Load the a game from a specified file
        /// </summary>
        /// <param name="file">Name of the loading file</param>
        /// <returns>True if the loading was successful, false otherwise</returns>
        /// <exception cref="ArgumentNullException">File name is null or empty</exception>
        public bool LoadGame(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("File name is null or empty!");
            }
            return dataAccess.LoadGame(fileName);
        }

        /// <summary>
        /// Calculates the moving options from a specified field. Raises <c>FieldChanged</c> event to achive that.
        /// </summary>
        /// <param name="x">The X value of the starting coordinate</param>
        /// <param name="y">The Y value of the starting coordinate</param>
        /// <exception cref="ArgumentOutOfRangeException">Coordinates are out of the table</exception>
        public void CalculateOptions(int x, int y)
        {
            if(!IsValidCoordinate((x,y)))
            {
                throw new ArgumentOutOfRangeException("Coordinates are out of the table!");
            }
            
            //upwards
            int up = 1;
            while (x - up > -1 && table[x - up][y] == Entity.NONE)
                up++;

            if (x - up > -1)
            {
                if (table[x - up][y] != Entity.BLACK_HOLE) up--;
                if (x != x - up) FieldChanged(this, new FieldChangedEventArgs(x - up, y, Entity.NONE));
            }
            else if (x != x - up + 1)
            {
                FieldChanged(this, new FieldChangedEventArgs(x - up + 1, y, Entity.NONE));
            }
            
            
            //leftwards
            int left = 1;
            while (y - left > -1 && table[x][y - left] == Entity.NONE)
                left++;
            if (y - left > -1)
            {
                if (table[x][y - left] != Entity.BLACK_HOLE) left--;
                if (y != y - left) FieldChanged(this, new FieldChangedEventArgs(x, y - left, Entity.NONE));
            }
            else if (y != y - left + 1)
            {
                FieldChanged(this, new FieldChangedEventArgs(x, y - left + 1, Entity.NONE));
            }
            
            //downwards
            int down = 1;
            while (x + down < tableSize && table[x + down][y] == Entity.NONE)
                down++;
            if (x + down < tableSize)
            {
                if (table[x + down][y] != Entity.BLACK_HOLE) down--;
                if (x != x + down) FieldChanged(this, new FieldChangedEventArgs(x + down, y, Entity.NONE));
            }
            else if (x != x + down - 1)
            {
                FieldChanged(this, new FieldChangedEventArgs(x + down - 1, y, Entity.NONE));
            }
            
            //rightwards
            int right = 1;
            while (y + right < tableSize && table[x][y + right] == Entity.NONE)
                right++;
            if (y + right < tableSize)
            {
                if (table[x][y + right] != Entity.BLACK_HOLE) right--;
                if (y != y + right) FieldChanged(this, new FieldChangedEventArgs(x, y + right, Entity.NONE));

            }
            else if (y != y + right - 1)
            {
                FieldChanged(this, new FieldChangedEventArgs(x, y + right - 1, Entity.NONE));
            }

        }
        #endregion   
    }
}

