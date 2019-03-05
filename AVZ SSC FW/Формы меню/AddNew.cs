using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AVZ_SSC_FW
{
    public partial class AddNew : Form
    {
        Main main;

        public AddNew()
        {
            InitializeComponent();
        }

        private void AddNew_Load(object sender, EventArgs e)
        {
            main = this.Owner as Main;
            main.Enabled = false;

            for (int i = 0; i < main.group.Count; i++)
            {
                comboBox1.Items.Add(main.group[i].Name);
            }

            comboBox1.Text = comboBox1.Items[0].ToString();
            this.ActiveControl = textBox1;
        }

        private void AddNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            main.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Added();
        }

        private void Added()
        {
            IPAddress iPAddress = null;

            //Проверяем, не пустые ли поля
            if (textBox1.Text == "" && textBox2.Text == "") label5.Text = "Заполните хотя бы одно поле!";
            else
            {

                if (textBox2.Text != "") //Если заполнили IP адрес
                {

                    string pattern = @"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}";
                    if (Regex.IsMatch(textBox2.Text, pattern))
                    {
                        if (IPAddress.TryParse(textBox2.Text, out iPAddress))
                        {
                            if (textBox1.Text == "") label6.Text = "Стараюсь определить имя хоста...";
                        }
                        else label5.Text = "Неверный формат IP адреса";
                    }
                    else label5.Text = "Неверный формат IP адреса";
                }

                string group = "";

                try
                {
                    group = comboBox1.SelectedItem.ToString(); 
                }
                catch
                {
                    group = "Без группы";
                }

                IPAddress ipad;
                if (iPAddress != null) ipad = iPAddress;
                else ipad = IPAddress.Parse("0.0.0.0");

                    Items item = new Items //Создаем предмет коллекции
                    {
                        hostName = textBox1.Text,
                        ip = ipad,
                        group = group
                    };

                bool notItem = true;

                foreach (var curItem in main.items) //И проверяем, нет ли его уже в списке добавленных
                {
                    if (item.ip == null)
                    {
                        if (item.hostName == curItem.hostName && textBox1.Text == "")
                        {
                            notItem = false;
                            MessageBox.Show("Данный компьютер уже находится в списке отслеживаемых!", "Упс!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        if ((item.hostName == curItem.hostName || curItem.ip == item.ip) && textBox1.Text == "")
                        {
                            notItem = false;
                            MessageBox.Show("Данный компьютер уже находится в списке отслеживаемых!", "Упс!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

                if (notItem)
                {
                    main.AddItemList(item); //И если же нет, то добавляем
                    this.Close();
                }
            }
        }

        private void AddNew_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Added();
            }
        }

    }
}
