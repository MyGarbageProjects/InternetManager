using System;
using MetroFramework.Forms;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace IMNotification
{
    public class Notification : TrayPos
    {
        private MetroForm frm = new MetroForm();
        //PictureBox picNotif = new PictureBox();
        private Label lbl = new Label();
        private Timer OpenFormTimer = new Timer();
        private Timer CloseFormTimer = new Timer();
        private Timer LifeTimeForm = new Timer();
        private TextBox txtMessage = new TextBox();
        private Stopwatch sWatch = new Stopwatch();
        private CheckBox chkBoxCloseForm = new CheckBox();
        ushort posStop = 0;
        private int SecondLifeForm = 0;
        public void CreateNotification(System.Drawing.Color ColorTxt, double Opacity, MetroFramework.MetroThemeStyle Theme, MetroFramework.MetroColorStyle style)
        {
            //
            //Form
            frm.Size = new System.Drawing.Size(375, 120);
            frm.ShowInTaskbar = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Theme = Theme;
            frm.Style = style;
            frm.Resizable = false;
            frm.MaximizeBox = false;
            frm.MinimizeBox = false;
            frm.TopMost = false;
            frm.Name = "frm";
            frm.Opacity = Opacity;
            //
            //
            //
            //Label
            //lbl.Location = new System.Drawing.Point(10, 40);//113 40
            //lbl.Font = new System.Drawing.Font("Tahoma", 100, System.Drawing.FontStyle.Bold);
            //lbl.AutoSize = true;
            //lbl.MaximumSize = new System.Drawing.Size(0, 0);
            //lbl.BackColor = frm.BackColor;
            //lbl.ForeColor = ColorTxt;
            //
            //
            //TextBox
            txtMessage.Location = new System.Drawing.Point(113, 40);
            txtMessage.Size = new System.Drawing.Size(260, 90);
            txtMessage.BackColor = frm.BackColor;
            txtMessage.Font = new System.Drawing.Font("Tahoma", 10);
            txtMessage.BorderStyle = BorderStyle.None;
            txtMessage.ReadOnly = true;
            txtMessage.ForeColor = ColorTxt;
            txtMessage.TabStop = false;
            txtMessage.Multiline = true;
            //txtMessage.Enabled = true;
            //
            //PictureBox
            PictureBox pic = new PictureBox();
            pic.BackColor = frm.BackColor;
            pic.SizeMode = PictureBoxSizeMode.StretchImage;
            pic.Location = new System.Drawing.Point(17, 22);
            pic.Size = new System.Drawing.Size(80, 80);
            Program P = new Program();
            int Days = P.HowManyDaysLeft;
            Image img;
            if (Days > 3)
                img = new Bitmap("Images/IcoInfo64x64.ico");
            else
                img = new Bitmap("Images/IcoWarning64x64.ico");
            pic.Image = img;

            //Добавляем элементы на форму
            frm.Controls.Add(txtMessage);
            frm.Controls.Add(pic);
        }
        private void chkBoxCloseForm_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBoxCloseForm.Checked)
            {
                sWatch.Stop();
                //sWatch.Reset();
                frm.Opacity = 1.0f;
                LifeTimeForm.Enabled = false;
            }
            else
            {
                sWatch.Start();
                CloseForm(true);
            }
        }
        public void ShowNotification(string Notification, bool ShowDialog, int LifeTimeFormSecond, bool TopMost)
        {
            //lbl.Text = Notification;
            txtMessage.Text = Notification;
            SecondLifeForm = LifeTimeFormSecond;

            //Позиция Оповещения
            int left;
            int top;
            //
            //
            getXY(380, 120, out top, out left);
            frm.StartPosition = FormStartPosition.Manual;
            frm.Location = new System.Drawing.Point(left + 375, top);
            frm.TopMost = TopMost;
            //
            //
            //
            posStop = (ushort)left;
            OpenFormTimer.Interval = 1;
            OpenFormTimer.Tick += new System.EventHandler(OpenFormTimer_Tick);
            OpenFormTimer.Enabled = true;
            //
            //
            //
            //Close Form
            LifeTimeForm.Interval = 1;
            LifeTimeForm.Tick += new System.EventHandler(LifeTimeForm_Tick);
            LifeTimeForm.Enabled = true;
            //
            if (ShowDialog)
                frm.ShowDialog();
            else
                frm.Show();
        }
        int k = 1; bool none1 = true, none2 = false;
        private void OpenFormTimer_Tick(object sender, EventArgs e)
        {
            k++;
            if ((frm.Left > posStop - 20) && none1)
            {
                frm.Left -= 15;
            }
            else if (!none2)
            { k = 0; none1 = false; none2 = true; }

            if ((frm.Left < posStop) && none2)
            {
                frm.Left += 15; none1 = false;
            }
            else if (!none1 && none2)
            {
                k = 0; sWatch.Start(); OpenFormTimer.Enabled = false;
            }
        }
        private void CloseFormTimer_Tick(object sender, EventArgs e)
        {
            frm.Opacity -= 0.01f;
            if (frm.Opacity < 0.01f)
                frm.Close();
        }
        private void LifeTimeForm_Tick(object senderd, EventArgs e)
        {
            if ((sWatch.ElapsedMilliseconds / 1000) > SecondLifeForm)
            {
                CloseForm(true);
                LifeTimeForm.Enabled = false;
            }
        }
        public void CloseForm(bool Animation)
        {
            if (Animation)
            {
                CloseFormTimer.Interval = 5;
                CloseFormTimer.Tick += new System.EventHandler(CloseFormTimer_Tick);
                CloseFormTimer.Enabled = true;
            }
            else
            {
                Application.Exit();
            }
        }
    }
    public abstract class TrayPos
    {
        protected tpos pos;
        private int top;
        private int left;

        int pw = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
        int ph = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
        int waw = (int)System.Windows.SystemParameters.WorkArea.Width;
        // int wah = (int)SystemParameters.WorkArea.Height;

        protected void getXY(int width, int height, out int _top, out int _left)
        {
            switch (pos)
            {
                case tpos.top:
                    left = (pw - width) - 5;
                    top = (int)System.Windows.SystemParameters.WorkArea.Top + 5;
                    break;
                case tpos.left:
                    left = pw - height;
                    top = 25;
                    break;
                case tpos.right:
                    left = waw - 10;
                    top = 25;
                    break;
                case tpos.bottom:
                default:
                    left = (pw - width) - 25;
                    top = 50;
                    break;
            }

            _left = left;
            _top = top;

        }
    }
    public enum tpos
    {
        top, left,
        bottom, right
    }
}
