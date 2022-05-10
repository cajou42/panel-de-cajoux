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
    public partial class Score : Form
    {
        int id;
        public Score(int _id)
        {
            InitializeComponent();
            id = _id;
            var txt = new Label();
            var sku = new Label();
            txt.Text = "Name";
            highScoreTab.Controls.Add(txt, 0, 0);
            sku.Text = "Score";
            highScoreTab.Controls.Add(sku, 1, 0);
            string query = String.Format("SELECT user.Name, score FROM highScore INNER JOIN user ON highScore.UserId = user.Id ORDER BY score DESC LIMIT 10");
            string connString = "server=localhost;port=9000;user id=root; password=example; database=game-db; SslMode=none";
            MySqlConnection connection = new MySqlConnection(@connString);
            MySqlCommand command = new MySqlCommand(query, connection);
            try
            {
                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();
                int i = 0;
                var tab = new int[10];
                var tab2 = new string[10];
                while (reader.Read())
                {
                    Console.WriteLine(String.Format("{0}", reader.GetString(0)));
                    tab2[i] = reader.GetString(0);
                    tab[i] = reader.GetInt32(1);
                    i++;
                }
                i = 0;
                List<Label> labels = new List<Label>();
                for (int j = 0; j < 10; j++)
                {
                    labels.Add(new Label());
                    labels[j].Text = String.Format("{0}", tab[i]);
                    if(tab[i] != 0)
                        highScoreTab.Controls.Add(labels[j], 1, j+1);
                    Console.WriteLine(String.Format("{0}", tab[i]) + " " + i);
                    if (i != 9)
                    {
                        i++;
                    }
                    else if (i == 9)
                    {
                        break;
                    }
                }
                i = 0;
                List<Label> labelName = new List<Label>();
                for (int j = 0; j < 10; j++)
                {
                    labelName.Add(new Label());
                    labelName[j].Text = String.Format("{0}", tab2[i]);
                    if (tab[i] != 0)
                        highScoreTab.Controls.Add(labelName[j], 0, j + 1);
                    Console.WriteLine(String.Format("{0}", tab2[i]) + " " + i);
                    if (i != 9)
                    {
                        i++;
                    }
                    else if (i == 9)
                    {
                        break;
                    }
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

        private void button1_Click(object sender, EventArgs e)
        {
            Menu obj = new Menu(id);
            obj.Show();
            this.Hide();
        }
    }
}
