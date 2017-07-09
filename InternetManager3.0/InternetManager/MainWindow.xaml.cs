using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using ISP;
using System.Windows.Controls;
using System.Collections.Generic;

namespace IMWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Сообщения об ошибках
        const string ERROR_NOT_ENTERED_LOGIN_PASS = "Пожалуйста войтиде в настройки             и авторизуйтесь";
        const string ERROR_NOT_CONNECTION = "Отсутствует интернет подключение,либо сайт компании\r\n не работет";
        const string ERROR_INVALID_LOGIN_PASS = "Неверно введен логин или пароль,не могу авторизоваться";
        #endregion

        INI ini = new INI(AppDomain.CurrentDomain.BaseDirectory.ToString() + "settings.ini");
        System.Windows.Media.Effects.DropShadowEffect effect;
        VusualControl vscl = new VusualControl();
        InternetServiceProvider provider;
        string Login, Password;
        InternetCompany Company;
        private List<string> fields;
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
            provider = new InternetServiceProvider();
            //internetCompany = new Authorization(ref LabelsContainer);
            provider.CheckInternetConnection();
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
            imgClose.Source = new BitmapImage(new Uri("Resources/CloseActive.png", UriKind.Relative));
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
            imgSettings.Source = new BitmapImage(new Uri("Resources/SettingsActive.png", UriKind.Relative));
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
            LoadSettingsFromINI();
        }
        public void LoadSettingsFromINI()
        {
            #region Авторизация
            if (ini.IniReadValue("Auth", "Login") != "")
            {
                Login = ini.IniReadValue("Auth", "Login");//Получаем логин
                Password = ini.IniReadValue("Auth", "Pass");//Получаем пароль
                Enum.TryParse(ini.IniReadValue("Auth", "ServiceProvider"), out Company);//Получаем иcпользуемую интернет компанию
                SingIn();
            }
            else
            {
                lblError.Content = ERROR_NOT_ENTERED_LOGIN_PASS;
                imgNotAuth.Visibility = Visibility.Visible;
            }

            #endregion
            #region Цвета элементов
            RectTopACap.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ini.IniReadValue("Visual", "ColorTop")));//Получаем цвет шапки
            #region Цвет линий по контуру
            if (!(Convert.ToBoolean(ini.IniReadValue("Visual", "BoolRemoveRectangle"))))//если "Удалить линию по котуру" не включено то проверяем
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
        }
        private void SingIn()
        {
            if (provider.Connected)//Проверяем есть ли интернет подключение?Если есть 
            {
                provider.Authorization(Company, Login, Password, ref fields);//Авторизуемся
                if (provider.Authorized)//Если авторизация удачна,выводим инфу
                    AddLabelOnGrid(LabelsContainer);
                else//Иначе выводим сообщение об ошибке
                    lblError.Content = ERROR_INVALID_LOGIN_PASS;
            }
            else
                lblError.Content = ERROR_NOT_CONNECTION;
        }


        private void AddLabelOnGrid(Panel panel)
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
            for (int i = 1; i <= fields.Count - 1; i++)
            {
                #region статические настройки label
                lb = new Label();
                lb.Foreground = new SolidColorBrush(Color.FromRgb(160, 160, 160));
                lb.FontWeight = FontWeights.Bold;
                lb.FontSize = 16;
                lb.FontFamily = new FontFamily("Tahoma");
                #endregion
                lb.Margin = new Thickness(10, Height++, 0, 0);
                lb.Content = fields[i - 1];
                panel.Children.Add(lb);
            }
            Console.WriteLine();
            #region до такогото числа опалачено
            #region статические настройки label
            lb = new Label();
            lb.FontWeight = FontWeights.Bold;
            lb.FontSize = 16;
            lb.FontFamily = new FontFamily("Tahoma");
            #endregion
            if (fields[fields.Count - 1] == "Сегодня после 10:00 у вас отключат инернет")
                effect.Color = Colors.Red;
            else
                effect.Color = Colors.CornflowerBlue;
            lb.Effect = effect;
            lb.Foreground = new SolidColorBrush(Color.FromRgb(160, 160, 160));
            lb.Margin = new Thickness(10, Height++, 0, 0);
            lb.Content = fields[fields.Count - 1];
            panel.Children.Add(lb);
            #endregion
        }//Добавление на форму меток
    }
}
