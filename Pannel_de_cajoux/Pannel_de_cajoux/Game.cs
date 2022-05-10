using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Media;

namespace Pannel_de_cajoux
{
    public partial class Game : Form
    {
        private Graphics graph;
        private Pen blackPen = new Pen(Color.Black, 1);
        private int tick = 0;
        private int[,] area;
        private Graphics bases;
        private Bitmap baseImage;
        private Graphics insert;
        private int height = 15;
        private int cache = 6;
        private int CursorX = 5;
        private int CursorY = 15;
        private readonly Mutex mut = new Mutex();
        private int score;
        private int id;
        private string music;
        SoundPlayer NOST;
        public Game(int _id, string _music)
        {
            InitializeComponent();
            timer1.Interval = 1000;
            timer2.Interval = 2000;
            timer3.Interval = 100;
            timer1.Start();
            timer2.Start();
            timer3.Start();
            id = _id;
            music = _music;
            NOST = new SoundPlayer(@"..\..\music\" + music + ".wav");
            NOST.Play();
            NOST.PlayLooping();

            timerLabel.Text = "0 sec";
            gameGrid.Width = 300;
            gameGrid.Height = 400;
            area = new int[12, 16];
            baseImage = new Bitmap(gameGrid.Width, gameGrid.Height);
            bases = Graphics.FromImage(baseImage);
            bases.FillRectangle(Brushes.LightGray, 0, 0, baseImage.Width, baseImage.Height);
            gameGrid.Image = baseImage;
            begin();
            generateBlock();
            //displayCursor();

            Task t = Task.Run(() => { while (true) { displayCursor(); } });
        }

        private void gameGrid_Paint(object sender, PaintEventArgs e)
        {
            graph = e.Graphics;
            for (int i = 0; i < 21; i++)
            {
                graph.DrawLine(blackPen, 0, i * 25, 64 * 25, i * 25);
            }

            for (int x = 0; x < 16; ++x)
            {
                graph.DrawLine(blackPen, x * 25, 0, x * 25, 64 * 25);
            }
        }

        private void begin()
        {
            mut.WaitOne();
            try
            {
                insert = Graphics.FromImage(baseImage);
                for (int j = 0; j < 16; j++)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        Bitmap image = new Bitmap(@"..\..\images\case_vide.png");
                        image = resizeImage(image, new Size(25, 25));
                        TextureBrush tBrush = new TextureBrush(image);
                        insert.FillRectangle(tBrush, i * 25, j * 25, 25, 25);
                        area[i, j] = 0;
              
                    }
                }
            }
            finally
            {
                mut.ReleaseMutex();
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            Task t = Task.Run(() => { while (true) { displayCursor(); } });
            gravity();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            generateBlock();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tick++;
            timerLabel.Text = tick.ToString() + " sec";
        }

