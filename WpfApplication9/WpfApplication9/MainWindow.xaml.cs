using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Sanford.Multimedia.Midi;

namespace WpfApplication9
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public delegate void NoteOnEventHandler(ref Note _note);
    public delegate void NoteOffEventHandler(ref Note _note);

    public partial class MainWindow : Window
    {
        public OutputDevice outDevice;
        private int outDeviceID = 0;
        public Sequence seq;
        public Sequencer sq;

        DispatcherTimer dispatcherTimer;
        Stuff stuff;

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            stuff = new Stuff(ref cnv, 20);
            Random rnd = new Random();
            

            if (OutputDevice.DeviceCount == 0)
            {
                MessageBox.Show("No MIDI output devices available.");
            }
            else
            {
                outDevice = new OutputDevice(outDeviceID);
                seq = new Sequence("d:\\1.mid");
                seq.Format = 1;
                BuildVisualElements(seq);
            }

            stuff.NoteOnEvent += new NoteOnEventHandler(NoteOnHandler);
            stuff.NoteOffEvent += new NoteOffEventHandler(NoteOffHandler);

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            dispatcherTimer.Start();

            
        
        }

        int aaa=0;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            stuff.MakeTick();
            stuff.Show();
            dispatcherTimer.Start();

            // Forcing the CommandManager to raise the RequerySuggested event
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            
            

            
        }

        protected void BuildVisualElements(Sequence Aseq)
        {
            stuff.notes.Clear();
            int xchanel = 0;
            foreach (Track track in Aseq)
            {
                xchanel++;
                for (int i = 0; i < track.Count; i++)
                {
                    MidiEvent me = track.GetMidiEvent(i);
                    if (me.MidiMessage.MessageType == MessageType.Channel)
                    {
                        ChannelMessage cm = (ChannelMessage)me.MidiMessage;
                        if (cm.Command == ChannelCommand.NoteOn)
                        {
//                            VisualChanel vc = ve.chanels.SingleOrDefault(s => s.chanelNumber == cm.MidiChannel);
//                            if (vc == null)
//                            {
//                                vc = new VisualChanel();
//                                vc.chanelNumber = cm.MidiChannel;
//                                ve.chanels.Add(vc);
//                            }

                            Note vn = new Note(me.AbsoluteTicks,cm.Data1,1,true);
                            //vn.channel = xchanel; // неправильно
                            vn.startMidiMessage = me.MidiMessage;
                            stuff.notes.Add(vn);
                        }

                        if (cm.Command == ChannelCommand.NoteOff)
                        {
  //                          VisualChanel vc = ve.chanels.SingleOrDefault(s => s.chanelNumber == cm.MidiChannel);
   //                         if (vc == null)
   //                         {
    //                            vc = new VisualChanel();
     //                           vc.chanelNumber = cm.MidiChannel;
      //                          ve.chanels.Add(vc);
      //                      }


                            Note vn = stuff.notes.Where(v => v.Pitch == cm.Data1)
                                                    .OrderByDescending(v => v.Offset)
                                                    .First();
                            if (vn != null)
                            {
                                vn.Length = GetNoteLength(me.AbsoluteTicks - vn.Offset, Aseq.Division);
                                vn.stopMidiMessage = me.MidiMessage;

                            }
                        }

                    }
                }
            }



        }

        private int GetNoteLength(int ATicks, int ATicksPerQuarter)
        {

            return ATicks;
            //return (int)Math.Round((double)(ATicks / ATicksPerQuarter)) * 4;
        }


        protected void NoteOnHandler(ref Note _note) {
            if (_note.startMidiMessage!=null)
            outDevice.Send((ChannelMessage)_note.startMidiMessage);
        }

        protected void NoteOffHandler(ref Note _note)
        {
            if (_note.stopMidiMessage != null)
                outDevice.Send((ChannelMessage)_note.stopMidiMessage);
        }

        private void mainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }

    public enum NoteState{ntReady,ntPlaying,ntEnded}
    public class Note
    {
        public NoteState noteState;
        protected int pitch; // высота ноты
        protected int length; // длина ноты
        protected int offset; // смещение относительно начала
        protected bool forRightHand;

        public IMidiMessage startMidiMessage;
        public IMidiMessage stopMidiMessage;
        public int Pitch { get { return pitch; } }
        public int Length { get { return length; } set { length = value; } }
        public bool ForRightHand { get { return forRightHand; } }

        public int Offset { get { return offset; } set { offset = value; } }
        public Note(int _offset, int _pitch, int _length, bool _forRightHand)
        {
            offset = _offset;
            pitch = _pitch;
            length = _length;
            forRightHand = _forRightHand;
            noteState = NoteState.ntReady;
        }

        public Note(Note _note)
        {
            offset = _note.offset;
            pitch = _note.pitch;
            length = _note.length;
            noteState = _note.noteState;
            forRightHand = _note.forRightHand;
        }

    }
    public class Stuff
    {
        protected int step = 4;
        protected Canvas cnv;
        public List<Note> notes;
        public int Speed = 128;

        public int Length=1000; // условных тиков на экран
        protected int tickCounter;
        protected int realBorderLine;

        public event NoteOnEventHandler NoteOnEvent;
        public event NoteOffEventHandler NoteOffEvent;
        
        public int Position { get { return tickCounter; } }

        public Stuff(ref Canvas _cnv, int _borderLine) {
            cnv = _cnv;
            notes = new List<Note>();
            tickCounter = 0;
            realBorderLine =-Length+(_borderLine * Length / 100);
        }

        public void ShiftNotes()
        {
            for (int i=0; i<notes.Count;i++) notes[i].Offset=notes[i].Offset-Speed;
        }
        
        
        protected bool PlayNotes()
        {
            for (int i = 0; i < notes.Count; i++) {   // Проверка статуса ноты
                Note n = notes[i];
                if ((n.Offset/step < realBorderLine) && (n.noteState == NoteState.ntReady))
                {
                    n.noteState = NoteState.ntPlaying;
                    if (NoteOnEvent != null) NoteOnEvent(ref n);
                }
                else
                if (((n.Offset + n.Length)/step < realBorderLine) && (n.noteState != NoteState.ntEnded))
                {
                    n.noteState = NoteState.ntEnded;
                    if (NoteOffEvent != null) NoteOffEvent(ref n);

                }
            }
            return false; // если больше нечего играть...
        }

        public bool MakeTick()
        {
            ShiftNotes();
            tickCounter=tickCounter+512;
            return PlayNotes();
        }


        protected void DrawNoteTail(Note _note, ref double _xCoef, double _y)
        {
            Rectangle el = new Rectangle();

            double xLeft = cnv.ActualWidth +(_note.Offset*_xCoef/step);
            if (xLeft > 0)
            {
                el.Width = (_note.Length * _xCoef/step);
            }
            else
            {
                el.Width = (_note.Length * _xCoef/step) - xLeft;
                xLeft = 0;
            }

            el.Height = 5;
            switch (_note.noteState)
            {
                case NoteState.ntPlaying: el.Stroke = Brushes.Red; break;
                case NoteState.ntEnded: el.Stroke = Brushes.Gray; break;
                default: el.Stroke = Brushes.Black; break;
            }

            el.StrokeThickness = 1;
            el.SnapsToDevicePixels = true;


            Canvas.SetLeft(el, xLeft);
            Canvas.SetTop(el, _y);
            cnv.Children.Add(el);
        }

        protected void CalcNoteParams(Note _note,ref double _yCoef, out double _y, out bool _needLine) {
            _needLine = false;
            bool bemol;
            double l=0;
            if (_note.ForRightHand) {
                switch (_note.Pitch) {
                    case 22: l = 15;bemol = true; _needLine = true;  break;//B-1b
                    case 23: l = 15; _needLine = true; break;// B-1
                    case 24: l = 14.5; _needLine = true; break;//C0
                    case 25: l = 14.5;bemol = true; _needLine = true; break;// D0b
                    case 26: l = 14; _needLine = true; break;//D0
                    case 27: l = 13.5; _needLine = true; bemol = true; break;//E0b
                    case 28: l = 13.5; _needLine = true; break;//E0
                    case 29: l = 13; _needLine = true; break;//F0
                    case 30: l = 12.5; _needLine = true; bemol = true; break;//G0b
                    case 31: l = 12.5; _needLine = true; break;//G0
                    case 32: l = 12; _needLine = true; bemol = true; break; //A0b
                    case 33: l = 12; _needLine = true; break; //A0
                    case 34: l = 11.5; _needLine = true; bemol = true; break; //B0b
                    case 35: l = 11.5; _needLine = true; break; //B0

                    case 36: l = 11; _needLine = true; break; //C1
                    case 37: l = 10.5; _needLine = false; bemol = true; break; //D1b
                    case 38: l = 10.5; _needLine = false; break; // D1
                    case 39: l = 10; _needLine = false; bemol = true; break; //E1b
                    case 40: l = 10; _needLine = false; break; //E1
                    case 41: l = 9.5; _needLine = false; break; //F1
                    case 42: l = 10; _needLine = false; bemol = true; break; //G1b
                    case 43: l = 10; _needLine = false; break; //G1
                    case 44:
                        break;
                    case 45:
                        break;
                    case 46:
                        break;
                    case 47:
                        break;
                    case 48:
                        break;
                    case 49:
                        break;
                    case 50:
                        break;
                    case 51:
                        break;
                    case 52:
                        break;
                    case 53:
                        break;
                    case 54:
                        break;
                    case 55:
                        break;
                    case 56:
                        break;
                    case 57:
                        break;
                    case 58:
                        break;
                    case 59:
                        break;
                    case 60:
                        break;
                    case 61:
                        break;
                    case 62:
                        break;
                    case 63:
                        break;
                    case 64:
                        break;
                    case 65:
                        break;
                    case 66:
                        break;
                    case 67:
                        break;
                    case 68:
                        break;
                    case 69:
                        break;
                    case 70:
                        break;
                    case 71: break;
                    default: l = 0; break;
                }
                _y = (l -1)* _yCoef;
            }
            else { _needLine = false; _y = 0; }
            

        }

        protected void DrawNote(Note _note,ref double _xCoef, ref double _yCoef) {
            if (_note.Offset/step < -Length) return;
            //double y;
//            bool needLine;
//            CalcNoteParams(_note, ref _yCoef, out y, out needLine);
//            Ellipse el = new Ellipse();
//            el.Stroke = Brushes.Black;
//            el.StrokeThickness = 2;
//            el.Width = _xCoef*36;
//            el.Height = _yCoef;
//            double xLeft = cnv.ActualWidth - _note.Offset * _xCoef;
//            Canvas.SetLeft(el,xLeft );
//            Canvas.SetTop(el, y-(_yCoef/2));
//            cnv.Children.Add(el);

            DrawNoteTail(_note,ref _xCoef,100-_note.Pitch*_yCoef/22);

        }

        private void DrawLine(double _x1,double _y1,double _x2,double _y2,Brush _brush) {
            Line xBorderLine = new Line();
            xBorderLine.X1 = _x1;
            xBorderLine.Y1 = _y1;
            xBorderLine.X2 = _x2;
            xBorderLine.Y2 = _y2;
            xBorderLine.Stroke = _brush;
            xBorderLine.StrokeThickness= 1;
            xBorderLine.SnapsToDevicePixels = true;
            cnv.Children.Add(xBorderLine);
        } 
        protected void DrawLines(ref double yCoef) {
            for (int i = 1; i <= 24; i++) {
                if (((i > 4) && (i < 10)) | ((i > 14) && (i < 20)))
                    DrawLine(0,i*yCoef,cnv.ActualWidth,i*yCoef,Brushes.Black); 
            } 
        }
        public void Show() {
            double xCoef = cnv.ActualWidth / Length;
            double yCoef = cnv.ActualHeight / 22;
            cnv.Children.Clear();
            DrawLines(ref yCoef);
            foreach (Note n in notes.Where(x=>x.Offset<0)) DrawNote(n, ref xCoef, ref yCoef);
            DrawLine((Length+realBorderLine) * xCoef,0, (Length+realBorderLine) * xCoef,cnv.ActualHeight,Brushes.Blue);
        }

    }
}
