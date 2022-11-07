using System.Collections.Generic;

namespace JournalIt.Model
{
    using System.Xml;

    public class Company
    {
        #region Statics
        private static string Filename = "Companies.xml";

        public static void Delete(string name)
        {
            Companies.Remove(name);
        }

        public static void Save(string path)
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);


            //Get the full filename
            string fullname;
            if (path.IndexOf('\\') == path.Length - 1)
                fullname = path + Company.Filename;
            else
                fullname = path + '\\' + Company.Filename;



            const string xmlRowTemplate = "<Company>{0}</Company>\n";

            // Create the rows of companies
            var xmlRow = "";
            foreach (var name in Company.Companies)
                xmlRow += string.Format(xmlRowTemplate,
                    name);


            var xml = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Companies>\n{xmlRow}</Companies>";

            var dom = new XmlDocument();
            dom.LoadXml(xml);
            dom.Save(fullname);
        }

        public static void Load(string path)
        {

            // Get the full file name
            string fullname;
            if (path.IndexOf('\\') == path.Length - 1)
                fullname = path + Company.Filename;
            else
                fullname = path + '\\' + Company.Filename;


            //Does the file exist
            if (!System.IO.File.Exists(fullname))
                return;

            Companies.Clear();

            var xmlFile = new XmlDocument();


            // Load the file
            xmlFile.Load(fullname);

            var node = xmlFile.SelectSingleNode("Companies");
            
            if (node.InnerText == null)
                return;


            // Get the data
            foreach (XmlNode xmlNodeContact in node.ChildNodes)
                Companies.Add(xmlNodeContact.InnerText);
        }

        public static List<string> Companies = new List<string>();
        #endregion
    }
}