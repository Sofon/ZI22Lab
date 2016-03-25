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
using System.Security.Cryptography;
using Org.Mentalis.Security.Cryptography;
namespace ZI1
{

    public partial class Form1 : Form
    {
        Form6 passFrase = new Form6();
        // форма Вход в программу
        Form2 Enter;
        RC2CryptoServiceProvider rc2CSP;
        byte[] IV;
        // объект класса для генерации секретного ключа из парольной фразы
        PasswordDeriveBytes pdb;
        // буфер для парольной фразы
        byte[] pwd;
        // буфер для случайной примеси к ключу шифрования
        byte[] randBytes;
        // буфер для парольной фразы и случайной примеси
        byte[] buf;
        // объект для потока шифрования-расшифрования
        CryptoStream CrStream;
        // буфер для ввода-вывода данных из файла учетных записей
        byte[] bytes;
        // длина буфера ввода-вывода
        int numBytesToRead;
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
            Acc.AccMem = new MemoryStream();
            InitializeComponent();
            All.Enabled = false;
            KeyKey.Enabled = false;
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
                    if (Acc.AccMem != null) Acc.AccMem.Seek(0, SeekOrigin.Begin);
                    // открытие файла для чтения и записи
                    Acc.AccMem.Seek(0, SeekOrigin.Begin);
                    // сброс номера текущей учетной записи
                    Acc.RecCount = 0;
                    // чтение учетных записей и сравнение имен из них с введенным
                    // пользователем именем 
                    while (Acc.AccMem.Position < Acc.AccMem.Length)
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
                            Acc.AccMem.Seek((Acc.RecCount - 1) * Acc.AccLen,
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
                        KeyKey.Enabled = true;
                        // закрытие файла с учетными записями
                        Acc.AccMem.Seek(0, SeekOrigin.Begin);
                    }
                    else
                    {
                        All.Enabled = false;
                        New.Enabled = false;
                        KeyKey.Enabled = false;
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
                Acc.AccMem.Seek(0, SeekOrigin.Begin);
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
            Acc.AccMem.Seek((Acc.RecCount - 1) * Acc.AccLen, SeekOrigin.Begin);
                // помещение в учетную запись нового пароля и его длины
                Encoding.Unicode.GetBytes(ChangePass.Edit1.Text).
          CopyTo(Acc.UserAcc.UserPass, 0);
                Acc.UserAcc.PassLen = ChangePass.Edit1.Text.Length;
                // запись в файл учетной записи с новым паролем
                Acc.WriteAccount();
            }
            // если программа в режиме администратора, то закрытие файла
            if (All.Enabled)
                Acc.AccMem.Seek(0, SeekOrigin.Begin);

        }

