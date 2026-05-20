using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        // Variabilele globale ale jocului
        private List<Rectangle> Snake = new List<Rectangle>();
        private Rectangle food = new Rectangle();
        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        private string direction = "right";
        private int score = 0;
        private Random rand = new Random();

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true; // Permite formei sa vada tastele apasate
            SetupGame();
        }

        private void SetupGame()
        {
            this.Text = "Snake Game - Scor: 0";
            this.BackColor = Color.Black;
            this.DoubleBuffered = true; 

            gameTimer.Interval = 100; // Viteza 
            gameTimer.Tick -= UpdateGame; // Ne asiguram ca nu avem event uri dublate
            gameTimer.Tick += UpdateGame;
            gameTimer.Start();

            score = 0;
            direction = "right";

            // Cream sarpele initial 
            Snake.Clear();
            Snake.Add(new Rectangle(100, 100, 20, 20));
            Snake.Add(new Rectangle(80, 100, 20, 20));
            Snake.Add(new Rectangle(60, 100, 20, 20));

            SpawnFood();
        }

        private void SpawnFood()
        {
            // Mancarea apare random la coordonate multiplu de 20
            food = new Rectangle(rand.Next(0, 19) * 20, rand.Next(0, 19) * 20, 20, 20);
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            // Miscam capul
            Rectangle newHead = Snake[0];
            if (direction == "right") newHead.X += 20;
            if (direction == "left") newHead.X -= 20;
            if (direction == "up") newHead.Y -= 20;
            if (direction == "down") newHead.Y += 20;

            // Verificam coliziunea cu marginile sau cu propriul corp
            if (newHead.X < 0 || newHead.X >= 400 || newHead.Y < 0 || newHead.Y >= 400)
            {
                GameOver();
                return;
            }

            for (int i = 1; i < Snake.Count; i++)
            {
                if (newHead.IntersectsWith(Snake[i]))
                {
                    GameOver();
                    return;
                }
            }

            Snake.Insert(0, newHead);

            // Verificam daca a mancat
            if (newHead.IntersectsWith(food))
            {
                score += 10;
                this.Text = "Snake Game - Scor: " + score;
                SpawnFood();
            }
            else
            {
                Snake.RemoveAt(Snake.Count - 1);
            }

            this.Invalidate(); // Redesenam ecranul
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show("Game Over! Scor final: " + score);
            SetupGame();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillEllipse(Brushes.Red, food); // Desenam mancarea

            foreach (var segment in Snake)
            {
                g.FillRectangle(Brushes.Green, segment); // Desenam corpul
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right && direction != "left") direction = "right";
            if (e.KeyCode == Keys.Left && direction != "right") direction = "left";
            if (e.KeyCode == Keys.Up && direction != "down") direction = "up";
            if (e.KeyCode == Keys.Down && direction != "up") direction = "down";
        }
    }
}