using System;
using System.Collections.Generic;

namespace JournalIt.Model
{
    using System.Xml;

    public class Contact
    {
        public Contact()
        {
            //Create a unique Id
            var id = 0;
            while (Contacts[id] != null)
                id++;

            this.ContactId = id;

            Contacts.Add(id, this);
        }


        public int ContactId { get; set; }
        public string Name { get; set; }


        public static void Delete(int contactId)
        {
            Contacts.Remove(contactId);
        }


        #region Statics

        private const string Filename = "Contacts.xml";

        public static Contact Get(string idString, 
                                        string fullname)
        {

            if (idString == null)
                return null;


            if (!int.TryParse(idString, out var id))
                throw new Exception(fullname + " does not contain a valid contact ID.");


            if (!Contacts.ContainsKey(id))
                throw new Exception("The contacts list does not contain a contact with ID = " + id.ToString());


            Contacts.TryGetValue(id, out var contact);
            return contact;
        }



        public static void Save(string path)
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);


            const string xmlRowTemplate = "<Contact ContactId=\"{0}\" Name=\"{1}\"/>\n";
            var xmlRow = "";



            foreach (var kv in Contacts)
                xmlRow += string.Format(xmlRowTemplate,
                    kv.Key,
                    kv.Value);


            var xml = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Contacts>\n{xmlRow}</Contacts>";

            var dom = new XmlDocument();
            dom.LoadXml(xml);
            dom.Save(path);
        }


        public static void Load(string path)
        {
            // Get the full file name
            string fullname;
            if (path.IndexOf('\\') == path.Length - 1)
                fullname = path + Filename;
            else
                fullname = path + '\\' + Filename;


            //Does the file exist
            if (!System.IO.File.Exists(fullname))
                return;

            Contacts.Clear();

            var xmlFile = new XmlDocument();


            // Load the file
            xmlFile.Load(fullname);

            var node = xmlFile.SelectSingleNode("Contacts");
            if (node.InnerText == null)
                return;


            // Get the data
            foreach (XmlNode xmlNodeContact in node.ChildNodes)
            {
                var contact = Get(xmlNodeContact.InnerText, fullname);
                Contacts.Add(contact.ContactId, contact);
            }
        }

        public static Dictionary<int, Contact> Contacts = new Dictionary<int,Contact>();
        #endregion
    }
}