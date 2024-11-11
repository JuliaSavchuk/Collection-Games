using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Collection_Games
{
    public partial class Snake : Form
    {
        // Змінні для зберігання позиції фрукту, напрямку змійки, її розміру та поточного рахунку.
        private int rI, rJ, dirX = 1, dirY = 0, score = 0;
        private PictureBox fruit;
        private PictureBox[] snake = new PictureBox[400];
        private Label labelScore;

        // Параметри вікна гри.
        private const int _width = 900, _height = 800, _sizeOfSides = 40;

        public Snake()
        {
            InitializeComponent();
            this.Text = "Snake";  // Назва вікна гри.
            this.Width = _width;  // Ширина вікна.
            this.Height = _height; // Висота вікна.

            //відображення рахунку
            labelScore = new Label { Text = "Score: 0", Location = new Point(810, 10) };
            this.Controls.Add(labelScore);

            //перший сегмент змії
            snake[0] = CreateSnakeSegment(new Point(201, 201), Color.Red);

            //фрукт і початкове розташування
            fruit = CreateFruit();
            _generateMap();   //межі карти.
            _generateFruit(); //фрукт на екрані

            //таймер для оновлення гри кожні 200 мс
            timer.Tick += (sender, e) => _update();
            timer.Interval = 500;
            timer.Start();

            //натискання клавіш для управління змією
            this.KeyDown += OKP;
        }

        //створення сегмента змії
        private PictureBox CreateSnakeSegment(Point location, Color color)
        {
            var segment = new PictureBox
            {
                Location = location,
                Size = new Size(_sizeOfSides - 1, _sizeOfSides - 1), // Розмір сегмента
                BackColor = color
            };
            this.Controls.Add(segment);
            return segment;
        }

        // Метод для створення фрукту
        private PictureBox CreateFruit()
        {
            var pic = new PictureBox
            {
                BackColor = Color.Yellow,
                Size = new Size(_sizeOfSides, _sizeOfSides)
            };
            this.Controls.Add(pic);
            return pic;
        }

        //генерація випадкової позиції для фрукту
        private void _generateFruit()
        {
            Random r = new Random();
            rI = (r.Next(0, _height / _sizeOfSides) * _sizeOfSides) + 1;
            rJ = (r.Next(0, _width / _sizeOfSides) * _sizeOfSides) + 1;
            fruit.Location = new Point(rJ, rI);
        }

        // Метод для перевірки чи змійка не вийшла за межі карти
        private void _checkBorders()
        {
            // Якщо змійка вийшла за межі карти
            if (snake[0].Location.X < 0 || snake[0].Location.X >= _width || snake[0].Location.Y < 0 || snake[0].Location.Y >= _height)
            {
                ResetGame(); //скидаємо гру.
            }
        }

        // Метод для скидання гри
        private void ResetGame()
        {
            foreach (var segment in snake.Take(score + 1))
            {
                this.Controls.Remove(segment);
            }

            Array.Clear(snake, 0, snake.Length);
            score = 0;
            labelScore.Text = "Score: 0";
            dirX = 1;
            dirY = 0;
            snake[0] = CreateSnakeSegment(new Point(201, 201), Color.Red);
            _generateFruit();
            timer.Start();


            //// Видаляємо всі сегменти змійки і скидаємо рахунок
            //foreach (var segment in snake.Take(score + 1))
            //{
            //    this.Controls.Remove(segment);
            //}

            //score = 0;
            //labelScore.Text = "Score: 0";

            //dirX = 1;
            //dirY = 0;
        }

        //перевірка, чи змійка не з'їла саму себе
        private void _eatItself()
        {
            // Перевірка, чи змійка має хоча б один елемент
            if (score > 0)
            {
                // Пропускаємо перший елемент (голову змії) і перевіряємо на співпадіння місця з іншими сегментами змії
                if (snake.Skip(1).Any(segment => segment != null && segment.Location == snake[0].Location))
                {
                    ResetGame();  // Скидаємо гру, якщо змійка з'їла саму себе
                }
            }
        } 

        //private void _eatItself()
        //{
        //    if (snake.Skip(1).Any(segment => segment.Location == snake[0].Location))
        //    {
        //        ResetGame();
        //    }
        //}

        //перевірка, чи змійка з'їла фрукт.
        private void _eatFruit()
        {
            if (snake[0].Location == fruit.Location)
            {
                labelScore.Text = "Score: " + ++score;  // Збільшуємо рахунок.
                snake[score] = CreateSnakeSegment(
                    new Point(snake[score - 1].Location.X + _sizeOfSides * dirX,
                              snake[score - 1].Location.Y + _sizeOfSides * dirY),
                    Color.Red);
                _generateFruit();
            }
        }

        // Метод для генерації меж карти
        private void _generateMap()
        {
            for (int i = 0; i < _height / _sizeOfSides; i++)
            {
                var horLine = new PictureBox { BackColor = Color.Black, Location = new Point(0, i * _sizeOfSides), Size = new Size(_width, 1) };
                this.Controls.Add(horLine);

                var verLine = new PictureBox { BackColor = Color.Black, Location = new Point(i * _sizeOfSides, 0), Size = new Size(1, _height) };
                this.Controls.Add(verLine);
            }
        }

        //рух змійки
        private void _moveSnake()
        {
            for (int i = score; i >= 1; i--)
            {
                snake[i].Location = snake[i - 1].Location;
            }

            // Переміщаємо голову змії в новому напрямку.
            snake[0].Location = new Point(snake[0].Location.X + dirX * _sizeOfSides, snake[0].Location.Y + dirY * _sizeOfSides);

            // Перевіряємо, чи не з'їла змійка саму себе.
            _eatItself();
        }

        //оновлення гри: перевірка меж, їжа та рух змії
        private void _update()
        {
            _checkBorders();  // Перевірка меж.
            _eatFruit();      // Перевірка, чи з'їла змійка фрукт.
            _moveSnake();     // Рух змійки.
        }

        // Обробка натискання клавіш для зміни напрямку руху змії.
        private void OKP(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right: dirX = 1; dirY = 0; break;
                case Keys.Left: dirX = -1; dirY = 0; break;
                case Keys.Up: dirY = -1; dirX = 0; break;
                case Keys.Down: dirY = 1; dirX = 0; break;
            }
        }
    }
}
//namespace Collection_Games
//{
//    public partial class Snake : Form
//    {
//        private int rI, rJ;
//        private PictureBox fruit;
//        private PictureBox[] snake = new PictureBox[400];
//        private Label labelScore;
//        private int dirX, dirY;
//        private int _width = 900;
//        private int _height = 800;
//        private int _sizeOfSides = 40;
//        private int score = 0;
//        public Snake()
//        {
//            InitializeComponent();
//            this.Text = "Snake";
//            this.Width = _width;
//            this.Height = _height;
//            dirX = 1;
//            dirY = 0;
//            labelScore = new Label();
//            labelScore.Text = "Score: 0";
//            labelScore.Location = new Point(810, 10);
//            this.Controls.Add(labelScore);
//            snake[0] = new PictureBox();
//            snake[0].Location = new Point(201, 201);
//            snake[0].Size = new Size(_sizeOfSides - 1, _sizeOfSides - 1);
//            snake[0].BackColor = Color.Red;
//            this.Controls.Add(snake[0]);
//            fruit = new PictureBox();
//            fruit.BackColor = Color.Yellow;
//            fruit.Size = new Size(_sizeOfSides, _sizeOfSides);
//            _generateMap();
//            _generateFruit();
//            timer.Tick += new EventHandler(_update);
//            timer.Interval = 200;
//            timer.Start();
//            this.KeyDown += new KeyEventHandler(OKP);
//        }

