using System;
using System.Windows;

namespace JournalIt.View
{
    /// <summary>
    /// Interaction logic for frmStopwatch.xaml
    /// </summary>
    public partial class FrmStopwatch : Window
    {
        private ViewModel.Stopwatch _stopwatch = null;


        public FrmStopwatch(ViewModel.Stopwatch stopwatch)
        {
            InitializeComponent();
            Stopwatch = stopwatch;
       }



        #region Properties
        public ViewModel.Stopwatch Stopwatch
        {
            get => _stopwatch;
            set
            {
                _stopwatch = value;
                Loaded += (s, e) =>
                {
                    DataContext = _stopwatch;
                };
            }
        }
        #endregion


        #region Methods
        private new bool Close()
        {
            var bSuccess = true;

            if (!ViewModel.Stopwatch.IsValidTime(txtTimeStart.Text))
            {
                MessageBox.Show("The time is not valid.",
                                Stopwatch.CogBox.Common.UI.ProductName);
                bSuccess = false;
            }

            if (!bSuccess || ViewModel.Stopwatch.IsValidDuration(txtDuration.Text))
                return bSuccess;

            MessageBox.Show("The duration is not valid.",
                Stopwatch.CogBox.Common.UI.ProductName);
            bSuccess = false;


            return bSuccess;
        }
        #endregion



        private void chkStopwatch_Click(object sender, RoutedEventArgs e)
        {
            if (chkStopwatch.IsChecked != null
                && (bool)chkStopwatch.IsChecked)
                _stopwatch.CogBox.Start();
            else
                _stopwatch.CogBox.Stop();
        }




        private void cboProject_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cboProject.SelectedIndex == -1)
                {
                    if (cboProject.Text.Length > 0)
                    {
                        //This is a new project
                        // 1. Add a new if it does not exist.
                        // 2. Add the new project to the lists.
                        // 3. Save the model list.
                        var project = ViewModel.Project.New(cboProject.Text);
                        Stopwatch.Project = project;
                    }
                    else
                    {
                        //No project was selected
                        Stopwatch.Project = null;
                    }
                }
                else
                {
                    //Assign an existing project to the stopwatches project
                    Stopwatch.Project = (ViewModel.Project)cboProject.SelectedItem;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,
                                Stopwatch.CogBox.Common.UI.ProductName);
            }
        }


        private void cboCompany_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cboCompany.SelectedIndex == -1)
                {
                    if (cboCompany.Text.Length > 0)
                    {
                        //This is a new Company
                        // 1. Add a new if it does not exist.
                        // 2. Add the new Company to the lists.
                        // 3. Save the model list.
                        ViewModel.Company.New(cboCompany.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                                Stopwatch.CogBox.Common.UI.ProductName);
            }
        }


        private void cboActivity_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cboActivity.SelectedIndex != -1)
                    return;

                if (cboActivity.Text.Length > 0)
                {
                    //This is a new Activity
                    // 1. Add a new if it does not exist.
                    // 2. Add the new Activity to the lists.
                    // 3. Save the model list.
                    ViewModel.Activity.New(cboActivity.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                                Stopwatch.CogBox.Common.UI.ProductName);
            }
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var bSuccess = Close();

            if (bSuccess)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _stopwatch.FrmStopwatch = null;

                        var parent = Window.GetWindow(this);
                        parent.Close();
                    }));
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Close())
                _stopwatch.FrmStopwatch = null;
            else
                e.Cancel = true;
        }
    }
}