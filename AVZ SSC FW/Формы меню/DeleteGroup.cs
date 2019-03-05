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
    public partial class DeleteGroup : Form
    {
        Main main;

        public DeleteGroup()
        {
            InitializeComponent();
        }

        private void DeleteGroup_Load(object sender, EventArgs e)
        {
            main = this.Owner as Main;
            main.Enabled = false;

            for (int i = 0; i < main.group.Count; i++)
            {   
                if(main.group[i].Name != "Без группы")
                comboBox1.Items.Add(main.group[i].Name);
            }
        }

        private void DeleteGroup_FormClosing(object sender, FormClosingEventArgs e)
        {
            main.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string name = comboBox1.SelectedItem.ToString();

                DialogResult result = MessageBox.Show(
                        $"Вы действительно хотите удалить группу *{name}*?",
                        "Добавление группы",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                        );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        foreach (var item in main.items)
                        {
                            if (item.group == name) item.group = "Без группы";
                        }

                        for (int i = 0; i < main.group.Count; i++)
                        {
                            if(main.group[i].Name == name)
                            {
                                main.group.RemoveAt(i);
                            }
                        }
                        main.refresh = true;
                        main.RefreshViewList();
                        main.OnWriteItems();
                        this.Close();
                    }
                    catch
                    {
                        MessageBox.Show("хуй");
                    }
                }
                else
                {
                    this.Close();
                }
            }
            catch
            {

            }
        }
    }
}
