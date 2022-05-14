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
    public partial class Register : Form
    {
        private MySqlCommand req;
        public Register()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string user = textBox1.Text;
            string password = textBox2.Text;
            string email = textBox3.Text;
            string verif = String.Format("SELECT Id FROM user WHERE Name = '{0}'", user);
            string query = String.Format("INSERT INTO user (Name, Password, Email) VALUES ('{0}', '{1}', '{2}')", user, password, email);
            string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
            MySqlConnection connection = new MySqlConnection(@connString);
            req = new MySqlCommand(verif, connection);
            MySqlCommand command = new MySqlCommand(query, connection);
            try
            {
                connection.Open();

                command.ExecuteNonQuery();

                object reader = req.ExecuteScalar();
                var temp = Convert.ToInt32(reader);
                string test = String.Format("INSERT INTO bonus (Id, Bomb) VALUES ('{0}', '{1}')", temp, 1);
                command = new MySqlCommand(test, connection);

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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Connexion obj = new Connexion();
            obj.Show();
            this.Hide();
        }
    }
}
