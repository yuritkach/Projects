using System.Windows.Controls;
using Sanford.Multimedia.Midi;
using System.Windows;
using Microsoft.Win32;
using System.IO;

namespace MidiPlayer
{
    /// <summary>
    /// Логика взаимодействия для Player.xaml
    /// </summary>
    public partial class Player : Page
    {
        Stuff stuff;
        private OutputDevice outDevice;
        private int outDeviceID = 0;
        private Sequence seq;
        private Sequencer sq;
        protected int zi;


        public Player()
        {
            InitializeComponent();
            stuff = new Stuff();
            stuff.Draw(StuffCanvas);

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
                sq.Continue();


  //              sequence1.LoadProgressChanged += HandleLoadProgressChanged;
  ///             sequence1.LoadCompleted += HandleLoadCompleted;

            }




        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {

            outDevice.Send(e.Message);
            zi++;
            //            pianoControl1.Send(e.Message);
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
            stuff.SizeChanged();
        }
    }


   

}
