using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlackHoleGame.Model;

namespace BlackHoleGame.View
{
    public partial class BlackHoleGame : Form
    {
        #region Constants
        private readonly Color noneColor = Color.White;
        private readonly Color p1Color = Color.Crimson;
        private readonly Color p2Color = Color.LightGreen;
        private readonly Color selectedColor = Color.Yellow;
        private readonly Color blackholeColor = Color.Black;
        #endregion

        #region Fields
        private List<GridButton> grid;
        private BlackHoleModel model;
        private GridButton blackHole;
        private int gridSize;
        bool turnPhaseOne;
        GridButton previousButton;

        private int formWidth;
        private int formHeigth;

        private int buttonWidth;
        private int buttonHeigth;
        #endregion

        #region Event handlers
        private void OnModel_gameUpdated(object sender, GameUpdatedEventArgs e)
        {
            if (e.Table.Count != this.gridSize)
            {
                this.gridSize = e.Table.Count;
                buttonHeigth = gameField.Height / gridSize;
                buttonWidth = gameField.Width / gridSize;
                DeleteTable();
                GenerateTable(e.Table);
            }


            int index = 0;
            for (int j = 0; j < e.Table.Count; j++)
            {
                for (int i = 0; i < e.Table.Count; i++)
                {
                    switch (e.Table[j][i])
                    {
                        case Entity.PLAYER1:
                            grid[index].BackColor = p1Color;
                            if (model.FirstsTurn)
                                grid[index].Enabled = true;
                            else
                                grid[index].Enabled = false;
                            break;
                        case Entity.PLAYER2:
                            grid[index].BackColor = p2Color;
                            if (!model.FirstsTurn)
                                grid[index].Enabled = true;
                            else
                                grid[index].Enabled = false;
                            break;
                        case Entity.BLACK_HOLE:
                            grid[index].BackColor = blackholeColor;
                            grid[index].Enabled = false;
                            break;
                        case Entity.NONE:
                            grid[index].BackColor = noneColor;
                            grid[index].Enabled = false;
                            break;
                    }
                    index++;
                }
            }

        }
        private void SaveGame(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "txt files (*.txt)|*.txt";
            saveDialog.InitialDirectory = @".\Saves";
            saveDialog.RestoreDirectory = true;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                model.SaveGame(saveDialog.FileName);
            }
        }
        private void LoadGame(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "txt files (*.txt)|*.txt";
            openDialog.InitialDirectory = @".\Saves";
            openDialog.RestoreDirectory = true;
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                model.LoadGame(openDialog.FileName);
            }
        }
        private void On_5x5newGame(object sender, EventArgs e)
        {
            NewGame(5);
        }
        private void On_7x7newGame(object sender, EventArgs e)
        {
            NewGame(7);
        }
        private void On_9x9newGame(object sender, EventArgs e)
        {
            NewGame(9);
        }
        private void OnModel_fieldChanged(object sender, FieldChangedEventArgs e)
        {
            int i = 0;
            while (i < grid.Count && (grid[i].X != e.X || grid[i].Y != e.Y))
                i++;

            switch (e.Entity)
            {
                case Entity.PLAYER1:
                    grid[i].BackColor = p1Color;
                    break;
                case Entity.PLAYER2:
                    grid[i].BackColor = p2Color;
                    break;
                case Entity.BLACK_HOLE:
                    break;
                case Entity.NONE:
                    grid[i].BackColor = selectedColor;
                    grid[i].Enabled = true;
                    grid[i].Padding = new Padding(2, 2, 2, 2);
                    break;
                default:
                    break;
            }
        }
        private void OnModel_gameOver(object sender, GameOverEventArgs e)
        {
            MessageBox.Show("A nyertes a(z) " + (e.Player == Entity.PLAYER1 ? p1Color.ToString() : p2Color.ToString()), "Game over!");
            NewGame(gridSize);
        }
        private void On_tableButtonClicked(object sender, EventArgs e)
        {
            GridButton senderButton = (GridButton)sender;

            if (turnPhaseOne)
            {
                previousButton = senderButton;
                model.CalculateOptions(senderButton.X, senderButton.Y);


                foreach (GridButton b in grid)
                {
                    if (b == senderButton) continue;
                    if (model.FirstsTurn)
                    {
                        if (b.BackColor == p1Color) b.Enabled = false;
                    }
                    else
                    {
                        if (b.BackColor == p2Color) b.Enabled = false;
                    }
                }

                turnPhaseOne = false;
            }
            else
            {

                if (previousButton != senderButton)
                {
                    previousButton.Enabled = false;
                    previousButton.BackColor = noneColor;

                    if (senderButton == blackHole)
                    {
                        senderButton.BackColor = blackholeColor;
                    }
                    else
                    {
                        senderButton.BackColor = model.FirstsTurn ? p1Color : p2Color;
                    }
                    senderButton.Enabled = false;


                    foreach (GridButton b in grid)
                    {

                        if (b.BackColor == selectedColor)
                        {
                            b.BackColor = noneColor;
                            b.Enabled = false;
                        }
                        else if (b.BackColor == (model.FirstsTurn ? p2Color : p1Color))
                        {

                            b.Enabled = true;
                        }
                        else
                            b.Enabled = false;

                    }
                    model.Move((previousButton.X, previousButton.Y), (senderButton.X, senderButton.Y), model.FirstsTurn ? Entity.PLAYER1 : Entity.PLAYER2);
                    model.FirstsTurn = !model.FirstsTurn;
                }


                foreach (GridButton b in grid)
                {
                    if (previousButton == senderButton && b.BackColor == senderButton.BackColor)
                    {
                        b.Enabled = true;
                    }
                    else if (b.BackColor == selectedColor)
                    {
                        b.Enabled = false;
                        b.BackColor = noneColor;
                    }
                }
                grid[model.TableSize * model.TableSize / 2].BackColor = blackholeColor;

                turnPhaseOne = true;
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Generates a new grid with a specified size
        /// </summary>
        /// <param name="size">The widht and height of the grid</param>
        private void GenerateTable(int size)
        {
            grid.Clear();
            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < size; i++)
                {
                    GridButton button = new GridButton(gameField, i, j);

                    if (i == size / 2 && j == size / 2) blackHole = button;

                    button.Click += On_tableButtonClicked;
                    gameField.Controls.Add(button);
                    button.Margin = new Padding(0, 0, 0, 0);
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.MinimumSize = new Size(buttonWidth, buttonHeigth);
                    button.MaximumSize = button.MinimumSize;
                    button.Visible = true;
                    button.Enabled = false;
                    button.BackColor = noneColor;
                    if (i == j || i == size - 1 - j)
                    {
                        if (j <= size / 2)
                        {
                            button.BackColor = p1Color;
                            button.Enabled = true;
                        }
                        else
                        {
                            button.BackColor = p2Color;
                            button.Enabled = false;
                        }
                    }
                    if (i == size / 2 && j == i)
                    {
                        button.BackColor = blackholeColor;
                        button.Enabled = false;
                    }
                    grid.Add(button);
                }
            }
        }

        /// <summary>
        /// Generates a new grid from a matrix of <see cref="Model.Entity"/>
        /// </summary>
        /// <param name="table">The matrix of <see cref="Model.Entity"/></param>
        private void GenerateTable(List<List<Entity>> table)
        {
            grid.Clear();
            for (int j = 0; j < table.Count; j++)
            {
                for (int i = 0; i < table.Count; i++)
                {
                    GridButton button = new GridButton(gameField, j, i);

                    button.Click += On_tableButtonClicked;
                    gameField.Controls.Add(button);
                    button.Margin = new Padding(0, 0, 0, 0);
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.MinimumSize = new Size(buttonWidth, buttonHeigth);
                    button.MaximumSize = button.MinimumSize;
                    button.Visible = true;
                    button.BackColor = noneColor;

                    grid.Add(button);
                }
            }
        }

        /// <summary>
        /// New game with a specified grid size
        /// </summary>
        /// <param name="size">The size of the new grid</param>
        public void NewGame(int size)
        {
            gridSize = size;
            buttonHeigth = gameField.Height / gridSize;
            buttonWidth = gameField.Width / gridSize;

            DeleteTable();
            GenerateTable(size);
            model.NewGame(size);
            gridSize = size;
            turnPhaseOne = true;
            model.FirstsTurn = true;
            previousButton = null;
        }

        /// <summary>
        /// Remove the existing grid from the UI, and delete it
        /// </summary>
        private void DeleteTable()
        {
            gameField.Controls.Clear();
            grid.Clear();
        }
        #endregion

        #region Constructors
        public BlackHoleGame()
        {
            InitializeComponent();
            grid = new List<GridButton>();
            blackHole = new GridButton(this, 0, 0);
            previousButton = new GridButton(this, 0, 0);
            model = new BlackHoleModel();
            model.GameOver += this.OnModel_gameOver;
            model.GameUpdated += this.OnModel_gameUpdated;
            toolStripMenuItem8.Click += SaveGame;
            toolStripMenuItem9.Click += LoadGame;
            turnPhaseOne = true;
            model.FirstsTurn = true;
            model.FieldChanged += this.OnModel_fieldChanged;

            gridSize = 9;
            model.NewGame(gridSize);
            formWidth = 500;
            formHeigth = 500 + menuStrip1.Height;

            gameField.Margin = new Padding(0, 0, 0, 0);
            gameField.Padding = new Padding(0, 0, 0, 0);

            this.Size = new Size(formWidth, formHeigth);

            buttonHeigth = gameField.Height / gridSize;
            buttonWidth = gameField.Width / gridSize;

            this.MaximumSize = this.Size;
            this.MinimumSize = this.MaximumSize;
            GenerateTable(gridSize);

            this.toolStripMenuItem3.Click += On_5x5newGame;
            this.toolStripMenuItem4.Click += On_7x7newGame;
            this.toolStripMenuItem5.Click += On_9x9newGame;
        }
        #endregion
    }
}
