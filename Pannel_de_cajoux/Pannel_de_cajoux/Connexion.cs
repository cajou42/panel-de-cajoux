using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Pannel_de_cajoux
{
    public partial class Connexion : Form
    {
        private MySqlCommand query;
        private MySqlCommand query2;
        public Connexion()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string user = textBox1.Text;
            string password = textBox2.Text;
            string test = String.Format("SELECT COUNT(*) FROM `user` Where Name = '{0}' and Password = '{1}'", user, password);
            string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
            MySqlConnection connection = new MySqlConnection(@connString);
            query = new MySqlCommand(test, connection);
            string test2 = String.Format("SELECT Id FROM `user` WHERE Name = '{0}'", user);
            query2 = new MySqlCommand(test2, connection);
            MySqlCommand command = new MySqlCommand(test, connection);
            try
            {
                connection.Open();

                object pass = query.ExecuteScalar();
                object ids = query2.ExecuteScalar();
                int id = Convert.ToInt32(ids);

                //command.ExecuteNonQuery();

                Console.WriteLine(command.ExecuteNonQuery());
                Console.WriteLine("ok");

                if (Convert.ToInt32(pass) > 0)
                {
                    Menu obj = new Menu(id);
                    obj.Show();
                    this.Hide();
                }

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