//        private void _generateFruit()
//        {
//            Random r = new Random();
//            rI = r.Next(0, _height - _sizeOfSides);
//            int tempI = rI % _sizeOfSides;
//            rI -= tempI;
//            rJ = r.Next(0, _height - _sizeOfSides);
//            int tempJ = rJ % _sizeOfSides;
//            rJ -= tempJ;
//            rI++;
//            rJ++;
//            fruit.Location = new Point(rI, rJ);
//            this.Controls.Add(fruit);
//        }

//        private void _checkBorders()
//        {
//            if (snake[0].Location.X < 0)
//            {
//                for (int _i = 1; _i <= score; _i++)
//                {
//                    this.Controls.Remove(snake[_i]);
//                }
//                score = 0;
//                labelScore.Text = "Score: " + score;
//                dirX = 1;
//            }
//            if (snake[0].Location.X > _height)
//            {
//                for (int _i = 1; _i <= score; _i++)
//                {
//                    this.Controls.Remove(snake[_i]);
//                }
//                score = 0;
//                labelScore.Text = "Score: " + score;
//                dirX = -1;
//            }
//            if (snake[0].Location.Y < 0)
//            {
//                for (int _i = 1; _i <= score; _i++)
//                {
//                    this.Controls.Remove(snake[_i]);
//                }
//                score = 0;
//                labelScore.Text = "Score: " + score;
//                dirY = 1;
//            }
//            if (snake[0].Location.Y > _height)
//            {
//                for (int _i = 1; _i <= score; _i++)
//                {
//                    this.Controls.Remove(snake[_i]);
//                }
//                score = 0;
//                labelScore.Text = "Score: " + score;
//                dirY = -1;
//            }
//        }

