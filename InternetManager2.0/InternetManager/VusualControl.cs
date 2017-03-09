using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Shapes;

namespace IMWPF
{
    class VusualControl
    {
        public Rectangle rect = new System.Windows.Shapes.Rectangle();
        Grid grid;
        Task thread;
        //Button btn;
        bool Animation;
        System.Windows.Media.Effects.DropShadowEffect effect;
        public void AddRectangle(Grid grid)
        {
            //Rectangle Stroke="Teal" StrokeThickness="1" HorizontalAlignment="Left" Height="371" Margin="0,-2,0,0" VerticalAlignment="Top" Width="600"            
            rect.Stroke = new SolidColorBrush(Colors.Transparent);
            rect.StrokeThickness = 1;
            rect.HorizontalAlignment = HorizontalAlignment.Left;
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.Height = 370;
            rect.Width = 600;
            rect.Margin = new Thickness(0, -3, 0, 0);
            grid.Children.Add(rect);
        }
        public Brush SetColorRectangle
        {
            get { return rect.Stroke; }
            set { rect.Stroke = value; }
        }
        //==================================================
        public void Start()
        {
            thread = new Task(StartAnimation);
            thread.Start();
            Animation = false;
        }//Запуск нового потока где будет происходить анимация
        private void StartAnimation()
        {
            while (!Animation)
            {
                for (float i = 0; i < 1.0f; i += 0.01f)
                {
                    if (Animation)
                        break;
                    grid.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        // if (AnimationButton)
                        // { effect.Color = Rainbow(i); btn.Effect = effect;}
                        if (!Animation)
                         rect.Stroke = new SolidColorBrush(Rainbow(i));
                    });
                    Thread.Sleep(75);
                }
            }
        }//Непосредственно сама анимация
        //---
        //public bool SetAnimationRectangle
        //{
        //    set { AnimationButton = value; }
        //}
        //-----
        public void AddAnimationControl(Grid gr,Rectangle rectangle)
        {
            //btn = button;
            rectangle = rect;
            //lb = label;
            grid = gr;
            //
            effect = new System.Windows.Media.Effects.DropShadowEffect();
            effect.BlurRadius = 10;
            effect.Direction = 180;
            effect.Opacity = 100;
            effect.RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance;
            effect.ShadowDepth = 0;
        }//Подготовка значений преде присваиванием
        private static Color Rainbow(float progress)
        {
            var div = (Math.Abs(progress % 1) * 6);
            var ascending = (int)((div % 1) * 255);
            var descending = 255 - ascending;

            switch ((int)div)
            {
                case 0:
                    return Color.FromArgb(255, 255, (byte)ascending, 0);//255, 255, ascending, 0
                case 1:
                    return Color.FromArgb(255, (byte)descending, 255, 0);//255, descending, 255, 0
                case 2:
                    return Color.FromArgb(255, 0, 255, (byte)ascending);//255, 0, 255, ascending 
                case 3:
                    return Color.FromArgb(255, 0, (byte)descending, 255);//255, 0, descending, 255 
                case 4:
                    return Color.FromArgb(255, (byte)ascending, 0, 255);//255, ascending, 0, 255
                default: // case 5:
                    return Color.FromArgb(255, 255, 0, (byte)descending);//255, 255, 0, descending
            }
        }//Подсчет цвета
        public bool AnimationStop
        {
            set { Animation = value; }
        }//Свойство для остановки потока анимации
        //
        //
    }
}
