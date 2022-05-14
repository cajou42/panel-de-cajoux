using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pannel_de_cajoux
{
    public partial class Menu : Form
    {
        public int id;
        public string music;
        public string[] tab = new string[] { "Z", "S", "Q", "D", "SPACE", "P", "E" };
        public Menu(int ids, string _music = "Tetris_theme")
        {
            InitializeComponent();
            id = ids;
            music = _music;
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            Game obj = new Game(id,music);
            obj.Show();
            this.Hide();
        }

        private void optionButton_Click(object sender, EventArgs e)
        {
            Options obj = new Options(id, music);
            obj.Show();
            this.Hide();
        }

        private void scoreButton_Click(object sender, EventArgs e)
        {
            Score obj = new Score(id);
            obj.Show();
            this.Hide();
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connexion obj = new Connexion();
            obj.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Register obj = new Register();
            obj.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            profileParameter  obj = new profileParameter(id);
            obj.Show();
            this.Hide();
        }

        private void ChooseBonus_Click(object sender, EventArgs e)
        {
            ChooseBonus obj = new ChooseBonus(id);
            obj.Show();
            this.Hide();
        }
    }
}
