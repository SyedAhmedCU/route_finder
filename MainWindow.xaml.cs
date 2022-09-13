using System.Windows;

namespace route_finder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VM vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = VM.Instance;
            DataContext = vm;
        }

        private void GetRouteBtn_Click(object sender, RoutedEventArgs e)
        {
            vm.init();
        }

        private void SaveRouteBtn_Click(object sender, RoutedEventArgs e)
        {
            vm.SaveFoundRoute();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            vm.ClearOutput();
        }

        private void BrowseInputFileBtn_Click(object sender, RoutedEventArgs e)
        {
            vm.InputFile();
        }
    }
}
