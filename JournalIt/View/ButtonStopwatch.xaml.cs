using System.Windows.Controls.Primitives;


namespace JournalIt.View
{
    /// <summary>
    /// Interaction logic for ButtonStopwatch.xaml
    /// </summary>
    public partial class ButtonStopwatch : ToggleButton
    {
        public ButtonStopwatch()
        {
            InitializeComponent();
        }

        public ButtonStopwatch(ViewModel.Stopwatch stopwatch)
        {
            InitializeComponent();


            Stopwatch = stopwatch;
            Stopwatch.ButtonStopwatch = this;

            Loaded += (s, e) =>
            {
                DataContext = Stopwatch;
            };
        }

        public ViewModel.Stopwatch Stopwatch { get; } = null;
    }
}
