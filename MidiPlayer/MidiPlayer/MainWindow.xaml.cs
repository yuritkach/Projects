using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
