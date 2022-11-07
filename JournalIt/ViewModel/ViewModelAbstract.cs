namespace JournalIt.ViewModel
{
    using System.ComponentModel;


    public abstract class ViewModelAbstract : INotifyPropertyChanged
    {
        private bool _saved = true;


        public bool Saved
        {
            get => _saved;
            set
            {
                if (_saved == value)
                    return;

                _saved = value;
                OnPropertyChanged("Saved");
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}