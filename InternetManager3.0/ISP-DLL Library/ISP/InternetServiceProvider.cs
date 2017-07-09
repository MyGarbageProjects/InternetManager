using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ISP
{
    //Если вы хотите добавить свою компанию
    //Добавте сюда название компании
    //Так же добавте класс своей компании,в папку Companies
    public enum InternetCompany
    {
        Subnet,
        RLine
    }
    public class InternetServiceProvider
    {
        private bool authorized;
        private bool connected;
        public bool Authorized
        {
            private set => authorized = value;
            get => authorized;
        }
        public bool Connected
        {
            private set => connected = value;
            get => connected;
        }

        public double Balance { get => provides.Balance; }
        public double SpentMoney { get => provides.SpentMoney; }
        ICompany provides;//Internet Servise Provides

        public void Authorization(InternetCompany CompanyName, string Login, string Password, ref List<string> fields)
        {
            switch (CompanyName)
            {
                case InternetCompany.Subnet:
                    try
                    {
                        provides = new Companies.Subnet();
                        provides.Auth(Login, Password, out fields);
                        Authorized = true;
                    }
                    catch (Exception)
                    {
                        Authorized = false;
                    }
                    break;

                case InternetCompany.RLine:
                    try
                    {
                        provides = new Companies.Rline();
                        provides.Auth(Login, Password, out fields);
                        Authorized = true;
                    }
                    catch (Exception)
                    {
                        Authorized = false;
                    }
                    break;
            }
        }
        public void CheckInternetConnection()
        {
            try
            {
                String host = "example.com";
                //String host = "192.168.0.1";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = new Ping().Send(host, timeout, buffer, pingOptions);
                if (reply.Status == IPStatus.Success) Connected = true;
            }
            catch (Exception)
            {
                Connected = false;
            }
        }//Проверка интернет соединения
    }
}