//        private void _eatItself()
//        {
//            for (int _i = 1; _i < score; _i++)
//            {
//                if (snake[0].Location == snake[_i].Location)
//                {
//                    for (int _j = _i; _j <= score; _j++)
//                        this.Controls.Remove(snake[_j]);
//                    score = score - (score - _i + 1);
//                    labelScore.Text = "Score: " + score;
//                }
//            }
//        }

//        private void _eatFruit()
//        {
//            if (snake[0].Location.X == rI && snake[0].Location.Y == rJ)
//            {
//                labelScore.Text = "Score: " + ++score;
//                snake[score] = new PictureBox();
//                snake[score].Location = new Point(snake[score - 1].Location.X + 40 * dirX, snake[score - 1].Location.Y - 40 * dirY);
//                snake[score].Size = new Size(_sizeOfSides - 1, _sizeOfSides - 1);
//                snake[score].BackColor = Color.Red;
//                this.Controls.Add(snake[score]);
//                _generateFruit();
//            }
//        }

//        private void _generateMap()
//        {
//            for (int i = 0; i < _width / _sizeOfSides; i++)
//            {
//                PictureBox pic = new PictureBox();
//                pic.BackColor = Color.Black;
//                pic.Location = new Point(0, _sizeOfSides * i);
//                pic.Size = new Size(_width - 100, 1);
//                this.Controls.Add(pic);
//            }
//            for (int i = 0; i <= _height / _sizeOfSides; i++)
//            {
//                PictureBox pic = new PictureBox();
//                pic.BackColor = Color.Black;
//                pic.Location = new Point(_sizeOfSides * i, 0);
//                pic.Size = new Size(1, _width);
//                this.Controls.Add(pic);
//            }
//        }

//        private void _moveSnake()
//        {
//            for (int i = score; i >= 1; i--)
//            {
//                snake[i].Location = snake[i - 1].Location;
//            }
//            snake[0].Location = new Point(snake[0].Location.X + dirX * (_sizeOfSides), snake[0].Location.Y + dirY * (_sizeOfSides));
//            _eatItself();
//        }

//        private void _update(Object myObject, EventArgs eventsArgs)
//        {
//            _checkBorders();
//            _eatFruit();
//            _moveSnake();
//        }

//        private void OKP(object sender, KeyEventArgs e)
//        {
//            switch (e.KeyCode.ToString())
//            {
//                case "Right":
//                    dirX = 1;
//                    dirY = 0;
//                    break;
//                case "Left":
//                    dirX = -1;
//                    dirY = 0;
//                    break;
//                case "Up":
//                    dirY = -1;
//                    dirX = 0;
//                    break;
//                case "Down":
//                    dirY = 1;
//                    dirX = 0;
//                    break;
//            }
//        }
//    }
//}
