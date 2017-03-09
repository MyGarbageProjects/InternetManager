using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using xNet;
using System.IO;
namespace IMWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        INI ini = new INI(AppDomain.CurrentDomain.BaseDirectory.ToString() + "settings.ini");
        System.Windows.Media.Effects.DropShadowEffect effect;
        VusualControl vscl = new VusualControl();
        Authorization auth;
        string Login, Password, Company;
        public MainWindow()
        {
            InitializeComponent();
            #region настройки Блюра
            effect = new System.Windows.Media.Effects.DropShadowEffect();
            effect.BlurRadius = 10;
            effect.Direction = 180;
            effect.Opacity = 100;
            effect.RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance;
            effect.ShadowDepth = 0;
            #endregion
            auth = new Authorization(LabelsContainer);
            //f = new AnimationControl(MainForm, MainGrid);
        }
        #region Перемещение формы за головку
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ReleaseCapture();
            SendMessage(new System.Windows.Interop.WindowInteropHelper(this).Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        #endregion
        #region Анимация настрока кнопок управления
        //imgClose
        private void imgClose_MouseMove(object sender, MouseEventArgs e)
        {
            imgClose.Source = new BitmapImage(new Uri("Resources/CloseActive.png",UriKind.Relative));
        }
        private void imgClose_MouseLeave(object sender, MouseEventArgs e)
        {
            imgClose.Source = new BitmapImage(new Uri("Resources/CloseUnActive.png", UriKind.Relative));
        }
        private void imgClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
        //imgMinimized
        private void imgMinimize_MouseMove(object sender, MouseEventArgs e)
        {
             imgMinimized.Source = new BitmapImage(new Uri("Resources/MinimizeActive.png", UriKind.Relative));
        }
        private void imgMinimize_MouseLeave(object sender, MouseEventArgs e)
        {
            imgMinimized.Source = new BitmapImage(new Uri("Resources/MinimizeUnActive.png", UriKind.Relative)); 
        }
        private void imgMinimize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        //imgSetting
        private void imgSettings_MouseMove(object sender, MouseEventArgs e)
        {
            imgSettings.Source = new BitmapImage(new Uri("Resources/SettingsActive.png",UriKind.Relative));
        }
        private void imgSettings_MouseLeave(object sender, MouseEventArgs e)
        {
            imgSettings.Source = new BitmapImage(new Uri("Resources/SettingsUnActive.png", UriKind.Relative));
        }
        private void imgSettings_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("Раздел еще не создан.Доработай");
            Window1 f = new Window1();
            f.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            f.Owner = this;
            //this.Visibility = Visibility.Hidden;
            f.ShowInTaskbar = false;
            f.ShowDialog();
        }
        #endregion
        private void MainForm_Loaded(object sender, RoutedEventArgs e)
        {
            vscl.AddRectangle(GridMain);//добавляем линию по контуру
            //
            LoadSettingsFromINI();
            //
            //
        }
        public void LoadSettingsFromINI()
        {
            //
            //
            #region Авторизация
            if(ini.IniReadValue("Auth","Login")!="")
            {
                Login = ini.IniReadValue("Auth", "Login");//получение логина
                Password = ini.IniReadValue("Auth", "Pass");//получение пароля
                Company = ini.IniReadValue("Auth", "ServiceProvider");//получение имя компании
                if (!auth.AuthDoing)
                {
                    auth.Auth(Company, Login, Password);
                }
                lbl0.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;
            }
            else
            {
                //добовляю на форму лабел с информацией что нужно бы ввести пароль
                lbl0.Visibility = Visibility.Visible;
                image.Visibility = Visibility.Visible;
            }
            #endregion
            //
            //
            #region Цвета элементов
            RectTopACap.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ini.IniReadValue("Visual", "ColorTop")));//устанавливаем цвет шапки
            #region насчет Линий по контуру
            if (!(Convert.ToBoolean(ini.IniReadValue("Visual","BoolRemoveRectangle"))))//если "Удалить линию по котуру" не включено то проверяем
            {
                if (Convert.ToBoolean(ini.IniReadValue("Visual", "BoolAnimationRectangle")))//включена ли "Анимация по контуру" если да то 
                {
                    System.Windows.Shapes.Rectangle refRectangle = vscl.rect;//создаем прямоугольник
                    vscl.AddAnimationControl(GridMain, refRectangle);//добовляем анимированный
                    vscl.Start();//и анимируем
                }
                else//иначе
                    vscl.SetColorRectangle = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ini.IniReadValue("Visual", "ColorRectangle")));//устанавливаем цвет прямоугольнику по контуру
            }
            #endregion
            #endregion
            //
            //
        }
    }
}
class Authorization
{
    private string html = "";
    private byte LenghtItem = 0;
    string[] Items;
    StackPanel Panel;
    bool BoolAuth = false;
    public Authorization(StackPanel panel)
    {
        Panel = panel;
    }
    public void Auth(string NameCompany,string Login,string Password)
    {
        try
        {
            using (HttpRequest net = new HttpRequest())
            {
                string[] Links = this.NameCompany(NameCompany);
                string GetInfo = Links[0];
                net.UserAgent = Http.FirefoxUserAgent();
                CookieDictionary cookie = new CookieDictionary(false);
                net.Cookies = cookie;
                var reqParams = new RequestParams();
                reqParams["user"] = Login;
                reqParams["pswd"] = Password;
                string content = net.Post(Links[1], reqParams).ToString();
                string ID = content.Substring("var contractId = ", ";");
                html = net.Get(GetInfo + ID).ToString();
                Items = getValuesite(html, LenghtItem);
                CalculateInfo(NameCompany);
                AddLabelOnGrid(Panel, LenghtItem);
                BoolAuth = true;
            }
        }
        catch
        {
            MessageBox.Show("Вы ввели неверный логин или пароль.Пожалуйста перепроверте!");
            BoolAuth = false;
        }
    }//Авторизуемся
    private string[] NameCompany(string company)
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
            case "subnet":
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
    }//Название комнапии предостовляющая интернет услуги
    private string[] getValuesite(string html, byte LenghtItem)
    {
        string[] TempItems = new string[LenghtItem+2];
        string table = html.Substring("<tbody>", "</tbody>");
        //int k = -1;
        for (int i = 0; i < TempItems.Length-2; i++)
        {
            string valueStr = table.Substring(";\">", "</td>");
            string valueNum = table.Substring("<td>", "</td>");
            if (Char.IsDigit(valueStr[0]) || valueStr == "Абонплата")
            { table = table.Remove(0, table.IndexOf("</tr>") + 5); i--; continue; }
            //if (valueStr.IndexOf("i") == -1)
            //   {
            //      k++;
            TempItems[i] = valueStr + ": " + valueNum + "₽";
            table = table.Remove(0, table.IndexOf("</tr>") + 5);
            // }

        }
        return TempItems;
    }//Считываем с html нужные нам значения
    private void AddLabelOnGrid(Panel panel,byte count)
    {
        ushort Height = 3;
        Label lb = new Label();
        #region Blur на лбл 1
        System.Windows.Media.Effects.DropShadowEffect effect = new System.Windows.Media.Effects.DropShadowEffect();
        effect.BlurRadius = 35;
        effect.Direction = 160;
        effect.Opacity = 100;
        effect.RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance;
        effect.ShadowDepth = 0;
        #endregion
        for (int i = 1; i <= count+1; i++)
        {
            #region статические настройки label
            lb = new Label();
            lb.Foreground = new SolidColorBrush(Color.FromRgb(160, 160, 160));
            lb.FontWeight = FontWeights.Bold;
            lb.FontSize = 16;
            lb.FontFamily = new FontFamily("Tahoma");
            #endregion
            lb.Margin = new Thickness(10, Height++,0, 0);
            lb.Content = Items[i-1];
            panel.Children.Add(lb);
        }
        //
        #region до такогото числа опалачено
        #region статические настройки label
        lb = new Label();
        lb.FontWeight = FontWeights.Bold;
        lb.FontSize = 16;
        lb.FontFamily = new FontFamily("Tahoma");
        #endregion
        if(Items[Items.Length-1] == "Сегодня после 10:00 у вас отключат инернет")
            effect.Color = Colors.Red;
        else
            effect.Color = Colors.CornflowerBlue;
        lb.Effect = effect;
        lb.Foreground = new SolidColorBrush(Color.FromRgb(160, 160, 160));
        lb.Margin = new Thickness(10, Height++, 0, 0);
        lb.Content = Items[Items.Length - 1];
        panel.Children.Add(lb);
        #endregion
    }//Добавление на форму меток
    public bool AuthDoing
    {
        get { return BoolAuth; }
    }//Авторизованны ли сейчас?
    //-------------------------------
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

    }
    private void CalculateSubscriptionFee(string Subscriptionfee, string OutDay)//Вычесляем до какого нам хватит денег
    {
        string lblInfo;
        string lbloutmonth;
        DateTime day = DateTime.Now;
        double TempOutDay = Convert.ToDouble(OutDay.Replace(".",","));
        double TempSubscriptionfee = Convert.ToDouble(Subscriptionfee.Replace(".", ","));
        if (TempSubscriptionfee != 0)
        {
            double TempDayOfMonth = DateTime.DaysInMonth(day.Year, day.Month);
            double Templbloutmonth = 0.0f;
            double paymantDay;
            if (day.Hour < 10)
                paymantDay = TempSubscriptionfee / (day.Day - 1);
            else
                paymantDay = TempSubscriptionfee / day.Day;
            double paymantMonth = paymantDay * TempDayOfMonth;//узнаем сколько конкретно за этот месяц должны мы
            int HowMuchIsEnough = ((int)(TempOutDay / paymantDay));
            DateTime dayOfpaymant;
            if (day.Hour<10)
                dayOfpaymant = AddDate(day, HowMuchIsEnough);
            else
                dayOfpaymant = AddDate(day, HowMuchIsEnough+1);

            if (dayOfpaymant.Day != day.Day)
                lblInfo = "До " + dayOfpaymant.ToLongDateString().ToString() + " 10:00 Оплачено!";
            else
                lblInfo = "Сегодня после 10:00 у вас отключат инернет";

            Templbloutmonth = (TempOutDay - ((TempDayOfMonth - day.Day) * paymantDay));

            if (Templbloutmonth > 0.0f)
                lbloutmonth = String.Format("На 1e {0} у вас останется:{1:0.00}₽", DateTime.Now.AddMonths(1).ToLongDateString().Remove(0, 2), Templbloutmonth);
            else
                lbloutmonth = String.Format("На 1e {0} у вас останется:0 ₽", DateTime.Now.AddMonths(1).ToLongDateString().Remove(0, 2));

        }
        else
        {
            lblInfo= "У вас \"Наработка за месяц\" равна 0,программа не может подсчитать";
            lbloutmonth = String.Format("На 1e {0} у вас останется: {1}₽", DateTime.Now.AddMonths(1).ToLongDateString().Remove(0, 2), OutDay);
        }
        Items[Items.Length - 2] = lbloutmonth;
        Items[Items.Length - 1] = lblInfo;
    }
    private DateTime AddDate(DateTime Date, int DayAdd)
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
}