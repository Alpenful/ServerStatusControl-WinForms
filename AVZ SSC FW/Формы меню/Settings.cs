using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AVZ_SSC_FW
{
    public partial class Settings : Form
    {
        Main main;
        public SettingApplication set;

        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            main = this.Owner as Main;
            main.Enabled = false;

            //Задаем окну соответствие текущим настройкам
            if (set.startWithWindows) checkBox1.Checked = true;
            else checkBox1.Checked = false;

            if (set.startInTray) checkBox2.Checked = true;
            else checkBox2.Checked = false;

            textBox1.Text = set.computersPath;
            textBox2.Text = set.reportPath;
            textBox3.Text = set.logPath;
            textBox4.Text = set.serverLogPath;

            numericUpDown1.Value = Convert.ToDecimal(set.hourPeriod);
            numericUpDown2.Value = Convert.ToDecimal(set.minutePeriod);
            numericUpDown3.Value = Convert.ToDecimal(set.secondPeriod);

        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            main.Enabled = true;
        }

        private void button3_Click_1(object sender, EventArgs e) // Обзор на папку с компьютерами
        {
            
            this.textBox1.Text = SetPath(this.textBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e) // Обзор на папку с репостами
        {
            this.textBox2.Text = SetPath(this.textBox2.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.textBox3.Text = SetPath(this.textBox3.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.textBox4.Text = SetPath(this.textBox4.Text);
        }

        private void button1_Click(object sender, EventArgs e) //Кнопка *Применить*
        {
            int hour = Convert.ToInt32(numericUpDown1.Value);
            int minute = Convert.ToInt32(numericUpDown2.Value);
            int seconds = Convert.ToInt32(numericUpDown3.Value);

            set.ApplySettings(checkBox1.Checked, checkBox2.Checked, textBox1.Text, textBox2.Text,textBox3.Text,textBox4.Text , hour , minute, seconds);
            main.TimerPreset();
            this.Close();
        }






        //Метод для задания места хранения
        private string SetPath(string ret)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.MyComputer
            };
            if (FBD.ShowDialog() == DialogResult.OK) return FBD.SelectedPath;
            else return ret;
            
        }

        
    }
}
