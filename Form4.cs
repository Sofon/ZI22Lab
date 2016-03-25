using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace ZI1
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
          
        }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
                if (UserName.Text == "")
                {
                    MessageBox.Show("Имя не может быть пустым!", "Ошибка",
         MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // завершение обработки события
                    e.Cancel = true;
                }
                else
                {
                    // получение ссылки на объект учетной записи главной формы
                    Account Acc = ((Form1)(Application.OpenForms[0])).Acc;
                    // смещение к началу файла с учетными записями 
                    Acc.AccMem.Seek(0, SeekOrigin.Begin);
                    // чтение учетных записей из файла для проверки 
                    // уникальности введенного имени
                    while (Acc.AccMem.Position < Acc.AccMem.Length)
                    {
                        Acc.ReadAccount();
                        // если учетная запись с введенным именем уже существует,
                        // то прекращение чтения
                        if (UserName.Text == Encoding.Unicode.GetString
      (Acc.UserAcc.UserName, 0, UserName.Text.Length * 2))
                            break;
                    }
                    // если учетная запись с введенным именем уже существует 
                    // (не достигнут конец файла)
                    if (UserName.Text == Encoding.Unicode.GetString
        (Acc.UserAcc.UserName, 0, UserName.Text.Length * 2))
                    {
                        // вывод сообщения об ошибке 
                        MessageBox.Show("Пользователь " + UserName.Text +
      "\nуже зарегистрирован!",
                              "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // завершение обработки события
                        e.Cancel = true;
                    }
                }

        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }
    }
}
