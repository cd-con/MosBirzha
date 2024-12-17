using MosBirzha_23var.Companies;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MosBirzha_23var
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Test company = new Test();
        public MainWindow()
        {
            InitializeComponent();

            //  DispatcherTimer setup
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0,100);
            dispatcherTimer.Start();


        }
        private CandleTimeSpan current = CandleTimeSpan.DEBUG;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            company.Tick(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            PriceDisplay.Text = $"{company.Price}$";
            Chart._candles = Common.ConvertToCandles(company.graph, current);
            Chart.Draw();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            current = (CandleTimeSpan)(Enum.GetValues(typeof(CandleTimeSpan))).GetValue((sender as ComboBox).SelectedIndex);
            Chart._candles = Common.ConvertToCandles(company.graph, current);
            Chart.Draw();
        }
    }
}