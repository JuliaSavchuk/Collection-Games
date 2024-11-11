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
    public partial class TicTacToes : Form
    {
        private Button[,] cells = new Button[3, 3]; // Матриця клітинок 3х3
        private bool isXTurn = true; // Черга гравця (X починає)
        private Label labelTurn; // Відображення ходу
        private Label labelResult; // Відображення результату гри
        private Random random = new Random(); // Для вибору випадкових ходів комп'ютера

        public TicTacToes()
        {
            InitializeComponent();
            this.Text = "Tic Tac Toe";  // Назва вікна гри
            this.Width = 400;
            this.Height = 450;

            labelTurn = new Label
            {
                Text = "Turn: X",
                Location = new Point(10, 10),
                Font = new Font("Arial", 12),
                AutoSize = true
            };
            this.Controls.Add(labelTurn);

            labelResult = new Label
            {
                Text = "",
                Location = new Point(10, 380),
                Font = new Font("Arial", 12),
                AutoSize = true
            };
            this.Controls.Add(labelResult);

            InitializeGrid();
        }

        // Ініціалізація ігрового поля
        private void InitializeGrid()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    cells[i, j] = new Button
                    {
                        Font = new Font("Arial", 24),
                        Location = new Point(10 + j * 100, 40 + i * 100),
                        Size = new Size(100, 100),
                        BackColor = Color.White
                    };
                    cells[i, j].Click += CellClick;
                    this.Controls.Add(cells[i, j]);
                }
            }
        }

        // Обробка кліку по клітинці гравця X
        private void CellClick(object sender, EventArgs e)
        {
            Button cell = sender as Button;
            if (cell == null || cell.Text != "" || !isXTurn)
            {
                return; // Пропускаємо, якщо клітинка зайнята або зараз хід комп'ютера
            }

            cell.Text = "X"; // Гравець X робить хід
            labelTurn.Text = "Turn: O (Computer)";

            if (CheckWin())
            {
                labelResult.Text = "Player X Wins!";
                DisableGrid();
            }
            else if (IsDraw())
            {
                labelResult.Text = "It's a Draw!";
            }
            else
            {
                isXTurn = false;
                ComputerMove(); // Хід комп'ютера
            }
        }

        // Хід комп'ютера
        private void ComputerMove()
        {
            var emptyCells = cells.Cast<Button>().Where(cell => cell.Text == "").ToList();

            if (emptyCells.Count > 0)
            {
                Button chosenCell = emptyCells[random.Next(emptyCells.Count)];
                chosenCell.Text = "O"; // Комп'ютер ставить "O"
                labelTurn.Text = "Turn: X";

                if (CheckWin())
                {
                    labelResult.Text = "Computer Wins!";
                    DisableGrid();
                }
                else if (IsDraw())
                {
                    labelResult.Text = "It's a Draw!";
                }
                else
                {
                    isXTurn = true; // Повертаємо чергу гравцю X
                }
            }
        }

        // Перевірка, чи є переможець
        private bool CheckWin()
        {
            for (int i = 0; i < 3; i++)
            {
                // Перевірка рядків
                if (cells[i, 0].Text != "" && cells[i, 0].Text == cells[i, 1].Text && cells[i, 1].Text == cells[i, 2].Text)
                    return true;

                // Перевірка стовпців
                if (cells[0, i].Text != "" && cells[0, i].Text == cells[1, i].Text && cells[1, i].Text == cells[2, i].Text)
                    return true;
            }

            // Перевірка діагоналей
            if (cells[0, 0].Text != "" && cells[0, 0].Text == cells[1, 1].Text && cells[1, 1].Text == cells[2, 2].Text)
                return true;

            if (cells[0, 2].Text != "" && cells[0, 2].Text == cells[1, 1].Text && cells[1, 1].Text == cells[2, 0].Text)
                return true;

            return false;
        }

        // Перевірка нічиї
        private bool IsDraw()
        {
            return cells.Cast<Button>().All(cell => cell.Text != "");
        }

        // Відключення клітинок після завершення гри
        private void DisableGrid()
        {
            foreach (var cell in cells)
            {
                cell.Enabled = false;
            }
        }

        // Скидання гри для нової партії
        private void ResetGame()
        {
            foreach (var cell in cells)
            {
                cell.Text = "";
                cell.Enabled = true;
            }

            labelResult.Text = "";
            isXTurn = true;
            labelTurn.Text = "Turn: X";
        }

        // Обробка натискання клавіші для нової гри
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R) // Натискання 'R' для перезапуску гри
            {
                ResetGame();
            }
            base.OnKeyDown(e);
        }
    }
}
