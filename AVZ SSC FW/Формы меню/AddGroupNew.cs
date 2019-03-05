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
    public partial class AddGroupNew : Form
    {
        Main main;

        public AddGroupNew()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                bool duplicate = false;
                foreach (var item in main.group)
                {
                    if (textBox1.Text == item.Name) duplicate = true;
                }

                if (!duplicate)
                {
                    ListViewGroup viewGroup = new ListViewGroup
                    {
                        Name = textBox1.Text,
                        Header = textBox1.Text
                    };
                    main.group.Add(viewGroup);
                    main.refresh = true;
                    main.RefreshViewList();
                    this.Close();
                }
                else
                {
                    DialogResult result = MessageBox.Show(
                        $"Группа с именем {textBox1.Text} уже существует? Добавить группу с другим именем?",
                        "Добавление группы",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                        );

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            textBox1.Text = "";
                        }
                        catch { }
                    }
                    else
                    {
                        this.Close();
                    }
                    
                }
            }
        }

        private void AddGroupNew_Load(object sender, EventArgs e)
        {
            main = this.Owner as Main;
            main.Enabled = false;
            this.ActiveControl = textBox1;
        }

        private void AddGroupNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            main.Enabled = true;
        }
    }
}
