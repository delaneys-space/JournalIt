using System.Windows.Controls;

namespace JournalIt.View
{
    /// <summary>
    /// Interaction logic for CtrlExportOptions.xaml
    /// </summary>
    public partial class CtrlExportOptions : UserControl
    {
        private ViewModel.CogBox cogbox;

        public CtrlExportOptions()
        {
            InitializeComponent();
        }


        public ViewModel.CogBox CogBox
        {
            get => cogbox;
            set
            {
                DataContext = value;

                Loaded += (e, s) =>
                {
                    cogbox = value;

                    txtExcelFullname.Text = cogbox.Common.ExcelFileFullname;
                    txtExcelSheet.Text = cogbox.Common.ExcelSheet;
                    chkExportExcel.IsChecked = cogbox.Common.ExportToExcel;
                };
            }
        }
    }
}