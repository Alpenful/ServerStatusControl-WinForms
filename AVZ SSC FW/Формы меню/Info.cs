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
    public partial class Info : Form
    {
        Main main;

        public Info()
        {
            InitializeComponent();
        }

        private void Info_Load(object sender, EventArgs e)
        {
            label5.Visible = false;
            label7.Visible = false;

            main = this.Owner as Main;

            for (int i = 0; i < main.group.Count; i++)
            {
                comboBox1.Items.Add(main.group[i].Name);
            }

            //Задаем окну соответствие текущим настройкам
            Items current = main.items[main.selectedItem];

            this.Text = current.displayName;
            textBox1.Text = current.description;
            textBox2.Text = current.displayName;
            textBox3.Text = current.hostName;
            textBox4.Text = current.ip.ToString();
            comboBox1.SelectedItem = current.group;
            checkBox1.Checked = current.ignoreNotification;
            checkBox2.Checked = current.logWrite;


            if(current.status == 0)
            {
                label5.Visible = false;
                label7.Visible = true;
            }
            else
            {
                label5.Visible = true;
                label7.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            main.items[main.selectedItem] = new Items
            {
                id = main.selectedItem,
                displayName = textBox2.Text,
                hostName = textBox3.Text,
                ip = IPAddress.Parse(textBox4.Text),
                status = main.items[main.selectedItem].status,
                description = textBox1.Text,
                group = comboBox1.SelectedItem.ToString(),
                report = false,
                ignoreNotification = checkBox1.Checked,
                lastView = main.items[main.selectedItem].lastView,
                logWrite = checkBox2.Checked
            };

            main.refresh = true;
            main.RefreshViewList();
            main.OnWriteItems();
            this.Close();
        }
    }
}
