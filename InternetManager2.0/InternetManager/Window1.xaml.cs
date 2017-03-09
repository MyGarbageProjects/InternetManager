using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using Microsoft.Win32;

namespace IMWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        INI ini = new INI(AppDomain.CurrentDomain.BaseDirectory.ToString() + "settings.ini");
        VusualControl vscl = new VusualControl();
        public Window1()
        {
            InitializeComponent();
            GetAllColor();
            vscl.AddRectangle(GridSettings);//добавляем линию по контуру
            LoadSettingsFromINI();
        }
        private void GetAllColor()//Получение всех цветов их пространства System.Window.Media.Color
        {
            Type t = typeof(System.Windows.Media.Colors);
            PropertyInfo[] P = t.GetProperties();
            ComboBoxItem Tempitem;
            for (int i = 0; i < 141; i++)
            {
                Tempitem = new ComboBoxItem();
                Tempitem.Content = ColorConverter.ConvertFromString(P[i].Name).ToString();
                Tempitem.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(P[i].Name));
                Tempitem.Background = Tempitem.Foreground;
                Tempitem.BorderBrush = Tempitem.Foreground;
                cmbBoxColorControl.Items.Add(Tempitem);
            }
            // cmbBoxColorControl.Items.Add(P[i].Name);

            Console.WriteLine();
        }
        #region Кнопка управления
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
            MainWindow MW = this.Owner as MainWindow;
            if (MW != null)
                MW.LoadSettingsFromINI();
            this.Close();
        }
        #endregion
        #region Перемещение формы за головку
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void TopACap_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ReleaseCapture();
            SendMessage(new System.Windows.Interop.WindowInteropHelper(this).Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        #endregion
        #region Работа с настройками
        private void btnSaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            //
            //Visual
            ini.IniWriteValue("Visual", "ColorTop", ColorConverter.ConvertFromString(RectTopACap.Fill.ToString()).ToString());//запись цвета шапки settings.ini
            ini.IniWriteValue("Visual", "ColorRectangle", ColorConverter.ConvertFromString(vscl.SetColorRectangle.ToString()).ToString());//запись цвета ободка settings.ini
            ini.IniWriteValue("Visual", "BoolRemoveRectangle", chckBxRmRect.IsChecked.Value.ToString());//запись бул значения удаления ободка settings.ini
            ini.IniWriteValue("Visual", "BoolAnimationRectangle", chckBxAnimRect.IsChecked.Value.ToString());//запись бул значения анимирование ободка settings.ini
            //
            //Auth
            if (txtLogin.Text != "" && txtPass.Password != "" && cmbBoxCompany.SelectedIndex != -1)
            {
                ini.IniWriteValue("Auth", "Login", txtLogin.Text);//запись логина в settings.ini
                ini.IniWriteValue("Auth", "Pass", txtPass.Password);//запись пароль в settings.ini
                ini.IniWriteValue("Auth", "ServiceProvider", cmbBoxCompany.Text);//запись название компании в settings.ini
                lbl5.Content = "Настройки успешно сохранены.Вернитесь к главному меню.";
            }
            else
            {
                lbl5.Content = "Вы пропустили одно из полей в \"Авторизация\"";
            }
            //
            //AutoStart
            ini.IniWriteValue("Notification", "Theme", cmbBoxThemeNotif.Text);
            ini.IniWriteValue("Notification", "Style", cmbBoxStyleNotif.Text);
            ini.IniWriteValue("Notification", "ColorText", cmbBoxNotifTxtCl.Text);
            ini.IniWriteValue("Notification", "LifeSecond", txtBoxLifeSocondForm.Text);
            ini.IniWriteValue("Notification", "Opacity", SliderOpacity.Value.ToString());
            ini.IniWriteValue("Startup", "Do", chckBxAutoStart.IsChecked.Value.ToString());
            if (chckBxAutoStart.IsChecked.Value)
                AddInRegister();
            //
            //
            lbl5.Visibility = Visibility.Visible;
            //
            //
        }//Сохраняем настройки
        void LoadSettingsFromINI()
        {
            //
            //
            //image PasswordBox
            imgPassBox.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images\\Show.png"));
            //
            //
            #region Авторизация
            if (ini.IniReadValue("Auth", "Login") != "")
            {
                txtLogin.Text = ini.IniReadValue("Auth", "Login");//Загрузка логина в текст бокс
                txtPass.Password = ini.IniReadValue("Auth", "Pass");//Загрузка логина в текст бокс
                cmbBoxCompany.SelectedIndex = SearchIndex(cmbBoxCompany, ini.IniReadValue("Auth", "ServiceProvider"));//Загрузка инернет компании
                                                                                                                      //Authorization auth = new Authorization(GridMain, LabelsContainer);
                                                                                                                      //auth.Auth(Company, Login, Password);
                                                                                                                      // lbl0.Visibility = Visibility.Hidden;
                                                                                                                      // image.Visibility = Visibility.Hidden;
            }
            else
            {
                //добовляю на форму лабел с информацией что нужно бы ввести пароль
                //lbl0.Visibility = Visibility.Visible;
                //image.Visibility = Visibility.Visible;

            }
            #endregion
            ///
            #region Цвета элементов
            chckBxRmRect.IsChecked = BoolReadINI("Visual", "BoolRemoveRectangle");
            if (!chckBxRmRect.IsChecked.Value)
            {
                chckBxAnimRect.IsChecked = BoolReadINI("Visual", "BoolAnimationRectangle");
                #region Запуск анимации
                if (chckBxAnimRect.IsChecked.Value)
                {

                    chckBxRmRect.IsEnabled = false;
                    System.Windows.Shapes.Rectangle refRectangle = vscl.rect;
                    vscl.AddAnimationControl(GridSettings, refRectangle);
                    vscl.Start();
                    cmbBoxColorControl.SelectedIndex = -1;
                    cmbBoxColorControl.IsEnabled = false;
                }
                #endregion
                if (!chckBxAnimRect.IsChecked.Value)
                {
                    vscl.SetColorRectangle = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ini.IniReadValue("Visual", "ColorRectangle")));//цвет линии по кругу
                    RectTopACap.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ini.IniReadValue("Visual", "ColorTop")));//шапка
                }

            }
            #endregion
            ///
            #region Автозагрузка
            chckBxAutoStart.IsChecked = BoolReadINI("Startup", "Do");
            cmbBoxThemeNotif.SelectedIndex = SearchIndex(cmbBoxThemeNotif, ini.IniReadValue("Notification", "Theme"));//комбобокс темы
            cmbBoxStyleNotif.SelectedIndex = SearchIndex(cmbBoxStyleNotif, ini.IniReadValue("Notification", "Style"));//комбобокс стиль
            cmbBoxNotifTxtCl.SelectedIndex = SearchIndex(cmbBoxNotifTxtCl, ini.IniReadValue("Notification", "ColorText"));//комбобокс цвет текста
            txtBoxLifeSocondForm.Text = ini.IniReadValue("Notification", "LifeSecond");//текстбокс время жизни окна
            SliderOpacity.Value = float.Parse(ini.IniReadValue("Notification", "Opacity"));//прозрачность окна
            if (chckBxAutoStart.IsChecked.Value)
                AddInRegister();
            else
                groupBoxAutoStart.IsEnabled = false;
            #endregion
            //
            //
        }//Подгружаем настройки
        bool BoolReadINI(string Selection, string Key)
        {
            return Convert.ToBoolean(ini.IniReadValue(Selection, Key));
        }//возврат бул значения из ini файла
        #endregion
        #region Работа с сранными комбобоксами   
        private void cmbBoxControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string tmp = "System.Windows.Controls.ComboBoxItem: #FFFFFFFF";
            if (cmbBoxControl.SelectedIndex == -1)
            {
                cmbBoxColorControl.IsEnabled = false;
            }
            else
            {
                cmbBoxColorControl.IsEnabled = true;
            }
            //
            //
            //
            if (cmbBoxControl.SelectedIndex == 0)//Цвет шапки
            {
                chckBxRmRect.IsEnabled = false;
                chckBxAnimRect.IsEnabled = false;
                cmbBoxColorControl.IsEnabled = true;
                // cmbBoxColorControl.SelectedIndex = cmbBoxColorControl.Items.IndexOf(ini.IniReadValue("Visual","ColorTop"));
                cmbBoxColorControl.SelectedIndex = SearchIndex(cmbBoxColorControl, ini.IniReadValue("Visual", "ColorTop"));
            }
            else if (cmbBoxControl.SelectedIndex == 1)//Цвет и т.дю  ректангл
            {
                chckBxRmRect.IsEnabled = true;
                chckBxAnimRect.IsEnabled = true;

                if (chckBxAnimRect.IsChecked.Value)
                {
                    cmbBoxColorControl.IsEnabled = false;
                    cmbBoxColorControl.SelectedIndex = -1;
                }

                if (chckBxRmRect.IsChecked.Value)
                {
                    cmbBoxColorControl.SelectedIndex = -1;
                    cmbBoxColorControl.IsEnabled = false;
                    vscl.SetColorRectangle = new SolidColorBrush(Colors.Transparent);
                }
                else if (!chckBxAnimRect.IsChecked.Value)
                {
                    chckBxAnimRect.IsEnabled = true;
                    //cmbBoxColorControl.SelectedIndex = cmbBoxColorControl.Items.IndexOf(ini.IniReadValue("Visual", "ColorRectangle"));
                    cmbBoxColorControl.SelectedIndex = SearchIndex(cmbBoxColorControl, ini.IniReadValue("Visual", "ColorRectangle"));
                }
                //chbkboxAnumation.Visibility = Visibility.Hidden;
            }
        }
        private void cmbBoxColorControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBoxColorControl.SelectedIndex != -1)
            {
                string tmpColor = (cmbBoxColorControl.Items[cmbBoxColorControl.SelectedIndex] as ComboBoxItem).Content.ToString();
                if (cmbBoxControl.SelectedIndex == 0)//цвет шапки
                    RectTopACap.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(tmpColor));
                else if (cmbBoxControl.SelectedIndex == 1)//цвет линий по контуру
                {
                    if (!chckBxRmRect.IsChecked.Value && cmbBoxColorControl.SelectedIndex != -1)
                        vscl.SetColorRectangle = new SolidColorBrush((Color)ColorConverter.ConvertFromString(tmpColor));
                }
            }
        }
        int SearchIndex(ComboBox cmbox, string colorSelected)
        {
            string tmp;
            for (int i = 0; i < 141; i++)
            {
                tmp = (cmbox.Items[i] as ComboBoxItem).Content.ToString();
                if (tmp == colorSelected)
                    return i;

            }
            return -1;
        }
        //
        //
        private void chckBxAnimRect_Click(object sender, RoutedEventArgs e)
        {
            if (chckBxAnimRect.IsChecked.Value)//Если флажок анимации влючен
            {
                cmbBoxColorControl.SelectedIndex = -1;//отключает комбобокс
                cmbBoxColorControl.IsEnabled = false;//отключает комбобокс
                chckBxRmRect.IsEnabled = false;//отключаем чекбокс удаления линии по контуру
                //
                //
                System.Windows.Shapes.Rectangle refRectangle = vscl.rect;//создаем ссылку на прямоугольник
                vscl.AddAnimationControl(GridSettings, refRectangle);//настройка анимации
                vscl.Start();//Запуск анимации
            }
            else//когда мы отключаем флажек анимации
            {
                vscl.AnimationStop = true;//выключаем анимацию
                chckBxRmRect.IsEnabled = true;//включаем чекбокс у удалением ободка
                cmbBoxColorControl.IsEnabled = true;//включаем комбобокс с цветами
                cmbBoxColorControl.SelectedIndex = SearchIndex(cmbBoxColorControl, ini.IniReadValue("Visual", "ColorRectangle"));//устанавливаем предыдущий цвет
            }
        }
        private void chckBxRmRect_Click(object sender, RoutedEventArgs e)
        {
            if (chckBxRmRect.IsChecked.Value)//ecли чекбоск удалить ободок включен
            {
                cmbBoxColorControl.SelectedIndex = -1;//отключает комбобокс
                cmbBoxColorControl.IsEnabled = false;//отключает комбобокс
                chckBxAnimRect.IsEnabled = false;//отключаем чекбокс анимации ободка
                //
                //
                vscl.SetColorRectangle = Brushes.Transparent;//устанавливаем невидивый цвет уот так уот
            }
            else//ecли чекбоск удалить ободок выключен
            {
                chckBxAnimRect.IsEnabled = true;//включаем чекбокс у удалением ободка
                cmbBoxColorControl.IsEnabled = true;//включаем комбобокс с цветами
                cmbBoxColorControl.SelectedIndex = SearchIndex(cmbBoxColorControl, ini.IniReadValue("Visual", "ColorRectangle"));//устанавливаем предыдущий цвет
            }
        }
        #endregion
        #region Автозагрузка
        private void chckBxAutoStart_Click(object sender, RoutedEventArgs e)
        {
            #region настройки Блюра
            System.Windows.Media.Effects.DropShadowEffect effect = new System.Windows.Media.Effects.DropShadowEffect();
            effect.Direction = 180;
            effect.Opacity = 100;
            effect.RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance;
            effect.ShadowDepth = 0;
            effect.Color = Colors.BlanchedAlmond;
            #endregion

            if (chckBxAutoStart.IsChecked.Value)
            {
                effect.BlurRadius = 10;
                groupBoxAutoStart.IsEnabled = true;
            }
            else
            {
                effect.BlurRadius = 0;
                groupBoxAutoStart.IsEnabled = false;
            }
            btnShowNotif.Effect = effect;
        }
        private void btnShowNotif_Click(object sender, RoutedEventArgs e)
        {
            Notification notif = new Notification();
            string example = "Пример:\r\nНа сегодняшний день у вас 9999,00₽\r\nДенег еще хватит на 999 дней";
            //string tmp = (cmbox.Items[i] as ComboBoxItem).Content.ToString();
            #region создаем фомру Дохера
            notif.CreateNotification(System.Drawing.ColorTranslator.FromHtml((cmbBoxNotifTxtCl.Items[cmbBoxNotifTxtCl.SelectedIndex] as ComboBoxItem).Content.ToString()),
                    SliderOpacity.Value,
                      MetroTheme((cmbBoxThemeNotif.Items[cmbBoxThemeNotif.SelectedIndex] as ComboBoxItem).Content.ToString()),
                       MetroStyle((cmbBoxStyleNotif.Items[cmbBoxStyleNotif.SelectedIndex] as ComboBoxItem).Content.ToString()));
            #endregion
            notif.ShowNotification(example, true, int.Parse(txtBoxLifeSocondForm.Text), true);
            //System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "IMNotification.exe");
        }
        private MetroFramework.MetroThemeStyle MetroTheme(string strTheme)
        {
            switch (strTheme)
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
        private MetroFramework.MetroColorStyle MetroStyle(string strStyle)
        {
            switch (strStyle)
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

        private void AddInRegister()
        {
            try
            {
                using (RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (chckBxAutoStart.IsChecked.Value)
                    {
                        reg.SetValue("IMWPF", AppDomain.CurrentDomain.BaseDirectory + "IMNotification.exe");
                        //ini.IniWriteValue("Startup", "Notification", StartUpping.Checked.ToString());
                        groupBoxAutoStart.IsEnabled = true;
                    }
                    else
                    {
                        reg.DeleteValue("KadiBatya");
                        //ini.IniWriteValue("Startup", "Notification", StartUpping.Checked.ToString());
                        groupBoxAutoStart.IsEnabled = false;
                    }
                }
            }
            catch
            {

                //ini.IniWriteValue("Startup", "Notification", StartUpping.Checked.ToString());
                groupBoxAutoStart.IsEnabled = false;
            }
        }//------------------
        #endregion
        #region PasswordBox Show and Hide
        private void txtPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtPass.Password.Length > 0)
                imgPassBox.Visibility = Visibility.Visible;
            else
                imgPassBox.Visibility = Visibility.Hidden;
        }
        private void HidePassword()
        {
            imgPassBox.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images\\Show.png"));
            txtShowPass.Visibility = Visibility.Hidden;
            txtPass.Visibility = Visibility.Visible;
            txtPass.Focus();
        }
        private void ShowPassword()
        {
            imgPassBox.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images\\Hide.png"));
            txtShowPass.Visibility = Visibility.Visible;
            txtPass.Visibility = Visibility.Hidden;
            txtShowPass.Text = txtPass.Password;
        }
        private void imgPassBox_MouseLeave(object sender, MouseEventArgs e)
        {
            HidePassword();
        }
        private void imgPassBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            HidePassword();
        }
        private void imgPassBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowPassword();
        }
        #endregion
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
