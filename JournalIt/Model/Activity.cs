using System.Collections.Generic;

namespace JournalIt.Model
{
    using System.Xml;

    public class Activity
    {
        #region Statics

        private const string Filename = "Activities.xml";

        public static void Delete(string name)
        {
            Activity.Activities.Remove(name);
        }

        public static void Save(string path)
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            var fullname = "";
            if (path.IndexOf('\\') == path.Length - 1)
                fullname = path + Activity.Filename;
            else
                fullname = path + '\\' + Activity.Filename;


            const string xmlRowTemplate = "<Activity>{0}</Activity>\n";


            // Create a list of types
            var xmlRow = "";
            foreach (var name in Activity.Activities)
                xmlRow += string.Format(xmlRowTemplate,
                    name);


            var xml = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Activities>\n{xmlRow}</Activities>";

            var dom = new XmlDocument();
            dom.LoadXml(xml);
            dom.Save(fullname);
        }

        public static void Load(string path)
        {

            // Get the full file name
            string fullname;
            if (path.IndexOf('\\') == path.Length - 1)
                fullname = path + Activity.Filename;
            else
                fullname = path + '\\' + Activity.Filename;


            //Does the file exist
            if (System.IO.File.Exists(fullname))
            {
                Activity.Activities.Clear();

                var xmlFile = new XmlDocument();


                // Load the file
                xmlFile.Load(fullname);

                var node = xmlFile.SelectSingleNode("Activities");


                foreach (XmlNode xmlNode in node.ChildNodes)
                    Activity.Activities.Add(xmlNode.InnerText);
            }
        }

        public static List<string> Activities = new List<string>();
        #endregion
    }
}