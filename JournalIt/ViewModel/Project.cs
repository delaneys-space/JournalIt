using System.Collections.Generic;
using System.Linq;

namespace JournalIt.ViewModel
{
    using System.Collections.ObjectModel;

    public class Project : ViewModelAbstract
    {
        private readonly Model.Project _project;

        public Project(Model.Project project)
        {
            _project = project;

            Projects.Add(this);
        }



        #region Properties
        public int ProjectId
        {
            get => _project.ProjectId;
            set
            {
                if (_project.ProjectId == value)
                    return;

                _project.ProjectId = value;
                OnPropertyChanged("ProjectId");
            }
        }

        public string Name
        {
            get => _project.Name;
            set
            {
                if (_project.Name == value)
                    return;

                _project.Name = value;
                OnPropertyChanged("Name");
            }
        }
        #endregion

        #region Static Methods
        public static ObservableCollection<Project> Projects;

        public static bool Exists(string name)
        {
            //Is the name distinct
            return Projects.Any(project => project.Name == name);
        }

        public static Project New(string name)
        {
            var newProject = Projects.FirstOrDefault(project => project.Name == name);


            if (newProject != null)
                return newProject;

            // Create the new project
            newProject = new Project(new Model.Project())
            {
                Name = name
            };

            Save();

            return newProject;
        }

        public static void Save()
        {
            Model.Project.Save(Setting.Common.GetDataFolder());
        }

        public static void Load()
        {
            Projects = new ObservableCollection<Project>();

            var path = Setting.Common.GetDataFolder();

            Model.Project.Load(path);
            ViewModel.Project project;
            foreach (var kv in Model.Project.Projects)
                project = new Project(kv.Value);
        }
        #endregion  
    }
}