        private void New_Click(object sender, EventArgs e)
        {

            // отображение формы для добавления пользователя
            if (Add.ShowDialog() == DialogResult.OK)
            {
                // смещение в конец файла
                Acc.AccMem.Seek(0, SeekOrigin.End);
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
            Acc.AccMem.Seek(0, SeekOrigin.Begin);
            // очистка редактора для ввода следующего имени пользователя
            Add.UserName.Clear();


        }

        private void All_Click(object sender, EventArgs e)
        {
            // открытие файла с учетными записями для чтения и записи
            Acc.AccMem.Seek(0, SeekOrigin.Begin);
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
            if (Acc.AccMem.Length == Acc.RecCount * Acc.AccLen)
            {
                List.Next.Enabled = false;
                // смещение к началу первой учетной записи
                Acc.AccMem.Seek(0, SeekOrigin.Begin);
            }
            // снятие блокировки с кнопки Следующий
            else
                List.Next.Enabled = true;
            // отображение окна просмотра (редактирования) учетных записей
            List.ShowDialog();
            // закрытие файла
            Acc.AccMem.Seek(0, SeekOrigin.Begin);

        }

        private void Form1_Shown(object sender, EventArgs e)
        {

            // создание формы для ввода парольной фразы для расшифрпования


            // блок с возможной генерацией исключительных ситуаций
            try
            {
                if (passFrase.ShowDialog() != DialogResult.OK)
                    throw new Exception("Работа программы невозможна!");
                // создание объекта для криптоалгоритма
                rc2CSP = new RC2CryptoServiceProvider();
                // определение режима блочного шифрования
                rc2CSP.Mode = CipherMode.CFB;
                // декодирование парольной фразы
                pwd = Encoding.Unicode.GetBytes(passFrase.Edit1.Text);
                
                // создание буфера для случайной примеси
                randBytes = new byte[8];
                randBytes[1] = 1;
                randBytes[5] = 1;
                // выделение памяти для буфера
                buf = new byte[pwd.Length + randBytes.Length];
                // копирование в буфер парольной фразы
                pwd.CopyTo(buf, 0);
                // освобождение ресурсов формы для ввода парольной фразы
                //passFrase.Dispose();
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
    
else
      {
                    // создание объекта для зашифрованного файла учетных записей
                    Acc.AccFile = new FileStream(Account.SECFILE, FileMode.Open);
                    randBytes = new byte[8];
                    randBytes[1] = 1;
                    randBytes[5] = 1;
                    // чтение случайной примеси из начала зашифрованного файла
                    Acc.AccFile.Read(randBytes, 0, 8);
                    // создание объекта для вывода ключа из парольной фразы
                    pdb = new PasswordDeriveBytes(pwd, randBytes);
                    // восстановление начального вектора из файла
                    IV = new byte[rc2CSP.BlockSize / 8];
                    Acc.AccFile.Read(IV, 0, rc2CSP.BlockSize / 8);
                    rc2CSP.IV = IV;
                    // вывод ключа расшифрования
                    rc2CSP.Key = pdb.CryptDeriveKey("RC2", "SHA", rc2CSP.KeySize,
         rc2CSP.IV);
                    // создание объекта расшифрования
                    ICryptoTransform decryptor = rc2CSP.CreateDecryptor(rc2CSP.Key,
         rc2CSP.IV);
                    // создание объекта для потока расшифрования
                    CrStream = new CryptoStream(Acc.AccFile, decryptor, CryptoStreamMode.Read);
                    // выделение памяти для буфера ввода-вывода
                    int numerik;
                    numerik = (int)(Acc.AccFile.Length - (8 + (rc2CSP.BlockSize / 8)));
                    bytes = new byte[numerik];                
                    // задание количества непрочитанных байт
                    numBytesToRead = (int)((Acc.AccFile.Length) - (8 + (rc2CSP.BlockSize/8)));
                    // ввод данных из исходного файла
                    int n = CrStream.Read(bytes, 0, numBytesToRead);
                    // сохранение фактического количества расшифрованных байт
                    numBytesToRead = n;
                    // запись в расшифрованный поток в памяти
                    Acc.AccMem.Write(bytes, 0, numBytesToRead);
                    // очистка памяти с объектом криптоалгоритма
                    rc2CSP.Clear();
                    // закрытие криптографического потока
                    CrStream.Close();
                    // закрытие зашифрованного файла
                    Acc.AccFile.Close();
                    // чтение первой учетной записи (администратора)
                    Acc.AccMem.Seek(0, SeekOrigin.Begin);
                    Acc.ReadAccount();
                    // если имя первой учетной записи не совпадает с ADMIN
                    if (Encoding.Unicode.GetString(Acc.UserAcc.UserName, 0, 10)
        != "ADMIN")
                        throw new Exception("Неверный ключ расшифрования!");
                }
            }
            // обработка исключения
            catch (Exception ex)
            {
                // вывод сообщения об ошибке
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                // завершение работы программы
                Application.Exit();
            }


            /* если файл с учетными записями пользователей не существует 
   (первый запуск программы) */
          

        }

        private void Relogin_Click(object sender, EventArgs e)
        {

        }

        private void relogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.Visible = true;
            All.Enabled = false;
            New.Enabled = false;
            KeyKey.Enabled = false;
            Change.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // если форму закрывает пользователь (нормальное завершение работы программы)
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                pwd = Encoding.Unicode.GetBytes(passFrase.Edit1.Text);
                buf = new byte[pwd.Length + randBytes.Length];
                // создание объекта для генерации случайной примеси
                RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
                // создание буфера для случайной примеси
                randBytes = new byte[8];
                randBytes[1] = 1;
                randBytes[5] = 1;
                // получение примеси для секретного ключа
                //rand.GetBytes(randBytes);
                // создание объекта для вывода ключа из парольной фразы
                pdb = new PasswordDeriveBytes(pwd, randBytes);
                // копирование в буфер примеси
                randBytes.CopyTo(buf, pwd.Length);
                // генерация начального вектора для блочного шифрования
                rc2CSP.GenerateIV();
                // вывод ключа шифрования из парольной фразы и примеси
                rc2CSP.Key = pdb.CryptDeriveKey("RC2", "SHA", rc2CSP.KeySize, rc2CSP.IV);
                // создание объекта шифрования
                ICryptoTransform encryptor = rc2CSP.CreateEncryptor(rc2CSP.Key, rc2CSP.IV);
                // создание нового файла
                Acc.AccFile = new FileStream(Account.SECFILE, FileMode.Create);
                // запись в начало зашифрованного файла случайной примеси
                Acc.AccFile.Write(randBytes, 0, 8);
                // сохранение в файле начального вектора
                Acc.AccFile.Write(rc2CSP.IV, 0, rc2CSP.BlockSize / 8);
                // создание объекта для потока шифрования
                CrStream = new CryptoStream(Acc.AccFile, encryptor, CryptoStreamMode.Write);
                // смещение к началу потока в памяти
                Acc.AccMem.Seek(0, SeekOrigin.Begin);
                // выделение памяти для буфера ввода-вывода
                bytes = new byte[Acc.AccMem.Length];
                // задание количества непрочитанных байт
                numBytesToRead = (int)Acc.AccMem.Length;
                // получение данных из потока в памяти
                int n = Acc.AccMem.Read(bytes, 0, numBytesToRead);
                // сохранение фактического количества прочитанных байт
                numBytesToRead = n;
                // запись в зашифрованный файл
                CrStream.Write(bytes, 0, numBytesToRead);
                // очистка памяти с конфиденциальными данными
                rc2CSP.Clear();
                // закрытие потока шифрования
                CrStream.Close();
                // закрытие файла и потока в памяти
                Acc.AccMem.Close();
                Acc.AccFile.Close();
            }

        }

        private void KeyKey_Click(object sender, EventArgs e)
        {
            passFrase.label1.Text = "Новая парольная фраза";
            passFrase.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
