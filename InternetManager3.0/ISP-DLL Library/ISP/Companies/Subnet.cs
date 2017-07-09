using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xNet;

namespace ISP.Companies
{
    class Subnet : BaseCompanyClass, ICompany
    {
        public const string name = "Subnet";
        private double balance=0;
        private double spentMoney=0;
        public string Name { get => name; }

        public double Balance { get => balance; }

        public double SpentMoney { get => spentMoney; }
        public void Auth(string Login, string Password, out List<string> fields)
        {
            fields = new List<string>();
            using (HttpRequest client = new HttpRequest())
            {
                client.UserAgent = Http.FirefoxUserAgent();
                CookieDictionary cookie = new CookieDictionary(false);
                client.Cookies = cookie;
                var reqParams = new RequestParams();
                reqParams["user"] = Login;
                reqParams["pswd"] = Password;
                string content = client.Post("http://lk.subnet05.ru/", reqParams).ToString();
                string ID = content.Substring("var contractId = ", ";");
                if (ID == "")
                    throw new Exception("ERROR: NOT AUTHORIZED,INVALID LOGIN OR PASSWORD");
                GetValue(client.Get("http://lk.subnet05.ru/webexecuter?action=GetBalance&mid=0&module=contract&contractId=" + ID).ToString(), ref fields);
                CalculateSubscriptionFee(balance, spentMoney, ref fields);
                //GetValue(client.Get("http://lk.subnet05.ru/webexecuter?action=GetBalance&module=contract&mid=0&year=2017&month=5").ToString(), ref fields);
            }
        }

        public void GetValue(string html, ref List<string> Items)
        {
            html = html.Substring("<tbody>", "</tbody>");
            html = System.Text.RegularExpressions.Regex.Replace(html, @"\t|\n|\r", "");
            string valueNum, valueStr = " ";
            while (true)
            {
                valueStr = html.Substring(";\">", "</td>");
                valueNum = html.Substring("<td>", "</td>");
                if (valueStr == String.Empty && valueNum == String.Empty)
                    break;
                if (valueStr == "Исходящий остаток на конец месяца")
                    balance = double.Parse(valueNum.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));//Данная строка заменяем точку в числе на тот символ,которые использует данный компьюте в качестве разделения не целого числа
                if (valueStr == "Абонплата")
                    spentMoney = double.Parse(valueNum.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));

                html = html.Remove(0, html.IndexOf("</tr>") + 5);
                if (Char.IsDigit(valueStr[0]) || valueStr == "")
                    continue;
                Items.Add(valueStr + ": " + valueNum + "₽");
            }
        }//Считываем с html нужные нам значения
    }
}
