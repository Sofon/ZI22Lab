using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ZI1
{

    public partial class Form1 : Form
    {   
        // форма Вход в программу
        Form2 Enter;
        // форма Список пользователей
        Form3 List;
        // форма Добавление пользователя
        Form4 Add;
        // форма Смена пароля
        Form5 ChangePass;
        // учетная запись пользователя
        public Account Acc;
        // счетчик попыток входа в программу
        static int EnterCount;

        public Form1()
        {
            Enter = new Form2();
            List = new Form3();
            Add = new Form4();
            ChangePass = new Form5();
            Acc = new Account();
            InitializeComponent();
            All.Enabled = false;
            New.Enabled = false;
            relogToolStripMenuItem.Enabled = false;
            Change.Enabled = false;
            /* если файл с учетными записями пользователей не существует 
   (первый запуск программы) */

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // запрос и проверка имени учетной записи и пароля
            // временный буфер дляполей учетной записи
            byte[] tmp;
            try
            {
                // очистка редакторов для ввода имени и пароля
                Enter.Login.Clear();
                Enter.Password.Clear();
                // отображение формы для ввода имени и пароля
                if (Enter.ShowDialog() == DialogResult.OK)
                {
                    // если повторная попытка входа, то закрытие файла учетных записей
                    if (Acc.AccFile != null) Acc.AccFile.Close();
                    // открытие файла для чтения и записи
                    Acc.AccFile = new FileStream(Account.SECFILE, FileMode.Open);
                    // сброс номера текущей учетной записи
                    Acc.RecCount = 0;
                    // чтение учетных записей и сравнение имен из них с введенным
                    // пользователем именем 
                    while (Acc.AccFile.Position < Acc.AccFile.Length)
                    {
                        // чтение очередной учетной записи
                        Acc.ReadAccount();
                        // увеличение относительного номера учетной записи
                        Acc.RecCount++;
                        // прекращение чтения из файла, если обнаружено совпадение имен
                        if (Enter.Login.Text == Encoding.Unicode.GetString
      (Acc.UserAcc.UserName, 0, Enter.Login.Text.Length * 2))
                            break;
                    }

                    // если совпадения не найдено (достигнут конец файла)
                    if (Enter.Login.Text != Encoding.Unicode.GetString
        (Acc.UserAcc.UserName, 0, Enter.Login.Text.Length* 2))
                        // генерация исключительной ситуации (пользователь не зарегистрирован)
                        throw new Exception("Вы не зарегистрированы!");
                    // если пароль отсутствует (первый вход пользователя в программу)
                    else if (Acc.UserAcc.PassLen == 0)
                    {
                        // очистка редакторов для ввода пароля и подтверждения
                        ChangePass.Edit1.Clear();
                        ChangePass.Edit2.Clear();
                        ChangePass.Edit3.Clear();
                        // отображение формы для ввода (смены) пользователем пароля
                        if (ChangePass.ShowDialog() == DialogResult.OK)
                        {
                            // смещение к началу текущей учетной записи в файле 
                            Acc.AccFile.Seek((Acc.RecCount - 1) * Acc.AccLen,
     SeekOrigin.Begin);
                            // добавление введенного пароля и его длины в учетную
                            // запись
                            Encoding.Unicode.GetBytes(ChangePass.Edit1.Text).
                            CopyTo(Acc.UserAcc.UserPass, 0);
                            Acc.UserAcc.PassLen = ChangePass.Edit1.Text.Length;
                            // запись в файл учетной записи с начальным паролем
                            Acc.WriteAccount();
                            ChangePass.Edit1.Text ="";
                            ChangePass.Edit2.Text ="";
                            ChangePass.Edit3.Text ="";
                        }
                        // если пользователь не ввел пароль, то выход из функции
                        else return;
                    }
                    // если пользователь уже имел пароль
                    else
          
                    {
                        // сравнение пароля из учетной записи и введенного пароля
                        if (Enter.Password.Text == "" || Enter.Password.Text
!= Encoding.Unicode.GetString(Acc.UserAcc.UserPass,
0, Acc.UserAcc.PassLen * 2))
                            // если пароли не совпадают и число попыток превысило 2
                            if (++EnterCount > 2)
                            {
                                // скрытие кнопки «Вход» 
                                button1.Visible = false;
                                // генерация исключительной ситуации
                                throw new Exception("Вход в программу невозможен!");
                            }
                            // если пароли не совпадают и число попыток не превысило 2
                            else
                                // генерация исключительной ситуации 
                                throw new Exception("Неверный пароль!");
                        // если пароли совпадают, то продолжение работы
                        else;
                    }
                    // если учетная запись заблокирована администратором
                    if (Acc.UserAcc.Block)
                        // генерация исключительной ситуации (пользователь заблокирован)
                        throw new Exception("Вы заблокированы!");
                    // проверка полномочий пользователя
                    // если пользователь является администратором
                    if (Encoding.Unicode.GetString(Acc.UserAcc.UserName, 0,
     Enter.Login.Text.Length * 2) == "ADMIN")
                    {
                        // снятие блокировки с команд меню «Все пользователи» и 
                        // «Новый пользователь» 
                        All.Enabled = true;
                        New.Enabled = true;
                        // закрытие файла с учетными записями
                        Acc.AccFile.Close();
                    }
                    else
                    {
                        All.Enabled = false;
                        New.Enabled = false;
                        Change.Enabled = false;
                    }
                    // снятие блокировки с команды меню «Смена пароля» 
                    Change.Enabled = true;
                    // скрытие кнопки «Вход»
                    button1.Visible = false;
                    relogToolStripMenuItem.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK,
           MessageBoxIcon.Error);
            }

        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // создание объекта для диалога О программе
            AboutBox1 ab = new AboutBox1();
            // выполнение диалога
            ab.ShowDialog();

        }

        private void Change_Click(object sender, EventArgs e)
        {
            // если программа в режиме администратора (команда «Все пользователи»
            // разблокирована)
            if (All.Enabled)
            {
                // открытие файла с учетными записями для чтения и записи
                Acc.AccFile = new FileStream(Account.SECFILE, FileMode.Open);
                // сброс номера текущей учетной записи
                Acc.RecCount = 0;
                // чтение учетной записи администратора (первой учетной записи)
                Acc.ReadAccount();
                Acc.RecCount++;
            }
            // отображение формы для смены пароля
            if (ChangePass.ShowDialog() == DialogResult.OK)
            {   if (ChangePass.Edit3.Text == "")
                {
                    MessageBox.Show("Пароль не может быть пустым","Ошибка", MessageBoxButtons.OK,MessageBoxIcon.Error);

                }
                // смещение к началу текущей учетной записи в файле 
                Acc.AccFile.Seek((Acc.RecCount - 1) * Acc.AccLen, SeekOrigin.Begin);
                // помещение в учетную запись нового пароля и его длины
                Encoding.Unicode.GetBytes(ChangePass.Edit1.Text).
          CopyTo(Acc.UserAcc.UserPass, 0);
                Acc.UserAcc.PassLen = ChangePass.Edit1.Text.Length;
                // запись в файл учетной записи с новым паролем
                Acc.WriteAccount();
            }
            // если программа в режиме администратора, то закрытие файла
            if (All.Enabled)
                Acc.AccFile.Close();

        }

        private void New_Click(object sender, EventArgs e)
        {
            // открытие файла учетных записей
            Acc.AccFile = new FileStream(Account.SECFILE, FileMode.Open);

            // отображение формы для добавления пользователя
            if (Add.ShowDialog() == DialogResult.OK)
            {
                // смещение в конец файла
                Acc.AccFile.Seek(0, SeekOrigin.End);
                // сохранение в учетной записи введенного имени пользователя
                Encoding.Unicode.GetBytes(Add.UserName.Text).
                  CopyTo(Acc.UserAcc.UserName, 0);
                // подготовка других полей новой учетной запис
                Encoding.Unicode.GetBytes("").CopyTo(Acc.UserAcc.UserPass, 0);
                Acc.UserAcc.PassLen = 0;
                Acc.UserAcc.Block = false;
                Acc.UserAcc.Restrict = true;
                // запись в файл учетной записи нового пользователя
                Acc.WriteAccount();
            }
            // закрытие файла
            Acc.AccFile.Close();
            // очистка редактора для ввода следующего имени пользователя
            Add.UserName.Clear();


        }

        private void All_Click(object sender, EventArgs e)
        {
            // открытие файла с учетными записями для чтения и записи
            Acc.AccFile = new FileStream(Account.SECFILE, FileMode.Open);
            // сброс номера текущей учетной записи
            Acc.RecCount = 0;
            // чтение первой учетной записи
            Acc.ReadAccount();
            Acc.RecCount++;
            // отображение имени учетной записи
            List.UserName.Text = Encoding.Unicode.GetString
           (Acc.UserAcc.UserName, 0, (int)Acc.UserAcc.UserName.Length);
            // отображение признака блокировки учетной запис
            List.checkBox1.Checked = Acc.UserAcc.Block;
            // отображение признака ограничения на пароль
            List.checkBox2.Checked = Acc.UserAcc.Restrict;
            // если следующей учетной записи нет, то блокирование кнопки «Следующий» в окне 
            //просмотра (редактирования) учетных записей
            if (Acc.AccFile.Length == Acc.RecCount * Acc.AccLen)
            {
                List.Next.Enabled = false;
                // смещение к началу первой учетной записи
                Acc.AccFile.Seek(0, SeekOrigin.Begin);
            }
            // снятие блокировки с кнопки Следующий
            else
                List.Next.Enabled = true;
            // отображение окна просмотра (редактирования) учетных записей
            List.ShowDialog();
            // закрытие файла
            Acc.AccFile.Close();

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            /* если файл с учетными записями пользователей не существует 
   (первый запуск программы) */
            if (!File.Exists(Account.SECFILE))
            {
                // создание нового файла
                Acc.AccFile = new FileStream(Account.SECFILE, FileMode.Create);
                // подготовка учетной записи администратора
                Encoding.Unicode.GetBytes("ADMIN").CopyTo(Acc.UserAcc.UserName, 0);
                Encoding.Unicode.GetBytes("").CopyTo(Acc.UserAcc.UserPass, 0);
                Acc.UserAcc.PassLen = 0;
                Acc.UserAcc.Block = false;
                Acc.UserAcc.Restrict = true;
                // запись в файл учетной записи администратора
                Acc.WriteAccount();
                // закрытие файла
                Acc.AccFile.Close();
                // сброс счетчика попыток входа в программу
                EnterCount = 0;
            }

        }

        private void Relogin_Click(object sender, EventArgs e)
        {

        }

        private void relogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.Visible = true;
            All.Enabled = false;
            New.Enabled = false;
            Change.Enabled = false;
        }
    }
}
