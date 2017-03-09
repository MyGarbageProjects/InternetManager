using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using IWshRuntimeLibrary;

namespace IMInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string DefaultPath;
        public MainWindow()
        {
            InitializeComponent();

            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion", true))
            {
                textBox.Text = (string)regKey.GetValue("ProgramFilesDir")+"\\Internet Manager\\";
            }
            DefaultPath = textBox.Text;
        }
        #region Перемещение формы
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        // ReSharper disable once InconsistentNaming
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ReleaseCapture();
            SendMessage(new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        #endregion
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }//закрыть приложение
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Выберите папку для установки IM";
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBox.Text = folderBrowserDialog.SelectedPath + "\\";
                }
            }
        }//выбор папки установки
        private void button_Click(object sender, RoutedEventArgs e)//извлечь файлы
        {
            //Выключаем кнопки
            btnExit.IsEnabled = false;
            btnSelectPath.IsEnabled = false;
            btnInstall.IsEnabled = false;
            chckBoxCreatShortcut.IsEnabled = false;
            chckBoxStartToEnd.IsEnabled = false;
            //
            #region Извлекаем
            try
            {
                if (textBox.Text == DefaultPath)
                    Directory.CreateDirectory(DefaultPath);
                lblPersent.Visibility = Visibility.Visible;
                ///
                string[] resource = getNameRes();
                for (int i = 0; i < 6; i++)
                {
                    ExtractFiles(textBox.Text, "ExtFl", resource[i]);
                }
                lblPersent.Content = "50%";
                Directory.CreateDirectory(textBox.Text + "Images");

                for (int i = 6; i < 10; i++)
                {
                    ExtractFiles(textBox.Text + "Images\\", "ExtFl.Images", resource[i]);
                }
                lblPersent.Content = "90%";

                System.IO.File.Move(textBox.Text + "ResImg", textBox.Text + "ResImg.resx");
                lblPersent.Content = "100%";
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
            #endregion
            //включаем кнопки
            btnExit.IsEnabled = true;
            btnSelectPath.IsEnabled = true;
            btnInstall.IsEnabled = true;
            chckBoxCreatShortcut.IsEnabled = true;
            chckBoxStartToEnd.IsEnabled = true;

            //создаем ярлык
            if (chckBoxCreatShortcut.IsChecked.Value)
            {
                WshShell shell = new WshShell();
                string shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Internet Manager.lnk";//путь к ярлыку
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);//создаем объект ярлыка
                shortcut.Description = "Ярлык Internet Manager by whale in the sky";//описание ярлыка в всплывающей подсказке
                shortcut.TargetPath = textBox.Text + "IMWPF.exe";
                shortcut.Save();
            }
            //запуск программы
            if (chckBoxStartToEnd.IsChecked.Value)
                System.Diagnostics.Process.Start(textBox.Text + "IMWPF.exe");
        }
        private static void ExtractFiles(string outFile,string internalFilePath,string resourceName)
        {
            string nameSpace = "IMInstaller";
            Assembly assembly = Assembly.GetCallingAssembly();
            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
                using (BinaryReader r = new BinaryReader(s))
                    using (FileStream fs = new FileStream(outFile+"\\"+resourceName,FileMode.OpenOrCreate))
                        using (BinaryWriter w = new BinaryWriter(fs))
                            w.Write(r.ReadBytes((int)s.Length));
        }
        private static string[] getNameRes()
        {
            string[] tempResName = new string[10];
            tempResName[0] = "IMNotification.exe";
            tempResName[1] = "IMWPF.exe";
            tempResName[2] = "MetroFramework.dll";
            tempResName[3] = "settings.ini";
            tempResName[4] = "xNet.dll";
            tempResName[5] = "ResImg";
            tempResName[6] = "Hide.png";
            tempResName[7] = "IcoInfo64x64.ico";
            tempResName[8] = "IcoWarning64x64.ico";
            tempResName[9] = "Show.png";
            return tempResName;
        }
    }
}
