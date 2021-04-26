using System;
using System.Collections.Generic;
using BlackHoleGame.Model;
using System.IO;

namespace BlackHoleGame.Persistance
{
    public class BlackHoleDataAccess
    {
        #region Constructors
        public BlackHoleDataAccess()
        {
        }
        #endregion

        #region Events
        public event EventHandler<GameLoadedEventArgs> GameLoaded;
        #endregion

        #region Public methods
        /// <summary>
        /// Loads the gamedata from a specified file. Raises<see cref="BlackHoleDataAccess.GameLoaded"/>
        /// </summary>
        /// <param name="file"></param>
        /// <returns>True if the loading was successful, false otherwise</returns>
        public bool LoadGame(string file)
        {
            try
            {
                StreamReader sr = new StreamReader(file);
                bool firstsTurn = true;
                _ = sr.ReadLine() == "True" ? firstsTurn = true : firstsTurn = false;
                int tableSize = int.Parse(sr.ReadLine());
                int p1Score = int.Parse(sr.ReadLine());
                int p2Score = int.Parse(sr.ReadLine());
                List<List<Entity>> table = new List<List<Entity>>();
                for (int i = 0; i < tableSize; i++)
                {
                    string[] line = sr.ReadLine().Split(" ");
                    List<Entity> sup = new List<Entity>();
                    for (int j = 0; j < tableSize; j++)
                    {
                        switch (line[j])
                        {
                            case "1":
                                sup.Add(Entity.PLAYER1);
                                break;
                            case "2":
                                sup.Add(Entity.PLAYER2);
                                break;
                            case "3":
                                sup.Add(Entity.BLACK_HOLE);
                                break;
                            case "4":
                                sup.Add(Entity.NONE);
                                break;
                            default:
                                break;
                        }
                    }
                    table.Add(sup);
                }
                sr.Close();
                GameLoaded(this, new GameLoadedEventArgs(tableSize, p1Score, p2Score, table, firstsTurn));
            }
            catch (Exception)
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Saves the current standings to a specified file
        /// </summary>
        /// <param name="file">The name of the saving file</param>
        /// <param name="table"></param>
        /// <param name="p1Points"></param>
        /// <param name="p2Points"></param>
        /// <param name="firstsTurn"></param>
        /// <returns></returns>
        public bool SaveGame(string file, List<List<Entity>> table, int p1Points, int p2Points, bool firstsTurn)
        {
            try
            {
                StreamWriter sw = new StreamWriter(file);
                sw.WriteLine(firstsTurn.ToString());
                sw.WriteLine(table.Count);
                sw.WriteLine(p1Points);
                sw.WriteLine(p2Points);
                for (int i = 0; i < table.Count; i++)
                {
                    for (int j = 0; j < table.Count; j++)
                    {
                        int value = 0;
                        switch (table[j][i])
                        {
                            case Entity.PLAYER1:
                                value = 1;
                                break;
                            case Entity.PLAYER2:
                                value = 2;
                                break;
                            case Entity.BLACK_HOLE:
                                value = 3;
                                break;
                            case Entity.NONE:
                                value = 4;
                                break;
                            default:
                                break;
                        }
                        sw.Write(value.ToString() + " ");
                    }
                    sw.WriteLine();
                }
                sw.Close();
            }
            catch (Exception)
            {
                return false;
            }
            
            return true;
        }
        #endregion        
    }
}
