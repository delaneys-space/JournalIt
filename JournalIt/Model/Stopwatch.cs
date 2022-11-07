using System;
using System.Collections.Generic;

namespace JournalIt.Model
{
    using System.Xml;

    public class Stopwatch
    {

        public  Stopwatch(string path,
                          string filename)
        {
            Path = path;
            Filename = filename;
            Subject = "";
            Notes = "";
            Stopwatches.Add(filename, this);

        }



        #region Properties
        public string Path { get; set; }
        public string Filename { get; set; }

        public string Fullname
        {
            get
            {
                if (Path.IndexOf('\\') == Path.Length - 1)
                    return Path + Filename;

                return Path + '\\' + Filename;
            }
        }

        public string Subject { get; set; }
        public string Notes { get; set; }
        public string Activity { get; set; }
        public DateTime DateTimeStart { get; set; }
        public int Seconds { get; set; }
        public string Company { get; set; }
        public int? ProjectId { get; set; }
        public List<int> ContactIds { get; set; }
        public bool Saved { get; set; }

        #endregion

        #region Methods
        public bool CanExport()
        {
            return Company != null 
                   && ProjectId != null 
                   && Notes != null;
        }

        public void Save()
        {
            var subject        = $"  <Subject>{Subject}</Subject>\n";
            var notes          = $"  <Notes>{Notes}</Notes>\n";
            var activity       = $"  <Activity>{Activity}</Activity>\n";
            var dateTimeStart  = $"  <DateTimeStart>{DateTimeStart}</DateTimeStart>\n";
            var seconds        = $"  <Seconds>{Seconds}</Seconds>\n";
            var company        = $"  <Company>{Company}</Company>\n";
            var projectId      = $"  <ProjectId>{ProjectId}</ProjectId>\n";

            const string xmlTemplateContact = "    <ContactId>{0}/ContactId>\n";
            var contactIds = "";
            if (ContactIds != null)
                foreach (var contactId in ContactIds)
                    contactIds += string.Format(xmlTemplateContact, contactId.ToString());

            contactIds = $"  <Contacts>\n{contactIds}</Contacts>\n";




            var value = subject +
                        notes +
                        activity +
                        dateTimeStart +
                        seconds +
                        company +
                        projectId +
                        contactIds;
            var xml = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<JournalItTimeEntry>\n{value}</JournalItTimeEntry>";


            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            // Create the folder if it does not exist.
            if (!System.IO.Directory.Exists(Path))
                System.IO.Directory.CreateDirectory(Path);


            // Save the stopwatch
            xmlDocument.Save(Fullname);


            Saved = true;
        }
        public void Delete()
        {
            if (System.IO.File.Exists(Fullname))
                System.IO.File.Delete(Fullname);
        }
        #endregion


        #region Static Methods
        public static string FileExtension = ".stopwatch";

        private static Stopwatch Load(string path, 
                                      string filename)
        {
            string fullname = "";
            if (path.IndexOf('\\') == path.Length - 1)
                fullname = path + filename;
            else
                fullname = path + '\\' + filename;




            if (!System.IO.File.Exists(fullname)) 
                return null;


            if (System.IO.Path.GetExtension(fullname) != FileExtension)
                return null;


            Stopwatch stopwatch = null;

            try
            {
                var xmlFile = new XmlDocument();

                const string topLevelNode = "JournalItTimeEntry";


                xmlFile.Load(fullname);
                XmlNode node;

                var subject = "";
                node = xmlFile.SelectSingleNode(topLevelNode + "/Subject");
                if (node?.InnerText != null)
                    subject = node.InnerText;

                var notes = "";
                node = xmlFile.SelectSingleNode(topLevelNode + "/Notes");
                if (node?.InnerText != null)
                    notes = node.InnerText;


                //ToDo: Remove the /Type lines
                var type = "";
                node = xmlFile.SelectSingleNode(topLevelNode + "/Type");

                if (node?.InnerText != null)
                    type = node.InnerText;


                var activity = "";
                node = xmlFile.SelectSingleNode(topLevelNode + "/Activity");
                if (node?.InnerText != null)
                    activity = node.InnerText;



                var sDateTimeStart = "";
                node = xmlFile.SelectSingleNode(topLevelNode + "/DateTimeStart");
                if (node?.InnerText != null)
                    sDateTimeStart = node.InnerText;

                DateTime dateTimeStart = Model.Stopwatch.GetDateTimeFromString(sDateTimeStart);


                int seconds = 0;
                node = xmlFile.SelectSingleNode(topLevelNode + "/Seconds");
                if (node?.InnerText != null)
                    int.TryParse(node.InnerText, out seconds);

                var company = "";
                node = xmlFile.SelectSingleNode(topLevelNode + "/Company");
                if (node?.InnerText != null)
                    company = node.InnerText;


                var projectId = -1;
                node = xmlFile.SelectSingleNode(topLevelNode + "/ProjectId");
                if (node?.InnerText != null)
                    int.TryParse(node.InnerText, out projectId);


                // Get the contacts from the xml file.
                var contactIds = new List<int>();
                node = xmlFile.SelectSingleNode(topLevelNode + "/Contacts");
                if (node?.InnerText != null)
                {
                    foreach (XmlNode xmlNodeContact in node.ChildNodes)
                    {
                        var contact = Contact.Get(xmlNodeContact.InnerText, fullname);
                        contactIds.Add(contact.ContactId);
                    }
                }



                stopwatch = new Stopwatch(path, filename)
                {
                    Subject = subject,
                    Notes = notes,
                    Activity = activity,
                    DateTimeStart = dateTimeStart,
                    Seconds = seconds,
                    Company = company,
                    ProjectId = projectId,
                    ContactIds = contactIds
                };
            }
            catch
            {
                // ignored
            }

            return stopwatch;
        }


        private static DateTime GetDateTimeFromString(string sDateTime)
        {
            var sDateStartParts = sDateTime.Split(new Char[] { ' ' });

            var sDate = sDateStartParts[0];
            var sTime = sDateStartParts[1];


            var sDateParts = sDate.Split(new Char[] { '/' });
            var sTimeSpanParts = sTime.Split(new Char[] { ':' });

            DateTime dateTime;
            try
            {
                dateTime = new DateTime(Convert.ToInt16(sDateParts[2]),
                                        Convert.ToInt16(sDateParts[1]),
                                        Convert.ToInt16(sDateParts[0]),
                                        Convert.ToInt16(sTimeSpanParts[0]),
                                        Convert.ToInt16(sTimeSpanParts[1]),
                                        Convert.ToInt16(sTimeSpanParts[2]));
            }
            catch
            {
                dateTime = DateTime.MinValue;
            }

            return dateTime;
        }


        public static void LoadAll(string path)
        {
            if (!System.IO.Directory.Exists(path)) 
                return;


            var files = System.IO.Directory.GetFiles(path);

            Stopwatches.Clear();

            foreach (var fullname in files)
            {
                var stopwatch = Load(path,
                                     System.IO.Path.GetFileName(fullname));
            }
        }


        // Store the filename and the stopwatch. The filename is the key.
        public static Dictionary<string, Model.Stopwatch> Stopwatches = new Dictionary<string, Stopwatch>();

        #endregion


        public void Replace(int id)
        {
            Delete();

            Filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + id + FileExtension;
            Save();
        }
    }
}