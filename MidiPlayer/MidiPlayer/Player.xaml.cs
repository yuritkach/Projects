using System.Windows.Controls;
using Sanford.Multimedia.Midi;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;

namespace MidiPlayer
{
    /// <summary>
    /// Логика взаимодействия для Player.xaml
    /// </summary>
    public partial class Player : Page
    {
        NoteControl note;
        private OutputDevice outDevice;
        private int outDeviceID = 0;
        private Sequence seq;
        public Sequencer sq;
        protected int zi;
        public IList<VisualElements> visualElements = new List<VisualElements>();

        public Player()
        {
            InitializeComponent();

            if (OutputDevice.DeviceCount == 0) {
                MessageBox.Show("No MIDI output devices available.");
            }
            else {
                outDevice = new OutputDevice(outDeviceID);
                seq = new Sequence("d:\\1.mid");
                seq.Format = 1;
                //      string filename="";

                //                OpenFileDialog openFileDialog = new OpenFileDialog();
                //                if (openFileDialog.ShowDialog() == true)
                //                   filename = openFileDialog.FileName;

                //     seq.LoadAsync(filename);

                zi = 1;

                sq = new Sequencer();
                sq.Position = 0;
//                sq.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
                sq.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.HandleChannelMessagePlayed);
                sq.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
                sq.Chased += new System.EventHandler<Sanford.Multimedia.Midi.ChasedEventArgs>(this.HandleChased);
                sq.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);


                sq.Sequence = seq;

                BuildVisualElements(seq);

  //              sq.Continue();  // старт перенесен на точку, когда нота доходит до точки проигрывания!!!


  //              sequence1.LoadProgressChanged += HandleLoadProgressChanged;
  ///             sequence1.LoadCompleted += HandleLoadCompleted;

            }

      
        }



        public class VisualNote {
            public IMidiMessage link;
            public int offset;
            public int value;
            public int length;
            public int channel;
        }

        public class VisualChanel {
            public int chanelNumber;
            public IList<VisualNote> notes = new List<VisualNote>();
        }

        public class VisualElements {
            public int TickCount;
            public IList<VisualChanel> chanels = new List<VisualChanel>();     
        } 
        protected void BuildVisualElements(Sequence Aseq)
        {
            visualElements.Clear();
            int xchanel = 0;
            foreach (Track track in Aseq){
                xchanel++;
                VisualElements ve = new VisualElements();
                visualElements.Add(ve);

                for (int i = 0; i < track.Count; i++) {
                    MidiEvent me = track.GetMidiEvent(i);
                    if (me.MidiMessage.MessageType == MessageType.Channel) {
                        ChannelMessage cm = (ChannelMessage) me.MidiMessage ;
                        if (cm.Command == ChannelCommand.NoteOn)
                        {
                            VisualChanel vc = ve.chanels.SingleOrDefault(s => s.chanelNumber == cm.MidiChannel);
                            if (vc == null) {
                                vc = new VisualChanel();
                                vc.chanelNumber = cm.MidiChannel;
                                ve.chanels.Add(vc);
                            }

                            VisualNote vn = new VisualNote();
                            vn.link = me.MidiMessage;
                            vn.channel = xchanel; // неправильно


                            vn.offset = me.AbsoluteTicks;
                            vn.value = cm.Data1;
                            vn.length = -1;
                            vc.notes.Add(vn);

                        }

                        if (cm.Command == ChannelCommand.NoteOff)
                        {
                            VisualChanel vc = ve.chanels.SingleOrDefault(s => s.chanelNumber == cm.MidiChannel);
                            if (vc == null)
                            {
                                vc = new VisualChanel();
                                vc.chanelNumber = cm.MidiChannel;
                                ve.chanels.Add(vc);
                            }


                            VisualNote vn = vc.notes.Where(v => v.value == cm.Data1)
                                                    .OrderByDescending(v => v.offset)
                                                    .First();
                            if (vn != null) {


                                vn.length = GetNoteLength(me.AbsoluteTicks - vn.offset,Aseq.Division);

                            }

                        }




                    }
                }
            }



        }

        private int GetNoteLength(int ATicks, int ATicksPerQuarter) {

            return (int)Math.Round((double)(ATicks / ATicksPerQuarter))*4;

            

        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {

            outDevice.Send(e.Message);
            zi++;
            if (e.Message.Command == ChannelCommand.NoteOn) {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    foreach (VisualElements ve in visualElements) {
                        foreach (VisualChanel ch in ve.chanels) {
                            if (ch.chanelNumber == e.Message.MidiChannel) {
                                if (ch.notes.Count != 0) {

                                    var vn = ch.notes.Where(n => n.link == e.Message).FirstOrDefault();
                                    if (vn != null) {
                                        stuff.Send(vn);
                                    }
                                    
                                }
                                
                            }
                        }
                    }
                }));


            }



        }

        private void HandleChased(object sender, ChasedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }
        }

        private void HandleSysExMessagePlayed(object sender, SysExMessageEventArgs e)
        {
            //     outDevice.Send(e.Message); Sometimes causes an exception to be thrown because the output device is overloaded.
        }

        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
              }

            MessageBox.Show(zi.ToString());
        }




        private void Page_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            //stuff.SizeChanged();
        }

        private void StuffControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            seq.Division = seq.Division - 1;
        }
    }


   

}
