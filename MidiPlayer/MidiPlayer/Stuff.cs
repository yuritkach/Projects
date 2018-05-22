using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MidiPlayer
{
    public class Note:INotifyPropertyChanged {
        protected int _value;
        public int Value {
            get { return this._value; }
            set { this._value = value; }
        }

        protected int _offset;
        public int Offset {
            get { return this._offset; }
            set { this._offset = value;}
        }

        protected int _length;
        public int Length {
            get { return this._length; }
            set { this._length = value;}
        }

        public Note(int AValue,int AOffset,int ALength) {
            _value = AValue;
            _offset = AOffset;
            _length = ALength;
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
    }



    public class Stuff
    {
        public enum StuffType { stBass, stTreble, stBoth };
        protected Canvas cnv;
        protected StuffType stuffType;
     

        public Stuff()
        {
            stuffType = StuffType.stBoth;
             }

            protected void DrawBars() { }
        protected void DrawNotes() {
            Ellipse aaa = new Ellipse();
            aaa.Height = 30;
            aaa.Width = 30;
            
            cnv.Children.Add(aaa);
        }
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
         
        }


    }
}
