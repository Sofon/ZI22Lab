using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ZI1
{
    public class Account
    {
         // подключение пространства имен классов ввода-вывода
// максимальная длина имени учетной записи
public const int MAXNAME = 20;
    // максимальная длина пароля
    public const int MAXPASS = 20;
    // имя файла с учетными записями пользователей
    public const string SECFILE = "security.db";
    // структурный тип для хранения учетной записи
    public struct AccountType
    {
        public byte[] UserName; // имя
        public byte[] UserPass; // пароль
        public int PassLen; // длина пароля
        public bool Block; // признак блокировки учетной записи администратором
                           // признак включения администратором ограничений на выбираемые
                           // пользователями пароли
        public bool Restrict;
    };
    // объект для хранения одной учетной записи
    public AccountType UserAcc;
    // поток для чтения и записи в файл с учетными записями
    public FileStream AccFile;
    // номер текущей учетной записи
    public int RecCount;
    // буфер для учетной записи
    public byte[] buf;
    // текущая длина буфера для учетной записи
    public int pos;
    // длина учетной записи
    public int AccLen;
    // конструктор класса
    public Account()
    {
        // создание объекта для учетной записи
        UserAcc = new AccountType();
        UserAcc.UserName = new byte[MAXNAME * 2];
        UserAcc.UserPass = new byte[MAXPASS * 2];
        AccLen = MAXNAME * 2 + MAXPASS * 2 + sizeof(int) + sizeof(bool) * 2;
        buf = new byte[AccLen];
    }
    // метод для записи учетной записи в файл
    public void WriteAccount()
    {
        // преобразование учетной записи в массив байт
        // сброс текущей позиции в буфере для учетной записи
        pos = 0;
        // запись в буфер имени пользователя
        UserAcc.UserName.CopyTo(buf, pos);
        pos += MAXNAME * 2;
        // запись в буфер пароля
        UserAcc.UserPass.CopyTo(buf, pos);
        pos += MAXPASS * 2;
        // преобразование и запись длины пароля
        BitConverter.GetBytes(UserAcc.PassLen).CopyTo(buf, pos);
        pos += sizeof(int);
        // преобразование и запись признака блокировки учетной записи
        BitConverter.GetBytes(UserAcc.Block).CopyTo(buf, pos);
        pos += sizeof(bool);
        // преобразование и запись признака ограничений на пароль
        BitConverter.GetBytes(UserAcc.Restrict).CopyTo(buf, pos);
        pos += sizeof(bool);
        // запись буфера с учетной записью в файл
        AccFile.Write(buf, 0, pos);
    }
    // метод для чтение учетной записи из файла
    public void ReadAccount()
    {
        // чтение имени пользователя
        AccFile.Read(UserAcc.UserName, 0, MAXNAME * 2);
        // чтение пароля
        AccFile.Read(UserAcc.UserPass, 0, MAXPASS * 2);
        // выделение памыти под временный буфер
        byte[] tmp = new byte[sizeof(int)];
        // чтение и преобразование длины пароля
        AccFile.Read(tmp, 0, sizeof(int));
        UserAcc.PassLen = BitConverter.ToInt32(tmp, 0);
        // чтение и преобразование признака юлокировки учетной записи
        AccFile.Read(tmp, 0, sizeof(bool));
        UserAcc.Block = BitConverter.ToBoolean(tmp, 0);
        // чтение и преобразование признака ограничений на пароль
        AccFile.Read(tmp, 0, sizeof(bool));
        UserAcc.Restrict = BitConverter.ToBoolean(tmp, 0);
    }

}
}
