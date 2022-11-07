using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalIt.Model
{
    using System.Xml;

    public class Project
    {
        public Project()
        {
            //Create a unique Id
            var id = -1;
            var isUnique = false;
            while ( !isUnique )
            {
                id++;
                isUnique = true;
                foreach (var kv in Projects)
                {
                    if (kv.Key != id)
                        continue;

                    isUnique = false;
                    break;
                }
            }

            ProjectId = id;

            Projects.Add(id, this);
        }

        public Project(int projectId, string name)
        {
            ProjectId = projectId;
            Name = name;

            Projects.Add(projectId, this);
        }

        public int ProjectId { get; set; }
        public string Name { get; set; }


        #region Statics

        private const string Filename = "Projects.xml";



        public static void Delete(int projectId)
        {
            Projects.Remove(projectId);
        }

        public static void Save(string path)
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            var fullname = "";
            if (path.IndexOf('\\') == path.Length - 1)
                fullname = path + Filename;
            else
                fullname = path + '\\' + Filename;

            var xmlRowTemplate = "<Project ProjectId=\"{0}\" Name=\"{1}\"/>\n";
            var xmlRow = "";



            foreach (var kv in Projects)
                xmlRow += string.Format(xmlRowTemplate,
                    kv.Key,
                    kv.Value.Name);


            var xml = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Projects>\n{xmlRow}</Projects>";

            var dom = new XmlDocument();
            dom.LoadXml(xml);
            dom.Save(fullname);
        }


        public static void Load(string path)
        {
            // Get the full file name
            var fullname = "";
            if (path.IndexOf('\\') == path.Length - 1)
                fullname = path + Filename;
            else
                fullname = path + '\\' + Filename;


            //Does the file exist
            if (!System.IO.File.Exists(fullname))
                return;

            Projects.Clear();

            var xmlFile = new XmlDocument();


            // Load the file
            xmlFile.Load(fullname);
            XmlNode node;

            node = xmlFile.SelectSingleNode("Projects");
            if (node.InnerText != null)
            {
                // Get the data
                foreach (XmlNode xmlNodeProject in node.ChildNodes)
                {
                    if (int.TryParse(xmlNodeProject.Attributes["ProjectId"].Value, out var id))
                    {
                        var name = xmlNodeProject.Attributes["Name"].Value;

                        var project = new Project(id, name);
                    }
                }
            }
        }

        public static Dictionary<int, Model.Project> Projects = new Dictionary<int, Project>();
        #endregion
    }
}
