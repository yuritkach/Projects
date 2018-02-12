using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MidiPlayer
{
    public class Stuff
    {
        public enum StuffType { stBass, stTreble, stBoth };
        protected Canvas cnv;
        protected StuffType stuffType;
        protected double interLineOffset = 21; // Смещение между строчками стана
        protected SolidColorBrush br;

        public Stuff()
        {
            stuffType = StuffType.stBoth;
            br = new SolidColorBrush();
            br.Color = Color.FromRgb(0, 0, 0);

        }

        protected void MakeLine(double X1,double Y,double X2) {
            

            Line ln = new Line();
            ln.X1 = X1;
            ln.Y1 = Y;
            ln.X2 = X2;
            ln.Y2 = ln.Y1;
            ln.Stroke = br;
            ln.StrokeThickness = 2;
            ln.SnapsToDevicePixels = true;
            cnv.Children.Add(ln);
        }
        protected void DrawLines()
        {
            for (int i = cnv.Children.Count-1; i>=0; i--) {
                if (cnv.Children[i] is Line)
                    cnv.Children.RemoveAt(i);
            }

            double xOffset = cnv.ActualHeight / 21;
            if (stuffType == StuffType.stBoth)
            {

                for (int i = 0; i < 5; i++)
                {
                    MakeLine(10, 4 * xOffset + xOffset * i, cnv.ActualWidth-10);
                    MakeLine(10, 13 * xOffset + xOffset * i, cnv.ActualWidth - 10);

                }

            }
        }
        protected void DrawBars() { }
        protected void DrawNotes() { }
        protected void DrawSign() { }


        public void Draw(Canvas _cnv)
        {
            cnv = _cnv;
      //      DrawLines(); 
            DrawBars();
            DrawNotes();
            DrawSign();
        }

        public void SizeChanged() {
            // перерисовываем строчки
            DrawLines();

        }


    }
}
