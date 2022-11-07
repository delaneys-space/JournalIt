using System.Linq;

namespace JournalIt.ViewModel
{
    using System.Collections.ObjectModel;

    public class Company: ViewModelAbstract
    {
        #region Static Methods
        public static ObservableCollection<string> Companies;
        public static void Load()
        {
            Companies = new ObservableCollection<string>();

            var path = Setting.Common.GetDataFolder();

            Model.Company.Load(path);
            foreach (var name in Model.Company.Companies)
                Companies.Add(name);
        }

        public static void New(string company)
        {
            // 1. Add a new if it does not exist.
            // 2. Add the new project to the lists.
            // 3. Save the model list.
            if (Exists(company))
                return;

            Companies.Add(company);
            Model.Company.Companies.Add(company);
            Model.Company.Save(Properties.Settings.Default.DataFolder);
        }


        public static void Delete(string name)
        {
            try
            {
                Companies.Remove(name);
                Model.Company.Companies.Remove(name);
            }
            catch
            {
                // Do nothing
            }
        }

        public static bool Exists(string name)
        {
            // Is the name distinct
            return Model.Company.Companies.Any(company => company == name);
        }
        #endregion
    }
}