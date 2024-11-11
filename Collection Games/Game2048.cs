using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Collection_Games
{
    public partial class Game2048 : Form
    {
        private const int GridSize = 4;
        private int[,] board = new int[GridSize, GridSize];
        private Label[,] labels = new Label[GridSize, GridSize];
        private Random random = new Random();

        public Game2048()
        {
            InitializeComponent();
            InitializeGameBoard();
            GenerateNewTile();
            GenerateNewTile();
        }

        private void InitializeGameBoard()
        {
            this.tableLayoutPanel1.ColumnCount = GridSize;
            this.tableLayoutPanel1.RowCount = GridSize;
            this.tableLayoutPanel1.Controls.Clear();

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    labels[i, j] = new Label
                    {
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Arial", 24, FontStyle.Bold),
                        BackColor = Color.LightGray,
                        Text = ""
                    };
                    tableLayoutPanel1.Controls.Add(labels[i, j], j, i);
                }
            }
        }

        private void GenerateNewTile()
        {
            var emptyCells = board.Cast<int>().Select((value, index) => new { value, index })
                                   .Where(x => x.value == 0)
                                   .Select(x => new { Row = x.index / GridSize, Col = x.index % GridSize })
                                   .ToList();

            if (emptyCells.Count > 0)
            {
                var cell = emptyCells[random.Next(emptyCells.Count)];
                board[cell.Row, cell.Col] = random.Next(10) == 0 ? 4 : 2;
                UpdateBoard();
            }
        }

        private void UpdateBoard()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    labels[i, j].Text = board[i, j] == 0 ? "" : board[i, j].ToString();
                    labels[i, j].BackColor = GetTileColor(board[i, j]);
                }
            }
        }

        // Метод для отримання кольору залежно від значення плитки
        private Color GetTileColor(int value)
        {
            switch (value)
            {
                case 2: return Color.LightYellow;
                case 4: return Color.LightGoldenrodYellow;
                case 8: return Color.Orange;
                case 16: return Color.OrangeRed;
                case 32: return Color.Red;
                case 64: return Color.DarkRed;
                case 128: return Color.Yellow;
                case 256: return Color.LightGreen;
                case 512: return Color.Green;
                case 1024: return Color.LightBlue;
                case 2048: return Color.Blue;
                default: return Color.Teal; // Кольори для чисел більше 2048
            }
        }

        private void MoveTiles(int dx, int dy)
        {
            bool moved = false;

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    int x = i;
                    int y = j;
                    while (true)
                    {
                        int nx = x + dx, ny = y + dy;
                        if (nx < 0 || nx >= GridSize || ny < 0 || ny >= GridSize || board[nx, ny] != 0 && board[nx, ny] != board[x, y])
                            break;

                        if (board[nx, ny] == 0)
                        {
                            board[nx, ny] = board[x, y];
                            board[x, y] = 0;
                            x = nx;
                            y = ny;
                            moved = true;
                        }
                        else if (board[nx, ny] == board[x, y])
                        {
                            board[nx, ny] *= 2;
                            board[x, y] = 0;
                            moved = true;
                            break;
                        }
                    }
                }
            }

            if (moved)
            {
                GenerateNewTile();
                CheckGameOver();
            }

            UpdateBoard();
        }

        private void CheckGameOver()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (board[i, j] == 0 || (i < GridSize - 1 && board[i, j] == board[i + 1, j]) ||
                        (j < GridSize - 1 && board[i, j] == board[i, j + 1]))
                        return;
                }
            }

            MessageBox.Show("Game Over!");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                    MoveTiles(-1, 0);
                    return true;
                case Keys.Down:
                    MoveTiles(1, 0);
                    return true;
                case Keys.Left:
                    MoveTiles(0, -1);
                    return true;
                case Keys.Right:
                    MoveTiles(0, 1);
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
