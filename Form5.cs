using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace ZI1
{
    public partial class Form5 : Form
    {
        // метод для проверки ограничений на пароль
        private bool CheckPassword()
        {  
            // признаки наличия в парольной фразе прописных и строчных букв, 
            // цифр и математических символов
            bool RUS = false, ENG = false, Zn = false;
            RUS = Regex.IsMatch(Edit1.Text, "[а-яА-ЯеЁ]");
            ENG = Regex.IsMatch(Edit1.Text, "[A-z]");          
            Zn = Regex.IsMatch(Edit1.Text, "[-‘.’_,:;!?()”+$$]");
            return (RUS && ENG && Zn);
        }

        public Form5()
        {   

            InitializeComponent();
            

        }

        private void Form5_FormClosing(object sender, FormClosingEventArgs e)
        {
            Account Acc = ((Form1)(Application.OpenForms[0])).Acc;
            if (DialogResult == DialogResult.OK && Edit1.Text == "")
            {
                MessageBox.Show("Пароль не может быть пустым!", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                // обработка события завершена 
                Edit1.Text = "";
                Edit2.Text = "";
                Edit3.Text = "";
                e.Cancel = true;
            }

            else if (DialogResult == DialogResult.OK && Edit3.Text != Encoding.Unicode.GetString(Acc.UserAcc.UserPass,
            0, Edit3.Text.Length * 2))
            {
                MessageBox.Show("Текущий пароль неверен!", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                // обработка события завершена 
                Edit1.Text = "";
                Edit2.Text = "";
                Edit3.Text = "";
                e.Cancel = true;
            }
            else if (DialogResult == DialogResult.OK && Edit1.Text != Edit2.Text)
            {
                MessageBox.Show("Пароль и подтверждение не совпадают!", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                // обработка события завершена 
                Edit1.Text = "";
                Edit2.Text = "";
                Edit3.Text = "";
                e.Cancel = true;
            }
            else if (DialogResult == DialogResult.OK && Edit1.Text != Edit2.Text && Edit3.Text == Encoding.Unicode.GetString(Acc.UserAcc.UserPass,
            0, Edit3.Text.Length * 2))
            {
                MessageBox.Show("Пароль и подтверждение не совпадают!", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                // обработка события завершена 
                Edit1.Text = "";
                Edit2.Text = "";
                Edit3.Text = "";
                e.Cancel = true;
            }
            else if (DialogResult == DialogResult.OK && Edit1.Text == Edit2.Text && Edit3.Text != Encoding.Unicode.GetString(Acc.UserAcc.UserPass,
            0, Edit3.Text.Length * 2))
            {
                MessageBox.Show("Текущий пароль неверен!", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                // обработка события завершена 
                Edit1.Text = "";
                Edit2.Text = "";
                Edit3.Text = "";
                e.Cancel = true;
            }
            else if (DialogResult == DialogResult.OK &&
            ((Form1)(Application.OpenForms[0])).Acc.UserAcc.Restrict &&
            !CheckPassword())
            {
                MessageBox.Show("Пароль не соответствует ограничениям!", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                // обработка события завершена 
                Edit1.Text = "";
                Edit2.Text = "";
                Edit3.Text = "";
                e.Cancel = true;
            }

        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
    }

