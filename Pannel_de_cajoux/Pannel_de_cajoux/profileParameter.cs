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
    public partial class profileParameter : Form
    {
        private MySqlCommand query;
        private int id;
        public profileParameter(int ids)
        {
            InitializeComponent();
            id = ids;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string user = textBox1.Text;
            string test = String.Format("UPDATE `user` SET Name = '{0}' WHERE Id = '{1}'", user, id);
            string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
            MySqlConnection connection = new MySqlConnection(@connString);
            query = new MySqlCommand(test, connection);
            MySqlCommand command = new MySqlCommand(test, connection);
            try
            {
                connection.Open();

                command.ExecuteNonQuery();
                Console.WriteLine(id);
                infoLabel.Text = "User name was change";

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
            string password = textBox2.Text;
            string test = String.Format("UPDATE `user` SET Password = '{0}' WHERE Id = '{1}'", password, id);
            string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
            MySqlConnection connection = new MySqlConnection(@connString);
            query = new MySqlCommand(test, connection);
            MySqlCommand command = new MySqlCommand(test, connection);
            try
            {
                connection.Open();

                command.ExecuteNonQuery();
                infoLabel.Text = "Password was change";

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
            string email = textBox3.Text;
            string test = String.Format("UPDATE `user` SET Email = '{0}' WHERE Id = '{1}'", email, id);
            string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
            MySqlConnection connection = new MySqlConnection(@connString);
            query = new MySqlCommand(test, connection);
            MySqlCommand command = new MySqlCommand(test, connection);
            try
            {
                connection.Open();

                command.ExecuteNonQuery();
                infoLabel.Text = "Email was change";

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
            string test = String.Format("DELETE FROM `user` WHERE Id = '{0}'", id) ;
            string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
            MySqlConnection connection = new MySqlConnection(@connString);
            query = new MySqlCommand(test, connection);
            MySqlCommand command = new MySqlCommand(test, connection);
            try
            {
                connection.Open();

                command.ExecuteNonQuery();

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
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Menu obj = new Menu(id);
            obj.Show();
            this.Hide();
        }
    }
}