        public static Bitmap resizeImage(Bitmap imgToResize, Size size)
        {
            return (new Bitmap(imgToResize, size));
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private void generateBlock()
        {
            mut.WaitOne();
            try 
            {
                insert = Graphics.FromImage(baseImage);
                for (int i = 0; i < 12; i++)
                {
                    string color = ramdomBlock();
                    Bitmap image = new Bitmap(color);
                    image = resizeImage(image, new Size(25, 25));
                    TextureBrush tBrush = new TextureBrush(image);
                    insert.FillRectangle(tBrush, i * 25, height * 25, 25, 25);
                    if (height != 0)
                    {
                        area[i, height] = blockValue(color, i);
                    }
                    else
                    {
                        GameOver();
                        return;
                    }

                }
                height--;
                //mount(height);
                eraseBlock();
                gameGrid.Image = baseImage;
            }
            finally
            {
                mut.ReleaseMutex();
            }
           
        }

        //private void mount(int row)
        //{
        //    var temp = new int[12,16];
        //    mut.WaitOne();
        //    try
        //    {
        //        for (int h = 15; h > 0; h--)
        //        {
        //            for (int i = 0; i < 12; i++)
        //            {
        //                if(h == 15)
        //                {
        //                    if(temp[i, h] != 0)
        //                    {
        //                        temp[i, h] = area[i, h];
        //                    }
        //                    area[i, h] = 0;
        //                }
        //                else
        //                {
        //                    temp[i, h] = area[i, h];
        //                    area[i, h] = temp[i, h - 1];
        //                }

        //            }
        //        }
        //        for (int h = 15; h > 0; h--)
        //        {
        //            for (int i = 0; i < 12; i++)
        //            {
        //                Bitmap image = new Bitmap(cursorDespawn(i, h));
        //                image = resizeImage(image, new Size(25, 25));
        //                TextureBrush tBrush = new TextureBrush(image);
        //                insert.FillRectangle(tBrush, i * 25, row - h * 25, 25, 25);
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        mut.ReleaseMutex();
        //    }
        //}

        private void GameOver()
        {
            if(height == 0)
            {
                timer1.Stop();
                timer2.Stop();
                NOST.Stop();
                string query = String.Format("INSERT INTO highScore (UserId, score) VALUES ('{0}', '{1}')", id, score);
                string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
                MySqlConnection connection = new MySqlConnection(@connString);
                MySqlCommand command = new MySqlCommand(query, connection);
                try
                {
                    connection.Open();

                    command.ExecuteNonQuery();

                    Console.WriteLine("ok");

                    Connexion obj = new Connexion();
                    obj.Show();
                    this.Hide();

                }
                catch (MySqlException f)
                {
                    Console.WriteLine(f.Message + connString);
                }
                finally
                {
                    connection.Close();
                }
                MessageBox.Show($"Game Over");
                Application.Exit();
                Application.Restart();
            }
        }

        private string ramdomBlock()
        {
            string[] tab = new string[5] { @"..\..\images\case_rouge.png", @"..\..\images\case_bleu.png", @"..\..\images\case_verte.png", @"..\..\images\case_violette.png", @"..\..\images\case_jaune_fruit.png" };
            var rand = new Random();
            int intermediare = rand.Next(tab.Length);
            while(cache == intermediare)
            {
                intermediare = rand.Next(tab.Length);
            }
            string image = tab[intermediare];
            cache = intermediare;
            return image;
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            int vertical = 0;
            int horizontal = 0;
            if (e.KeyCode == Keys.Q)
            {
                horizontal--;
            }
            else if (e.KeyCode == Keys.D)
            {
                horizontal++;
            }
            else if (e.KeyCode == Keys.S)
            {
                vertical++;
            }
            else if (e.KeyCode == Keys.Z)
            {
                vertical--;
            }
            else if (e.KeyCode == Keys.Space)
            {
                exchange();
            }
            else
            {
                return;
            }
            moveCursor(horizontal,vertical);
        }

        private void moveCursor(int X, int Y)
        {
            eraseCursor();
            CursorX += X;
            CursorY += Y;
            if(CursorX < 0)
            {
                CursorX = 0;
            }
            if (CursorY < 0)
            {
                CursorY = 0;
            }
            if (CursorX > 12)
            {
                CursorX = 12;
            }
            if (CursorY < height)
            {
                CursorY = height;
            }
        }

        public void displayCursor()
        {
            mut.WaitOne();
            try
            {
                var SL = cursorSpawnL();
                Bitmap simage = new Bitmap(SL);
                simage = resizeImage(simage, new Size(25, 25));
                TextureBrush sBrush = new TextureBrush(simage);
                insert.FillRectangle(sBrush, CursorX * 25, CursorY * 25, 25, 25);

                var SR = cursorSpawnR();
                Bitmap vimage = new Bitmap(SR);
                vimage = resizeImage(vimage, new Size(25, 25));
                TextureBrush vBrush = new TextureBrush(vimage);
                insert.FillRectangle(vBrush, (CursorX + 1) * 25, CursorY * 25, 25, 25);
            }
            finally
            {
                mut.ReleaseMutex();
            }
        }

        public void eraseCursor()
        {
            mut.WaitOne();
            try
            {
                var SL = cursorDespawn(CursorX,CursorY);
                Bitmap simage = new Bitmap(SL);
                simage = resizeImage(simage, new Size(25, 25));
                TextureBrush sBrush = new TextureBrush(simage);
                insert.FillRectangle(sBrush, CursorX * 25, CursorY * 25, 25, 25);

                var SR = cursorDespawn(CursorX + 1, CursorY);
                Bitmap vimage = new Bitmap(SR);
                vimage = resizeImage(vimage, new Size(25, 25));
                TextureBrush vBrush = new TextureBrush(vimage);
                insert.FillRectangle(vBrush, (CursorX + 1) * 25, CursorY * 25, 25, 25);
            }
            finally
            {
                mut.ReleaseMutex();
            }
        }

        private void exchange()
        {
            mut.WaitOne();
            try
            {
                int L = blockValue(cursorDespawn(CursorX, CursorY), area[CursorX, CursorY]);
                int R = blockValue(cursorDespawn(CursorX + 1, CursorY), area[CursorX + 1, CursorY]);

                area[CursorX, CursorY] = R;
                area[CursorX + 1, CursorY] = L;

                var SL = cursorSpawnL();
                Bitmap simage = new Bitmap(SL);
                simage = resizeImage(simage, new Size(25, 25));
                TextureBrush sBrush = new TextureBrush(simage);
                insert.FillRectangle(sBrush, CursorX * 25, CursorY * 25, 25, 25);

                var SR = cursorSpawnR();
                Bitmap vimage = new Bitmap(SR);
                vimage = resizeImage(vimage, new Size(25, 25));
                TextureBrush vBrush = new TextureBrush(vimage);
                insert.FillRectangle(vBrush, (CursorX + 1) * 25, CursorY * 25, 25, 25);
            }
            finally
            {
                mut.ReleaseMutex();
            }
        }

        public void eraseBlock()
        {
            mut.WaitOne();
            try
            {
                Bitmap image = new Bitmap(@"..\..\images\case_vide.png");
                image = resizeImage(image, new Size(25, 25));
                TextureBrush tBrush = new TextureBrush(image);
                for (int h = 0; h < 16; h++)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        int j = i + 1;
                        int u = i + 2;
                        int f = h + 1;
                        int y = h + 2;
                        if (j < 12 && u < 12 && area[i, h] != 0 && area[i, h] == area[j, h] && area[i, h] == area[u,h])
                        {
                            insert.FillRectangle(tBrush, i * 25, h * 25, 25, 25);
                            insert.FillRectangle(tBrush, j * 25, h * 25, 25, 25);
                            insert.FillRectangle(tBrush, u * 25, h * 25, 25, 25);
                            area[i, h] = 0;
                            area[j, h] = 0;
                            area[u, h] = 0;
                            score++;
                            scoreLabel.Text = "score : " + score;
                            timer2.Interval += 10;
                        }
                        if (f < 16 && y < 16 && area[i, h] != 0 && area[i, h] == area[i, f] && area[i, h] == area[i, y])
                        {
                            insert.FillRectangle(tBrush, i * 25, h * 25, 25, 25);
                            insert.FillRectangle(tBrush, i * 25, f * 25, 25, 25);
                            insert.FillRectangle(tBrush, i * 25, y * 25, 25, 25);
                            area[i, h] = 0;
                            area[i, f] = 0;
                            area[i, y] = 0;
                            score++;
                            scoreLabel.Text = "score : " + score;
                            timer2.Interval += 10;
                        }
                    }
                }
            }
            finally
            {
                mut.ReleaseMutex();
            }
        }

