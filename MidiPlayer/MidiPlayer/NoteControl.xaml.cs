using System.ComponentModel;
using System.Windows.Controls;
using Sanford.Multimedia.Midi;
using System;

namespace MidiPlayer
{
    /// <summary>
    /// Логика взаимодействия для StuffControl.xaml
    /// </summary>
    /// 


    public partial class NoteControl : UserControl
    {
        protected int length { get; set; }
        public int value;
        public int X;
        public int Y;


        public NoteControl()
        {
            InitializeComponent();
        }

    }
}
