using System;
using xNet;

namespace IMNotification
{
    class Program
    {
        private static int HowMuchIsEnough;
        static void Main(string[] args)
        {
            Authorize auth = new Authorize();
            HowMuchIsEnough = auth.ReturnHowMuchIsEnough;
            Whale_On_Sky f = new Whale_On_Sky(auth.ReturnText);
        }

        public int ReturnHowMuchIsEnough
        {
            get { return HowMuchIsEnough; }
        }
    }
    class Whale_On_Sky
    {
        INI ini = new INI(AppDomain.CurrentDomain.BaseDirectory+ "settings.ini");
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

            notif.CreateNotification(System.Drawing.ColorTranslator.FromHtml(ini.IniReadValue("Notification", "ColorText")),double.Parse(ini.IniReadValue("Notification", "Opacity")), MetroTheme("Notification", "Theme"), MetroStyle("Notification", "Style"));
            notif.ShowNotification(text, true, GetInt("Notification", "LifeSecond"), true);
        }
}
    class Authorize
    {
        INI ini = new INI(AppDomain.CurrentDomain.BaseDirectory + "settings.ini");
        //
        //
        //
        private string Login = "";
        private string Pass = "";
        string Company = "";
        //
        //
        //
        string[] Items;
        string html = "";
        byte LenghtItem = 0;
        int HowMuchIsEnough = 0;
        string returnText;
        //
        //
        //
        public Authorize()
        {
            Auth();
        }
        private void Auth()
        {
            GetInfoAuth();
            try
            {
                using (HttpRequest net = new HttpRequest())
                {
                    string[] Links = WhatCompany(Company);//узнаем что за провайдер
                    net.UserAgent = Http.FirefoxUserAgent();//и так понятно
                    CookieDictionary cookie = new CookieDictionary(false);//тоже понятно
                    net.Cookies = cookie;//и тут тоже
                    var reqParams = new RequestParams();//шлет пост запрос
                    reqParams["user"] = Login;//шлет логин в поле user
                    reqParams["pswd"] = Pass;//шлет пароль в поле pswd
                    string content = net.Post(Links[1], reqParams).ToString();//шлем запрос на узказанную ссылку с логином и паролем
                    string ID = content.Substring("var contractId = ", ";");//узнаю свой ID чтоб прыгать по страницам(перейти к вкладки баланс)
                    html = net.Get(Links[0] + ID).ToString();//на сайте переходим на страницу баланс и получаем весь html
                                                             //
                                                             //
                                                             //
                    Items = getValueSite(html, LenghtItem);
                    CalculateInfo(Company);
                }
            }
            catch
            {
                returnText = "Нет подключения к интернету";
            }
        }//Авторизуемся
        private void CalculateInfo(string company)
        {
            string Subscriptionfee = "";
            string OutDay = "";
            switch (company)
            {
                case "R-line":
                case "Subnet":
                    {
                        Subscriptionfee = Items[3].Substring(Items[3].IndexOf(":") + 2).Replace("₽", "");
                        OutDay = Items[4].Substring(Items[4].IndexOf(":") + 2).Replace("₽", "");
                        break;
                    }
            }
            CalculateSubscriptionFee(Subscriptionfee, OutDay);
        }//Подсчитываем инфо и потом вызываем до какого хватит денег
        private void CalculateSubscriptionFee(string Subscriptionfee, string OutDay)//Вычесляем до какого нам хватит денег
        {
            DateTime day = DateTime.Now;
            double TempOutDay = Convert.ToDouble(OutDay.Replace(".",","));
            double TempSubscriptionfee = Convert.ToDouble(Subscriptionfee.Replace(".", ","));
            double TempDayOfMonth = DateTime.DaysInMonth(day.Year, day.Month);
            double paymantDay;
            if (day.Hour < 11)
                paymantDay = TempSubscriptionfee / (day.Day - 1);
            else
                paymantDay = TempSubscriptionfee / day.Day;
            double paymantMonth = paymantDay * TempDayOfMonth;//узнаем сколько конкретно за этот месяц должны мы
            HowMuchIsEnough = ((int)(TempOutDay / paymantDay));
            returnText = String.Format("На сегодняшний день у вас {0}₽\r\nДенег еще хватит на {1} {2}", TempOutDay, HowMuchIsEnough, WithEnd(HowMuchIsEnough));
        }
        private string[] WhatCompany(string company)
        {
            string[] reLinks = new string[4];
            switch (company)
            {
                case "R-line":
                    {
                        reLinks[0] = "https://state.r-line.ru:8443/bgbilling/webexecuter?action=GetBalance&mid=0&module=contract&contractId=";
                        reLinks[1] = "https://state.r-line.ru:8443/bgbilling/webexecuter";
                        LenghtItem = 6;
                        return reLinks;
                    }
                case "Subnet":
                    {
                        reLinks[0] = "http://lk.subnet05.ru/webexecuter?action=GetBalance&mid=0&module=contract&contractId=";
                        reLinks[1] = "http://lk.subnet05.ru/";
                        LenghtItem = 6;
                        return reLinks;
                    }

                default:
                    break;
            }
            return reLinks;
        }//Узнаем что за компания
        private string WithEnd(int days)
        {
            string tempDays = days.ToString();
            tempDays = tempDays.Remove(0, tempDays.Length - 1);
            switch (tempDays)
            {
                case "1":
                    return "день";
                case "2":
                case "3":
                case "4":
                    return "дня";
                default:
                    break;
            }
            return "дней";

        }//следим за правильным окончанием))
        private void GetInfoAuth()
        {
            if (ini.IniReadValue("Auth", "Login") != "")
            {
                Login = ini.IniReadValue("Auth", "Login");
                Pass = ini.IniReadValue("Auth", "Pass");
                Company = ini.IniReadValue("Auth", "ServiceProvider");
            }
        }//читаем с settings.ini
        public  string ReturnText
        {
            get {return returnText; }
        }//Возвращаем текст на фомру
        public int ReturnHowMuchIsEnough
        {
            get { return HowMuchIsEnough; }
        }
        private string[] getValueSite(string html, byte LenghtItem)
        {
            string[] Items = new string[LenghtItem];
            string table = html.Substring("<tbody>", "</tbody>");
            for (int i = 0; i < Items.Length; i++)
            {
                string valueStr = table.Substring(";\">", "</td>");
                string valueNum = table.Substring("<td>", "</td>");
                if (Char.IsDigit(valueStr[0]) || valueStr == "Абонплата")
                { table = table.Remove(0, table.IndexOf("</tr>") + 5); i--; continue; }
                Items[i] = valueStr + ": " + valueNum + "₽";
                table = table.Remove(0, table.IndexOf("</tr>") + 5);
            }
            return Items;
        }//Считываем с html нужные нам значения
    }
}