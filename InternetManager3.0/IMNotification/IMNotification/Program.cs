using System;
using xNet;
using ISP;
using System.Collections.Generic;

namespace IMNotification
{
    class Program
    {
        private static INI ini = new INI(AppDomain.CurrentDomain.BaseDirectory + "settings.ini");
        private static List<string> fields;
        private static string Login = "";
        private static string Password = "";
        private static InternetCompany Company;
        private static InternetServiceProvider isp;
        public int HowManyDaysLeft
        {
            get
            {
                if (isp.Connected)
                {
                    double paymantPerDay = DateTime.Now.Hour > 10 ? isp.SpentMoney / DateTime.Now.Day : isp.SpentMoney / (DateTime.Now.Day - 1);
                    return (int)(isp.Balance / paymantPerDay);
                }
                else
                    return 0;
            }
        }
        static void Main(string[] args)
        {
            isp = new InternetServiceProvider();
            isp.CheckInternetConnection();
            LoadSettingsFromINI();
        }
        static void LoadSettingsFromINI()
        {
            #region Авторизация
            if (ini.IniReadValue("Auth", "Login") != "")
            {
                Login = ini.IniReadValue("Auth", "Login");//Получаем логин
                Password = ini.IniReadValue("Auth", "Pass");//Получаем пароль
                Enum.TryParse(ini.IniReadValue("Auth", "ServiceProvider"), out Company);//Получаем иcпользуемую интернет компанию
                SingIn();
            }
            #endregion
        }
        private static void SingIn()
        {
            Whale_On_Sky f;
            if (isp.Connected)//Проверяем есть ли интернет подключение?Если есть 
            {
                //На сегодняшний день у вас 546.69₽
                //Денег еще хватит на 2 дня
                isp.Authorization(Company, Login, Password, ref fields);//Авторизуемся                                                  //
                double paymantPerDay = DateTime.Now.Hour > 10 ? isp.SpentMoney / DateTime.Now.Day : isp.SpentMoney / (DateTime.Now.Day - 1);
                //
                f = new Whale_On_Sky(String.Format("На сегодняшний день у вас:{0}\r\nДенег еще хватит на {1} д.", isp.Balance, (int)(isp.Balance / paymantPerDay)));
            }
            else
                f = new Whale_On_Sky("Нет интернет соединения!");
            // lblError.Content = ERROR_NOT_CONNECTION;
        }
    }
    class Whale_On_Sky
    {
        INI ini = new INI(AppDomain.CurrentDomain.BaseDirectory + "settings.ini");
        Notification notif = new Notification();
        public Whale_On_Sky(string text)
        {
            CreateNotificationAndShow(text);
        }
        private MetroFramework.MetroThemeStyle MetroTheme(string Selection, string Key)
        {
            switch (ini.IniReadValue("Notification", "Theme"))
            {
                case "Dark":
                    return MetroFramework.MetroThemeStyle.Dark;
                case "Light":
                    return MetroFramework.MetroThemeStyle.Light;
                default:
                    break;
            }
            return MetroFramework.MetroThemeStyle.Default;
        }
        private MetroFramework.MetroColorStyle MetroStyle(string Selection, string Key)
        {
            switch (ini.IniReadValue("Notification", "Style"))
            {
                case "Black":
                    return MetroFramework.MetroColorStyle.Black;
                case "Blue":
                    return MetroFramework.MetroColorStyle.Blue;
                case "Brown":
                    return MetroFramework.MetroColorStyle.Brown;
                case "Green":
                    return MetroFramework.MetroColorStyle.Green;
                case "Lime":
                    return MetroFramework.MetroColorStyle.Lime;
                case "Magenta":
                    return MetroFramework.MetroColorStyle.Magenta;
                case "Orange":
                    return MetroFramework.MetroColorStyle.Orange;
                case "Pink":
                    return MetroFramework.MetroColorStyle.Pink;
                case "Purpure":
                    return MetroFramework.MetroColorStyle.Purple;
                case "Red":
                    return MetroFramework.MetroColorStyle.Red;
                case "Silver":
                    return MetroFramework.MetroColorStyle.Silver;
                case "Teal":
                    return MetroFramework.MetroColorStyle.Teal;
                case "White":
                    return MetroFramework.MetroColorStyle.White;
                case "Yellow":
                    return MetroFramework.MetroColorStyle.Yellow;
                default:
                    break;
            }
            return MetroFramework.MetroColorStyle.Default;
        }
        private int GetInt(string Selection, string Key)
        {
            return int.Parse(ini.IniReadValue(Selection, Key));
        }
        private void CreateNotificationAndShow(string text)
        {
            notif.CreateNotification(System.Drawing.ColorTranslator.FromHtml(ini.IniReadValue("Notification", "ColorText")), double.Parse(ini.IniReadValue("Notification", "Opacity")), MetroTheme("Notification", "Theme"), MetroStyle("Notification", "Style"));
            notif.ShowNotification(text, true, GetInt("Notification", "LifeSecond"), true);
        }
    }

}