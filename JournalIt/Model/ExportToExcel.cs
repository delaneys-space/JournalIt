using System;
using System.Collections.Generic;
using System.Text;

namespace JournalIt.Model
{
    using Excel = Microsoft.Office.Interop.Excel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public class ExportToExcel
    {
        [DllImport("Oleacc.dll")]
        public static extern int AccessibleObjectFromWindow( int hwnd, 
                                                             uint dwObjectID, 
                                                             byte[] riid,
                                                             ref Microsoft.Office.Interop.Excel.Window ptr);

        public delegate bool EnumChildCallback( int hwnd, 
                                                ref int lParam);

        [DllImport("User32.dll")]
        public static extern bool EnumChildWindows( int hWndParent, 
                                                    EnumChildCallback lpEnumFunc,
                                                    ref int lParam);

        [DllImport("User32.dll")]
        public static extern int GetClassName( int hWnd, 
                                               StringBuilder lpClassName, 
                                               int nMaxCount);



        public static bool Export(Stopwatch stopwatch)
        {

            if (!Properties.Settings.Default.ExportToExcel || Properties.Settings.Default.ExcelSheet.Length <= 0 ||
                Properties.Settings.Default.ExcelFileFullname.Length <= 0) 
                return false;


            var fullname = ViewModel.Setting.Common.GetExcelFileFullname();
            var sheetName = Properties.Settings.Default.ExcelSheet;
            var isOpen = false;

            Excel.Application excel = null;
            var sheet = GetExcelWorksheet(fullname,
                sheetName,
                ref excel,
                ref isOpen);


            var usedRange = sheet.UsedRange;

            Excel.Range row = sheet.Rows[usedRange.Rows.Count + 1];

            ExportToExcel.AddRow(row,
                stopwatch);

            sheet.Parent.Save();
            if (isOpen)
                return true;


            sheet.Parent.Close(Type.Missing,
                Type.Missing,
                Type.Missing);

            excel.Quit();


            return true;
        }


        public static bool ExportAll()
        {

            if (!Properties.Settings.Default.ExportToExcel || Properties.Settings.Default.ExcelSheet.Length <= 0 ||
                Properties.Settings.Default.ExcelFileFullname.Length <= 0) 
                return false;

            var fullname = ViewModel.Setting.Common.GetExcelFileFullname();
            var sheetName = Properties.Settings.Default.ExcelSheet;
            var isOpen = false;

            Excel.Application excel = null;
            var sheet = GetExcelWorksheet(fullname,
                sheetName,
                ref excel,
                ref isOpen);


            var usedRange = sheet.UsedRange;

            var count = 1;
            foreach (var kv in Stopwatch.Stopwatches)
            {
                var stopwatch = kv.Value;


                // Only add the row if the seconds are greater than 0.
                if (stopwatch.Seconds <= 0 || !stopwatch.CanExport())
                    continue;

                Excel.Range row = sheet.Rows[usedRange.Rows.Count + count];
                count++;

                ExportToExcel.AddRow(row,
                    stopwatch);
            }


            sheet.Parent.Save();
            if (isOpen)
                return true;

            sheet.Parent.Close(Type.Missing,
                Type.Missing,
                Type.Missing);

            excel.Quit();


            return true;
        }


        private static void AddRow(Excel.Range row,
                                   Stopwatch stopwatch)
        {
            // Get the rounded up minutes
            var minimumMinutes = Properties.Settings.Default.MinimumMinutes;
            var minutes = stopwatch.Seconds / 60;

            minutes = (minutes / minimumMinutes) * minimumMinutes;

            if (minutes == 0
                && stopwatch.Seconds > 0)
                minutes += Properties.Settings.Default.MinimumMinutes;
            else if (((float)stopwatch.Seconds / 60) % minutes > 0)
                minutes += Properties.Settings.Default.MinimumMinutes;


            try
            {

                row.Cells[1, 1].value = stopwatch.Subject;
                row.Cells[1, 2].value = stopwatch.Activity;
                row.Cells[1, 3].value = stopwatch.DateTimeStart;
                row.Cells[1, 4].value = stopwatch.Seconds;
                row.Cells[1, 5].value = minutes;
                row.Cells[1, 6].value = new TimeSpan(0, minutes, 0).ToString(@"hh\:mm");
                row.Cells[1, 7].value = stopwatch.Company;

                if (stopwatch.ProjectId != null)
                {
                    row.Cells[1, 8].value = stopwatch.ProjectId;
                    if (Project.Projects.ContainsKey((int)stopwatch.ProjectId))
                        row.Cells[1, 9].value = Project.Projects[(int)stopwatch.ProjectId].Name;
                }


                if (stopwatch.ContactIds != null)
                    foreach (var contactId in stopwatch.ContactIds)
                    {
                        var contact = Contact.Contacts[contactId];
                    }

                row.Cells[1, 12].value = stopwatch.Notes;
            }
            catch
            {
                throw new Exception("Please finishing editing the Excel cell before exporting.");
            }
        }

        static Excel.Worksheet GetExcelWorksheet(string Fullname,
                                                 string sheetName,
                                                 ref Excel.Application excel,
                                                 ref bool isOpen)
        {
            Excel.Worksheet oSheetJournal = null;

            try
            {
                EnumChildCallback cb;
                var processes = new List<Process>();
                processes.AddRange(Process.GetProcessesByName("excel"));


                foreach (var p in processes)
                {

                    if ((int)p.MainWindowHandle > 0)
                    {
                        var childWindow = 0;
                        cb = EnumChildProc;

                        EnumChildWindows((int)p.MainWindowHandle,
                                         cb,
                                         ref childWindow);

                        if (childWindow > 0)
                        {

                            const uint OBJID_NATIVEOM = 0xFFFFFFF0;
                            var IID_IDispatch = new Guid("{00020400-0000-0000-C000-000000000046}");
                            Excel.Window excelWindow = null;

                            var res = AccessibleObjectFromWindow(childWindow,
                                                                 OBJID_NATIVEOM,
                                                                 IID_IDispatch.ToByteArray(),
                                                                 ref excelWindow);

                            if (res >= 0)
                            {
                                excel = excelWindow.Application;

                                foreach (Excel.Workbook oWorkbook in excel.Workbooks)
                                {
                                    if (Fullname.CompareTo(oWorkbook.FullName) == 0)
                                    {

                                        foreach (Excel.Worksheet oSheet in oWorkbook.Worksheets)
                                        {
                                            if (oSheet.Name.CompareTo(sheetName) == 0)
                                            {
                                                isOpen = true;
                                                oSheetJournal = oSheet;
                                                break;
                                            }
                                        }

                                    }

                                    if (oSheetJournal != null)
                                        break;
                                }
                            }
                        }
                    }

                    if (oSheetJournal != null)
                        break;
                }

                if(oSheetJournal == null) 
                    oSheetJournal = ExportToExcel.OpenOrCreateWorkSheet(Fullname,
                                                                        sheetName,
                                                                        ref excel);
            }
            catch
            {
                throw;
            }


            return oSheetJournal;
        }


        private static bool EnumChildProc(int hwndChild, 
                                          ref int lParam)
        {
            var buf = new StringBuilder(128);
            GetClassName(hwndChild, buf, 128);
            if (buf.ToString() == "EXCEL7")
            {
                lParam = hwndChild;
                return false;
            }
            return true;
        }

        private static Excel.Worksheet OpenOrCreateWorkSheet(string Fullname,
                                                             string sheetName,
                                                             ref Excel.Application excel)
        {
            Excel.Worksheet sheet = null;

            excel = new Excel.Application
            {
                Visible = true
            };


            //Open of create a workbook
            Excel.Workbook workbook;

            var fullname = ViewModel.Setting.Common.GetExcelFileFullname();
            if (System.IO.File.Exists(fullname))
            {
                workbook = excel.Workbooks.Open(fullname,
                    Type.Missing,
                    false,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    false,
                    Type.Missing,
                    Type.Missing);
            }
            else
            {
                workbook = excel.Workbooks.Add(Type.Missing);

                workbook.SaveAs(fullname,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    Type.Missing,
                    false,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing);
            }



            // Open or create a sheet
            var sheetExists = false;
            foreach (Excel.Worksheet sheetTemp in workbook.Worksheets)
            {
                if (sheetTemp.Name == sheetName)
                {
                    sheetExists = true;
                    sheet = sheetTemp;
                    break;
                }
            }

            if (!sheetExists)
            {
                sheet = workbook.Worksheets.Add(Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing);
                sheet.Name = sheetName;

                sheet.Cells[1, 1].value = "Subject";
                sheet.Cells[1, 2].value = "Type";
                sheet.Cells[1, 3].value = "DateTimeStart";
                sheet.Cells[1, 4].value = "Seconds";
                sheet.Cells[1, 5].value = "Interval (Minutes rounded)";
                sheet.Cells[1, 6].value = "Interval (hh:mm)";
                sheet.Cells[1, 7].value = "Company";
                sheet.Cells[1, 8].value = "ProjectId";
                sheet.Cells[1, 9].value = "Project";
                sheet.Cells[1, 10].value = "Contacts";
                sheet.Cells[1, 11].value = "Summerised";
                sheet.Cells[1, 12].value = "Notes";
            }

            return sheet;
        }
    }
}