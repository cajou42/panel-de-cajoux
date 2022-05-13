using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Pannel_de_cajoux
{
    public partial class ChooseBonus : Form
    {
        int id;
        private MySqlCommand query;
        public ChooseBonus(int _id)
        {
            InitializeComponent();
            id = _id;
            pictureBox1.Image = resizeImage(new Bitmap(@"..\..\images\pixel_bomb.png"), new Size(50, 50));
            pictureBox2.Image = resizeImage(new Bitmap(@"..\..\images\pixel_thunder.png"), new Size(50, 50));
            pictureBox3.Image = resizeImage(new Bitmap(@"..\..\images\pixel_hourglass.png"), new Size(50, 50));
            pictureBox4.Image = resizeImage(new Bitmap(@"..\..\images\pixel_circular_blade.png"), new Size(50, 50));

            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox4.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        public static Bitmap resizeImage(Bitmap imgToResize, Size size)
        {
            return (new Bitmap(imgToResize, size));
        }

        private void button1_Click(object sender, EventArgs e)
        {   
            string verif = String.Format("SELECT * FROM bonus WHERE Id = '{0}'", id);
            string test = String.Format("INSERT INTO bonus (Id, Bomb) VALUES ('{0}', '{1}')", id, 1);
            string test2 = String.Format("UPDATE bonus SET Bomb = 1, Thunder = 0, TimeStop = 0, circularBlade = 0 WHERE Id = '{0}'", id);
            string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
            MySqlConnection connection = new MySqlConnection(@connString);
            query = new MySqlCommand(verif, connection);
            MySqlCommand command = new MySqlCommand(test, connection);
            try
            {
                connection.Open();

                object reader = query.ExecuteScalar();

                if (Convert.ToInt32(reader) == id)
                {
                    command = new MySqlCommand(test2, connection);
                }

                command.ExecuteNonQuery();

                Console.WriteLine("ok");

                Menu obj = new Menu(id);
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string verif = String.Format("SELECT * FROM bonus WHERE Id = '{0}'", id);
            string test = String.Format("INSERT INTO bonus (Id, Thunder) VALUES ('{0}', '{1}')", id, 1);
            string test2 = String.Format("UPDATE bonus SET Bomb = 0, Thunder = 1, TimeStop = 0, circularBlade = 0 WHERE Id = '{0}'", id);
            string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
            MySqlConnection connection = new MySqlConnection(@connString);
            query = new MySqlCommand(verif, connection);
            MySqlCommand command = new MySqlCommand(test, connection);
            try
            {
                connection.Open();

                object reader = query.ExecuteScalar();

                if (Convert.ToInt32(reader) == id)
                {
                    command = new MySqlCommand(test2, connection);
                }

                command.ExecuteNonQuery();

                Console.WriteLine("ok");

                Menu obj = new Menu(id);
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
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string verif = String.Format("SELECT * FROM bonus WHERE Id = '{0}'", id);
            string test = String.Format("INSERT INTO bonus (Id, TimeStop) VALUES ('{0}', '{1}')", id, 1);
            string test2 = String.Format("UPDATE bonus SET Bomb = 0, Thunder = 0, TimeStop = 1, circularBlade = 0 WHERE Id = '{0}'", id);
            string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
            MySqlConnection connection = new MySqlConnection(@connString);
            query = new MySqlCommand(verif, connection);
            MySqlCommand command = new MySqlCommand(test, connection);
            try
            {
                connection.Open();

                object reader = query.ExecuteScalar();

                if (Convert.ToInt32(reader) == id)
                {
                    command = new MySqlCommand(test2, connection);
                }

                command.ExecuteNonQuery();

                Console.WriteLine("ok");

                Menu obj = new Menu(id);
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
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string verif = String.Format("SELECT * FROM bonus WHERE Id = '{0}'", id);
            string test = String.Format("INSERT INTO bonus (Id, circularBlade) VALUES ('{0}', '{1}')", id, 1);
            string test2 = String.Format("UPDATE bonus SET Bomb = 0, Thunder = 0, TimeStop = 0, circularBlade = 1 WHERE Id = '{0}'", id);
            string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
            MySqlConnection connection = new MySqlConnection(@connString);
            query = new MySqlCommand(verif, connection);
            MySqlCommand command = new MySqlCommand(test, connection);
            try
            {
                connection.Open();

                object reader = query.ExecuteScalar();

                if (Convert.ToInt32(reader) == id)
                {
                    command = new MySqlCommand(test2, connection);
                }

                command.ExecuteNonQuery();

                Console.WriteLine("ok");

                Menu obj = new Menu(id);
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
        }
    }
}
