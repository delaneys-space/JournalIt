using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Input;

namespace JournalIt.ViewModel
{
    public class CogBox: ViewModelAbstract
    {
        private bool _isRunning;
        private Stopwatch _stopwatchSelected;
        private readonly View.FrmJournalIt _frmJournalIt;

        private string _message = "";
        private Timer _seconds;
        private Timer _autoSaveStopwatch;
        private ObservableCollection<string> _companies;
        private ObservableCollection<Contact> _contacts;
        private ObservableCollection<Project> _projects;
        private ObservableCollection<string> _activities;
        private ObservableCollection<Stopwatch> _stopwatches;




        private readonly Setting.Common _common = new Setting.Common();


        public CogBox(View.FrmJournalIt frm)
        {
            _frmJournalIt = frm;
            Company.Load();
            Companies = Company.Companies;

            Contact.Load();
            Contacts = Contact.Contacts;

            Project.Load();
            Projects = Project.Projects;

            Activity.Load();
            Activities = Activity.Activities;

            Stopwatch.Load(this);
            Stopwatches = Stopwatch.Stopwatches;
        }



        #region Properties
        public bool Running
        {
            get => _isRunning;
            set
            {
                if (_isRunning == value)
                    return;

                _isRunning = value;
                OnPropertyChanged("Running");
            }
        }


        public Stopwatch StopwatchSelected
        {
            get => _stopwatchSelected;
            set
            {
                if (_stopwatchSelected == value)
                    return;

                _stopwatchSelected = value;

                if (_stopwatchSelected == null)
                    OnPropertyChanged("SecondsIcon");
                else
                {
                    if (_stopwatchSelected.DateTimeStart == DateTime.MinValue)
                        _stopwatchSelected.DateTimeStart = DateTime.Now;
                }
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                if (_message == value)
                    return;

                _message = value;
                OnPropertyChanged("Message");
            }
        }

        //This is pasted into the Title of journal it application.
        //It will show the duration when a stopwatch is running.
        public string Title
        {
            get
            {
                string str;

                if (_stopwatchSelected != null)
                {
                    if (_stopwatchSelected.IsOn)
                    {
                        var timeSpan = new TimeSpan(0, 0, _stopwatchSelected.Seconds);
                        str = timeSpan.ToString() + " - " + Properties.Settings.Default.ProductName;
                    }
                    else
                        str = Properties.Settings.Default.ProductName;
                }
                else
                    str = Properties.Settings.Default.ProductName;


                return str;
            }
        }

        public string SecondsIcon
        {
            get
            {
                const string prefix = "/View/Images/PieClocks/";
                try
                {
                    if(_stopwatchSelected == null)
                        return prefix + "00.png";

                    var timeSpan = new TimeSpan(0, 0, _stopwatchSelected.Seconds);
                    var iMinutes = timeSpan.Minutes;


                    if (iMinutes >= 55)
                        return prefix + "55.png";
                    if (iMinutes >= 50)
                        return prefix + "50.png";
                    if (iMinutes >= 45)
                        return prefix + "45.png";
                    if (iMinutes >= 40)
                        return prefix + "40.png";
                    if (iMinutes >= 35)
                        return prefix + "35.png";
                    if (iMinutes >= 30)
                        return prefix + "30.png";
                    if (iMinutes >= 25)
                        return prefix + "25.png";
                    if (iMinutes >= 20)
                        return prefix + "20.png";
                    if (iMinutes >= 15)
                        return prefix + "15.png";
                    if (iMinutes >= 10)
                        return prefix + "10.png";
                    if (iMinutes >= 5)
                        return prefix + "05.png";

                    return prefix + "00.png";
                }
                catch
                {
                    return prefix + "00.png";
                }
            }
        }

        public Setting.Common Common => _common;


        public View.FrmJournalIt FrmJournalIt => _frmJournalIt;

