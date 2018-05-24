using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MidiPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Player player;
        Settings settings;

        protected void init() {
            player = new Player();
            settings = new Settings();
        }
        public MainWindow()
        {
            InitializeComponent();
            init();
        }

        private void playerbutton_Click(object sender, RoutedEventArgs e)
        {
            mainframe.Content = player;
        }

        private void settingsbutton_Click(object sender, RoutedEventArgs e)
        {
          mainframe.Content = settings;
        }
 

    }


}
