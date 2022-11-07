using System;
using System.Windows;

namespace JournalIt.View
{
    /// <summary>
    /// Interaction logic for FrmOptions.xaml
    /// </summary>
    public partial class FrmOptions : Window
    {
        private readonly ViewModel.CogBox _cogBox;

        public FrmOptions(ViewModel.CogBox cogBox)
        {
            _cogBox = cogBox;
            
            InitializeComponent();

            Loaded += (e, s) =>
                {
                    DataContext = _cogBox;
                    ctrlDefaultOptions.CogBox = cogBox;
                    ctrlExportOptions.CogBox = cogBox;
                    ctrlViewOptions.DataContext = cogBox;
                    ctrlAbout.DataContext = _cogBox.Common.UI;
                };
        }






        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _cogBox.FrmOptions = null;
            Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Save();
            _cogBox.FrmOptions = null;
            Close();
        }

        private void Save()
        {
            if (ctrlDefaultOptions.cboProject.SelectedItem == null)
                _cogBox.Common.DefaultProjectId = -1;
            else
            {
                var project = (ViewModel.Project)ctrlDefaultOptions.cboProject.SelectedItem;
                _cogBox.Common.DefaultProjectId = project.ProjectId;
            }


            _cogBox.Common.DefaultCompany = ctrlDefaultOptions.cboCompany.Text;
            _cogBox.Common.DefaultType = ctrlDefaultOptions.cboType.Text;
            _cogBox.Common.MinimumMinutes = Convert.ToInt16(ctrlDefaultOptions.txtMinimumMinutes.Text);
                
            //Save Excel Export data
            _cogBox.Common.ExcelFileFullname = ctrlExportOptions.txtExcelFullname.Text;
            _cogBox.Common.ExcelSheet = ctrlExportOptions.txtExcelSheet.Text;
            if (ctrlExportOptions.chkExportExcel.IsChecked != null)
                _cogBox.Common.ExportToExcel = (bool) ctrlExportOptions.chkExportExcel.IsChecked;

            _cogBox.Common.Save();
        }



        private void btnResetPosition_Click(object sender, RoutedEventArgs e)
        {
            _cogBox.Common.UI.JournalIt.Top = -1;
            _cogBox.Common.UI.JournalIt.Left = -1;

            _cogBox.FrmJournalIt?.SetPositionAndSize();
        }

        private void FromOptions_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _cogBox.FrmOptions = null;
        }

    }
}
