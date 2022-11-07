using System.Linq;

namespace JournalIt.ViewModel
{
    using System.Collections.ObjectModel;

    public class Activity
    {
        #region Static Methods
        public static ObservableCollection<string> Activities;
        public static void Load()
        {
            Activities = new ObservableCollection<string>();

            var path = Setting.Common.GetDataFolder();

            Model.Activity.Load(path);
            foreach (var name in Model.Activity.Activities)
                Activities.Add(name);
        }
        public static void New(string type)
        {
            // 1. Add a new if it does not exist.
            // 2. Add the new project to the lists.
            // 3. Save the model list.
            if (Exists(type))
                return;

            Activities.Add(type);
            Model.Activity.Activities.Add(type);
            Model.Activity.Save(Setting.Common.GetDataFolder());
        }

        public static void Save()
        {
            var path = Setting.Common.GetDataFolder();

            Model.Activity.Save(path);
        }

        public static void Delete(string name)
        {
            try
            {
                Activities.Remove(name);
                Model.Activity.Activities.Remove(name);
            }
            catch
            {
                // Do nothing
            }
        }
        public static bool Exists(string name)
        {
            // Is the name distinct
            return Model.Activity.Activities.Any(type => type == name);
        }
        #endregion    
    }
}
