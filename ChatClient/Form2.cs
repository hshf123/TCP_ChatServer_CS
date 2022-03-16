using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form2 : Form
    {
        public static Form2 Form_2;
        public Form2()
        {
            InitializeComponent();
            Form_2 = this;
        }

        private void btn_Entrance_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_userName.Text))
            {
                MessageBox.Show("이름을 입력해 주세요.");
                return;
            }

            Form1 form1 = new Form1(tb_userName.Text);
            form1.Show();
            this.Close();
        }
    }
}
