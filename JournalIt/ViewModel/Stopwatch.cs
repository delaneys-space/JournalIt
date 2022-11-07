using System;
using System.Linq;

namespace JournalIt.ViewModel
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Text.RegularExpressions;

    public class Stopwatch : ViewModelAbstract
    {
        private bool _isOn = false;
        private readonly Model.Stopwatch _stopwatch;


        public Stopwatch(Model.Stopwatch stopwatch, 
                         CogBox cogBox)
        {
            _stopwatch = stopwatch;
            CogBox = cogBox;

            #region Get the ViewModel.Project
            foreach (var project in cogBox.Projects)
            {
                if (project.ProjectId != stopwatch.ProjectId)
                    continue;

                Project = project;
                Project.PropertyChanged += OnPropertyChangedEvent;
                break;
            }
            #endregion

            #region Get the ViewModel.Contacts
            _contacts = new ObservableCollection<Contact>();
            if (stopwatch.ContactIds != null)
                foreach (var contactId in stopwatch.ContactIds)
                    foreach (var contact in cogBox.Contacts)
                        if (contact.ContactId == contactId)
                            _contacts.Add(contact);
            #endregion

            Stopwatches.Add(this);
        }


        #region Properties
        public string Path
        {
            get => _stopwatch.Path;
            set
            {
                if (_stopwatch.Path == value)
                    return;

                Saved = false;

                _stopwatch.Path = value;
                OnPropertyChanged("Path");
            }
        }

        public string Filename
        {
            get => _stopwatch.Filename;
            set
            {
                if (_stopwatch.Filename == value)
                    return;

                Saved = false;

                _stopwatch.Filename = value;
                OnPropertyChanged("Filename");
            }
        }

        public string Subject
        {
            get => _stopwatch.Subject;
            set
            {
                if (_stopwatch.Subject == value)
                    return;

                Saved = false;

                _stopwatch.Subject = value;
                OnPropertyChanged("Caption");
                OnPropertyChanged("Subject");
            }
        }


        public string Notes
        {
            get => _stopwatch.Notes;
            set
            {
                if (_stopwatch.Notes == value)
                    return;

                Saved = false;

                _stopwatch.Notes = value;
                OnPropertyChanged("Notes");
            }
        }

        private Project _project;
        public Project Project
        {
            get => _project;
            set
            {
                if (_project == value)
                    return;

                _project = value;


                //Assign the projectId to the Model.Stopwatch, to keep everything in sync.
                if (_project == null)
                    _stopwatch.ProjectId = -1;
                else
                    _stopwatch.ProjectId = Project.ProjectId;

                //Broadcast to the view elements that there has been a change.
                OnPropertyChanged("Project");
                OnPropertyChanged("Caption");
            }
        }

        public string Company
        {
            get => _stopwatch.Company;
            set
            {
                if (_stopwatch.Company == value)
                    return;

                _stopwatch.Company = value;
                OnPropertyChanged("Company");
            }
        }


        private ObservableCollection<Contact> _contacts;
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


        public string Activity
        {
            get => _stopwatch.Activity;
            set
            {
                if (_stopwatch.Activity == value)
                    return;

                _stopwatch.Activity = value;
                OnPropertyChanged("Activity");
            }
        }


        public DateTime DateTimeStart
        {
            get => _stopwatch.DateTimeStart;
            set
            {
                if (value == DateTime.MinValue)
                    value = DateTime.Now;

                if (_stopwatch.DateTimeStart == value)
                    return;

                Saved = false;

                _stopwatch.DateTimeStart = value;
                OnPropertyChanged("DateTimeStart");
            }
        }

        public int Seconds
        {
            get => _stopwatch.Seconds;
            set
            {
                if (_stopwatch.Seconds == value)
                    return;

                Saved = false;

                _stopwatch.Seconds = value;
                OnPropertyChanged("Caption");
                OnPropertyChanged("Seconds");
                OnPropertyChanged("TimeEllapsedString");
            }
        }

        public string TimeEllapsedString
        {
            get
            {
                var timeSpan = new TimeSpan(0, 0, Seconds);

                return timeSpan.ToString();
            }
            set
            {
                TimeSpan.TryParse(value, out var duration);

                Seconds = duration.Seconds;
                OnPropertyChanged("TimeEllapsedString");
            }
        }

        public DateTime DateStart
        {
            get => DateTimeStart.Date;
            set
            {
                DateTimeStart = new DateTime(value.Year, 
                                                  value.Month, 
                                                  value.Day, 
                                                  DateTimeStart.Hour, 
                                                  DateTimeStart.Minute, 
                                                  DateTimeStart.Second);
            }
        }
        public int TimeStart
        {
            get => (int)DateTimeStart.TimeOfDay.TotalSeconds;
            set
            {
                var timeSpan = new TimeSpan(0, 0, value);
                DateTimeStart = new DateTime(DateTimeStart.Year,
                                                  DateTimeStart.Month, 
                                                  DateTimeStart.Day,
                                                  timeSpan.Hours,
                                                  timeSpan.Minutes,
                                                  timeSpan.Seconds);
            }
        }
    
        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (_isOn == value)
                    return;

                _isOn = value;
                OnPropertyChanged("Caption");
                OnPropertyChanged("IsOn");
            }
        }

        public CogBox CogBox { get; }

        public View.ButtonStopwatch ButtonStopwatch { get; set; }

        public View.FrmStopwatch FrmStopwatch { get; set; }
        #endregion


        #region Methods
        public bool CanExport()
        {
            return _stopwatch.CanExport();
        }
        
        public void Reset()
        {
            Seconds = 0;

            DateTimeStart = DateTime.Now;
            Notes = "";
            Save();
            CogBox.OnPropertyChanged("Title");
        }

        public void Save()
        {
            _stopwatch.Save();
        }

        public void Delete()
        {
            _stopwatch.Delete();
            Saved = false;
            CogBox.OnPropertyChanged("Title");
        }

        public void OpenDialogue()
        {
            if (FrmStopwatch == null)
                FrmStopwatch = new View.FrmStopwatch(this);


            FrmStopwatch.Show();
            FrmStopwatch.Activate();
        }
        #endregion


        #region Event Handler Methods
        public void OnPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
                OnPropertyChanged("Caption");
        }
        #endregion


        #region Static Methods
        public static Stopwatch New(CogBox cogBox)
        {
            var path = Setting.Common.GetDataFolder();
            if (path.IndexOf('\\') == path.Length - 1)
                path = path + Properties.Settings.Default.StopwatchFolderName;
            else
                path = path + '\\' + Properties.Settings.Default.StopwatchFolderName;

            var stopwatchModel = new Model.Stopwatch(path,
                                                     DateTime.Now.ToString("yyyyMMddHHmmssfff") + Model.Stopwatch.FileExtension);

            var stopwatchViewModel = new Stopwatch(stopwatchModel, cogBox)
            {
                Company = Properties.Settings.Default.DefaultCompany,
                Activity = Properties.Settings.Default.DefaultType
            };

            // Assign the default project.
            var exists = Project.Projects.Any(project => project.ProjectId == Properties.Settings.Default.DefaultProjectId);

            if (exists)
                stopwatchViewModel.Project = Project.Projects[Properties.Settings.Default.DefaultProjectId];


            cogBox.StopwatchSelected = stopwatchViewModel;

            cogBox.Start();

            return stopwatchViewModel;
        }


        public static ObservableCollection<Stopwatch> Stopwatches;
        public static void Load(CogBox cogBox)
        {
            Stopwatches = new ObservableCollection<Stopwatch>();

            var path = Setting.Common.GetDataFolder();
            if (path.IndexOf('\\') != path.Length - 1)
                path += "\\";

            path += Properties.Settings.Default.StopwatchFolderName;
            if (path.IndexOf('\\') == path.Length - 1)
                path = path.Substring(0, path.Length - 1);



            Model.Stopwatch.LoadAll(path);
            foreach (var kv in Model.Stopwatch.Stopwatches)
            {
                var s = new Stopwatch(kv.Value,
                    cogBox);
            }
        }
        
        
        public static void SaveAll()
        {
            foreach (var stopwatch in Stopwatches)
            {
                stopwatch.Save();
            }
        }

        public static string HoursMinutes(int iTotalMinutes)
        {
            var iHours = iTotalMinutes / 60;
            var iMinutes = iTotalMinutes - iHours * 60;

            var sHoursMinutes = $"{iHours:00}" + ":" + $"{iMinutes:00}";
            return sHoursMinutes;
        }

        public static bool IsValidTime(string theTime)
        {
            var checkTime = new Regex(@"^(?:(?:([01]?\d|2[0-3]):)?([0-5]?\d):)?([0-5]?\d)$");

            return checkTime.IsMatch(theTime);
        }

        public static bool IsValidDuration(string theTime)
        {
            var checkTime = new Regex(@"^(?:(?:(\d*):)?([0-5]?\d):)?([0-5]?\d)$");

            return checkTime.IsMatch(theTime);
        }
        #endregion  

    
        public bool Export()
        {
            return Model.ExportToExcel.Export(_stopwatch);
        }

        public static bool ExportAll()
        {
            return Model.ExportToExcel.ExportAll();
        }

        public void Replace(int id)
        {
            _stopwatch.Replace(id);
        }
    }
}