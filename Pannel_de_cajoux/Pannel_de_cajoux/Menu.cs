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
        public int background;
        public Menu(int ids, int _background = 1, string _music = "Tetris_theme")
        {
            InitializeComponent();
            id = ids;
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
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            Game obj = new Game(id,music, background);
            obj.Show();
            this.Hide();
        }

        private void optionButton_Click(object sender, EventArgs e)
        {
            Options obj = new Options(id, music, background);
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
