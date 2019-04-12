using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();

        public Form1()
        {
            InitializeComponent();

            //Установка параметров по умолчанию
            new Settings();

            //Установка скорости и начало таймира
            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            //Начало новой игры
            StartGame();
        }

        private void StartGame()
        {
            //Отключение видимости строки для текста по среди игрового поля
            lblGameOver.Visible = false;

            //Установка параметров по умолчанию
            new Settings();

            //Очистка данных с предыдущей игры
            Snake.Clear(); 

            //Создание нового игрового объекта
            Circle head = new Circle();
            head.X = 10; //(int) pbCanvas.Size.Width / Settings.Width / 2; Установить по середине !!!!!!!!!!!!!!!!!!!!!!!!
            head.Y = 5;
            Snake.Add(head);

            lblScore.Text = Settings.Score.ToString();
            GenerateFood();
        }

        //Распоожение еды в случайном месте
        private void GenerateFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Circle();
            food.X = random.Next(0, maxXPos);
            food.Y = random.Next(0, maxYPos);
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            if(Settings.GameOver == true)
            {
                if(Input.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                if (Input.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                    Settings.direction = Direction.Right;
                else if (Input.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                    Settings.direction = Direction.Left;
                else if (Input.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                    Settings.direction = Direction.Up;
                else if (Input.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                    Settings.direction = Direction.Down;

                MovePlayer();
            }

            //Каждая часть при вызове перересовке будет перересована (обновится)
            pbCanvas.Invalidate();
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if(!Settings.GameOver)
            {
                //Установка цвета змеи
                Brush snakeColour;

                //Нарисовать змею
                for (int i = 0; i < Snake.Count; i++)
                {
                    if (i == 0)
                        snakeColour = Brushes.Black; //Рисуем голову
                    else
                        snakeColour = Brushes.Gray; //Рисуем хвост

                    //Непосредственно рисуем змею
                    canvas.FillEllipse(snakeColour, new Rectangle(Snake[i].X * Settings.Width,
                                                                  Snake[i].Y * Settings.Height,
                                                                  Settings.Width, Settings.Height));

                    //Непосредственно рисуем еду
                    canvas.FillEllipse(Brushes.Red, new Rectangle(food.X * Settings.Width,
                                                                  food.Y * Settings.Height,
                                                                  Settings.Width, Settings.Height));
                }
            }
            else
            {
                string gameOver = "Game over \nYour final score is: " + Settings.Score + "\nPress Enter to try again";
                lblGameOver.Text = gameOver;
                lblGameOver.Visible = true;
            }
        }


        private void MovePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                //Передвижение головы
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                    }

                    //Получение максимальной x и y позиции
                    int maxXPos = pbCanvas.Size.Width / Settings.Width;
                    int maxYPos = pbCanvas.Size.Height / Settings.Height;

                    //Отсдеживать не врезалась ли змея в стену (край поля)
                    if (Snake[i].X < 0 || Snake[i].Y < 0 || Snake[i].X >= maxXPos || Snake[i].Y >= maxXPos)
                    {
                        Die();
                    }

                    //Отслеживать не врезалась ли голова в хвост
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            Die();
                        }
                    }

                    //Отслеживет съела ли змея еду
                    if(Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        Eat();
                    }
                }
                else
                {
                    //Передвижение хвоста
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }

            }
        }

        private void Die()
        {
            Settings.GameOver = true;
        }

        private void Eat()
        {
            //Добавить часть к хвосту
            Circle food = new Circle();
            food.X = Snake[Snake.Count - 1].X;
            food.Y = Snake[Snake.Count - 1].Y;

            Snake.Add(food);

            Settings.Score += Settings.Points;
            lblScore.Text = Settings.Score.ToString();

            GenerateFood();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }
    }
}
