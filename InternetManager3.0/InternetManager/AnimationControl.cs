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

namespace IMWPF
{
    class AnimationControl
    {
        Label lb;
        Button btn;
        Grid grid;
        Task thread;
        bool Animation;
        System.Windows.Media.Effects.DropShadowEffect effect;
        public void Start()
        {
            thread = new Task(StartAnimation);
            thread.Start();
        }
        public bool AnimationStop
        {
            get { return Animation; }
            set { Animation = value; }
        }
        private void StartAnimation()
        {
            while (Animation)
            {
                for (float i = 0; i < 1.0f; i += 0.01f)
                {
                    if (!AnimationStop)
                        break;
                    grid.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        effect.Color = Rainbow(i);
                        btn.Effect = effect;
                        lb.Content = i.ToString();
                    });
                    //System.Windows.Forms.Application.DoEvents();
                    Thread.Sleep(75);
                }
            }
        }
        public AnimationControl(Label label, Button button, Window w, Grid gr)
        {
            btn = button;
            lb = label;
            grid = gr;
            //
            effect = new System.Windows.Media.Effects.DropShadowEffect();
            effect.BlurRadius = 10;
            effect.Direction = 180;
            effect.Opacity = 100;
            effect.RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance;
            effect.ShadowDepth = 0;
        }
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
        }
    }
}
