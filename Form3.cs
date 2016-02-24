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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();

        }


        private void Next_Click(object sender, EventArgs e)
        {
            // получение ссылки на объект учетной записи главной формы
            Account Acc = ((Form1)(Application.OpenForms[0])).Acc;
            // чтение учетной записи из файла
            Acc.ReadAccount();
            // увеличение номера текущей учетной записи
            Acc.RecCount++;
            // отображение имени учетной записи
            UserName.Text = Encoding.Unicode.GetString(Acc.UserAcc.UserName, 0,
            (int)Acc.UserAcc.UserName.Length);
            // отображение признака блокировки учетной запис
            checkBox1.Checked = Acc.UserAcc.Block;
            // отображение признака ограничения на пароль
            checkBox2.Checked = Acc.UserAcc.Restrict;
            // если следующей учетной записи нет, то блокирование кнопки «Следующий» в окне 
            //просмотра (редактирования) учетных записей
            if (Acc.AccFile.Length == Acc.RecCount * Acc.AccLen)
                Next.Enabled = false;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // получение ссылки на объект учетной записи главной формы
            Account Acc = ((Form1)(Application.OpenForms[0])).Acc;
            // смещение к началу редактируемой учетной записи в файле
            Acc.AccFile.Seek((Acc.RecCount - 1) * Acc.AccLen, SeekOrigin.Begin);
            // получение значений полей измененной учетной записи
            Acc.UserAcc.Block = checkBox1.Checked;
            Acc.UserAcc.Restrict = checkBox2.Checked;
            // запись в файл
            Acc.WriteAccount();
            // смещение для чтения следующей учетной записи
            Acc.AccFile.Seek(Acc.RecCount * Acc.AccLen, SeekOrigin.Begin);

        }
    }
}
