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
using MySql.Data.MySqlClient;
using System.Media;
using System.Windows.Threading;
using System.Runtime.InteropServices;

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
        private Graphics insertCusor;
        private int height = 15;
        private int cache = 6;
        private int CursorX = 5;
        private int CursorY = 15;
        private readonly Mutex mut = new Mutex();
        private int score;
        private int id;
        private string music;
        SoundPlayer NOST;
        private Bitmap alu;
        private MySqlCommand query2;
        private MySqlCommand query3;
        private MySqlCommand query4;
        private MySqlCommand query5;
        private string control;
        private string bonus;
        private int cooldown;
        private Task task;
        private int baseSpeed = 2000;
        private bool gameOn = true;
        private bool playing = true;
        private int background;

        public Game(int _id, string _music, int _background)
        {
            InitializeComponent();
            //int timerControl = 100;
            timer1.Interval = 1000;
            timer1.Start();

            id = _id;
            music = _music;
            background = _background;

            switch (background)
            {
                case 1:
                    this.BackgroundImage = Properties.Resources.bacground;
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                    break;
                case 2:
                    this.BackgroundImage = Properties.Resources.background2;
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                    break;
                case 3:
                    this.BackgroundImage = Properties.Resources.background3;
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                    break;
            }

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
            task = Task.Run(() => generateBlock());
            displayCursor();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            tick++;
            timerLabel.Text = tick.ToString() + " sec";
            if(cooldown > 0)
            {
                cooldown--;
                labelBonus.Text = "bonus ready in " + cooldown;
            }
            else if (cooldown == 0)
            {
                labelBonus.Text = "bonus : ok";
            }
        }

        public static Bitmap resizeImage(Bitmap imgToResize, Size size)
        {
            return (new Bitmap(imgToResize, size));
        }

        private Task generateBlock()
        {
            while (gameOn)
            {
                mut.WaitOne();
                try
                {
                    if (!playing)
                    {
                        playing = true;
                        NOST.Play();
                        NOST.PlayLooping();
                    }
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
                            mut.ReleaseMutex();
                            break;
                        }

                    }
                    height--;
                    eraseBlock();
                    gravity();
                    gameGrid.Image = baseImage;
                }
                catch(Exception e)
                {
                }
                finally
                {
                    mut.ReleaseMutex();
                }
                Thread.Sleep(baseSpeed);
            }
            return Task.FromResult<object>(null);
        }

        private void GameOver()
        {
            if(height == 0)
            {
                timer1.Stop();
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
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        this.Hide();
                    }));

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
            if ((e.KeyCode == Keys.Q && control == "A") || (e.KeyCode == Keys.Left && control == "B"))
            {
                horizontal--;
            }
            else if ((e.KeyCode == Keys.D && control == "A") || (e.KeyCode == Keys.Right && control == "B"))
            {
                horizontal++;
            }
            else if ((e.KeyCode == Keys.S && control == "A") || (e.KeyCode == Keys.Down && control == "B"))
            {
                vertical++;
            }
            else if ((e.KeyCode == Keys.Z && control == "A") || (e.KeyCode == Keys.Up && control == "B"))
            {
                vertical--;
            }
            else if (e.KeyCode == Keys.Space)
            {
                exchange();
            }
            else if(e.KeyCode == Keys.E)
            {
                Console.WriteLine(bonus);
                executeBonus(bonus);
            }
            else if(e.KeyCode == Keys.P)
            {
                pause();
            }
            else
            {
                return;
            }
            moveCursor(horizontal,vertical);
        }

        private void moveCursor(int X, int Y)
        {
            mut.WaitOne();
            try
            {
                eraseCursor();
                CursorX += X;
                CursorY += Y;
                if (CursorX < 0)
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
                displayCursor();
                gameGrid.Update();
            }
            finally
            {
                mut.ReleaseMutex();
            }

        }

        public void displayCursor()
        {

            alu = new Bitmap(baseImage);
            insertCusor = Graphics.FromImage(alu);
            var SL = cursorSpawnL();
            Bitmap simage = new Bitmap(SL);
            simage = resizeImage(simage, new Size(25, 25));
            TextureBrush sBrush = new TextureBrush(simage);
            insertCusor.FillRectangle(sBrush, CursorX * 25, CursorY * 25, 25, 25);

            var SR = cursorSpawnR();
            Bitmap vimage = new Bitmap(SR);
            vimage = resizeImage(vimage, new Size(25, 25));
            TextureBrush vBrush = new TextureBrush(vimage);
            insertCusor.FillRectangle(vBrush, (CursorX + 1) * 25, CursorY * 25, 25, 25);
        }

        public void eraseCursor()
        {
            alu = new Bitmap(baseImage);
            insertCusor = Graphics.FromImage(alu);
            var SL = cursorDespawn(CursorX,CursorY);
            Bitmap simage = new Bitmap(SL);
            simage = resizeImage(simage, new Size(25, 25));
            TextureBrush sBrush = new TextureBrush(simage);
            insertCusor.FillRectangle(sBrush, CursorX * 25, CursorY * 25, 25, 25);

            var SR = cursorDespawn(CursorX + 1, CursorY);
            Bitmap vimage = new Bitmap(SR);
            vimage = resizeImage(vimage, new Size(25, 25));
            TextureBrush vBrush = new TextureBrush(vimage);
            insertCusor.FillRectangle(vBrush, (CursorX + 1) * 25, CursorY * 25, 25, 25);
        }

        private void exchange()
        {
            mut.WaitOne();
            try
            {
                alu = new Bitmap(baseImage);
                insertCusor = Graphics.FromImage(alu);
                int L = blockValue(cursorDespawn(CursorX, CursorY), area[CursorX, CursorY]);
                int R = blockValue(cursorDespawn(CursorX + 1, CursorY), area[CursorX + 1, CursorY]);

                area[CursorX, CursorY] = R;
                area[CursorX + 1, CursorY] = L;

                var SL = cursorSpawnL();
                Bitmap simage = new Bitmap(SL);
                simage = resizeImage(simage, new Size(25, 25));
                TextureBrush sBrush = new TextureBrush(simage);
                insertCusor.FillRectangle(sBrush, CursorX * 25, CursorY * 25, 25, 25);

                var SR = cursorSpawnR();
                Bitmap vimage = new Bitmap(SR);
                vimage = resizeImage(vimage, new Size(25, 25));
                TextureBrush vBrush = new TextureBrush(vimage);
                insertCusor.FillRectangle(vBrush, (CursorX + 1) * 25, CursorY * 25, 25, 25);
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
                            baseSpeed -= 10;
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                scoreLabel.Text = "score : " + score;
                            }));
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
                            baseSpeed -= 10;
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                scoreLabel.Text = "score : " + score;
                            }));
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

        private void pause()
        {
            NOST.Stop();
            timer1.Stop();
            gameOn = false;
            var result = MessageBox.Show("Want to resume ?", "Pause", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                NOST.Play();
                timer1.Start();
                gameOn = true;
                task = Task.Run(() => generateBlock());
            }
            else
            {
                string query = String.Format("INSERT INTO highScore (UserId, score, has_quit) VALUES ('{0}', '{1}', '{2}')", id, score, 1);
                string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
                MySqlConnection connection = new MySqlConnection(@connString);
                MySqlCommand command = new MySqlCommand(query, connection);
                try
                {
                    connection.Open();

                    command.ExecuteNonQuery();

                    Console.WriteLine("ok");

                    Menu obj = new Menu(id,background,music);
                    obj.Show();
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        this.Hide();
                    }));

                }
                catch (MySqlException f)
                {
                    Console.WriteLine(f.Message + connString);
                }
                finally
                {
                    connection.Close();
                }
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

        private void executeBonus(string bonus)
        {
            mut.WaitOne();
            try
            {
                Bitmap image = new Bitmap(@"..\..\images\case_vide.png");
                image = resizeImage(image, new Size(25, 25));
                TextureBrush tBrush = new TextureBrush(image);
                if(cooldown == 0)
                {
                    switch (bonus)
                    {
                        case "bomb":
                            Console.WriteLine("Bomb!");
                            insert.FillRectangle(tBrush, CursorX * 25, CursorY * 25, 25, 25);
                            insert.FillRectangle(tBrush, CursorX + 1 * 25, CursorY * 25, 25, 25);
                            insert.FillRectangle(tBrush, CursorX - 1 * 25, CursorY * 25, 25, 25);
                            insert.FillRectangle(tBrush, CursorX * 25, CursorY + 1 * 25, 25, 25);
                            insert.FillRectangle(tBrush, CursorX * 25, CursorY - 1 * 25, 25, 25);
                            area[CursorX, CursorY] = 0;
                            area[CursorX + 1, CursorY] = 0;
                            area[CursorX - 1, CursorY] = 0;
                            area[CursorX, CursorY + 1] = 0;
                            area[CursorX, CursorY - 1] = 0;
                            score += 2;
                            scoreLabel.Text = "score : " + score;
                            SoundPlayer boum = new SoundPlayer(@"..\..\music\explosion.wav");
                            NOST.Stop();
                            boum.Play();
                            playing = false;
                            cooldown = 60;
                            return;
                        case "Thunder":
                            Console.WriteLine("Thunder!");
                            var value = area[CursorX, CursorY];
                            for (int h = 0; h < 16; h++)
                            {
                                for (int i = 0; i < 12; i++)
                                {
                                    if (area[i, h] == value)
                                    {
                                        insert.FillRectangle(tBrush, i * 25, h * 25, 25, 25);
                                        area[i, h] = 0;
                                    }
                                }
                            }
                            NOST.Stop();
                            SoundPlayer zap = new SoundPlayer(@"..\..\music\thunder.wav");
                            zap.Play();
                            playing = false;
                            cooldown = 60;
                            return;
                        case "TimeStop":
                            Console.WriteLine("Stop!");
                            var pause = new Stopwatch();
                            pause.Start();
                            while (pause.ElapsedMilliseconds != 10000)
                            {
                                gameOn = false;
                                continue;
                            }
                            pause.Stop();
                            gameOn = true;
                            task = Task.Run(() => generateBlock());
                            cooldown = 30;
                            Console.WriteLine("finished!");
                            return;
                        case "circularBlade":
                            Console.WriteLine("circular Blade!");
                            for (int i = 0; i < 12; i++)
                            {
                                insert.FillRectangle(tBrush, i * 25, 15 * 25, 25, 25);
                                insert.FillRectangle(tBrush, i * 25, 14 * 25, 25, 25);
                                area[i, 15] = 0;
                                area[i, 14] = 0;
                            }
                            score += 3;
                            scoreLabel.Text = "score : " + score;
                            SoundPlayer zing = new SoundPlayer(@"..\..\music\circular_blade.wav");
                            NOST.Stop();
                            zing.Play();
                            playing = false;
                            cooldown = 70;
                            return;
                    }
                }

            }
            finally
            {
                mut.ReleaseMutex();
            }
        }

        private void Game_Load(object sender, EventArgs e)
        {
            string bonus1 = String.Format("SELECT Bomb FROM bonus WHERE Id = '{0}'", id);
            string bonus2 = String.Format("SELECT Thunder FROM bonus WHERE Id = '{0}'", id);
            string bonus3 = String.Format("SELECT TimeStop FROM bonus WHERE Id = '{0}'", id);
            string bonus4 = String.Format("SELECT circularBlade FROM bonus WHERE Id = '{0}'", id);
            string queryControl = String.Format("SELECT ControlType FROM user WHERE Id = '{0}'", id);
            string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
            MySqlConnection connection = new MySqlConnection(@connString);
            query2 = new MySqlCommand(bonus2, connection);
            query3 = new MySqlCommand(bonus3, connection);
            query4 = new MySqlCommand(bonus4, connection);
            query5 = new MySqlCommand(queryControl, connection);
            MySqlCommand command = new MySqlCommand(bonus1, connection);
            try
            {
                connection.Open();

                object reader2 = query2.ExecuteScalar();
                object reader3 = query3.ExecuteScalar();
                object reader4 = query4.ExecuteScalar();
                control = Convert.ToString(query5.ExecuteScalar());

                bonus = "bomb";
                pictureBox1.Image = resizeImage(new Bitmap(@"..\..\images\pixel_bomb.png"), new Size(50, 50));

                if (Convert.ToInt32(reader2) == 1)
                {
                    command = new MySqlCommand(bonus2, connection);
                    bonus = "Thunder";
                    pictureBox1.Image = resizeImage(new Bitmap(@"..\..\images\pixel_thunder.png"), new Size(50, 50));

                }
                else if (Convert.ToInt32(reader3) == 1)
                {
                    command = new MySqlCommand(bonus3, connection);
                    bonus = "TimeStop";
                    pictureBox1.Image = resizeImage(new Bitmap(@"..\..\images\pixel_hourglass.png"), new Size(50, 50));

                }
                else if (Convert.ToInt32(reader4) == 1)
                {
                    command = new MySqlCommand(bonus4, connection);
                    bonus = "circularBlade";
                    pictureBox1.Image = resizeImage(new Bitmap(@"..\..\images\pixel_circular_blade.png"), new Size(50, 50));

                }
                Console.WriteLine("ok");

            }
            catch (MySqlException f)
            {
                Console.WriteLine(f.Message + connString);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
