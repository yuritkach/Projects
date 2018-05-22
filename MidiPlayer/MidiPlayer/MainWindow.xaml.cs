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

            button3 = new Button { Content = "Button" };
            Canvas.SetLeft(button3, 78);
            Canvas.SetTop(button3, 119);
            button3.Padding = new Thickness(10, 2, 10, 2);
            player.stuff.cnv.Children.Add(button3);

                 }

        private void settingsbutton_Click(object sender, RoutedEventArgs e)
        {
          mainframe.Content = settings;
        }
 

    }


}
