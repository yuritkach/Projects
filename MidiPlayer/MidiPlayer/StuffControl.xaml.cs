using System.ComponentModel;
using System.Windows.Controls;
using Sanford.Multimedia.Midi;
using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;

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
            //  DispatcherTimer setup
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0,1);
            dispatcherTimer.Start();
        }
        public void Send(NoteControl ANote)
        {

            ANote.X = 500;
            ANote.Y = ANote.value * 10;
            Canvas.SetLeft(ANote, ANote.X);
            Canvas.SetTop(ANote, ANote.Y);

            cnv.Children.Add(ANote);  
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < cnv.Children.Count; i++) {
            //    if cnv.Children[i]

            }


            // Forcing the CommandManager to raise the RequerySuggested event
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }


    }
}
  