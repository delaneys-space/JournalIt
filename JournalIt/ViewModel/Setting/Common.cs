using System;

namespace JournalIt.ViewModel.Setting
{
    public class Common: ViewModelAbstract
    {

        int minimumMinutes;
        int defaultProjectId;
        string defaultType;
        string defaultCompany;

        public Common()
        {
            UI = new UI();

            minimumMinutes = Properties.Settings.Default.MinimumMinutes;
            defaultProjectId = Properties.Settings.Default.DefaultProjectId;
            defaultType = Properties.Settings.Default.DefaultType;
            defaultCompany = Properties.Settings.Default.DefaultCompany;
            OnPropertyChanged("Email");
            OnPropertyChanged("ProductName");
            OnPropertyChanged("Version");
        }

        public UI UI { get; }


        public int MinimumMinutes
        {
            get => minimumMinutes;
            set
            {
                if (minimumMinutes == value)
                    return;

                minimumMinutes = value;
                OnPropertyChanged("MinimumMinutes");
            }
        }

        public int DefaultProjectId
        {
            get => defaultProjectId;
            set
            {
                if (defaultProjectId == value)
                    return;

                defaultProjectId = value;
                OnPropertyChanged("DefaultProjectId");
                OnPropertyChanged("DefaultProject");
            }
        }

        public Project DefaultProject
        {
            get
            {
                foreach (var project in Project.Projects )
                {
                    if (project.ProjectId == DefaultProjectId)
                        return project;
                }

                return null;
            }

        }

        public string DefaultType
        {
            get => defaultType;
            set
            {
                if (defaultType == value)
                    return;

                defaultType = value;
                OnPropertyChanged("DefaultType");
            }
        }

        public string DefaultCompany
        {
            get => defaultCompany;
            set
            {
                if (defaultCompany == value)
                    return;

                defaultCompany = value;
                OnPropertyChanged("DefaultCompany");
            }
        }

        public double FormTop
        {
            get => Properties.Settings.Default.FormTop;
            set
            {
                if (Properties.Settings.Default.FormTop == value)
                    return;

                Properties.Settings.Default.FormTop = value;
                OnPropertyChanged("FormTop");
            }
        }


        public double FormLeft
        {
            get => Properties.Settings.Default.FormLeft;
            set
            {
                if (Properties.Settings.Default.FormLeft == value)
                    return;

                Properties.Settings.Default.FormLeft = value;
                OnPropertyChanged("FormLeft");
            }
        }

        public string ExcelFileFullname
        {
            get => GetExcelFileFullname();
            set
            {
                if (GetExcelFileFullname() == value)
                    return;

                Properties.Settings.Default.ExcelFileFullname = value;
                OnPropertyChanged("ExcelFileFullname");
            }
        }

        public string ExcelSheet
        {
            get => Properties.Settings.Default.ExcelSheet;
            set
            {
                if (Properties.Settings.Default.ExcelSheet == value)
                    return;


                Properties.Settings.Default.ExcelSheet = value;
                OnPropertyChanged("ExcelSheet");
            }
        }

        public string DataFolder
        {
            get => GetDataFolder();
            set
            {
                if (GetDataFolder() == value)
                    return;

                Properties.Settings.Default.DataFolder = value;
                OnPropertyChanged("DataFolder");
            }
        }


        public bool ExportToExcel
        {
            get => Properties.Settings.Default.ExportToExcel;
            set
            {
                if (Properties.Settings.Default.ExportToExcel == value)
                    return;

                Properties.Settings.Default.ExportToExcel = value;
                OnPropertyChanged("ExportToExcel");
            }
        }
        public void Save()
        {
            Properties.Settings.Default.MinimumMinutes = MinimumMinutes;
            Properties.Settings.Default.DefaultProjectId = DefaultProjectId;
            Properties.Settings.Default.DefaultType = DefaultType;
            Properties.Settings.Default.DefaultCompany = DefaultCompany;

            Properties.Settings.Default.Save();

            UI.Save();
        }

        public static string GetDataFolder()
        {
            var s = Properties.Settings.Default.DataFolder;
            const string specialString = "%documents%";
            if (s.IndexOf(specialString, StringComparison.Ordinal) >= 0)
            {
                s = s.Replace(specialString,
                              Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }

            return s;
        }



        public static string GetExcelFileFullname()
        {
            var s = Properties.Settings.Default.ExcelFileFullname;
            const string specialString = "%documents%";
            if (s.IndexOf(specialString, StringComparison.Ordinal) >= 0)
            {
                s = s.Replace(specialString,
                              Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }

            return s;
        }
    }
}
