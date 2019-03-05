using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AVZ_SSC_FW
{
    public partial class DiagramForOneHost : Form
    {
        Main main;

        string item;
        DateTime ot;
        DateTime po;
        bool serv;

        public DiagramForOneHost()
        {
            InitializeComponent();
        }



        private void DiagramForOneHost_FormClosing(object sender, FormClosingEventArgs e)
        {
            main.Enabled = true;
        }

        private void DiagramForOneHost_Load(object sender, EventArgs e)
        {
            main = this.Owner as Main;
            main.Enabled = false;

            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            int Day = DateTime.Now.Day;

            DateTime date = new DateTime(Year, Month, Day, 0, 0, 0);
            dateTimePicker1.Value = date;

            for (int i = 0; i < main.items.Count; i++)
            {
                string text = "";
                if (main.items[i].group == "Без группы") text = main.items[i].displayName + "/"+ main.items[i].hostName + "/" + main.items[i].ip;
                else text = main.items[i].group + "/"+ main.items[i].displayName + "/" + main.items[i].hostName + "/" + main.items[i].ip;

                comboBox1.Items.Add(text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "" && dateTimePicker1.Value <= dateTimePicker2.Value)
            {
                item = comboBox1.SelectedItem.ToString();
                ot = dateTimePicker1.Value;
                po = dateTimePicker2.Value;
                serv = checkBox1.Checked;

                backgroundWorker1.RunWorkerAsync();
                button1.Visible = false;
                button2.Visible = false;
            }
            else MessageBox.Show("Укажите корректные данные для формирования отчета!", "Ошибка!");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<LogEvent> logs = main.logger.ReadLog(item, ot, po, serv, main.setting);
            if (logs.Count != 0)
            {
                ReportExcellCreate report = new ReportExcellCreate();
                report.createReportExcell(main.setting, logs, "AveragePingTime");
                //MessageBox.Show("Успех");
            }
            else
            {
                MessageBox.Show("Нет данных для отображения отчета", "Упс!");
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Visible = true;
            button2.Visible = true;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
