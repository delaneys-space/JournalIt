using System.Linq;

namespace JournalIt.ViewModel
{
    using System.Collections.ObjectModel;

    public class Contact : ViewModelAbstract
    {
        private readonly Model.Contact _contact;



        public Contact(Model.Contact contact)
        {
            _contact = contact;

            //Add to the Model lost and the ViewModel list
            Contacts.Add(this);
        }



        public int ContactId => _contact.ContactId;


        public string Name
        {
            get => _contact.Name;
            set
            {
                if (_contact.Name == value)
                    return;

                _contact.Name = value;
                OnPropertyChanged("Name");
            }
        }



        #region Static Methods
        public static ObservableCollection<Contact> Contacts;

        public static bool Exists(string name)
        {
            //Is the name distinct
            return Contacts.Any(contact => contact.Name == name);
        }

        public Contact New(string name)
        {
            return Contacts.FirstOrDefault(contact => contact.Name == name) ?? new Contact(new Model.Contact());
        }

        public static void Load()
        {
            Contacts = new ObservableCollection<Contact>();

            var path = Setting.Common.GetDataFolder();

            Model.Contact.Load(path);
            foreach (var kv in Model.Contact.Contacts)
                Contacts.Add(new Contact(kv.Value));
        }
        #endregion
    }
}