using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP.Companies
{
    class BaseCompanyClass
    {
        public void CalculateSubscriptionFee(double Balance, double SpentMoney, ref List<string> fields)
        {
            string lblInfo;
            string lblOutMonth;
            DateTime day = DateTime.Now;
            if (SpentMoney != 0)
            {
                double TempDayOfMonth = DateTime.DaysInMonth(day.Year, day.Month);//Получаем количество дней в месяце
                double TemplblOutDay = 0.0f;
                double paymentPertDay;
                if (day.Hour < 10)//Если сейчас раньше 10 часов,то отнимает 1 день т.к. списание происходит после 10
                    paymentPertDay = SpentMoney / (day.Day - 1);
                else
                    paymentPertDay = SpentMoney / day.Day;
                double paymantMonth = paymentPertDay * TempDayOfMonth;//узнаем сколько конкретно за этот месяц должны мы
                int HowMuchIsEnough = ((int)(Balance / paymentPertDay));//на сколько дней у нас еще есть деньги
                DateTime dayOfpaymant;//тут хранится дата окончания средств на считу
                if (day.Hour < 10)
                    dayOfpaymant = AddDate(day, HowMuchIsEnough);
                else
                    dayOfpaymant = AddDate(day, HowMuchIsEnough + 1);

                if (dayOfpaymant.Day != day.Day)
                    lblInfo = "До " + dayOfpaymant.ToLongDateString().ToString() + " 10:00 Оплачено!";
                else
                    lblInfo = "Сегодня после 10:00 у вас отключат инернет";

                TemplblOutDay = (Balance - ((TempDayOfMonth - day.Day) * paymentPertDay));

                if (TemplblOutDay > 0.0f)
                    lblOutMonth = String.Format("На 1e {0} у вас останется:{1:0.00}₽", LabelMonth((byte)DateTime.Now.AddMonths(1).Month), TemplblOutDay);
                else
                    lblOutMonth = String.Format("На 1e {0} у вас останется:0 ₽", LabelMonth((byte)DateTime.Now.AddMonths(1).Month));
            }
            else
            {
                lblInfo = "У вас \"Наработка за месяц\" равна 0,программа не может подсчитать";
                lblOutMonth = String.Format("На 1e {0} у вас останется: {1}₽", DateTime.Now.AddMonths(1).ToLongDateString().Remove(0, 2), SpentMoney);
            }
            fields.Add(lblOutMonth);
            fields.Add(lblInfo);
        }//Устанавливаем дату окончания средств на счету
        public DateTime AddDate(DateTime Date, int DayAdd)
        {
            DateTime tempDay = Date;
            int dayOfmonth = DateTime.DaysInMonth(Date.Year, Date.Month);
            int Month = 0;
            int Year = 0;
            while (DayAdd > dayOfmonth)
            {
                DayAdd -= dayOfmonth;
                Month++;
                if ((Date.Month + Month) > 12)
                {
                    Month = 1;
                    Year++;
                }
            }
            tempDay = tempDay.AddYears(Year);
            tempDay = tempDay.AddMonths(Month);
            tempDay = tempDay.AddDays(DayAdd);
            return tempDay;

        }//Относится к CalculateSubscriptionFee. Прибавляет дни(штатными нельзя).AddDay не подходит
        public string LabelMonth(byte NubmerMonth)
        {
            switch (NubmerMonth)
            {
                case 1:
                    return "Января";
                case 2:
                    return "Февраля";
                case 3:
                    return "Марта";
                case 4:
                    return "Апреля";
                case 5:
                    return "Мая";
                case 6:
                    return "Июня";
                case 7:
                    return "Июля";
                case 8:
                    return "Августа";
                case 9:
                    return "Сентября";
                case 10:
                    return "Октября";
                case 11:
                    return "Ноября";
                case 12:
                    return "Декабря";
            }
            return "";
        }
    }
}
