using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pannel_de_cajoux
{
    public partial class Options : Form
    {
        private string[] tab = new string[] { "Tetris_theme", "Oceanic Breeze", "magical-girl", "hades-house-of-hades", "scattered-and-lost" };
        public int click;
        public string music = "Tetris_theme";
        public int id;
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
        public Options(int _id, string _music)
        {
            InitializeComponent();
            info.Text = tab[0];
            label2.Text = "composer : Hirokazu Tanaka \n from : Tetris gameBoy";
            uint CurrVol = 0;
            waveOutGetVolume(IntPtr.Zero, out CurrVol);
            ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
            trackBar1.Value = CalcVol / (ushort.MaxValue / 10);
            music = _music;
            id = _id;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int NewVolume = ((ushort.MaxValue / 10) * trackBar1.Value);
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
        }

        private void changeMusic_Click(object sender, EventArgs e)
        {
            click++;
            if (click == tab.Length)
            {
                click = 0;
            }
            info.Text = tab[click];
            music = tab[click];
            switch (click)
            {
                case 0:
                    label2.Text = "composer : Hirokazu Tanaka \n from : Tetris gameBoy";
                    break;
                case 1:
                    label2.Text = "composer : flashygoodness \n cover : PrototypeRaptor - OC ReMix \n remix from : fraymaker \n originaly from : Rivals of Aether";
                    break;
                case 2:
                    label2.Text = "composer : Guitar Vader \n from : Jet Set Radio";
                    break;
                case 3:
                    label2.Text = "composer : Darren Korb \n from : Hades";
                    break;
                case 4:
                    label2.Text = "composer : Lena Raine \n from : celeste";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Menu obj = new Menu(id, music);
            obj.Show();
            this.Hide();
        }
    }
}
