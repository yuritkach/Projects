using System.ComponentModel;
using System.Windows.Controls;
using Sanford.Multimedia.Midi;
using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using static MidiPlayer.Player;

namespace MidiPlayer
{
    /// <summary>
    /// Логика взаимодействия для StuffControl.xaml
    /// </summary>
    /// 

    public partial class StuffControl : UserControl
    {
        //  protected NoteControl note;
        Random rnd = new Random();
        DispatcherTimer dispatcherTimer;

        
      

        public StuffControl()
        {
            InitializeComponent();
            //  DispatcherTimer setup 44
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0,1);
            dispatcherTimer.Start();
        }
        public void Send(VisualNote ANote)
        {
            var note = new Ellipse();
            note.Height = 7;
            note.Width = 15;
            if (ANote.channel == 3)
            {
                note.Stroke = Brushes.Black;
            }
            else {
                note.Stroke = Brushes.Red;
            }
            
            note.StrokeThickness = 2;

            Canvas.SetLeft(note, cnv.ActualWidth);
            Canvas.SetTop(note, cnv.ActualHeight-((cnv.ActualHeight/21)*(ANote.value)*0.25));

            cnv.Children.Add(note);  
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < cnv.Children.Count; i++) {

                if (cnv.Children[i] is Ellipse) {
                    Ellipse el =(Ellipse) cnv.Children[i];
                    var xleft=Canvas.GetLeft(el);
                    if (xleft < 0) {
                        cnv.Children.RemoveAt(i);
                    }
                    else {
                        Canvas.SetLeft(el, xleft - 1);
                    }
                    
                }


            }


            // Forcing the CommandManager to raise the RequerySuggested event
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }


    }
}
  