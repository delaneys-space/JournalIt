namespace JournalIt.ViewModel.Setting
{
    public class UI: ViewModelAbstract
    {
        private double _scale;
        private readonly double _scaleMin;
        private readonly double _scaleMax;
        private readonly double _scaleChange;

        private double _stopwatchWidth;
        private double _stopwatchMinWidth;
        private readonly double _stopwatchMaxWidth;
        private readonly double _stopwatchWidthChange;
        private double _stopwatchHeight;
        private double _stopwatchFontSizeSelected;
        private double _stopwatchFontSize;

        public UI()
        {
            JournalIt = new JournalIt();

            _scale = Properties.UI.Default.Scale;
            _scaleMin = Properties.UI.Default.ScaleMin;
            _scaleMax = Properties.UI.Default.ScaleMax;
            _scaleChange = Properties.UI.Default.ScaleChange;

            _stopwatchWidth = Properties.UI.Default.StopwatchWidth;
            _stopwatchMinWidth = Properties.UI.Default.StopwatchMinWidth;
            _stopwatchMaxWidth = Properties.UI.Default.StopwatchMaxWidth;
            _stopwatchWidthChange = Properties.UI.Default.StopwatchWidthChange;
            _stopwatchHeight = Properties.UI.Default.StopwatchHeight;
            
            _stopwatchFontSize = Properties.UI.Default.StopwatchFontSize;
            _stopwatchFontSizeSelected = Properties.UI.Default.StopwatchFontSizeSelected;

        }

        public string Email => Properties.Settings.Default.email;

        public string ProductName => Properties.Settings.Default.ProductName;

        public string ProductNameOptions => Properties.Settings.Default.ProductName + " - Options";

        public string ProductNameStopwatch => Properties.Settings.Default.ProductName + " - Stopwatch";

        public string Version => "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        public double Scale
        {
            get => _scale;
            set
            {
                if (_scale == value)
                    return;

                _scale = value;
                OnPropertyChanged("Scale");
            }
        }

        public double ScaleMin => _scaleMin;

        public double ScaleMax => _scaleMax;

        public double ScaleChange => _scaleChange;

        public double StopwatchWidth
        {
            get => _stopwatchWidth;
            set
            {
                if (_stopwatchWidth == value)
                    return;

                _stopwatchWidth = value;
                OnPropertyChanged("StopwatchWidth");
            }
        }
        public double StopwatchMinWidth
        {
            get => _stopwatchMinWidth;
            set
            {
                if (_stopwatchMinWidth == value)
                    return;

                _stopwatchMinWidth = value;
                OnPropertyChanged("StopwatchMinWidth");
            }
        }
        public double StopwatchMaxWidth => _stopwatchMaxWidth;

        public double StopwatchWidthChange => _stopwatchWidthChange;

        public double StopwatchHeight
        {
            get => _stopwatchHeight;
            set
            {
                if (_stopwatchHeight == value)
                    return;

                _stopwatchHeight = value;
                OnPropertyChanged("StopwatchHeight");
            }
        }

        public double StopwatchFontSizeSelected
        {
            get => _stopwatchFontSizeSelected;
            set
            {
                if (_stopwatchFontSizeSelected == value)
                    return;

                _stopwatchFontSizeSelected = value;
                OnPropertyChanged("StopwatchFontSizeSelected");
            }
        }
        public double StopwatchFontSize
        {
            get => _stopwatchFontSize;
            set
            {
                if (_stopwatchFontSize == value)
                    return;

                _stopwatchFontSize = value;
                OnPropertyChanged("StopwatchFontSize");
            }
        }         
        public JournalIt JournalIt { get; }


        public void Save()
        {
            Properties.UI.Default.Scale = _scale;
            Properties.UI.Default.StopwatchWidth = _stopwatchWidth;

            Properties.UI.Default.Save();
        }
    }
}
