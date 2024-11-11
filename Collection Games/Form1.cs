namespace Collection_Games
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Snake game1 = new Snake();
            game1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TicTacToes game2 = new TicTacToes();
            game2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Game2048 game3 = new Game2048();
            game3.Show();
        }
    }
}