        public void gravity()
        {
            mut.WaitOne();
            try
            {
                for (int h = 0; h < 16; h++)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        if(area[i,h] == 0)
                        {
                            for (int k = h; k > 0; k--)
                            {
                                area[i, k] = area[i, k - 1];
                            }
                        }
                    }
                }
                for (int h = 0; h < 16; h++)
                {
                    for (int i = 0; i < 12; i++)
                    {   if(i == CursorX && h == CursorY)
                        {
                            Bitmap imageL = new Bitmap(cursorSpawnL());
                            imageL = resizeImage(imageL, new Size(25, 25));
                            TextureBrush tBrushL = new TextureBrush(imageL);
                            insert.FillRectangle(tBrushL, i * 25, h * 25, 25, 25);
                        }
                        else if (i == CursorX + 1 && h == CursorY)
                        {
                            Bitmap imageR = new Bitmap(cursorSpawnR());
                            imageR = resizeImage(imageR, new Size(25, 25));
                            TextureBrush tBrushR = new TextureBrush(imageR);
                            insert.FillRectangle(tBrushR, i * 25, h * 25, 25, 25);
                        }
                        else
                        {
                            Bitmap image = new Bitmap(cursorDespawn(i, h));
                            image = resizeImage(image, new Size(25, 25));
                            TextureBrush tBrush = new TextureBrush(image);
                            insert.FillRectangle(tBrush, i * 25, h * 25, 25, 25);
                        }
                    }
                }
            }
            finally
            {
                mut.ReleaseMutex();
            }
        }

        private string cursorSpawnL()
        {
            string s = "";
            switch (area[CursorX,CursorY])
            {
                case 0:
                    s = @"..\..\images\case_vide_CL.png";
                    break;
                case 1:
                    s = @"..\..\images\case_rouge_CL.png";
                    break;
                case 2:
                    s = @"..\..\images\case_bleu_CL.png";
                    break;
                case 3:
                    s = @"..\..\images\case_verte_CL.png";
                    break;
                case 4:
                    s = @"..\..\images\case_violette_CL.png";
                    break;
                case 5:
                    s = @"..\..\images\case_jaune_CL.png";
                    break;

            }
            return s;
        }

        private string cursorSpawnR()
        {
            string s = "";
            switch (area[CursorX+1, CursorY])
            {
                case 0:
                    s = @"..\..\images\case_vide_CR.png";
                    break;
                case 1:
                    s = @"..\..\images\case_rouge_CR.png";
                    break;
                case 2:
                    s = @"..\..\images\case_bleu_CR.png";
                    break;
                case 3:
                    s = @"..\..\images\case_verte_CR.png";
                    break;
                case 4:
                    s = @"..\..\images\case_violette_CR.png";
                    break;
                case 5:
                    s = @"..\..\images\case_jaune_CR.png";
                    break;

            }
            return s;
        }

        public string cursorDespawn(int i, int j)
        {
            string s = "";
            switch (area[i, j])
            {
                case 0:
                    s = @"..\..\images\case_vide.png";
                    break;
                case 1:
                    s = @"..\..\images\case_rouge.png";
                    break;
                case 2:
                    s = @"..\..\images\case_bleu.png";
                    break;
                case 3:
                    s = @"..\..\images\case_verte.png";
                    break;
                case 4:
                    s = @"..\..\images\case_violette.png";
                    break;
                case 5:
                    s = @"..\..\images\case_jaune_fruit.png";
                    break;
            }
            return s;
        }

        private int blockValue(string s, int i)
        {
            switch (s)
            {
                case @"..\..\images\case_vide.png":
                    return 0;
                case @"..\..\images\case_rouge.png":
                    return 1;
                case @"..\..\images\case_bleu.png":
                    return 2;
                case @"..\..\images\case_verte.png":
                    return 3;
                case @"..\..\images\case_violette.png":
                    return 4;
                case @"..\..\images\case_jaune_fruit.png":
                    return 5;
            }
            return (0);
        }
    }
}
