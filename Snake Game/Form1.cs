﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake_Game
{
    public partial class Form1 : Form
    {
        //snake Defaults
        PictureBox[] snakeParts;
        int snakeSize = 5;
        Point Location = new Point(120, 120);
        string direction = "Right";
        bool changingDirection = false;

        //Food Defaults
        PictureBox food = new PictureBox();
        Point foodLocation = new Point(0, 0);


        //Database varibales
        static String path = Path.GetFullPath(Environment.CurrentDirectory);
        static string dataBaseName = "data.mdf";
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=" + path + @"\" + dataBaseName + "; Integrated Security=True;";

        public Form1()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {

            //In case user want to play again when game is over

            //MessageBox.Show(this.gamePanel.Size.Width.ToString());
            //MessageBox.Show(this.gamePanel.Size.Height.ToString());

            gamePanel.Controls.Clear();
            snakeParts = null;
            scoreLabel.Text = "0";
            snakeSize = 5;
            direction = "Right";
            Location = new Point(120, 120);

            //Start game
            drawSnake();
            drawFood();

            timer1.Start();

            //Disable some controls
            trackBar1.Enabled = false;
            startButton.Enabled = false;
            nameBox.Enabled = false;

            //Enable stop button
            stopButton.Enabled = true
;
        }


        private void drawSnake()
        {
            snakeParts = new PictureBox[snakeSize];

            //Loop for drawing each snake part one after another
            for (int i = 0; i < snakeSize; i++)
            {
                snakeParts[i] = new PictureBox();
                snakeParts[i].Size = new Size(15, 15);
                snakeParts[i].BackColor = Color.Red;
                snakeParts[i].BorderStyle = BorderStyle.FixedSingle;
                snakeParts[i].Location = new Point(Location.X - (15 * i), Location.Y);
                gamePanel.Controls.Add(snakeParts[i]);
            }
        }
        private void drawFood()
        {
            Random rnd = new Random();
            int Xrand = rnd.Next(38) * 15;
            int Yrand = rnd.Next(30) * 15;

            bool isOnSnake = true;

            //check if food is on snake body
            while (isOnSnake)
            {
                for (int i = 0; i < snakeSize; i++)
                {
                    if (snakeParts[i].Location == new Point(Xrand,Yrand))
                    {
                        Xrand = rnd.Next(38) * 15;
                        Yrand = rnd.Next(30) * 15;
                    }
                    else
                    {
                        isOnSnake = false;
                    }
                }
            }

            //Now draw food
            if (isOnSnake == false)
            {
                foodLocation = new Point(Xrand, Yrand);
                food.Size = new Size(15, 15);
                food.BackColor = Color.Yellow;
                food.BorderStyle = BorderStyle.FixedSingle;
                food.Location = foodLocation;
                gamePanel.Controls.Add(food);
            }

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //change interval of timer with the speed trackbar
            timer1.Interval = 501 - (5 * trackBar1.Value);
        }

        private void timer1_Tick(object sender, EventArgs e)
        { 
            move();
        }

        private void move()
        {
            Point point = new Point(0, 0);

            //loop for moving each part of the snake according to direction
            for (int i = 0; i < snakeSize; i++)
            {
                if (i == 0)
                {
                    point = snakeParts[i].Location;
                    if (direction == "Left")
                    {
                        snakeParts[i].Location = new Point(snakeParts[i].Location.X - 15, snakeParts[i].Location.Y);
                    }
                    if (direction == "Right")
                    {
                        snakeParts[i].Location = new Point(snakeParts[i].Location.X + 15, snakeParts[i].Location.Y);
                    }
                    if (direction == "Top")
                    {
                        snakeParts[i].Location = new Point(snakeParts[i].Location.X, snakeParts[i].Location.Y - 15);
                    }
                    if (direction == "Down")
                    {
                        snakeParts[i].Location = new Point(snakeParts[i].Location.X, snakeParts[i].Location.Y + 15);
                    }
                }
                else
                {
                    Point newPoint = snakeParts[i].Location;
                    snakeParts[i].Location = point;
                    point = newPoint;
                }
            }

            // if snake hits food
            if (snakeParts[0].Location == foodLocation)
            {
                eatFood();
                drawFood();
            }

            //if snake hits any wall
            if (snakeParts[0].Location.X < 0 || snakeParts[0].Location.X >= 570 || snakeParts[0].Location.Y < 0 || snakeParts[0].Location.Y >= 450)
            {
                stopGame();
            }


            //if snake hits itself
            for (int i = 3; i < snakeSize; i++)
            {
                if (snakeParts[0].Location == snakeParts[i].Location)
                {
                    stopGame();



                }

                changingDirection = false;
            }
        }

        //now handle user input to control snake
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData ==(Keys.Up) && direction != "down" && changingDirection != true)
            {
                direction = "Top";
                changingDirection = true;
            }

            if (keyData == (Keys.Down) && direction != "Top" && changingDirection != true)
            {
                direction = "Down";
                changingDirection = true;
            }

            if (keyData == (Keys.Left) && direction != "Right" && changingDirection != true)
            {
                direction = "Left";
                changingDirection = true;
            }

            if (keyData == (Keys.Right) && direction != "Left" && changingDirection != true)
            {
                direction = "Right";
                changingDirection = true;
            }

            return base.ProcessCmdKey(ref msg, keyData);

        }

        private void eatFood()
        {
            snakeSize++;

            //save old snake and use it to create new bigger snake with incremented rectangle
            PictureBox[] oldSnake = snakeParts;
            gamePanel.Controls.Clear();
            snakeParts = new PictureBox[snakeSize];

            for (int i = 0; i < snakeSize; i++)
            {
                snakeParts[i] = new PictureBox();
                snakeParts[i].Size = new Size(15, 15);
                snakeParts[i].BorderStyle = BorderStyle.FixedSingle;


                if (i == 0)
                {
                    snakeParts[i].Location = foodLocation;
                }

                else
                {
                    snakeParts[i].Location = oldSnake[i - 1].Location;
                }
                gamePanel.Controls.Add(snakeParts[i]);

            }

            //update Score
            int currentScores = Int32.Parse(scoreLabel.Text);
            int newScore = currentScores + 10;
            scoreLabel.Text = newScore + "";

        }


        private void stopGame()
        {
            timer1.Stop();
            startButton.Enabled = true;
            trackBar1.Enabled = true;
            stopButton.Enabled = false;
            nameBox.Enabled = true;

            //Game over Label
            Label over = new Label();
            over.Text = "Game\nOver";
            over.ForeColor = Color.White;
            over.Font = new Font("Arial", 100, FontStyle.Bold);
            over.Size = over.PreferredSize;
            over.TextAlign = ContentAlignment.MiddleCenter;

            //fint Center point to place label
            int X = gamePanel.Width / 2 - over.Width / 2;
            int Y = gamePanel.Width / 2 - over.Height / 2;
            over.Location = new Point(X, Y);


            gamePanel.Controls.Add(over);
            over.BringToFront();


            //Add current Scores and Update Score Board
            addCurrentScoresToDatabase();
            UpdateScoreBoard();


        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            stopGame();
        }

        private void UpdateScoreBoard()
        {
            //Get Data from database and show in data Grid View
            string query = "SELECT Date,Name,Scores FROM scores";

            using(SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);

                var ds = new DataSet();
                adapter.Fill(ds);

                dataGridView1.DataSource = ds.Tables[0];

                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                dataGridView1.Sort(this.dataGridView1.Columns[0], ListSortDirection.Descending);
            }
        }

        private void addCurrentScoresToDatabase()
        {
            //instert score label value in database with name and DateTime
            string query = "INSERT INTO scores(Date,Name,Scores) VALUES(@Date,@Name,@Scores);";

            using (SqlConnection con = new SqlConnection(connectionString))
            using(SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = nameBox.Text;
                cmd.Parameters.Add("@Scores", SqlDbType.Int).Value = scoreLabel.Text;


                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }



    }
}