using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JournalIt.View
{
    /// <summary>
    /// Interaction logic for CtrlDefaultOptions.xaml
    /// </summary>
    public partial class CtrlDefaultOptions : UserControl
    {
        private ViewModel.CogBox cogbox;

        public CtrlDefaultOptions()
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

                    cboCompany.Text = cogbox.Common.DefaultCompany;

                    var projectName = "";
                    foreach (var project in ViewModel.Project.Projects)
                    {
                        if (project.ProjectId == cogbox.Common.DefaultProjectId)
                            projectName = project.Name;
                    }

                    cboProject.Text = projectName;
                    cboType.Text = cogbox.Common.DefaultType;
                    txtMinimumMinutes.Text = cogbox.Common.MinimumMinutes.ToString();
                };
            }
        } 


        private void cboProject_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cboProject.SelectedIndex != -1)
                    return;

                if (cboProject.Text.Length > 0)
                {
                    //This is a new Project
                    // 1. Add a new if it does not exist.
                    // 2. Add the new Project to the lists.
                    // 3. Save the model list.
                    var project = ViewModel.Project.New(cboProject.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, CogBox.Common.UI.ProductName);
            }
        }

        private void cboCompany_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cboCompany.SelectedIndex != -1)
                return;

            if (cboCompany.Text.Length > 0)
            {
                //This is a new Company
                // 1. Add a new if it does not exist.
                // 2. Add the new Company to the lists.
                // 3. Save the model list.
                ViewModel.Company.New(cboCompany.Text);
            }
        }


        private void cboType_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cboType.SelectedIndex != -1)
                return;

            if (cboType.Text.Length > 0)
            {
                //This is a new Type
                // 1. Add a new if it does not exist.
                // 2. Add the new Type to the lists.
                // 3. Save the model list.
                ViewModel.Activity.New(cboType.Text);
            }
        }

    }
}
