using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication4
{
    /// <summary>
    /// Логика взаимодействия для Player.xaml
    /// </summary>
    public partial class Player : Page
    {
        Stuff stuff;
        public Player()
        {
            InitializeComponent();
            stuff = new Stuff();
            stuff.Draw(StuffCanvas);
        }
    }


    public class Stuff{
        public enum StuffType { stBass,stTreble,stBoth};
        protected Canvas cnv;
        protected StuffType stuffType;

        public Stuff() {
            stuffType = StuffType.stBoth;
        }

        protected void DrawLines() {
            if (stuffType == StuffType.stBoth) {
                var br = new SolidColorBrush();
                br.Color = Color.FromRgb(0,0,0);
                for (int i = 0; i < 5; i++) {
                    Line ln = new Line();
                    ln.X1 = 10;
                    ln.Y1 = 10*i;
                    ln.X2 = 100;
                    ln.Y2 = 10 * i;
                    ln.Stroke = br;

                    cnv.Children.Add(ln);
                }
                
            }
        }
        protected void DrawBars() { }
        protected void DrawNotes() { }
        protected void DrawSign() { }


        public void Draw(Canvas _cnv) {
            cnv = _cnv;
            DrawLines();
            DrawBars();
            DrawNotes();
            DrawSign();
        }

    }

}