        public ObservableCollection<string> Companies
        {
            get => _companies;
            set
            {
                if (_companies == value)
                    return;


                _companies = value;
                OnPropertyChanged("Companies");
            }
        }
        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set
            {
                if (_contacts == value)
                    return;


                _contacts = value;
                OnPropertyChanged("Contacts");
            }
        }
        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set
            {
                if (_projects == value)
                    return;

                _projects = value;
                OnPropertyChanged("Projects");
            }
        }
        public ObservableCollection<string> Activities
        {
            get => _activities;
            set
            {
                if (_activities == value)
                    return;

                _activities = value;
                OnPropertyChanged("Types");
            }
        }
        public ObservableCollection<Stopwatch> Stopwatches
        {
            get => _stopwatches;
            set
            {
                if (_stopwatches == value)
                    return;

                _stopwatches = value;
                OnPropertyChanged("Stopwatches");
            }
        }

        public View.FrmOptions FrmOptions { get; set; }
        
        #endregion


        #region Methods
        public void Start()
        {
            if (_stopwatchSelected != null)
            {
                OnPropertyChanged("SecondsIcon");
            }

            //Seconds
            if (_seconds == null)
            {
                _seconds = new Timer(1000);
                _seconds.Elapsed += OnTimedEvent;
            }

            _seconds.Enabled = true;
            _seconds.Start();

            //AutoSave
            if (_autoSaveStopwatch == null)
            {
                _autoSaveStopwatch = new Timer(Properties.Settings.Default.AutoSavePeriod);
                _autoSaveStopwatch.Elapsed += OnTimedAutoSaveEvent;
            }
            _autoSaveStopwatch.Enabled = true;
            _autoSaveStopwatch.Start();
        }

        public void Stop()
        {
            //Seconds
            if (_seconds != null)
            {
                _seconds.Stop();
                _seconds.Enabled = false;
            }

            //AutoSave
            if (_autoSaveStopwatch != null)
            {
                _autoSaveStopwatch.Stop();
                _autoSaveStopwatch.Enabled = false;
            }
        }

        
        public void OpenOptionsDialogue()
        {
            if (FrmOptions == null)
                FrmOptions = new View.FrmOptions(this);


            FrmOptions.Show();
            FrmOptions.Activate();
        }
        #endregion


        #region Relay Commands
        public ICommand ScaleUp => new RelayCommand(ScaleUp_Execute, ScaleUp_CanExecute);

        public ICommand ScaleDown => new RelayCommand(ScaleDown_Execute, ScaleDown_CanExecute);

        public ICommand StretchLeft => new RelayCommand(StretchLeft_Execute, StretchLeft_CanExecute);

        public ICommand StretchRight => new RelayCommand(StretchRight_Execute, StretchRight_CanExecute);

        #endregion



        #region Relay Commands Event Handler Methods
        private void ScaleUp_Execute()
        {
            if (Common.UI.Scale < Common.UI.ScaleMax)
            {
               // frmJournalIt.FadeInMessage();
                _frmJournalIt.FadeOutMessageCancel();

                Common.UI.Scale += Common.UI.ScaleChange;
                _frmJournalIt.ctrlMessage.Text = Common.UI.Scale.ToString("0.00");
                Common.Save();
            }
        }

        private bool ScaleUp_CanExecute()
        {
            return true;
        }

        private void ScaleDown_Execute()
        {
            if (Common.UI.Scale > Common.UI.ScaleMin)
            {
                //frmJournalIt.FadeInMessage();
                _frmJournalIt.FadeOutMessageCancel();

                Common.UI.Scale -= Common.UI.ScaleChange;
                _frmJournalIt.ctrlMessage.Text = Common.UI.Scale.ToString("0.00");

                Common.Save();
            }
        }

        private bool ScaleDown_CanExecute()
        {
            return true;
        }

        private void StretchLeft_Execute()
        {
            if (Common.UI.StopwatchWidth > Common.UI.StopwatchMinWidth)
            {
                //frmJournalIt.FadeInMessage();
                _frmJournalIt.FadeOutMessageCancel();


                Common.UI.StopwatchWidth -= Common.UI.StopwatchWidthChange;
                _frmJournalIt.ctrlMessage.Text = Common.UI.StopwatchWidth.ToString("0.00");
                Common.Save();
            }
        }

        private bool StretchLeft_CanExecute()
        {
            return true;
        }

        private void StretchRight_Execute()
        {
            if (Common.UI.StopwatchWidth < Common.UI.StopwatchMaxWidth)
            {
                //frmJournalIt.FadeInMessage();
                _frmJournalIt.FadeOutMessageCancel();


                Common.UI.StopwatchWidth += Common.UI.StopwatchWidthChange;
                _frmJournalIt.ctrlMessage.Text = Common.UI.StopwatchWidth.ToString("0.00");
                Common.Save();
            }

        }

        private bool StretchRight_CanExecute()
        {
            return true;
        }
        #endregion

        #region Events Handlers
        private void OnTimedEvent(object source, 
                                  ElapsedEventArgs e)
        {
            if (_stopwatchSelected != null)
            {
                if (_stopwatchSelected.Seconds == 0)
                    _stopwatchSelected.DateTimeStart = DateTime.Now;

                // This the title every second
                _stopwatchSelected.Seconds++;
                OnPropertyChanged("Title");


                // Change the icon every five seconds
                var timeSpan = new TimeSpan(0, 0, _stopwatchSelected.Seconds);

                if (timeSpan.Minutes % 5 == 0
                    && timeSpan.Seconds == 0)
                    OnPropertyChanged("SecondsIcon");
            }
            else
            {
                OnPropertyChanged("Title");
                Stop();
            }
        }

        private void OnTimedAutoSaveEvent(object source, 
                                          ElapsedEventArgs e)
        {
            try
            {
                Stopwatch.SaveAll();
            }
            catch
            {
                // ignored
            }
        }
        #endregion


        public void SaveAll(List<Stopwatch> stopwatches)
        {
            var id = 0;
            foreach (Stopwatch stopwatch in stopwatches)
            {
                stopwatch.Replace(id);
                id++;
            }
        }
    }
}
