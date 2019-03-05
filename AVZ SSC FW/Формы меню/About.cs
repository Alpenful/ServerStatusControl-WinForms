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
    public partial class About : Form
    {
        Main main;

        public About()
        {   
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            main = this.Owner as Main;
            main.Enabled = false;

            // Создаем подсказку при наведении
            ToolTip toolTip1 = new ToolTip();

            // Задаем время появления и работы подсказки
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Разрешаем работать ему
            toolTip1.ShowAlways = true;

            // И задаем когда же он будет появляться
            toolTip1.SetToolTip(this.label1, "Бета-версия приложения");
            toolTip1.SetToolTip(this.label7, "Разработчик программы");
            toolTip1.SetToolTip(this.label8, "Почтовый адрес для связи с разработчиком");
        }

        private void About_FormClosing(object sender, FormClosingEventArgs e)
        {
            main.Enabled = true;
        }
    }
}
