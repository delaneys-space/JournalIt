namespace JournalIt.ViewModel.Setting
{
    public class JournalIt: ViewModelAbstract
    {
        double _left;
        double _top;
        double _width;
        double _height;

        public JournalIt()
        {
            _left = Properties.JournalIt.Default.Left;
            _top = Properties.JournalIt.Default.Top;
            _width = Properties.JournalIt.Default.Width;
            _height = Properties.JournalIt.Default.Height;
        }
 
        public double Left
        {
            get => _left;
            set
            {
                if (_left == value)
                    return;

                _left = value;
                OnPropertyChanged("Left");
            }
        }
        public double Top
        { 
            get => _top;
            set
            {
                if (_top == value)
                    return;

                _top = value;
                OnPropertyChanged("Top");
            }
        }
        public double Width
        { 
            get => _width;
            set
            {
                if (_width == value)
                    return;

                _width = value;
                OnPropertyChanged("Width");
            }
        }
        public double Height
        { 
            get => _height;
            set
            {
                if (_height == value)
                    return;

                _height = value;
                OnPropertyChanged("Height");
            }
        }


        public void Save()
        {
             Properties.JournalIt.Default.Left = _left;
             Properties.JournalIt.Default.Top = _top;
             Properties.JournalIt.Default.Width = _width;
             Properties.JournalIt.Default.Height = _height;

             Properties.JournalIt.Default.Save();
        }
    }
}
