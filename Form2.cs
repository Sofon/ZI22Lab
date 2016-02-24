using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZI1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK && Login.Text == "")
            {
                MessageBox.Show("Имя не может быть пустым!", "Ошибка",
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                // обработка события завершена
                e.Cancel = true;
            }

        }


    }
}
